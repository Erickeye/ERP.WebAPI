using ERP.Data;
using ERP.Library.ViewModels.Login;
using ERP.Models.AMS;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TLRIMOA.Library.Enums;
using TLRIMOA.Library.ViewModels;

namespace ERP.Service.Services
{
    public interface IAuthService
    {
        Task<ResultModel<LoginResponse>> AuthenticateAsync(string account, string password);
        string GenerateAccessToken(t_user user);
        string GenerateRefreshToken();
    }

    public class AuthService : IAuthService
    {
        private readonly AMSContext _context;
        private readonly JwtSettings _jwtSettings;

        public AuthService(AMSContext context, IOptions<JwtSettings> jwtOptions)
        {
            _context = context;
            _jwtSettings = jwtOptions.Value;
        }

        public async Task<ResultModel<LoginResponse>> AuthenticateAsync(string account, string password)
        {
            var result = new ResultModel<LoginResponse>();
            var user = await _context.t_user
                .FirstOrDefaultAsync(u => u.f_account == account);

            if (user == null)
            {
                result.SetError(ErrorCodeType.IncorrectUsernameOrPassword);
                return result;
            }

            // 密碼比對 (建議加密比對，這裡示範明文比對)
            if (user.f_pwd != password) return null;
            if (user.f_isLock) return null;

            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            // 可把 refreshToken 儲存到資料庫，並與 user 關聯

            result.Data = new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                UserName = user.f_name,
                UserId = user.f_id,
                Role = user.f_role
            };
            return result;
        }

        public string GenerateAccessToken(t_user user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.f_id.ToString()),
            new Claim(ClaimTypes.Name, user.f_name),
            new Claim(ClaimTypes.Role, user.f_role.ToString()),
            new Claim("account", user.f_account)
        };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }

}
