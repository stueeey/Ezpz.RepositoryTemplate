# Task 001: Set up Database and Migrations

[← Back to Task Summary](./task-summary.md)

**Size**: Medium (3 files) - 🎯 Low Risk, 💪 Easy Effort  
**Dependencies**: None (can start immediately)

## Objective

Create the database context, entity model, and initial migration for the URL shortener service using SQLite.

## Implementation Steps

1. Create `Data/UrlShortenerDbContext.cs` in the App project
   ```csharp
   using Microsoft.EntityFrameworkCore;
   
   namespace Company.Platform.UrlShortener.App.Data;
   
   public class UrlShortenerDbContext : DbContext
   {
       public UrlShortenerDbContext(DbContextOptions<UrlShortenerDbContext> options) 
           : base(options) { }
       
       public DbSet<ShortUrl> ShortUrls => Set<ShortUrl>();
       
       protected override void OnModelCreating(ModelBuilder modelBuilder)
       {
           modelBuilder.Entity<ShortUrl>(entity =>
           {
               entity.HasKey(e => e.Id);
               entity.HasIndex(e => e.ShortCode).IsUnique();
               entity.Property(e => e.ShortCode).HasMaxLength(10).IsRequired();
               entity.Property(e => e.OriginalUrl).HasMaxLength(2048).IsRequired();
               entity.Property(e => e.CreatedAt).IsRequired();
           });
       }
   }
   ```

2. Create `Data/Entities/ShortUrl.cs`
   ```csharp
   namespace Company.Platform.UrlShortener.App.Data.Entities;
   
   public class ShortUrl
   {
       public int Id { get; set; }
       public required string ShortCode { get; set; }
       public required string OriginalUrl { get; set; }
       public int ClickCount { get; set; }
       public DateTime CreatedAt { get; set; }
       public DateTime? ExpiresAt { get; set; }
       public DateTime? LastClickedAt { get; set; }
   }
   ```

3. Update `Program.cs` to register DbContext
   ```csharp
   builder.Services.AddDbContext<UrlShortenerDbContext>(options =>
       options.UseSqlite(builder.Configuration.GetConnectionString("UrlShortener") 
           ?? "Data Source=urlshortener.db"));
   ```

4. Update `appsettings.json`
   ```json
   {
     "ConnectionStrings": {
       "UrlShortener": "Data Source=urlshortener.db"
     }
   }
   ```

5. Create initial migration
   ```bash
   cd src/services/UrlShortener/Company.Platform.UrlShortener.App
   dotnet ef migrations add InitialCreate --project ../Company.Platform.UrlShortener.App
   ```

6. Update Migrator.App/Program.cs to use the shared DbContext from App project

## Validation Criteria

- [ ] DbContext properly configured with SQLite
- [ ] ShortUrl entity has all required properties
- [ ] Unique index on ShortCode for fast lookups
- [ ] Migration created successfully
- [ ] Migrator.App can apply migrations
- [ ] Connection string configurable via appsettings.json

## Notes

- Use SQLite for simplicity and portability
- ShortCode column should have unique index for performance
- Consider adding composite index on (ShortCode, ExpiresAt) if expiration queries become common
- Maximum URL length of 2048 characters is standard for most browsers