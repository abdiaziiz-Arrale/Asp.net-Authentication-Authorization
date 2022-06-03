using System.ComponentModel.DataAnnotations;

namespace TodoApp.Models.DTOs.Request
{
      public class UserLoginDTO
      {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
            [Required]
            public string password { get; set; }
      }
}