using System.ComponentModel.DataAnnotations;

namespace ElectronChatAPI.Models
{
    public class ChannelDto
    {
        [Required(ErrorMessage = "ChannelName is required.")]
        [MinLength(3, ErrorMessage = "ChannelName should be minimum 3 characters.")]
        [MaxLength(20, ErrorMessage = "ChannelName should not exceed 20 characters.")]
        public string ChannelName { get; set; }
    }
}
