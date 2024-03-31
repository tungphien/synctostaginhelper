namespace SyncToStaging.Helper.Models
{
    public class OdsyncDataSettingModel
    {
        public Guid Id { get; set; }

        public string OddataType { get; set; }

        public string OsdataType { get; set; }

        public string Status { get; set; }

        public bool IsCreateDataChange { get; set; }

        public bool IsNotiUrgent { get; set; }
    }
}
