﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncToStaging.Helper.Models
{
    public class NotifyOutput
    {
        public IList<string> Messages { get; set; } = new List<string>();
        public object Data { get; set; }
        public bool Success { get; set; }
        /// <summary>
        /// Sẽ lưu thông tin input param gửi qua mobile
        /// </summary>
        public string NotifyMobileParamLog { get; set; }
        /// <summary>
        /// Sẽ lưu thông tin uri mobile đang gọi
        /// </summary>
        public string NotifyMobileUriLog { get; set; }

    }
}
