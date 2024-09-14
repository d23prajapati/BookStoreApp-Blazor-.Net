using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.API.DTOs.User {
   public class UserDTO : LoginUserDTO {
      [Required]
      [MaxLength(50)]
      public string FirstName { get; set; }
      [Required]
      [MaxLength(50)]   
      public string LastName { get; set; }
      [Required]
      public string Role { get; set; }
   }
}