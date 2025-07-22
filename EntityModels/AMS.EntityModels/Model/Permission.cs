using System.ComponentModel.DataAnnotations;
using ERP.Library.Enums.Login;

namespace New_ERP.Models.AMS
{
	public class t_permission
	{
		[Key]
		public int f_roleId { get; set; }
		public PermissionType f_pageId { get; set; }
		public bool f_type { get; set; }
	}
}
