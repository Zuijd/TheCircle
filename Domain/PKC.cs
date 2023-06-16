namespace Domain
{
    public class PKC
    {
        [Required]
        public string? Message { get; set; }
        [Required]
        public byte[]? Signature { get; set; }
        [Required]
        public byte[]? Certificate { get; set; }
    }
}
