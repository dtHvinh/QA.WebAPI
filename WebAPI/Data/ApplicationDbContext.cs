﻿#nullable disable

using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualBasic.FileIO;
using System.Security.Claims;
using System.Text.RegularExpressions;
using WebAPI.Data.Configurations;
using WebAPI.Model;
using WebAPI.Utilities;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Services;

namespace WebAPI.Data;

public partial class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>(options)
{
    private const string _fullTextDefaultCatalogName = "qa_default_catalog";

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<ApplicationUser>().Ignore(e => e.ConcurrencyStamp)
            .Ignore(e => e.SecurityStamp)
            .Ignore(e => e.LockoutEnabled)
            .Ignore(e => e.LockoutEnd)
            .Ignore(e => e.TwoFactorEnabled)
            .Ignore(e => e.PhoneNumberConfirmed)
            .Ignore(e => e.EmailConfirmed)
            .Ignore(e => e.AccessFailedCount);

        builder.Ignore<IdentityUserLogin<int>>();

        builder.Ignore<IdentityUserToken<int>>();
        builder.Ignore<IdentityUserRole<int>>();
        builder.Ignore<IdentityUserLogin<int>>();
        builder.Ignore<IdentityRoleClaim<int>>();

        foreach (var entity in GetEntities())
        {
            builder.Entity(entity);
        }

        builder
            .Entity<IdentityRole<int>>()
            .HasData(
            new IdentityRole<int>()
            {
                Id = 1,
                ConcurrencyStamp = null,
                Name = Constants.Roles.User,
                NormalizedName = Constants.Roles.User.ToUpperInvariant()
            },
            new IdentityRole<int>()
            {
                Id = 2,
                ConcurrencyStamp = null,
                Name = Constants.Roles.Moderator,
                NormalizedName = Constants.Roles.Moderator.ToUpperInvariant()
            },
             new IdentityRole<int>()
             {
                 Id = 3,
                 ConcurrencyStamp = null,
                 Name = Constants.Roles.Admin,
                 NormalizedName = Constants.Roles.Admin.ToUpperInvariant()
             });

        builder
            .Entity<Vote>().HasDiscriminator(e => e.VoteType)
            .HasValue<QuestionVote>(nameof(VoteTypes.Question))
            .HasValue<AnswerVote>(nameof(VoteTypes.Answer));

        builder
            .Entity<Comment>().HasDiscriminator(e => e.CommentType)
            .HasValue<QuestionComment>(nameof(Question))
            .HasValue<AnswerComment>(nameof(Answer));

        builder.ApplyConfiguration(new QuestionConfig());
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
                || i == typeof(IKeylessEntity)
                || (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityWithTime<>))
                || (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntity<>))));
    }

    public static async Task InitTag(WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var tagRepository = scope.ServiceProvider.GetRequiredService<WebAPI.Repositories.Base.ITagRepository>();
        var tabBodyRepo = dbContext.Set<TagBody>();
        var tabDescRepo = dbContext.Set<TagDescription>();
        string csvFilePath = "D:\\dev\\myproject\\qa_platform\\back-end\\WebAPI\\WebAPI\\Data\\tags.csv";

        dbContext.Database.SetCommandTimeout(300);

        // Read the second line of the CSV file
        using TextFieldParser parser = new(csvFilePath);

        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(",");

        // Skip the header line
        if (!parser.EndOfData)
        {
            parser.ReadLine();
        }

        List<Tag> tags = [];
        List<TagBody> tagBodies = [];
        List<TagDescription> tagDescriptions = [];

        int i = 1;
        // Read the second line
        while (!parser.EndOfData)
        {
            string[] fields = parser.ReadFields();

            // Extract the three values
            string tagName = fields[0];
            string excerpt = fields[1];
            string wikiBody = fields[2];

            tags.Add(new()
            {
                Name = tagName,
                NormalizedName = tagName.ToUpperInvariant(),
            });

            tagBodies.Add(new()
            {
                Content = wikiBody,
                TagId = i,
            });

            tagDescriptions.Add(new()
            {
                Content = excerpt,
                TagId = i,
            });

            i++;
        }

        tabBodyRepo.AddRange(tagBodies);
        tagRepository.AddRange(tags);
        tabDescRepo.AddRange(tagDescriptions);

        var a = await tagRepository.SaveChangesAsync();
    }
    public static async Task Init(WebApplication app, bool run)
    {
        if (!run)
            return;

        using var scope = app.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
        await roleManager.CreateAsync(new IdentityRole<int>(Constants.Roles.Admin));
        await roleManager.CreateAsync(new IdentityRole<int>(Constants.Roles.User));

        var user = new ApplicationUser()
        {
            Email = "admin@email.com",
            UserName = "admin@email.com",
            Reputation = 99999999,
            ProfilePicture = "https://ui-avatars.com/api/?name=V"
        };
        await userManager.CreateAsync(user, "0123456789");
        await userManager.AddClaimsAsync(user,
        [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, Constants.Roles.Admin),
        ]);
        await userManager.AddToRoleAsync(user, Constants.Roles.Admin);

        var user1 = new ApplicationUser()
        {
            Email = "vinh01234_2@email.com",
            UserName = "vinh01234_2@email.com",
            Reputation = 99999999,
            ProfilePicture = "https://ui-avatars.com/api/?name=V2"
        };
        await userManager.CreateAsync(user1, "01234vinh");
        await userManager.AddClaimsAsync(user1,
        [
            new Claim(ClaimTypes.NameIdentifier, user1.Id.ToString()),
            new Claim(ClaimTypes.Role, Constants.Roles.Admin),
        ]);

        await userManager.AddToRoleAsync(user1, Constants.Roles.Admin);

        var user2 = new ApplicationUser()
        {
            Email = "vinh01234_1@email.com",
            UserName = "vinh01234_1@email.com",
            Reputation = 99999999,
            ProfilePicture = "https://ui-avatars.com/api/?name=V1"
        };
        await userManager.CreateAsync(user2, "01234vinh");
        await userManager.AddClaimsAsync(user2,
        [
            new Claim(ClaimTypes.NameIdentifier, user2.Id.ToString()),
            new Claim(ClaimTypes.Role, Constants.Roles.Admin),
        ]);
        await userManager.AddToRoleAsync(user2, Constants.Roles.Admin);
    }
    public static async Task InitUser(WebApplication app, bool run)
    {
        if (!run)
            return;

        using var scope = app.Services.CreateScope();
        string csvFilePath = "D:\\dev\\myproject\\qa_platform\\back-end\\WebAPI\\WebAPI\\Data\\users.csv";
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var userDb = dbContext.Set<ApplicationUser>();
        var claimsDb = dbContext.Set<IdentityUserClaim<int>>();

        dbContext.Database.SetCommandTimeout(300);

        // Read the second line of the CSV file
        using TextFieldParser parser = new(csvFilePath);

        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(",");

        // Skip the header line
        if (!parser.EndOfData)
        {
            parser.ReadLine();
        }

        List<ApplicationUser> users = [];
        List<IdentityUserClaim<int>> userClaims = [];
        int id = 1;
        while (!parser.EndOfData)
        {

            string[] fields = parser.ReadFields();

            // Extract the three values
            string userName = fields[0].Replace(' ', '_');

            var user = new ApplicationUser()
            {
                Email = $"{userName}@email.com",
                UserName = $"{userName}",
                Reputation = 99999999,
                ProfilePicture = $"https://ui-avatars.com/api/?name={userName[0]}"
            };

            users.Add(user);

            var claim1 = new IdentityUserClaim<int>()
            {
                UserId = id,
                ClaimType = ClaimTypes.NameIdentifier,
                ClaimValue = user.Id.ToString(),
            };

            var claim2 = new IdentityUserClaim<int>()
            {
                UserId = id,
                ClaimType = ClaimTypes.Role,
                ClaimValue = "User",
            };

            userClaims.Add(claim1);
            userClaims.Add(claim2);

            id++;
        }

        userDb.AddRange(users);

        var a = await dbContext.SaveChangesAsync();

        if (a != 0)
        {
            claimsDb.AddRange(userClaims);
            var b = await dbContext.SaveChangesAsync();
        }
    }
    public static async Task InitUserClaims(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var claimsDb = dbContext.Set<IdentityUserClaim<int>>();

        dbContext.Database.SetCommandTimeout(300);

        List<IdentityUserClaim<int>> userClaims = [];
        int id = 1;
        var x = new List<int>() { 7, 4, 1, 10621, 10620 };
        while (id <= 10625)
        {
            var claim1 = new IdentityUserClaim<int>()
            {
                UserId = id,
                ClaimType = ClaimTypes.NameIdentifier,
                ClaimValue = id.ToString(),
            };

            var claim2 = new IdentityUserClaim<int>()
            {
                UserId = id,
                ClaimType = ClaimTypes.Role,
                ClaimValue = x.Contains(id) ? "Moderator" : "User",
            };
            userClaims.Add(claim1);
            userClaims.Add(claim2);

            id++;
        }
        dbContext.AddRange(userClaims);
        var a = await dbContext.SaveChangesAsync();
    }
    public static async Task InitUserRole(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.UserRoles.AddRange(dbContext.Users.Select(e => new IdentityUserRole<int>
        {
            UserId = e.Id,
            RoleId = 2,
        }));

        var a = await dbContext.SaveChangesAsync();
    }
    public static async Task InitQuestion(WebApplication app, bool run)
    {
        if (!run)
            return;

        using var scope = app.Services.CreateScope();
        string csvFilePath = "D:\\dev\\myproject\\qa_platform\\back-end\\WebAPI\\WebAPI\\Data\\questions.csv";
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var questionDb = dbContext.Set<Question>();

        dbContext.Database.SetCommandTimeout(300);

        // Read the second line of the CSV file
        using TextFieldParser parser = new(csvFilePath);

        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(",");

        // Skip the header line
        if (!parser.EndOfData)
        {
            parser.ReadLine();
        }

        List<Question> q = [];

        while (!parser.EndOfData)
        {
            string[] fields = parser.ReadFields();

            // Extract the three values
            string title = fields[0];
            string body = fields[1];

            q.Add(new()
            {
                Title = title,
                Content = body,
                Slug = title.GenerateSlug(),
                AuthorId = Random.Shared.Next(1, 10620),
            });
        }

        questionDb.AddRange(q);

        var a = await dbContext.SaveChangesAsync();
    }
    public static async Task InitQuestionTag(WebApplication app, bool run)
    {
        if (!run)
            return;

        using var scope = app.Services.CreateScope();
        string csvFilePath = "D:\\dev\\myproject\\qa_platform\\back-end\\WebAPI\\WebAPI\\Data\\questionTags.csv";
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var tagDb = dbContext.Set<Tag>();
        var questionDb = dbContext.Set<Question>();

        dbContext.Database.SetCommandTimeout(300);

        // Read the second line of the CSV file
        using TextFieldParser parser = new(csvFilePath);

        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(",");

        // Skip the header line
        if (!parser.EndOfData)
        {
            parser.ReadLine();
        }

        int i = 0;

        List<Question> qs = await questionDb.ToListAsync();

        while (!parser.EndOfData)
        {
            string[] fields = parser.ReadFields();

            // Extract the three values
            string tagNames = fields[0];

            // Regex pattern to match text inside angle brackets.
            var matches = TagF().Matches(tagNames);

            // Extract the first capturing group (the text inside the brackets)
            var extracted = matches
                .Cast<Match>()
                .Select(match => match.Groups[1].Value);

            var tags = await tagDb.Where(e => extracted.Contains(e.Name)).ToListAsync();

            qs[i].Tags = tags;

            i++;
        }

        dbContext.UpdateRange(qs);

        var a = await dbContext.SaveChangesAsync();
    }
    public static async Task UpdateTagCount(WebApplication app, bool run)
    {
        if (!run)
            return;

        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var tagDb = dbContext.Set<Tag>();

        dbContext.Database.SetCommandTimeout(300);

        List<Tag> t = await tagDb.ToListAsync();

        foreach (var tag in t)
        {
            await dbContext.Entry(tag).Collection(e => e.Questions).LoadAsync();
            tag.QuestionCount = tag.Questions.Count;
        }

        dbContext.UpdateRange(t);

        var a = await dbContext.SaveChangesAsync();
    }
    public static async Task LoadElasticSearchData(WebApplication app, bool run)
    {
        if (!run)
            return;

        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var es = scope.ServiceProvider.GetRequiredService<QuestionSearchService>();
        var escs = scope.ServiceProvider.GetRequiredService<ElasticsearchClientSettings>();

        var q = dbContext.Set<Question>();

        var client = new ElasticsearchClient(escs);

        dbContext.Database.SetCommandTimeout(300);

        List<Question> t = await q.Include(e => e.Author).Include(e => e.Tags).ToListAsync();

        var res1 = await client.Indices.CreateAsync<Question>("user_questions", c =>
           {
               c.Mappings(m =>
               {
                   m.Properties(p =>
                   {
                       p.Nested(n => n.Tags);
                   });
               });
           });

        var res2 = await es.IndexOrUpdateManyAsync(t, QuestionSearchService.QuestionIndexName);
    }
    public static async Task ResetReputation(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var users = dbContext.Set<ApplicationUser>();
        dbContext.Database.SetCommandTimeout(300);
        List<ApplicationUser> t = await users.ToListAsync();
        foreach (var user in t)
        {
            user.Reputation = 0;
        }
        dbContext.UpdateRange(t);
        var a = await dbContext.SaveChangesAsync();
    }

    [GeneratedRegex("<([^>]+)>")]
    private static partial Regex TagF();
}

