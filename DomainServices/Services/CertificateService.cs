namespace DomainServices.Services
{
    public class CertificateService : ICertificateService
    {
        public CertificateService() { }

        public RSA CreateKeyPair()
        {
            //return a RSA encryption provider with 2048-bit key size
            return new RSACryptoServiceProvider(2048);
        }

        public byte[] getPrivateKey(RSA rsa)
        {
            //get private key
            return rsa.ExportRSAPrivateKey();
        }

        public byte[] getPublicKeyOutOfUserCertificate(byte[] certificateBytes)
        {
            //get certificate out of bytes
            X509Certificate2 certificate = new X509Certificate2(certificateBytes);

            //extract public key from certificate
            return certificate.GetPublicKey();
        }

        public byte[] getPublicKeyOutOfServerCertificate()
        {
            //get certificate from server
            X509Certificate2 certificate = new X509Certificate2("./../Portal/Certificates/certificate.crt");

            //extract public key from certificate
            return certificate.GetPublicKey();
        }

        public byte[] CreateCertificate(string username, string email, RSA rsa)
        {
            //create certificate request with unique user attributes 
            var certificateRequest = new CertificateRequest($"CN={username}, E={email}", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            //sign the certificate and add the expiration date
            var certificate = certificateRequest.CreateSelfSigned(DateTime.Now, DateTime.Now.AddYears(2));

            //convert the certificate to a byte array in the X509ContentType.Cert format
            return certificate.Export(X509ContentType.Cert);
        }
        
        public byte[] CreateDigSig(string message, byte[] privateKey)
        {
            //create new rsa crypto service provider
            //used to create new keypairs, encryption and decryption
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            //import the public key
            rsa.ImportRSAPrivateKey(privateKey, out int bytesRead);

            //convert the plaintext message to a byte[]
            byte[] data = Encoding.UTF8.GetBytes(message);

            //using the crypto service provider; sign the digsig
            return rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

        public void VerifyDigSig(string message, byte[] signature, byte[] publicKey)
        {
            //create new rsa crypto service provider
            //used to create new keypairs, encryption and decryption
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            //import the public key
            rsa.ImportRSAPublicKey(publicKey, out int bytesRead);

            //convert the plaintext message to a byte[]
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            //verify the integrity of the message by hashing the plaintext message and comparing it with the hashed message in the signature
            bool isValid = rsa.VerifyData(messageBytes, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            Console.WriteLine(isValid);

            //throw exc when given bool is false (when digsig is invalid)
            if (!isValid)
            {
                throw new KeyException("FalseMessage", $"{message} is a false message!");
            }
        }
    }
}
