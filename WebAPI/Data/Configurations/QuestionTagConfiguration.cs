using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebAPI.Model;

namespace WebAPI.Data.Configurations;

public class QuestionConfig : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.HasMany(e => e.Tags)
            .WithMany(e => e.Questions)
            .UsingEntity(nameof(QuestionTag));

        builder.HasMany(e => e.Collections)
            .WithMany(e => e.Questions)
            .UsingEntity(nameof(QuestionCollection));
    }
}

public class TagConfig : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasMany(e => e.Questions)
            .WithMany(e => e.Tags)
            .UsingEntity(nameof(QuestionTag));
    }
}

public class CollectionConfig : IEntityTypeConfiguration<Collection>
{
    public void Configure(EntityTypeBuilder<Collection> builder)
    {
        builder.HasMany(e => e.Questions)
            .WithMany(e => e.Collections)
            .UsingEntity(nameof(QuestionCollection));
    }
}
