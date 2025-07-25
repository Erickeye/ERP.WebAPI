using ERP.Library.Enums.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Library.ViewModels.UserInfo
{
    public class UserInfo
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Account { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
    }
}
