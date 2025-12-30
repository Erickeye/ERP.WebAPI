using ERP.EntityModels.Context;
using ERP.Library.ViewModels.Login;
using ERP.Library.ViewModels.Sftp;
using ERP.Service.Helpers;
using ERP.WebAPI.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("Default API", new OpenApiInfo { Version = "v1", Title = "API首頁", });
    options.SwaggerDoc("_1000Company", new OpenApiInfo { Version = "v1", Title = "公司相關", });
    options.SwaggerDoc("_2000Costomer", new OpenApiInfo { Version = "v1", Title = "客戶資料", });
    options.EnableAnnotations();

    // 加入 Bearer token 認證方式
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "請輸入 JWT，格式為：Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // 全域套用 Bearer token 驗證
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
    // 載入 XML 註解
    //var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
//加入CORS 使前端界接
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddControllers(options =>
{
    // 全域套用 [Authorize]
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
})
    .ConfigureApiBehaviorOptions(options =>
    {
        // 關閉自動 400 回應，使進到 Controller 裡手動處理 ModelState
        options.SuppressModelStateInvalidFilter = true;
    });

//--------------------JWT Settings-------------------
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var key = Encoding.ASCII.GetBytes(jwtSettings!.SecretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),

        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            // 預設會回傳 401，這裡攔截自訂回應
            context.HandleResponse();

            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            var result = new
            {
                ErrorCode = 1008,
                Message = "無效的權杖"
            };

            var json = System.Text.Json.JsonSerializer.Serialize(result);

            return context.Response.WriteAsync(json);
        }
    };
});
//--------------------JWT Settings-------------------

builder.Services.AddDbContext<ERPDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ERP")));

builder.Services.Configure<SftpConfig>(builder.Configuration.GetSection("SftpConfig"));
//builder.Services.AddScoped<ILoginService, LoginService>();
//builder.Services.AddScoped<I_1000Service, _1000Service>();
//builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
//builder.Services.AddScoped<IUserInfoService, UserInfoService>();
//builder.Services.AddScoped<IUserService, UserService>();
//builder.Services.AddScoped<IRoleService, RoleService>();
//builder.Services.AddScoped<ISftpService, SftpService>();

// 註冊介面跟服務(自動註冊服務)
var assembly = Assembly.GetAssembly(typeof(EnumHelper));
var types = assembly!.GetTypes()
            .Where(c => c.Namespace != null)
            .Where(c => c.Namespace!.StartsWith("ERP.Service"))
            .Where(c => c.GetInterfaces().Any(x => x.Name == $"I{c.Name}"))
            .Where(c => !c.IsAbstract)
            .Where(c => !c.IsInterface)
            ;

foreach (var type in types)
{
    var interfaceType = type.GetInterfaces().FirstOrDefault(c => c.Name != null);
    if(interfaceType != null)
    {
        builder.Services.AddScoped(interfaceType,type);
        //Console.WriteLine($"已註冊 {interfaceType.Name} -> {type.Name}");
    }
}

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/Default API/swagger.json", "Default API");
        options.SwaggerEndpoint("/swagger/_1000Company/swagger.json", "_1000Company API");
        options.SwaggerEndpoint("/swagger/_2000Costomer/swagger.json", "_2000Costomer API");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("DevCors");
app.UseAuthorization();

app.UseMiddleware<ActionLoggingMiddleware>();

app.MapControllers();

app.Run();
