﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

[Index(nameof(NormalizedName), IsUnique = true)]
[Index(nameof(QuestionCount), IsDescending = [true])]
public class Tag : IEntityWithTime<int>
{
    public int Id { get; set; }

    [Column(TypeName = "nvarchar(50)")]
    public required string Name { get; set; }
    [Column(TypeName = "nvarchar(50)")]
    public string NormalizedName { get; set; } = default!;
    public int QuestionCount { get; set; }

    public TagDescription? Description { get; set; }
    public TagBody? WikiBody { get; set; }

    [JsonIgnore]
    public ICollection<Question> Questions { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
}
