using ERP.Library.Enums.Login;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Models.AMS
{
	public class User
	{
		[Key]
		public int Id { get; set; }

        [ForeignKey(nameof(Role))]
        [Required(ErrorMessage = "必填欄位")]
        public int RoleId { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now.ToLocalTime();

        [StringLength(32, ErrorMessage = "長度不可超過 32 個字元")]
        [Required(ErrorMessage = "必填欄位")]
        public string Name { get; set; } = null!;

        [StringLength(32, ErrorMessage = "長度不可超過 32 個字元")]
        [Required(ErrorMessage = "必填欄位")]
        public string Account { get; set; } = null!;

        [StringLength(128, ErrorMessage = "長度不可超過 128 個字元")]
        [Required(ErrorMessage = "必填欄位")]
        public string Pwd { get; set; } = null!;
		public bool IsLock { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        public virtual Role Role { get; set; } = null!;
    }
}
