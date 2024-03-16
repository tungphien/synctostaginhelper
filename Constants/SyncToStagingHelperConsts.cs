﻿using System;
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
    }
}
