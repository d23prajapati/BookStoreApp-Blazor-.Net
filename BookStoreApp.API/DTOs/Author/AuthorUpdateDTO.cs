using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.API.DTOs.Author {
   public class AuthorUpdateDTO : BaseDTO {
      [Required]
      [MaxLength(50)]
      public string FirstName { get; set; }
      [Required]
      [MaxLength(50)]
      public string Lastname { get; set; }
      [MaxLength(250)]
      public string Bio { get; set; }
   }
}
