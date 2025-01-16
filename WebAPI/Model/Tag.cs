using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

[Index(nameof(NormalizedName), IsUnique = true)]
public class Tag : IEntity<Guid>
{
    public Guid Id { get; set; }

    [Column(TypeName = "nvarchar(50)")]
    public required string Name { get; set; }
    [Column(TypeName = "nvarchar(50)")]
    public string NormalizedName { get; set; } = default!;
    [Column(TypeName = "nvarchar(1000)")]
    public string Description { get; set; } = default!;

    public ICollection<Question> Questions { get; set; } = default!;
}

public enum Tags
{
    CSharp,
    Java,
    Python,
    JavaScript,
    TypeScript,
    Angular,
    React,
    Vue,
    NextJS,
    NuxtJS,
    Blazor,
    ASPNETCore,
    EntityFrameworkCore,
    Dapper,
    SQLServer,
    PostgreSQL,
    MySQL,
    MongoDB,
    Redis,
    RabbitMQ,
    Kafka,
    Docker,
    Kubernetes,
    Jenkins,
    GitLab,
    Ruby,
    PHP,
    Swift,
    Kotlin,
    Go,
    Rust,
    Dart,
    SpringBoot,
    Flask,
    Django,
    Laravel,
    Express,
    Svelte,
    SQLite,
    Firebase,
    ElasticSearch,
    Ansible,
    Terraform,
    AWS,
    Azure,
    GCP,
    GraphQL,
    REST,
    WebAssembly,
    Microservices
}
