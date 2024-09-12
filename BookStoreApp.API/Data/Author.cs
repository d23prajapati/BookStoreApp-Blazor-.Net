using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.API.Data;

public partial class Author {
   public int Id { get; set; }

   public string? FirstName { get; set; }

   public string? LastName { get; set; }

   public string? Bio { get; set; }

   public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}


public static class AuthorEndpoints {
   public static void MapAuthorEndpoints(this IEndpointRouteBuilder routes) {
      var group = routes.MapGroup("/api/Author").WithTags(nameof(Author));

      group.MapGet("/", async (BookStoreDbContext db) => {
         return await db.Authors.ToListAsync();
      })
      .WithName("GetAllAuthors")
      .WithOpenApi();

      group.MapGet("/{id}", async Task<Results<Ok<Author>, NotFound>> (int id, BookStoreDbContext db) => {
         return await db.Authors.AsNoTracking()
             .FirstOrDefaultAsync(model => model.Id == id)
             is Author model
                 ? TypedResults.Ok(model)
                 : TypedResults.NotFound();
      })
      .WithName("GetAuthorById")
      .WithOpenApi();

      group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, Author author, BookStoreDbContext db) => {
         var affected = await db.Authors
             .Where(model => model.Id == id)
             .ExecuteUpdateAsync(setters => setters
               .SetProperty(m => m.Id, author.Id)
               .SetProperty(m => m.FirstName, author.FirstName)
               .SetProperty(m => m.LastName, author.LastName)
               .SetProperty(m => m.Bio, author.Bio)
               );
         return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
      })
      .WithName("UpdateAuthor")
      .WithOpenApi();

      group.MapPost("/", async (Author author, BookStoreDbContext db) => {
         db.Authors.Add(author);
         await db.SaveChangesAsync();
         return TypedResults.Created($"/api/Author/{author.Id}", author);
      })
      .WithName("CreateAuthor")
      .WithOpenApi();

      group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, BookStoreDbContext db) => {
         var affected = await db.Authors
             .Where(model => model.Id == id)
             .ExecuteDeleteAsync();
         return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
      })
      .WithName("DeleteAuthor")
      .WithOpenApi();
   }
}