using System;
using System.Security.Cryptography;

namespace Isolib.Encryptions
{
    /// <summary>Contains function/s that deals with the generation RSA Parameter data</summary>
    public class Rsa
    {
        /// <summary>Generates RSAParameters keys</summary>
        public static RSAParameters CreateKeys()
        {
            var parameters = new CspParameters();
            var provider = new RSACryptoServiceProvider(parameters);
            return provider.ExportParameters(true);
        }

        /// <summary>Generate a PKS1 Signature of SHA1 digest</summary>
        /// <param name="param">The RSA parameters keys</param>
        /// <param name="hash"> The SHA1 digest</param>
        public static byte[] GeneratePks1Signature(RSAParameters param, byte[] hash)
        {
            byte[] buffer;
            var key = new RSACryptoServiceProvider();
            try
            {
                key.ImportParameters(param);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
            var formatter = new RSAPKCS1SignatureFormatter();
            formatter.SetHashAlgorithm("SHA1");
            formatter.SetKey(key);
            try
            {
                buffer = formatter.CreateSignature(hash);
            }
            catch
            {
                throw new Exception("Error when taking signature");
            }
            return buffer;
        }

        /// <summary>Verify a PKS1 signature of SHA1 digest</summary>
        /// <param name="param">The RSA parameters keys</param>
        /// <param name="hash">SHA1 hash</param>
        /// <param name="signature">Outputs the signature</param>
        public static bool SignatureVerify(RSAParameters param, byte[] hash, byte[] signature)
        {
            var key = new RSACryptoServiceProvider();
            var deformatter = new RSAPKCS1SignatureDeformatter();
            try
            {
                key.ImportParameters(param);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
            deformatter.SetHashAlgorithm("SHA1");
            deformatter.SetKey(key);
            try
            {
                return deformatter.VerifySignature(hash, signature);
            }
            catch
            {
                throw new Exception("Unknown error when trying to verify signature");
            }
        }
    }
}

