namespace DomainServices.Interfaces.Services
{
    public interface ICertificateService
    {
        RSA CreateKeyPair();
        byte[] GetPrivateKey(RSA rsa);
        byte[] GetPrivateKeyFromServer();
        byte[] GetPublicKeyOutOfCertificate(byte[] certificateBytes);
        byte[] GetCertificateFromServer();
        byte[] CreateCertificate(string username, string email, RSA rsa);
        bool VerifyDigSig(string message, byte[] signature, byte[] publicKey);
        byte[] CreateDigSig(string message, byte[] privateKey);
        PKC CreatePost(string message, byte[] signature, byte[] certificate);
    }
}
