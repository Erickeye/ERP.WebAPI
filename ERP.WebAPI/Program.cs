using ERP.Data;
using ERP.Library.ViewModels.Login;
using ERP.Library.ViewModels.Sftp;
using ERP.Service.API._1000Company;
using ERP.Service.API.AMS;
using ERP.Service.Helpers;
using ERP.Service.Sftp;
using ERP.WebAPI.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("Default API", new OpenApiInfo { Version = "v1", Title = "API����", });
    options.SwaggerDoc("_1000Company", new OpenApiInfo { Version = "v1", Title = "���q����", });
    options.SwaggerDoc("_2000Costomer", new OpenApiInfo { Version = "v1", Title = "�Ȥ���", });
    options.EnableAnnotations();

    // �[�J Bearer token �{�Ҥ覡
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "�п�J JWT�A�榡���GBearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // ����M�� Bearer token ����
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

    // ���J XML ����
    //var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
builder.Services.AddControllers(options =>
{
    // ����M�� [Authorize]
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
})
    .ConfigureApiBehaviorOptions(options =>
    {
        // �����۰� 400 �^���A�϶i�� Controller �̤�ʳB�z ModelState
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
            // �w�]�|�^�� 401�A�o���d�I�ۭq�^��
            context.HandleResponse();

            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            var result = new
            {
                ErrorCode = 1008,
                Message = "�L�Ī��v��"
            };

            var json = System.Text.Json.JsonSerializer.Serialize(result);

            return context.Response.WriteAsync(json);
        }
    };
});
//--------------------JWT Settings-------------------

builder.Services.AddDbContext<ERPContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ERP")));

builder.Services.Configure<SftpConfig>(builder.Configuration.GetSection("SftpConfig"));
//builder.Services.AddScoped<ILoginService, LoginService>();
//builder.Services.AddScoped<I_1000Service, _1000Service>();
//builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
//builder.Services.AddScoped<IUserInfoService, UserInfoService>();
//builder.Services.AddScoped<IUserService, UserService>();
//builder.Services.AddScoped<IRoleService, RoleService>();
//builder.Services.AddScoped<ISftpService, SftpService>();

// ���U������A��(�۰ʵ��U�A��)
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
        //Console.WriteLine($"�w���U {interfaceType.Name} -> {type.Name}");
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

app.UseAuthorization();

app.UseMiddleware<ActionLoggingMiddleware>();

app.MapControllers();

app.Run();
