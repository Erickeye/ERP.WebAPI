using System.ComponentModel.DataAnnotations;

namespace ERP.Models.AMS
{
    public class Level
    {
        [Key]
        public int PermissionId { get; set; }
        public int PermissionLevel { get; set; }
        public int LevelAmount { get; set; }
    }
}
