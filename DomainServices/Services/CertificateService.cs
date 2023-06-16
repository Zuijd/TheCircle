namespace DomainServices.Services
{
    public class CertificateService : ICertificateService
    {
        public CertificateService() { }

        ////////////////////  -  AUTHENTICATION  -  \\\\\\\\\\\\\\\\\\\\

        public RSA CreateKeyPair()
        {
            //return a RSA encryption provider with 2048-bit key size
            return new RSACryptoServiceProvider(2048);
        }

        public byte[] GetPrivateKey(RSA rsa)
        {
            //get private key
            return rsa.ExportRSAPrivateKey();
        }

        public byte[] GetPrivateKeyFromServer() 
        {
            //read the content of private.key
            string privateKeyPath = "./../Portal/Certificates/private.key";
            string privateKeyPEM = File.ReadAllText(privateKeyPath);

            //remove unnecessery headers
            privateKeyPEM = privateKeyPEM.Replace("-----BEGIN PRIVATE KEY-----", "")
                                       .Replace("-----END PRIVATE KEY-----", "")
                                       .Replace("\n", "")
                                       .Replace("\r", "");

            //convert the PEM string to a byte array
            byte[] privateKeyDER = Convert.FromBase64String(privateKeyPEM);

            //load the private key
            RSA privateKeyRSA = RSA.Create();
            privateKeyRSA.ImportPkcs8PrivateKey(privateKeyDER, out int bytesRead);

            //convert the private key to a PKCS#1-format and return it
            return privateKeyRSA.ExportRSAPrivateKey();
        }

        public byte[] GetCertificateFromServer()
        {
            //get certificate from server
            X509Certificate2 certificate = new X509Certificate2("./../Portal/Certificates/certificate.crt");

            //return certificate exported to byte[]
            return certificate.Export(X509ContentType.Cert);
        }

        public byte[] GetPublicKeyOutOfCertificate(byte[] certificateBytes)
        {
            //get certificate out of bytes
            X509Certificate2 certificate = new X509Certificate2(certificateBytes);

            //extract public key from certificate
            return certificate.GetPublicKey();
        }

        //////////////////  -  VALIDATION  -  \\\\\\\\\\\\\\\\\\\\
        
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

        public bool VerifyDigSig(string message, byte[] signature, byte[] publicKey)
        {
            //create new rsa crypto service provider
            //used to create new keypairs, encryption and decryption
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            //import the public key
            rsa.ImportRSAPublicKey(publicKey, out int bytesRead);

            //convert the plaintext message to a byte[]
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            //verify the integrity of the message by hashing the plaintext message and comparing it with the hashed message in the signature
            return rsa.VerifyData(messageBytes, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

        //////////////////  -  DUMMY FUNCTION  -  \\\\\\\\\\\\\\\\\\\\

        public PKC CreatePost(string message, byte[] signature, byte[] certificate)
        {
            var publicKey = GetPublicKeyOutOfCertificate(certificate);

            //verify digital signature
            var isValid = VerifyDigSig(message, signature, publicKey);

            //verification is succesful ? perform action : throw corresponding error
            Console.WriteLine(isValid ? "CLIENT PACKET IS VALID" : "CLIENT PACKET IS INVALID");

            return new PKC()
            {
                Message = message,
                Signature = CreateDigSig(message, GetPrivateKeyFromServer()),
                Certificate = GetCertificateFromServer(),
            };
        }
    }
}
