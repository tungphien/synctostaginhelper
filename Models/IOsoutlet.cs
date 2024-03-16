using System;
using System.Collections.Generic;

namespace SyncToStaging.Helper.Models;

public interface IOsoutlet
{
    public Guid Id { get; set; }

    public string OutletCode { get; set; }

    public string OutletName { get; set; }

    public string OutletOwnerName { get; set; }

    public string PhoneNumber { get; set; }

    public string OutletFormatCode { get; set; }

    public string AvatarFileName { get; set; }

    public string AvatarFilePath { get; set; }

    public string AddressCountry { get; set; }

    public string AddressProvince { get; set; }

    public string AddressDistrict { get; set; }

    public string AddressWard { get; set; }

    public string AddressStreetNo { get; set; }

    public string Address { get; set; }

    public string VerifyStatus { get; set; }

    public string Status { get; set; }

    public string CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string OutletFormatName { get; set; }

    public string SystemCode { get; set; }

    public double? Longitude { get; set; }

    public double? Latitude { get; set; }

    public string VerifyReason { get; set; }

    public string VerifyReasonCode { get; set; }

    public string AddressCountryCode { get; set; }

    public string AddressProvinceCode { get; set; }

    public string AddressDistrictCode { get; set; }

    public string AddressWardCode { get; set; }
}
