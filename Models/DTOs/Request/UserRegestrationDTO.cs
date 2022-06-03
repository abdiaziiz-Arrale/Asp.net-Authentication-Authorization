using System.ComponentModel.DataAnnotations;

namespace TodoApp.Models.DTOs.Request
{
      public class UserRegestrationDTO
      {
            [Required]
            public string Username { get; set; }
            [Required]
            [EmailAddress]
            public string Email { get; set; }
            [Required]
            public string password { get; set; }
      }
}