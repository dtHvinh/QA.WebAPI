using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

public class Tag : IEntity<Guid>
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public required string Name { get; set; }

    public ICollection<QuestionTag> QuestionTags { get; set; } = default!;
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
