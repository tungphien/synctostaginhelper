using System;
using System.Collections.Generic;

namespace SyncToStaging.Helper.Models;

public interface IOsuser
{
    public Guid Id { get; set; }

    public string UserName { get; set; }

    public string PhoneNumber { get; set; }

    public string Password { get; set; }

    public string FullName { get; set; }

    public bool? Actived { get; set; }

    public bool? Locked { get; set; }

    public string CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
