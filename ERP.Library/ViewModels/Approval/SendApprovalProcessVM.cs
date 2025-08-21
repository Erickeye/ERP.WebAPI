using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Library.ViewModels.Approval
{
    public class SendApprovalProcessVM
    {
        public int ApprovalSettingsId { get; set; }
        public string TableId { get; set; } = null!;
    }
    public class ApprovalVM
    {
        public int RecordId { get; set; }
        public string? Memo { get; set; } = null!;
    }
}
