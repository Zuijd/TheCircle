using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

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
            // Read the content of private.key
            string privateKeyPath = "./../Portal/Certificates/private.key";
            string privateKeyPEM = File.ReadAllText(privateKeyPath);

            // Load the private key
            RSA privateKeyRSA = RSA.Create();

            // Import the private key from the PEM-encoded string
            privateKeyRSA.ImportFromPem(privateKeyPEM);

            // Convert the private key to PKCS#1 format and return it
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
        
        public byte[] CreateDigSig(object content, byte[] privateKey)
        {
            //create new rsa crypto service provider
            //used to create new keypairs, encryption and decryption
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            //import the public key
            rsa.ImportRSAPrivateKey(privateKey, out int bytesRead);

            //convert the plaintext object to a byte[]
            byte[] data = ConvertObjectToByteArray(content);

            //using the crypto service provider; sign the digsig
            return rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

        public bool VerifyDigSig(object content, byte[] signature, byte[] publicKey)
        {
            //create new rsa crypto service provider
            //used to create new keypairs, encryption and decryption
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            //import the public key
            rsa.ImportRSAPublicKey(publicKey, out int bytesRead);

            //convert the plaintext message to a byte[]
            byte[] messageBytes = ConvertObjectToByteArray(content);

            //verify the integrity of the message by hashing the plaintext message and comparing it with the hashed message in the signature
            return rsa.VerifyData(messageBytes, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

        //////////////////  -  DUMMY FUNCTION  -  \\\\\\\\\\\\\\\\\\\\

        public PKC CreatePost(object content, byte[] signature, byte[] certificate)
        {
            var publicKey = GetPublicKeyOutOfCertificate(certificate);

            //verify digital signature
            var isValid = VerifyDigSig(content, signature, publicKey);

            //verification is succesful ? perform action : throw corresponding error
            Console.WriteLine(isValid ? "CLIENT PACKET IS VALID" : "CLIENT PACKET IS INVALID");

            return new PKC()
            {
                Message = content,
                Signature = CreateDigSig(content, GetPrivateKeyFromServer()),
                Certificate = GetCertificateFromServer(),
            };
        }

        //////////////////  -  HELPER FUNCTION  -  \\\\\\\\\\\\\\\\\\\\

        // Serialize object to byte array
        public byte[] ConvertObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;

            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            using (MemoryStream memoryStream = new MemoryStream())
            {
                serializer.Serialize(memoryStream, obj);
                return memoryStream.ToArray();
            }
        }
    }
}
