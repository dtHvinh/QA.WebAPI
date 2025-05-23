﻿using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

public class Report : IEntityWithTime<int>
{
    public int Id { get; set; }
    public DateTimeOffset CreationDate { get; set; }
    public DateTimeOffset ModificationDate { get; set; }

    public string Description { get; set; } = default!;
    public string Type { get; set; } = default!;
    public string Status { get; set; } = default!;
    public int TargetId { get; set; }
}

public static class ReportStatus
{
    public const string Resolved = "Resolved";
    public const string Rejected = "Rejected";
}
