using System.ComponentModel.DataAnnotations;
using ERP.Library.Enums.Login;

namespace ERP.Models.AMS
{
	public class t_user
	{
		[Key]
		public int f_id { get; set; }
		public DateTime f_createDate { get; set; } = DateTime.Now.ToLocalTime();
		public string f_name { get; set; } = string.Empty;
		public string f_account { get; set; } = string.Empty;
		public string f_pwd { get; set; } = string.Empty;
		public bool f_isLock { get; set; }
		public string? f_sessionId { get; set; } = string.Empty;
		public RoleType f_role { get; set; }
	}
}
