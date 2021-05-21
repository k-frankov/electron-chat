using System.ComponentModel.DataAnnotations;

namespace ElectronChatAPI.Models
{
    public class UserDto
    {
        [Required(ErrorMessage = "UserName is required.")]
        [MinLength(3, ErrorMessage = "UserName should be minimum 3 characters.")]
        [MaxLength(20, ErrorMessage = "UserName should not exceed 20 characters.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(3, ErrorMessage = "Password should be minimum 3 characters.")]
        [MaxLength(20, ErrorMessage = "Password should not exceed 20 characters.")]
        public string Password { get; set; }

        public string JwtToken { get; set; }
    }
}
