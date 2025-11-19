using ERP.Library.Enums.Login;

namespace ERP.Library.ViewModels.Login
{
    public class LoginRequest
    {
        public required string Account { get; set; }
        public required string Password { get; set; }
    }

    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int Role { get; set; }
        public List<int> Permissions { get; set; } = new();
    }
}
