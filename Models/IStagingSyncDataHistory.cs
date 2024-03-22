using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncToStaging.Helper.Models
{
    public interface IStagingSyncDataHistory
    {
        public Guid Id { get; set; }
        public string TempId { get; set; }
        public string DataType { get; set; }
        public string RequestType { get; set; }
        public string ErrorMessage { get; set; }
        public string InsertStatus { get; set; }
        public string EventBusStatus { get; set; }
        public DateTime? TimeRunAdhoc { get; set; }
        public string FileError { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? RunAllDate { get; set; }
    }
}
