using ERP.Library.Enums.Login;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Models.AMS
{
	public class Permission
	{
        [Key]
        [ForeignKey(nameof(Role))]
        public int RoleId { get; set; }
		public PermissionType PageId { get; set; }

        public virtual Role Role { get; set; } = null!;
    }
}
