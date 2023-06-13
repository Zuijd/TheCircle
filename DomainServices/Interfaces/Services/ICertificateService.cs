namespace DomainServices.Interfaces.Services
{
    public interface ICertificateService
    {
        RSA CreateKeyPair();
        byte[] getPrivateKey(RSA rsa);
        byte[] getPublicKeyOutOfUserCertificate(byte[] certificateBytes);
        byte[] getPublicKeyOutOfServerCertificate();
        byte[] CreateCertificate(string username, string email, RSA rsa);
        void VerifyDigSig(string message, byte[] signature, byte[] publicKey);
        byte[] CreateDigSig(string message, byte[] privateKey);
    }
}
