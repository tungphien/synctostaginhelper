using System.ComponentModel.DataAnnotations;

namespace SyncToStaging.Helper.Models
{
    public class ODSyncInput
    {
        [Required]
        public string Token { get; set; }
        /// <summary>
        /// OD DataType
        /// </summary>
        [Required]
        public string DataType { get; set; }
        /// <summary>
        /// INSERT|UPDATE|DELETE|BULKINSERT
        /// </summary>
        [Required]
        public string RequestType { get; set; }
        [Required]
        public string Url { get; set; }
        /// <summary>
        /// PRINCIPAL|DISTRIBUTOR|OUTLET|SYSTEM
        /// </summary>
        [Required]
        public string OwnerType { get; set; }
        /// <summary>
        /// PRINCIPAL CODE|DISTRIBUTOR CODE|OUTLET CODE| NULL
        /// </summary>
        public string OwnerCode { get; set; }
        /// <summary>
        /// Đối với case DELETE thì sẽ truyền Id record vào dạng string
        /// </summary>
        public Object Data { get; set; }
        /// <summary>
        /// Default là true. Dùng cho case data reference không cần notify. Set IsSendNotification=false
        /// </summary>
        public bool IsSendNotification { get; set; } = true;

        /// <summary>
        /// Data lấy theo key: ODNotificationAPI
        /// Value dạng: https://fmcg-notification-api.rdos.online/api/v1/
        /// </summary>
        public string NotificationBaseAPI { get; set; }
        /// <summary>
        /// Biến này sẽ chứa danh sách guid sẽ xóa khỏi hệ thống trước khi bulkinsert vào
        /// </summary>
        public List<Guid> DeletedIds { get; set; } = new();
    }
}
