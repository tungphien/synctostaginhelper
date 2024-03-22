using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncToStaging.Helper.Models
{
    public interface IOdsyncDataSetting
    {
        public Guid Id { get; set; }

        public string OddataType { get; set; }

        public string OsdataType { get; set; }

        public string Status { get; set; }

        public bool IsCreateDataChange { get; set; }

        public bool IsNotiUrgent { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
