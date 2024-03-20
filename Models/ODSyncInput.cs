using System.ComponentModel.DataAnnotations;

namespace SyncToStaging.Helper.Models
{
    public class ODSyncInput
    {
        [Required]
        public string Token { get; set; }
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
        /// IsUrgent dùng cho khi noify mobile
        /// </summary>
        public bool IsUrgent { get; set; } = true;
        /// <summary>
        /// IsCreateDataChange dùng gửi qua staging để check và tạo OSUserDataChange, và biến này sẽ dùng notification
        /// </summary>
        public bool IsCreateDataChange { get; set; } = true;
    }
}
