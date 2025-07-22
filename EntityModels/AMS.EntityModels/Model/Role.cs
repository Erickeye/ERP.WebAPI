using System.ComponentModel.DataAnnotations;

namespace New_ERP.Models.AMS
{
	public class t_role
	{
		[Key]
		public int f_id { get; set; }
		public string f_roleName { get; set; } = string.Empty;
		public int? f_sessionTime { get; set; } = 30;
		public int? f_permissionLevel { get; set; } = 0;
		public int? f_approvalLevel { get; set; } = 30;
		public int? f_quotationLevel { get; set; } = 30;
		public int? f_procurementLevel { get; set; } = 30;		
	}
}
