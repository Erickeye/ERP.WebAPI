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

        [DataType(DataType.Date)]
        public DateTime DateTime { get; set; }

        public string Message { get; set; } = null!;

        public MessageType Type { get; set; }

        public string? TargetId { get; set; }
        public int UserId { get; set; }
    }
}
