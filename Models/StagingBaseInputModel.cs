namespace SyncToStaging.Helper.Models
{
    public class StagingBaseInputModel<T>
    {
        public T Data { get; set; }
        public Guid? Id { get; set; }
        public Guid TempId { get; set; }
        public string DataType { get; set; }
        public bool isUrgent { get; set; } = false;
        public bool IsCreateDataChange { get; set; }
    }
}
