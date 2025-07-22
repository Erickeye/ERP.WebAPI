using System.ComponentModel.DataAnnotations;

namespace ERP.Models.AMS
{
    public class t_level
    {
        [Key]
        public int f_permissionID { get; set; }
        public int f_permissionLevel { get; set; }
        public int f_levelAmount { get; set; }
    }
}
