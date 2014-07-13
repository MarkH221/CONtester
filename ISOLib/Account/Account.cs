using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using Isolib.Encryptions;

namespace Isolib.Account
{
    /// <summary>
    /// Account Encryption Class
    /// </summary>
    public class Encryption
    {
        private readonly byte[] RetailKey = new byte[] {
	0xe1,
	0xbc,
	0x15,
	0x9c,
	0x73,
	0xb1,
	0xea,
	0xe9,
	0xab,
	0x31,
	0x70,
	0xf3,
	0xad,
	0x47,
	0xeb,
	0xf3
};
        private readonly byte[] DevelopmentKey = new byte[] {
	0xda,
	0xb6,
	0x9a,
	0xd9,
	0x8e,
	40,
	0x76,
	0x4f,
	0x97,
	0x7e,
	0xe2,
	0x48,
	0x7e,
	0x4f,
	0x3f,
	0x68

};
        /// <summary>
        /// Console Type
        /// </summary>
       public enum ConsoleType
        {
           /// <summary>
           /// Retail Console
           /// </summary>
            Retail,
           /// <summary>
           /// Devkit Console
           /// </summary>
            Devkit
        }

        private byte[] Getkey(ConsoleType key)
        {
            switch (key)
            {
                case ConsoleType.Retail:
                    return RetailKey;
                case ConsoleType.Devkit:
                    return DevelopmentKey;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Decrypts Account File
        /// </summary>
        /// <param name="Account">Account Array</param>
        /// <param name="ConsoleType">The console Type, retail or devkit</param>
        /// <returns>Decrypted Data</returns>
        public byte[] DecryptAccount(  byte[] Account,  ConsoleType ConsoleType)
        {
            if (Account == null || Account.Length != 404)
                throw new Exception("Invalid account data");
            RC4 RC4 = new RC4();
            HMACSHA1 HMAC = new HMACSHA1(Getkey(ConsoleType));
            bool Success = false;
            //Initalize HMAC w/ retail or development key
            byte[] Seed = new byte[0];
            byte[] Key = new byte[0];
            byte[] Buffer = null;
            byte[] Hash = null;
            Array.Resize(ref Seed, 16);
            Array.Resize(ref Key, 16);
            Array.Copy(Account, 0, Seed, 0, 16);
            //Copy first 16 bytes of account used for seeding the crypto key
            Buffer = HMAC.ComputeHash(Seed, 0, 16);
            //Hash the seed for crypto key
            Array.Copy(Buffer, 0, Key, 0, 16);
            //Use only the first 16 bytes of the hash for the key
            Array.Resize<byte>(ref Buffer, 388);
            //Size buffer to hold data to decrypt
            Array.Copy(Account, 16, Buffer, 0, 388);
            //Copy data to decrypt into buffer
            RC4.Key = Key;
            //Set generated key for use with RC4 crypto
            RC4.TransformBlock(ref Buffer, 0, 388);
            //Decrypt data in buffer
            Hash = HMAC.ComputeHash(Buffer, 0, 388);
            //Compute hash over decrypted data
            Success = CompareBytes(ref Seed, ref Hash, 16);
            //Compare seed to computed hash, decrypt success if first 16 bytes match
            RC4.Dispose();
            HMAC.Clear();
            Seed = null;
            Key = null;
            if (!Success)
            {
                Buffer = null;
                throw new CryptographicException("Decryption failed!");
            }
            return Buffer;
        }

        /// <summary>
        /// Encrypt Account Data
        /// </summary>
        /// <param name="Account">Account Array</param>
        /// <param name="ConsoleType">The console Type, retail or devkit</param>
        /// <returns>Encrypted Data</returns>
        public byte[] EncryptAccount(byte[] Account, ConsoleType ConsoleType)
        {
            if (Account == null || Account.Length != 388)
                throw new Exception("Invalid account data");
            RC4 RC4 = new RC4();
            HMACSHA1 HMAC = new HMACSHA1(Getkey(ConsoleType));
            //Initalize HMAC w/ retail or development key
            byte[] Seed = new byte[0];
            byte[] Key = new byte[0];
            byte[] Hash = null;
            List<byte> Buffer = new List<byte>();
            Hash = HMAC.ComputeHash(Account, 0, 388);
            //Compute HMAC over account data for key seed
            Array.Copy(Hash, 0, Seed, 0, 16);
            //Copy first 16 bytes of hash of account data to use for key seed
            Hash = HMAC.ComputeHash(Seed, 0, 16);
            //Hash the seed for crypto key
            Array.Copy(Hash, 0, Key, 0, 16);
            //Use only the first 16 bytes of the hash for the key
            RC4.Key = Key;
            //Set generated key for use with RC4 crypto
            RC4.TransformBlock(ref Account, 0, 388);
            //Encrypt account data
            Buffer.AddRange(Seed);
            //Copy seed to buffer
            Buffer.AddRange(Account);
            //Copy encrypted data to buffer
            RC4.Dispose();
            HMAC.Clear();
            Seed = null;
            Key = null;
            return Buffer.ToArray();
        }

        private bool CompareBytes(ref byte[] x, ref byte[] y, Int32 Count)
        {
            for (Int32 i = 0; i <= Count - 1; i++)
            {
                if (x[i] != y[i])
                    return false;
            }
            return true;
        }

    }
}
