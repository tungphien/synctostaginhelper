﻿using System.ComponentModel.DataAnnotations;
using static SyncToStaging.Helper.Constants.SyncToStagingHelperConsts;

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
        public string StagingBaseAPICode { get; set; }
        [Required]
        public string StagingRequestPath { get; set; }
        /// <summary>
        /// PRINCIPAL|DISTRIBUTOR|OUTLET|SYSTEM
        /// </summary>
        public string OwnerType { get; set; } = OWNER_TYPE.SYSTEM;
        /// <summary>
        /// PRINCIPAL CODE|DISTRIBUTOR CODE|OUTLET CODE| NULL
        /// </summary>
        public string OwnerCode { get; set; } = null;
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
        public string NotificationBaseAPICode { get; set; }
        /// <summary>
        /// Biến này sẽ chứa danh sách guid sẽ xóa khỏi hệ thống trước khi bulkinsert vào
        /// </summary>
        public List<Guid> DeletedIds { get; set; } = new();
    }
}
