using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using WebAPI.Model;
using WebAPI.Utilities.Contract;

namespace WebAPI.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        foreach (var entity in GetEntities())
        {
            builder.Entity(entity);
        }

        builder
            .Entity<Report>().HasDiscriminator(e => e.ReportType)
            .HasValue<QuestionReport>(nameof(ReportTypes.Question))
            .HasValue<AnswerReport>(nameof(ReportTypes.Answer));


        builder
            .Entity<Vote>().HasDiscriminator(e => e.VoteType)
            .HasValue<QuestionVote>(nameof(VoteTypes.Question))
            .HasValue<AnswerVote>(nameof(VoteTypes.Answer));

        builder
            .Entity<Comment>().HasDiscriminator(e => e.CommentType)
            .HasValue<QuestionComment>(nameof(Question))
            .HasValue<AnswerComment>(nameof(Answer));
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    private static IEnumerable<Type> GetEntities()
    {
        return typeof(ApplicationDbContext).Assembly.GetTypes()
            .Where(type => type.IsClass && type.GetInterfaces()
                .Any(i => i == typeof(IKeylessEntityWithTime)
                || (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityWithTime<>))
                || (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntity<>))));
    }

    public static async Task Init(WebApplication app, bool run)
    {
        if (!run)
            return;

        using var scope = app.Services.CreateScope();
        //var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        //var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        //await roleManager.CreateAsync(new IdentityRole<Guid>(Constants.Roles.Admin));
        //await roleManager.CreateAsync(new IdentityRole<Guid>(Constants.Roles.User));

        var tagRepository = scope.ServiceProvider.GetRequiredService<WebAPI.Repositories.Base.ITagRepository>();

        var file = new StreamReader(
            new FileStream(
                path: "D:\\dev\\myproject\\qa_platform\\back-end\\WebAPI\\WebAPI\\Data\\tags.txt", mode: FileMode.Open));

        List<Tag> tags = [];

        while (!file.EndOfStream)
        {
            var line = await file.ReadLineAsync()!;
            var tokens = line?.Split(',');

            var name = tokens![0]!;
            var des = string.Join(',', tokens[1..]!);

            var tag = new Tag
            {
                Name = name,
                Description = des
            };

            tags.Add(tag);
        }
        tagRepository.CreateTags(tags);

        var a = await tagRepository.SaveChangesAsync();

        //var user = new AppUser()
        //{
        //    Email = "admin@email.com",
        //    UserName = "admin@email.com",
        //    Reputation = 99999999,
        //    ProfilePicture = "avc"
        //};
        //await userManager.CreateAsync(user, "0123456789");
        //await userManager.AddClaimsAsync(user,
        //[
        //    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        //    new Claim(ClaimTypes.Role, Constants.Roles.Admin),
        //]);
        //await userManager.AddToRoleAsync(user, Constants.Roles.Admin);
    }
}


