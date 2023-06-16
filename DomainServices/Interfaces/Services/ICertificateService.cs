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
        bool VerifyDigSig(object content, byte[] signature, byte[] publicKey);
        byte[] CreateDigSig(object content, byte[] privateKey);
        PKC CreatePost(object content, byte[] signature, byte[] certificate);
    }
}
