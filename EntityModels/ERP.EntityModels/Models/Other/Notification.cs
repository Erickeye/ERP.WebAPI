using ERP.EntityModels.Models._1000Company;
using ERP.Library.Enums.Other;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntityModels.Models._9000Other
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateTime { get; set; }

        public TableType Type { get; set; }

        [StringLength(32, ErrorMessage = "長度不可超過 32 個字元")]
        public string? TargetId { get; set; }

        [StringLength(256, ErrorMessage = "長度不可超過 256 個字元")]
        public string? Message { get; set; } = null!;
        public bool IsShow { get; set; }
        public bool IsRead { get; set; }
    }
}
