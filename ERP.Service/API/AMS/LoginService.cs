using ERP.Data;
using ERP.EntityModels.Models;
using ERP.EntityModels.Models._1000Company;
using ERP.Library.Enums;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels.Login;
using ERP.Library.Helpers;
using ERP.Models.AMS;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace ERP.Service.API.AMS
{
    public interface ILoginService
    {
        Task<ResultModel<LoginResponse>> AuthenticateAsync(LoginRequest login);
        Task<ResultModel<AccessRefreshToken>> RefreshTokenAsync(AccessRefreshToken toekn);
        Task<ResultModel<string>> Logout(string? userId);
    }

    public class LoginService : ILoginService
    {
        private readonly ERPContext _context;
        private readonly JwtSettings _jwtSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginService(ERPContext context, IOptions<JwtSettings> jwtOptions, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _jwtSettings = jwtOptions.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResultModel<LoginResponse>> AuthenticateAsync(LoginRequest login)
        {
            var result = new ResultModel<LoginResponse>();
            var user = await _context.User
                .FirstOrDefaultAsync(u => u.Account == login.Account);

            if (user == null)
            {
                result.SetError(ErrorCodeType.UserNotFound);
                return result;
            }

            if (PasswordHelper.IsPasswordValid(login.Password, user.Pwd!))
            {
                result.SetError(ErrorCodeType.IncorrectUsernameOrPassword);
                return result;
            }
            if (user.IsLock)
            {
                result.SetError(ErrorCodeType.UserLocked);
                return result;
            }

            var permissions = GetPermissions(user.RoleId);

            var accessToken = GenerateAccessToken(user, permissions);
            var refreshToken = GenerateRefreshToken();
            var AccessTokenExpirationTime = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes).ToLocalTime();
            var RefreshTokenExpirationTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays).ToLocalTime();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = RefreshTokenExpirationTime;
            _context.SaveChanges();

            result.Data = new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Expiration = AccessTokenExpirationTime,
                UserName = user.Name,
                UserId = user.Id,
                Role = user.RoleId,
                Permissions = permissions
            };
            //紀錄登入Log
            LoginLog(login, user.Id);
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

            var user = await _context.User.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            if (user == null || user.RefreshToken != toekn.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                result.SetError(ErrorCodeType.TokenExpiredOrInvalid);
                return result;
            }

            var permissions = GetPermissions(user.RoleId);
            // 發新 token
            var newAccessToken = GenerateAccessToken(user, permissions);
            var newRefreshToken = GenerateRefreshToken();

            // 更新 DB 的 refresh token
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
            await _context.SaveChangesAsync();

            result.Data = new AccessRefreshToken
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };

            return result;
        }

        public async Task<ResultModel<string>> Logout(string? userId)
        {
            var result = new ResultModel<string>();
            // 從 JWT Claims 取得使用者 Id
            if (userId == null)
            {
                result.SetError(ErrorCodeType.UserNotFound);
                return result;
            }

            var user = await _context.User.FirstOrDefaultAsync(u => u.Id == int.Parse(userId));
            if (user == null)
            {
                result.SetError(ErrorCodeType.UserNotFound);
                return result;
            }
            // 清除使用者的 refresh token 及過期時間
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _context.SaveChangesAsync();

            result.SetSuccess("成功登出");
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


        public string GenerateAccessToken(User user, List<int> permissions)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
            var permissionJson = JsonSerializer.Serialize(permissions);
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.RoleId.ToString()),
            new Claim("roleId", user.RoleId.ToString()),
            new Claim("account", user.Account),
            new Claim("permissions", permissionJson)
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

        private void LoginLog(LoginRequest login, int userId)
        {
            var log = new t_1700LoginLog
            {
                UserId = userId,
                Account = login.Account,
                IpAddress = GetClientIp(),
                CrateDate = DateTime.Now
            };
            _context.t_1700LoginLog!.Add(log);
            _context.SaveChangesAsync();
        }
        public string GetClientIp()
        {
            var ip = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

            if (_httpContextAccessor.HttpContext?.Request.Headers.ContainsKey("X-Forwarded-For") == true)
            {
                ip = _httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            }

            return ip ?? "Unknown";
        }
        private List<int> GetPermissions(int roleId) {
            var permissions = _context.Permission
                .Where(c => c.RoleId == roleId)
                .AsNoTracking()
                .Select(c => (int)c.PageId)
                .ToList();
            return permissions;
        }

        //internal static string HashPassword(string password)
        //{
        //    using SHA256 sha256Hash = SHA256.Create();
        //    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

        //    StringBuilder builder = new();
        //    return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        //}
        //private static bool IsPasswordValid(string pwd, string hashpwd)
        //{
        //    return HashPassword(pwd) == hashpwd;
        //}
    }

}
