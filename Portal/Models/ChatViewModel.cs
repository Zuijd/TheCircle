namespace Portal.Models
{
    public class ChatViewModel
    {
        [Required(ErrorMessage = "Username is required!")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Message must be entered")]
        public string? Message { get; set; }

    }
}
