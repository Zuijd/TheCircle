namespace Domain
{
    public class PKC
    {
        [Required]
        public object? Message { get; set; }
        [Required]
        public byte[]? Signature { get; set; }
        [Required]
        public byte[]? Certificate { get; set; }
    }
}
