namespace SyncToStaging.Helper.Models
{
    public class BaseSyncOutput<T>
    {
        public IList<string> Messages { get; set; } = new List<string>();
        public T Data { get; set; }
        public bool Success { get; set; }
        public string StrackTrace { get; set; }
        public int TotalCount { get; set; }
        /// <summary>
        /// Sẽ lưu thông tin input param gửi qua mobile
        /// </summary>
        public string NotifyMobileParamLog { get; set; }
        /// <summary>
        /// Sẽ lưu thông tin uri mobile đang gọi
        /// </summary>
        public string NotifyMobileUriLog { get; set; }
        public Guid TempId { get; set; }
    }
    public class ODSyncOutput
    {
    }
}
