using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncToStaging.Helper.Constants
{
    public static class SyncToStagingHelperConsts
    {
        public class REQUEST_TYPE
        {
            public static readonly string INSERT = "INSERT";
            public static readonly string UPDATE = "UPDATE";
            public static readonly string DELETE = "DELETE";
            public static readonly string BULKINSERT = "BULKINSERT";
        }
        public class NOTI_TYPE
        {
            public static readonly string NORMAL = "NORMAL";
            public static readonly string SHOWHOME = "SHOWHOME";
            public static readonly string URGENT = "URGENT";
        }
        public class OWNER_TYPE
        {
            public static readonly string PRINCIPAL = "PRINCIPAL";
            public static readonly string DISTRIBUTOR = "DISTRIBUTOR";
            public static readonly string OUTLET = "OUTLET";
            public static readonly string SYSTEM = "SYSTEM";
        }
        public class LOG_HISTORY_STATUS
        {
            public static readonly string FAILED = "FAILED";
        }
        public class ENTITY_TABLE
        {
            public static readonly string Osusers = "Osusers";
            public static readonly string Osoutlets = "Osoutlets";
            public static readonly string OsoutletLinkeds = "OsoutletLinkeds";
            public static readonly string OdsyncDataSettings = "OdsyncDataSettings";
            public static readonly string Services = "Services";
            public static readonly string StagingSyncDataHistories = "StagingSyncDataHistories";
        }
        public class STATUS
        {
            public static readonly string ACTIVE = "ACTIVE";
        }
    }
}
