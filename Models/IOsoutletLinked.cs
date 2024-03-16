using System;
using System.Collections.Generic;

namespace SyncToStaging.Helper.Models;

public interface IOsoutletLinked
{
    public Guid Id { get; set; }

    public string OutletCode { get; set; }

    public string DistributorCode { get; set; }

    public string DistributorName { get; set; }

    public string OutletType { get; set; }

    public string CustomerCode { get; set; }

    public string Status { get; set; }

    public string CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDefault { get; set; }

    public long? OrderQuantites { get; set; }

    public string RequestStatus { get; set; }

    public string CustomerShipTo { get; set; }

    public DateTime? LastestOrderDate { get; set; }

    public string LastestOrderNumber { get; set; }
}
