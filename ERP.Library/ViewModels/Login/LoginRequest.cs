using ERP.Library.Enums.Login;

namespace ERP.Library.ViewModels.Login
{
    public class LoginRequest
    {
        public required string Account { get; set; }
        public required string Password { get; set; }
    }
}
