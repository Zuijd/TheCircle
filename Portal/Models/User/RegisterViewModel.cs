namespace Portal.Models.User
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Username is required!")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Email address is required!")]
        public string? EmailAddress { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        public string? Password { get; set; }
    }
}
