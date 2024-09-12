namespace BookStoreApp.API.DTOs.Author {
   public class AuthorReadOnlyDTO : BaseDTO {
      public string FirstName { get; set; }
      public string Lastname { get; set; }
      public string Bio { get; set; }
   }
}