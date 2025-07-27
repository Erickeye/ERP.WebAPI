using ERP.Data;
using ERP.Library.ViewModels.Login;
using ERP.Models.AMS;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ERP.Library.Enums;
using ERP.Library.ViewModels;

namespace ERP.Service.Services
{
    public interface ILoginService
    {
        Task<ResultModel<LoginResponse>> AuthenticateAsync(string account, string password);
        Task<ResultModel<AccessRefreshToken>> RefreshTokenAsync(AccessRefreshToken toekn);
        Task<ResultModel<String>> Logout(string userId);
    }

    public class LoginService : ILoginService
    {
        private readonly AMSContext _context;
        private readonly JwtSettings _jwtSettings;

        public LoginService(AMSContext context, IOptions<JwtSettings> jwtOptions)
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
            var AccessTokenExpirationTime = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes).ToLocalTime();
            var RefreshTokenExpirationTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays).ToLocalTime();

            user.f_refreshToken = refreshToken;
            user.f_refreshTokenExpiryTime = RefreshTokenExpirationTime;
            _context.SaveChanges();

            result.Data = new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Expiration = AccessTokenExpirationTime,
                UserName = user.f_name,
                UserId = user.f_id,
                Role = user.f_role
            };
            return result;
        }

        public async Task<ResultModel<AccessRefreshToken>> RefreshTokenAsync(AccessRefreshToken toekn)
        {
            var result = new ResultModel<AccessRefreshToken>();

            // 解析 JWT Token，取出使用者 ID
            var principal = GetPrincipalFromExpiredToken(toekn.AccessToken);
            if (principal == null)
            {
                result.SetError(ErrorCodeType.InvalidToken);
                return result;
            }

            var userId = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                result.SetError(ErrorCodeType.InvalidToken);
                return result;
            }

            var user = await _context.t_user.FirstOrDefaultAsync(u => u.f_id.ToString() == userId);
            if (user == null || user.f_refreshToken != toekn.RefreshToken || user.f_refreshTokenExpiryTime <= DateTime.UtcNow)
            {
                result.SetError(ErrorCodeType.TokenExpiredOrInvalid);
                return result;
            }

            // 發新 token
            var newAccessToken = GenerateAccessToken(user);
            var newRefreshToken = GenerateRefreshToken();

            // 更新 DB 的 refresh token
            user.f_refreshToken = newRefreshToken;
            user.f_refreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
            await _context.SaveChangesAsync();

            result.Data = new AccessRefreshToken
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };

            return result;
        }

        public async Task<ResultModel<string>> Logout(string userId)
        {
            var result = new ResultModel<string>();
            // 從 JWT Claims 取得使用者 Id
            if (userId == null)
            {
                result.SetError(ErrorCodeType.UserNotFound);
                return result;
            }

            var user = await _context.t_user.FirstOrDefaultAsync(u => u.f_id == int.Parse(userId));
            if (user == null)
            {
                result.SetError(ErrorCodeType.UserNotFound);
                return result;
            }
            // 清除使用者的 refresh token 及過期時間
            user.f_refreshToken = null;
            user.f_refreshTokenExpiryTime = null;

            return result;
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false, // ← 不驗證過期，才能處理 refresh
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }
                return principal;
            }
            catch
            {
                return null;
            }
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
