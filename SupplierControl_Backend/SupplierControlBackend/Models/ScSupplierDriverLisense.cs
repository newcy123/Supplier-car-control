using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplierControlBackend.Models;

public partial class ScSupplierDriverLisense
{
    public string DriverNbr { get; set; } = null!;

    public string? Fname { get; set; }

    public string? Surn { get; set; }

    public string? DriverPicture { get; set; }

    public string? VenderCode { get; set; }

    public string? VenderName { get; set; }

    public string? VehicleNbr { get; set; }

    public string? DriverLicence { get; set; }

    public DateTime? DriverLicenceExpire { get; set; }

    public string? TcTel { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }

    public string? IssueBy { get; set; }

    public DateTime? IssueDate { get; set; }

    public string? DriverStatus { get; set; }

    [NotMapped]
    public IFormFile? FormFile { get; set; }

    [NotMapped]
    public string? RawImage { get; set; }
}
