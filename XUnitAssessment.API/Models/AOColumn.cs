using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace XUnitAssessment.API.Models;

public class AOColumn
{
    [Key]
    public Guid Id { get; set; }

    public Guid? TableId { get; set; }

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string? Description { get; set; }

    public string? DataType { get; set; }

    public int? DataSize { get; set; }

    public int? DataScale { get; set; }

    public string? Comment { get; set; }

    public int? Encrypted { get; set; }

    public string? Distortion { get; set; }

    [JsonIgnore]
    [ForeignKey("TableId")]
    public AOTable? aoTable { get; set; }

    [JsonIgnore]
    public Field? aoField { get; set; }
}
