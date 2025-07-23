using ERP.Library.Enums.Login;

namespace ERP.Library.ViewModels.Login
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public RoleType Role { get; set; }
    }
}
