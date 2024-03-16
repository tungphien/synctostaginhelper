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
        public Object Data { get; set; }
        /// <summary>
        /// Default là true. Dùng cho case data reference không cần notify. Set NeedNotify=false
        /// </summary>
        public bool NeedNotify { get; set; } = true;
    }
}
