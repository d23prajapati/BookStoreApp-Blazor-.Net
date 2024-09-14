using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.API.Data;

public partial class BookStoreDbContext : IdentityDbContext<ApiUser> {
   public BookStoreDbContext() {
   }

   public BookStoreDbContext(DbContextOptions<BookStoreDbContext> options)
       : base(options) {
   }

   public virtual DbSet<Author> Authors { get; set; }

   public virtual DbSet<Book> Books { get; set; }

   protected override void OnModelCreating(ModelBuilder modelBuilder) {

      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Author>(entity => {
         entity.HasKey(e => e.Id).HasName("PK__Authors__3214EC0794601CF3");

         entity.Property(e => e.Bio)
             .HasMaxLength(250)
             .IsFixedLength();
         entity.Property(e => e.FirstName)
             .HasMaxLength(50)
             .IsFixedLength();
         entity.Property(e => e.LastName)
             .HasMaxLength(50)
             .IsFixedLength();
      });

      modelBuilder.Entity<Book>(entity => {
         entity.HasKey(e => e.Id).HasName("PK__Books__3214EC074BEF5E57");

         entity.HasIndex(e => e.Isbn, "UQ__Books__447D36EA16A4243D").IsUnique();

         entity.Property(e => e.Image).HasMaxLength(50);
         entity.Property(e => e.Isbn)
             .HasMaxLength(50)
             .HasColumnName("ISBN");
         entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
         entity.Property(e => e.Summary).HasMaxLength(250);
         entity.Property(e => e.Title)
             .HasMaxLength(50)
             .IsFixedLength();

         entity.HasOne(d => d.Author).WithMany(p => p.Books)
             .HasForeignKey(d => d.AuthorId)
             .HasConstraintName("FK_Books_Authors");
      });

      modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole {
               Id = "eed7a290-74f9-40d0-b5c7-501c84c0c064",
               Name = "Admin",
               NormalizedName = "ADMIN"
            },
            new IdentityRole {
               Id = "f501824d-a20a-4524-ae40-3450fdaa3f2a",
               Name = "User",
               NormalizedName = "USER"
            }
         );

      var hasher = new PasswordHasher<ApiUser>();

      modelBuilder.Entity<ApiUser>().HasData(
            new ApiUser {
               Id = "789b5b6d-e101-4b83-8af0-3e57cc91f373",
               Email = "admin@bookstore.com",
               NormalizedEmail = "ADMIN@BOOKSTORE.COM",
               UserName = "admin@bookstore.com",
               NormalizedUserName = "ADMIN@BOOKSTORE.COM",
               PasswordHash = hasher.HashPassword(null, "Password1!"),
               FirstName = "System",
               LastName = "Admin"
            },
            new ApiUser {
               Id = "88cb08cd-bdb1-4795-9759-8de1471edee9",
               Email = "user@bookstore.com",
               NormalizedEmail = "USER@BOOKSTORE.COM",
               UserName = "user@bookstore.com",
               NormalizedUserName = "USER@BOOKSTORE.COM",
               PasswordHash = hasher.HashPassword(null, "Password1!"),
               FirstName = "System",
               LastName = "User",
            }
         );

      modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string> {
               RoleId = "eed7a290-74f9-40d0-b5c7-501c84c0c064",
               UserId = "789b5b6d-e101-4b83-8af0-3e57cc91f373"
            },
            new IdentityUserRole<string> {
               RoleId = "f501824d-a20a-4524-ae40-3450fdaa3f2a",
               UserId = "88cb08cd-bdb1-4795-9759-8de1471edee9"
            }
         );

      OnModelCreatingPartial(modelBuilder);
   }

   partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
