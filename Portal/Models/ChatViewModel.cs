namespace Portal.Models
{
    public class ChatViewModel
    {

        [Required(ErrorMessage = "Message must be entered")]
        public string? Message { get; set; }

    }
}
