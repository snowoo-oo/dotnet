using System.ComponentModel.DataAnnotations;

namespace login.Models.DTOs
{
    public class UserRegisterationRequestDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
