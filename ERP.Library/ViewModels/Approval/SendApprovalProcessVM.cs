using ERP.Library.Enums.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Library.ViewModels.Approval
{
    public class SendApprovalProcessVM
    {
        public TableType TableType { get; set; }
        public string TableId { get; set; } = null!;
    }
    public class ApprovalVM
    {
        public TableType TableType { get; set; }
        public string? TableId { get; set; } = null!;
        public string? Memo { get; set; } = null!;
    }
    public class RejectApprovalVM
    {
        public TableType TableType { get; set; }
        public string? TableId { get; set; } = null!;
        public string? Memo { get; set; } = null!;
    }
}
