using System;
using System.Collections.Generic;

namespace SupplierControlBackend.Models;

public partial class ScSuplierInOut
{
    public DateTime DeliveryDate { get; set; }

    public string DeliveryShift { get; set; } = null!;

    public string VenderCard { get; set; } = null!;

    public DateTime? EntryTime { get; set; }

    public DateTime? LeaveTime { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? UpdateBy { get; set; }

    public DateTime? UpdateDate { get; set; }
}
