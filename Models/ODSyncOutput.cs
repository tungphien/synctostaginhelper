namespace SyncToStaging.Helper.Models
{
    public class BaseSyncOutput<T>
    {
        public IList<string> Messages { get; set; } = new List<string>();
        public T Data { get; set; }
        public bool Success { get; set; }
        public string StrackTrace { get; set; }
        public int TotalCount { get; set; }
    }
    public class ODSyncOutput
    {
    }
}
