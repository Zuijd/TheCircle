namespace Domain
{
    public class UserIdentity : IdentityUser
    {
        [Required]
        public byte[]? Certificate { get; set; }
        [Required]
        public byte[]? PrivateKey { get; set; }
    }
}
