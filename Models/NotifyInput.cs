using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncToStaging.Helper.Models
{
    public class NotifyInput
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public object TotiImageFileName { get; set; }
        public object NotiImagePathFile { get; set; }
        public List<string> OutletCodeList { get; set; } = new List<string>();
        public string NotiType { get; set; } = "URGENT";
        public string Priority { get; set; } = "WARNING";
        public string NavigateType { get; set; } = "OS_SYNCDATA";
        public string NavigatePath { get; set; }
        public string Purpose { get; set; }
        public object TemplateData { get; set; }
        public string OwnerType { get; set; }
        public string OwnerCode { get; set; }
    }
}
