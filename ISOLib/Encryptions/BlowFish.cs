//Check http://en.wikipedia.org/wiki/Blowfish_(cipher) For Better Understanding ;)
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
namespace Isolib.Encryptions
{
    using System;
    using System.Collections.Generic;
    using System.Text;
  

    class BlowFish
    {
        private RNGCryptoServiceProvider randomSource;
        private uint[] bf_s0;
        private uint[] bf_s1;
        private uint[] bf_s2;
        private uint[] bf_s3;
        private uint[] bf_P;
        private byte[] Xkey;
        private uint xl_par;
        private uint xr_par;
        private byte[] InitVector;
        private bool IVSet;

        private bool nonStandardMethod;
        /// <summary>
        /// Constructor for hex key
        /// </summary>
        /// <param name="hexKey">Cipher key as a hex string</param>
        public BlowFish(string hexKey)
        {
            randomSource = new RNGCryptoServiceProvider();
            SetupKey(HexToBytes(hexKey));
        }

        /// <summary>
        /// Constructor for byte key
        /// </summary>
        /// <param name="Key">Cipher key as a byte array</param>
        public BlowFish(byte[] Key)
        {
            randomSource = new RNGCryptoServiceProvider();
            SetupKey(Key);
        }

        /// <summary>
        /// Encrypts a string in CBC mode
        /// </summary>
        /// <param name="pt">Plaintext data to encrypt</param>
        /// <returns>Ciphertext with IV appended to front</returns>
        public string Encrypt_CBC(string pt)
        {
            if (!IVSet)
            {
                SetRandomIV();
            }
            return ByteToHex(InitVector) + ByteToHex(Encrypt_CBC(Encoding.ASCII.GetBytes(pt)));
        }

        /// <summary>
        /// Decrypts a string in CBC mode
        /// </summary>
        /// <param name="ct">Ciphertext with IV appended to front</param>
        /// <returns>Plaintext</returns>
        public string Decrypt_CBC(string ct)
        {
            IV = HexToBytes(ct.Substring(0, 16));
            return Encoding.ASCII.GetString(Decrypt_CBC(HexToBytes(ct.Substring(16))));
        }

        /// <summary>
        /// Decrypts a byte array in CBC mode.
        /// IV must be created and saved manually.
        /// </summary>
        /// <param name="ct">Ciphertext data to decrypt</param>
        /// <returns>Plaintext</returns>
        public byte[] Decrypt_CBC(byte[] ct)
        {
            return Crypt_CBC(ct, true);
        }

        /// <summary>
        /// Encrypts a byte array in CBC mode.
        /// IV must be created and saved manually.
        /// </summary>
        /// <param name="pt">Plaintext data to encrypt</param>
        /// <returns>Ciphertext</returns>
        public byte[] Encrypt_CBC(byte[] pt)
        {
            return Crypt_CBC(pt, false);
        }

        /// <summary>
        /// Encrypt a string in ECB mode
        /// </summary>
        /// <param name="pt">Plaintext to encrypt as ascii string</param>
        /// <returns>hex value of encrypted data</returns>
        public string Encrypt_ECB(string pt)
        {
            return ByteToHex(Encrypt_ECB(Encoding.ASCII.GetBytes(pt)));
        }

        /// <summary>
        /// Decrypts a string (ECB)
        /// </summary>
        /// <param name="ct">hHex string of the ciphertext</param>
        /// <returns>Plaintext ascii string</returns>
        public string Decrypt_ECB(string ct)
        {
            return Encoding.ASCII.GetString(Decrypt_ECB(HexToBytes(ct)));
        }

        /// <summary>
        /// Encrypts a byte array in ECB mode
        /// </summary>
        /// <param name="pt">Plaintext data</param>
        /// <returns>Ciphertext bytes</returns>
        public byte[] Encrypt_ECB(byte[] pt)
        {
            return Crypt_ECB(pt, false);
        }

        /// <summary>
        /// Decrypts a byte array (ECB)
        /// </summary>
        /// <param name="ct">Ciphertext byte array</param>
        /// <returns>Plaintext</returns>
        public byte[] Decrypt_ECB(byte[] ct)
        {
            return Crypt_ECB(ct, true);
        }

        /// <summary>
        /// Initialization vector for CBC mode.
        /// </summary>
        public byte[] IV
        {
            get { return InitVector; }
            set
            {
                if (value.Length == 8)
                {
                    InitVector = value;
                    IVSet = true;
                }
                else
                {
                    throw new Exception("Invalid IV size.");
                }
            }
        }

        public bool NonStandard
        {
            get { return nonStandardMethod; }
            set { nonStandardMethod = value; }
        }

        /// <summary>
        /// Creates and sets a random initialization vector.
        /// </summary>
        /// <returns>The random IV</returns>
        public byte[] SetRandomIV()
        {
            InitVector = new byte[8];
            randomSource.GetBytes(InitVector);
            IVSet = true;
            return InitVector;
        }

        #region Cryptography
        private void SetupKey(byte[] cipherKey)
        {
            bf_P = SetupP();
            bf_s0 = SetupS0();
            bf_s1 = SetupS1();
            bf_s2 = SetupS2();
            bf_s3 = SetupS3();

            Xkey = new byte[cipherKey.Length];
            if (cipherKey.Length > 56)
            {
                throw new Exception("Key too long. 56 bytes required.");
            }

            Buffer.BlockCopy(cipherKey, 0, Xkey, 0, cipherKey.Length);
            int j = 0;
            for (int i = 0; i <= 17; i++)
            {
                uint d = Convert.ToUInt32((((Xkey[j % cipherKey.Length]) * 256 + Xkey[(j + 1) % cipherKey.Length]) * 256 + Xkey[(j + 2) % cipherKey.Length]) * 256 + Xkey[(j + 3) % cipherKey.Length]);
                bf_P[i] = bf_P[i] ^ d;
                j = (j + 4) % cipherKey.Length;
            }

            xl_par = 0;
            xr_par = 0;
            for (int i = 0; i <= 17; i += 2)
            {
                encipher();
                bf_P[i] = xl_par;
                bf_P[i + 1] = xr_par;
            }

            for (int i = 0; i <= 255; i += 2)
            {
                encipher();
                bf_s0[i] = xl_par;
                bf_s0[i + 1] = xr_par;
            }
            for (int i = 0; i <= 255; i += 2)
            {
                encipher();
                bf_s1[i] = xl_par;
                bf_s1[i + 1] = xr_par;
            }
            for (int i = 0; i <= 255; i += 2)
            {
                encipher();
                bf_s2[i] = xl_par;
                bf_s2[i + 1] = xr_par;
            }
            for (int i = 0; i <= 255; i += 2)
            {
                encipher();
                bf_s3[i] = xl_par;
                bf_s3[i + 1] = xr_par;
            }
        }

        private byte[] Crypt_ECB(byte[] text, bool decrypt)
        {
            int paddedLen = (text.Length % 8 == 0 ? text.Length : text.Length + 8 - (text.Length % 8));
            byte[] plainText = new byte[paddedLen];
            Buffer.BlockCopy(text, 0, plainText, 0, text.Length);
            byte[] block = new byte[8];
            for (int i = 0; i <= plainText.Length - 1; i += 8)
            {
                Buffer.BlockCopy(plainText, i, block, 0, 8);
                if (decrypt)
                {
                    BlockDecrypt(ref block);
                }
                else
                {
                    BlockEncrypt(ref block);
                }
                Buffer.BlockCopy(block, 0, plainText, i, 8);
            }
            return plainText;
        }

        private byte[] Crypt_CBC(byte[] text, bool decrypt)
        {
            if (!IVSet)
            {
                throw new Exception("IV not set.");
            }
            int paddedLen = (text.Length % 8 == 0 ? text.Length : text.Length + 8 - (text.Length % 8));
            byte[] plainText = new byte[paddedLen];
            Buffer.BlockCopy(text, 0, plainText, 0, text.Length);
            byte[] block = new byte[8];
            byte[] preblock = new byte[8];
            byte[] iv = new byte[8];
            Buffer.BlockCopy(InitVector, 0, iv, 0, 8);
            if (!decrypt)
            {
                for (int i = 0; i <= plainText.Length - 1; i += 8)
                {
                    Buffer.BlockCopy(plainText, i, block, 0, 8);
                    XorBlock(ref block, iv);
                    BlockEncrypt(ref block);
                    Buffer.BlockCopy(block, 0, iv, 0, 8);
                    Buffer.BlockCopy(block, 0, plainText, i, 8);
                }
            }
            else
            {
                for (int i = 0; i <= plainText.Length - 1; i += 8)
                {
                    Buffer.BlockCopy(plainText, i, block, 0, 8);

                    Buffer.BlockCopy(block, 0, preblock, 0, 8);
                    BlockDecrypt(ref block);
                    XorBlock(ref block, iv);
                    Buffer.BlockCopy(preblock, 0, iv, 0, 8);

                    Buffer.BlockCopy(block, 0, plainText, i, 8);
                }
            }
            return plainText;
        }

        private void XorBlock(ref byte[] block,  byte[] iv)
        {
            for (int i = 0; i <= block.Length - 1; i++)
            {
                block[i] = Convert.ToByte(block[i] ^ iv[i]);
            }
        }

        private void BlockEncrypt(ref byte[] block)
        {
            SetBlock(block);
            encipher();
            GetBlock(ref block);
        }

        private void BlockDecrypt(ref byte[] block)
        {
            SetBlock(block);
            decipher();
            GetBlock(ref block);
        }

        private void SetBlock(byte[] block)
        {
            byte[] block1 = new byte[4];
            byte[] block2 = new byte[4];
            Buffer.BlockCopy(block, 0, block1, 0, 4);
            Buffer.BlockCopy(block, 4, block2, 0, 4);
            if (nonStandardMethod)
            {
                xr_par = BitConverter.ToUInt32(block1, 0);
                xl_par = BitConverter.ToUInt32(block2, 0);
            }
            else
            {
                Array.Reverse(block1);
                Array.Reverse(block2);
                xl_par = BitConverter.ToUInt32(block1, 0);
                xr_par = BitConverter.ToUInt32(block2, 0);
            }
        }

        private void GetBlock(ref byte[] block)
        {
            byte[] block1 = new byte[4];
            byte[] block2 = new byte[4];
            if (nonStandardMethod)
            {
                block1 = BitConverter.GetBytes(xr_par);
                block2 = BitConverter.GetBytes(xl_par);
            }
            else
            {
                block1 = BitConverter.GetBytes(xl_par);
                block2 = BitConverter.GetBytes(xr_par);
                Array.Reverse(block1);
                Array.Reverse(block2);
            }
            Buffer.BlockCopy(block1, 0, block, 0, 4);
            Buffer.BlockCopy(block2, 0, block, 4, 4);
        }

        private void encipher()
        {
            xl_par = xl_par ^ bf_P[0];
            for (int i = 0; i <= 15; i += 2)
            {
                xr_par = round(xr_par, xl_par, i + 1);
                xl_par = round(xl_par, xr_par, i + 2);
            }
            xr_par = xr_par ^ bf_P[17];
            uint swap = xl_par;
            xl_par = xr_par;
            xr_par = swap;
        }

        private void decipher()
        {
            xl_par = xl_par ^ bf_P[17];
            for (int i = 16; i >= 1; i += -2)
            {
                xr_par = round(xr_par, xl_par, i);
                xl_par = round(xl_par, xr_par, i - 1);
            }
            xr_par = xr_par ^ bf_P[0];
            uint swap = xl_par;
            xl_par = xr_par;
            xr_par = swap;
        }

        private uint round(uint a, uint B, int n)
        {
            uint x1 = (bf_s0[wordByte0(B)] + bf_s1[wordByte1(B)]) ^ bf_s2[wordByte2(B)];
            uint x2 = x1 + bf_s3[this.wordByte3(B)];
            uint x3 = x2 ^ bf_P[n];
            return x3 ^ a;
        }

        #endregion

        #region BLOCKS
        private uint[] SetupP()
        {
            return new uint[] {
			0x243f6a88,
			0x85a308d3u,
			0x13198a2e,
			0x3707344,
			0xa4093822u,
			0x299f31d0,
			0x82efa98,
			0xec4e6c89u,
			0x452821e6,
			0x38d01377,
			0xbe5466cfu,
			0x34e90c6c,
			0xc0ac29b7u,
			0xc97c50ddu,
			0x3f84d5b5,
			0xb5470917u,
			0x9216d5d9u,
			0x8979fb1bu
		};
        }

        private uint[] SetupS0()
        {
            return new uint[] {
			0xd1310ba6u,
			0x98dfb5acu,
			0x2ffd72db,
			0xd01adfb7u,
			0xb8e1afedu,
			0x6a267e96,
			0xba7c9045u,
			0xf12c7f99u,
			0x24a19947,
			0xb3916cf7u,
			0x801f2e2,
			0x858efc16u,
			0x636920d8,
			0x71574e69,
			0xa458fea3u,
			0xf4933d7eu,
			0xd95748f,
			0x728eb658,
			0x718bcd58,
			0x82154aeeu,
			0x7b54a41d,
			0xc25a59b5u,
			0x9c30d539u,
			0x2af26013,
			0xc5d1b023u,
			0x286085f0,
			0xca417918u,
			0xb8db38efu,
			0x8e79dcb0u,
			0x603a180e,
			0x6c9e0e8b,
			0xb01e8a3eu,
			0xd71577c1u,
			0xbd314b27u,
			0x78af2fda,
			0x55605c60,
			0xe65525f3u,
			0xaa55ab94u,
			0x57489862,
			0x63e81440,
			0x55ca396a,
			0x2aab10b6,
			0xb4cc5c34u,
			0x1141e8ce,
			0xa15486afu,
			0x7c72e993,
			0xb3ee1411u,
			0x636fbc2a,
			0x2ba9c55d,
			0x741831f6,
			0xce5c3e16u,
			0x9b87931eu,
			0xafd6ba33u,
			0x6c24cf5c,
			0x7a325381,
			0x28958677,
			0x3b8f4898,
			0x6b4bb9af,
			0xc4bfe81bu,
			0x66282193,
			0x61d809cc,
			0xfb21a991u,
			0x487cac60,
			0x5dec8032,
			0xef845d5du,
			0xe98575b1u,
			0xdc262302u,
			0xeb651b88u,
			0x23893e81,
			0xd396acc5u,
			0xf6d6ff3,
			0x83f44239u,
			0x2e0b4482,
			0xa4842004u,
			0x69c8f04a,
			0x9e1f9b5eu,
			0x21c66842,
			0xf6e96c9au,
			0x670c9c61,
			0xabd388f0u,
			0x6a51a0d2,
			0xd8542f68u,
			0x960fa728u,
			0xab5133a3u,
			0x6eef0b6c,
			0x137a3be4,
			0xba3bf050u,
			0x7efb2a98,
			0xa1f1651du,
			0x39af0176,
			0x66ca593e,
			0x82430e88u,
			0x8cee8619u,
			0x456f9fb4,
			0x7d84a5c3,
			0x3b8b5ebe,
			0xe06f75d8u,
			0x85c12073u,
			0x401a449f,
			0x56c16aa6,
			0x4ed3aa62,
			0x363f7706,
			0x1bfedf72,
			0x429b023d,
			0x37d0d724,
			0xd00a1248u,
			0xdb0fead3u,
			0x49f1c09b,
			0x75372c9,
			0x80991b7bu,
			0x25d479d8,
			0xf6e8def7u,
			0xe3fe501au,
			0xb6794c3bu,
			0x976ce0bdu,
			0x4c006ba,
			0xc1a94fb6u,
			0x409f60c4,
			0x5e5c9ec2,
			0x196a2463,
			0x68fb6faf,
			0x3e6c53b5,
			0x1339b2eb,
			0x3b52ec6f,
			0x6dfc511f,
			0x9b30952cu,
			0xcc814544u,
			0xaf5ebd09u,
			0xbee3d004u,
			0xde334afdu,
			0x660f2807,
			0x192e4bb3,
			0xc0cba857u,
			0x45c8740f,
			0xd20b5f39u,
			0xb9d3fbdbu,
			0x5579c0bd,
			0x1a60320a,
			0xd6a100c6u,
			0x402c7279,
			0x679f25fe,
			0xfb1fa3ccu,
			0x8ea5e9f8u,
			0xdb3222f8u,
			0x3c7516df,
			0xfd616b15u,
			0x2f501ec8,
			0xad0552abu,
			0x323db5fa,
			0xfd238760u,
			0x53317b48,
			0x3e00df82,
			0x9e5c57bbu,
			0xca6f8ca0u,
			0x1a87562e,
			0xdf1769dbu,
			0xd542a8f6u,
			0x287effc3,
			0xac6732c6u,
			0x8c4f5573u,
			0x695b27b0,
			0xbbca58c8u,
			0xe1ffa35du,
			0xb8f011a0u,
			0x10fa3d98,
			0xfd2183b8u,
			0x4afcb56c,
			0x2dd1d35b,
			0x9a53e479u,
			0xb6f84565u,
			0xd28e49bcu,
			0x4bfb9790,
			0xe1ddf2dau,
			0xa4cb7e33u,
			0x62fb1341,
			0xcee4c6e8u,
			0xef20cadau,
			0x36774c01,
			0xd07e9efeu,
			0x2bf11fb4,
			0x95dbda4du,
			0xae909198u,
			0xeaad8e71u,
			0x6b93d5a0,
			0xd08ed1d0u,
			0xafc725e0u,
			0x8e3c5b2fu,
			0x8e7594b7u,
			0x8ff6e2fbu,
			0xf2122b64u,
			0x8888b812u,
			0x900df01cu,
			0x4fad5ea0,
			0x688fc31c,
			0xd1cff191u,
			0xb3a8c1adu,
			0x2f2f2218,
			0xbe0e1777u,
			0xea752dfeu,
			0x8b021fa1u,
			0xe5a0cc0fu,
			0xb56f74e8u,
			0x18acf3d6,
			0xce89e299u,
			0xb4a84fe0u,
			0xfd13e0b7u,
			0x7cc43b81,
			0xd2ada8d9u,
			0x165fa266,
			0x80957705u,
			0x93cc7314u,
			0x211a1477,
			0xe6ad2065u,
			0x77b5fa86,
			0xc75442f5u,
			0xfb9d35cfu,
			0xebcdaf0cu,
			0x7b3e89a0,
			0xd6411bd3u,
			0xae1e7e49u,
			0x250e2d,
			0x2071b35e,
			0x226800bb,
			0x57b8e0af,
			0x2464369b,
			0xf009b91eu,
			0x5563911d,
			0x59dfa6aa,
			0x78c14389,
			0xd95a537fu,
			0x207d5ba2,
			0x2e5b9c5,
			0x83260376u,
			0x6295cfa9,
			0x11c81968,
			0x4e734a41,
			0xb3472dcau,
			0x7b14a94a,
			0x1b510052,
			0x9a532915u,
			0xd60f573fu,
			0xbc9bc6e4u,
			0x2b60a476,
			0x81e67400u,
			0x8ba6fb5,
			0x571be91f,
			0xf296ec6bu,
			0x2a0dd915,
			0xb6636521u,
			0xe7b9f9b6u,
			0xff34052eu,
			0xc5855664u,
			0x53b02d5d,
			0xa99f8fa1u,
			0x8ba4799,
			0x6e85076a
		};
        }

        private uint[] SetupS1()
        {
            return new uint[] {
			0x4b7a70e9,
			0xb5b32944u,
			0xdb75092eu,
			0xc4192623u,
			0xad6ea6b0u,
			0x49a7df7d,
			0x9cee60b8u,
			0x8fedb266u,
			0xecaa8c71u,
			0x699a17ff,
			0x5664526c,
			0xc2b19ee1u,
			0x193602a5,
			0x75094c29,
			0xa0591340u,
			0xe4183a3eu,
			0x3f54989a,
			0x5b429d65,
			0x6b8fe4d6,
			0x99f73fd6u,
			0xa1d29c07u,
			0xefe830f5u,
			0x4d2d38e6,
			0xf0255dc1u,
			0x4cdd2086,
			0x8470eb26u,
			0x6382e9c6,
			0x21ecc5e,
			0x9686b3f,
			0x3ebaefc9,
			0x3c971814,
			0x6b6a70a1,
			0x687f3584,
			0x52a0e286,
			0xb79c5305u,
			0xaa500737u,
			0x3e07841c,
			0x7fdeae5c,
			0x8e7d44ecu,
			0x5716f2b8,
			0xb03ada37u,
			0xf0500c0du,
			0xf01c1f04u,
			0x200b3ff,
			0xae0cf51au,
			0x3cb574b2,
			0x25837a58,
			0xdc0921bdu,
			0xd19113f9u,
			0x7ca92ff6,
			0x94324773u,
			0x22f54701,
			0x3ae5e581,
			0x37c2dadc,
			0xc8b57634u,
			0x9af3dda7u,
			0xa9446146u,
			0xfd0030e,
			0xecc8c73eu,
			0xa4751e41u,
			0xe238cd99u,
			0x3bea0e2f,
			0x3280bba1,
			0x183eb331,
			0x4e548b38,
			0x4f6db908,
			0x6f420d03,
			0xf60a04bfu,
			0x2cb81290,
			0x24977c79,
			0x5679b072,
			0xbcaf89afu,
			0xde9a771fu,
			0xd9930810u,
			0xb38bae12u,
			0xdccf3f2eu,
			0x5512721f,
			0x2e6b7124,
			0x501adde6,
			0x9f84cd87u,
			0x7a584718,
			0x7408da17,
			0xbc9f9abcu,
			0xe94b7d8cu,
			0xec7aec3au,
			0xdb851dfau,
			0x63094366,
			0xc464c3d2u,
			0xef1c1847u,
			0x3215d908,
			0xdd433b37u,
			0x24c2ba16,
			0x12a14d43,
			0x2a65c451,
			0x50940002,
			0x133ae4dd,
			0x71dff89e,
			0x10314e55,
			0x81ac77d6u,
			0x5f11199b,
			0x43556f1,
			0xd7a3c76bu,
			0x3c11183b,
			0x5924a509,
			0xf28fe6edu,
			0x97f1fbfau,
			0x9ebabf2cu,
			0x1e153c6e,
			0x86e34570u,
			0xeae96fb1u,
			0x860e5e0au,
			0x5a3e2ab3,
			0x771fe71c,
			0x4e3d06fa,
			0x2965dcb9,
			0x99e71d0fu,
			0x803e89d6u,
			0x5266c825,
			0x2e4cc978,
			0x9c10b36au,
			0xc6150ebau,
			0x94e2ea78u,
			0xa5fc3c53u,
			0x1e0a2df4,
			0xf2f74ea7u,
			0x361d2b3d,
			0x1939260f,
			0x19c27960,
			0x5223a708,
			0xf71312b6u,
			0xebadfe6eu,
			0xeac31f66u,
			0xe3bc4595u,
			0xa67bc883u,
			0xb17f37d1u,
			0x18cff28,
			0xc332ddefu,
			0xbe6c5aa5u,
			0x65582185,
			0x68ab9802,
			0xeecea50fu,
			0xdb2f953bu,
			0x2aef7dad,
			0x5b6e2f84,
			0x1521b628,
			0x29076170,
			0xecdd4775u,
			0x619f1510,
			0x13cca830,
			0xeb61bd96u,
			0x334fe1e,
			0xaa0363cfu,
			0xb5735c90u,
			0x4c70a239,
			0xd59e9e0bu,
			0xcbaade14u,
			0xeecc86bcu,
			0x60622ca7,
			0x9cab5cabu,
			0xb2f3846eu,
			0x648b1eaf,
			0x19bdf0ca,
			0xa02369b9u,
			0x655abb50,
			0x40685a32,
			0x3c2ab4b3,
			0x319ee9d5,
			0xc021b8f7u,
			0x9b540b19u,
			0x875fa099u,
			0x95f7997eu,
			0x623d7da8,
			0xf837889au,
			0x97e32d77u,
			0x11ed935f,
			0x16681281,
			0xe358829,
			0xc7e61fd6u,
			0x96dedfa1u,
			0x7858ba99,
			0x57f584a5,
			0x1b227263,
			0x9b83c3ffu,
			0x1ac24696,
			0xcdb30aebu,
			0x532e3054,
			0x8fd948e4u,
			0x6dbc3128,
			0x58ebf2ef,
			0x34c6ffea,
			0xfe28ed61u,
			0xee7c3c73u,
			0x5d4a14d9,
			0xe864b7e3u,
			0x42105d14,
			0x203e13e0,
			0x45eee2b6,
			0xa3aaabeau,
			0xdb6c4f15u,
			0xfacb4fd0u,
			0xc742f442u,
			0xef6abbb5u,
			0x654f3b1d,
			0x41cd2105,
			0xd81e799eu,
			0x86854dc7u,
			0xe44b476au,
			0x3d816250,
			0xcf62a1f2u,
			0x5b8d2646,
			0xfc8883a0u,
			0xc1c7b6a3u,
			0x7f1524c3,
			0x69cb7492,
			0x47848a0b,
			0x5692b285,
			0x95bbf00,
			0xad19489du,
			0x1462b174,
			0x23820e00,
			0x58428d2a,
			0xc55f5ea,
			0x1dadf43e,
			0x233f7061,
			0x3372f092,
			0x8d937e41u,
			0xd65fecf1u,
			0x6c223bdb,
			0x7cde3759,
			0xcbee7460u,
			0x4085f2a7,
			0xce77326eu,
			0xa6078084u,
			0x19f8509e,
			0xe8efd855u,
			0x61d99735,
			0xa969a7aau,
			0xc50c06c2u,
			0x5a04abfc,
			0x800bcadcu,
			0x9e447a2eu,
			0xc3453484u,
			0xfdd56705u,
			0xe1e9ec9,
			0xdb73dbd3u,
			0x105588cd,
			0x675fda79,
			0xe3674340u,
			0xc5c43465u,
			0x713e38d8,
			0x3d28f89e,
			0xf16dff20u,
			0x153e21e7,
			0x8fb03d4au,
			0xe6e39f2bu,
			0xdb83adf7u
		};
        }

        private uint[] SetupS2()
        {
            return new uint[] {
			0xe93d5a68u,
			0x948140f7u,
			0xf64c261cu,
			0x94692934u,
			0x411520f7,
			0x7602d4f7,
			0xbcf46b2eu,
			0xd4a20068u,
			0xd4082471u,
			0x3320f46a,
			0x43b7d4b7,
			0x500061af,
			0x1e39f62e,
			0x97244546u,
			0x14214f74,
			0xbf8b8840u,
			0x4d95fc1d,
			0x96b591afu,
			0x70f4ddd3,
			0x66a02f45,
			0xbfbc09ecu,
			0x3bd9785,
			0x7fac6dd0,
			0x31cb8504,
			0x96eb27b3u,
			0x55fd3941,
			0xda2547e6u,
			0xabca0a9au,
			0x28507825,
			0x530429f4,
			0xa2c86da,
			0xe9b66dfbu,
			0x68dc1462,
			0xd7486900u,
			0x680ec0a4,
			0x27a18dee,
			0x4f3ffea2,
			0xe887ad8cu,
			0xb58ce006u,
			0x7af4d6b6,
			0xaace1e7cu,
			0xd3375fecu,
			0xce78a399u,
			0x406b2a42,
			0x20fe9e35,
			0xd9f385b9u,
			0xee39d7abu,
			0x3b124e8b,
			0x1dc9faf7,
			0x4b6d1856,
			0x26a36631,
			0xeae397b2u,
			0x3a6efa74,
			0xdd5b4332u,
			0x6841e7f7,
			0xca7820fbu,
			0xfb0af54eu,
			0xd8feb397u,
			0x454056ac,
			0xba489527u,
			0x55533a3a,
			0x20838d87,
			0xfe6ba9b7u,
			0xd096954bu,
			0x55a867bc,
			0xa1159a58u,
			0xcca92963u,
			0x99e1db33u,
			0xa62a4a56u,
			0x3f3125f9,
			0x5ef47e1c,
			0x9029317cu,
			0xfdf8e802u,
			0x4272f70,
			0x80bb155cu,
			0x5282ce3,
			0x95c11548u,
			0xe4c66d22u,
			0x48c1133f,
			0xc70f86dcu,
			0x7f9c9ee,
			0x41041f0f,
			0x404779a4,
			0x5d886e17,
			0x325f51eb,
			0xd59bc0d1u,
			0xf2bcc18fu,
			0x41113564,
			0x257b7834,
			0x602a9c60,
			0xdff8e8a3u,
			0x1f636c1b,
			0xe12b4c2,
			0x2e1329e,
			0xaf664fd1u,
			0xcad18115u,
			0x6b2395e0,
			0x333e92e1,
			0x3b240b62,
			0xeebeb922u,
			0x85b2a20eu,
			0xe6ba0d99u,
			0xde720c8cu,
			0x2da2f728,
			0xd0127845u,
			0x95b794fdu,
			0x647d0862,
			0xe7ccf5f0u,
			0x5449a36f,
			0x877d48fau,
			0xc39dfd27u,
			0xf33e8d1eu,
			0xa476341,
			0x992eff74u,
			0x3a6f6eab,
			0xf4f8fd37u,
			0xa812dc60u,
			0xa1ebddf8u,
			0x991be14cu,
			0xdb6e6b0du,
			0xc67b5510u,
			0x6d672c37,
			0x2765d43b,
			0xdcd0e804u,
			0xf1290dc7u,
			0xcc00ffa3u,
			0xb5390f92u,
			0x690fed0b,
			0x667b9ffb,
			0xcedb7d9cu,
			0xa091cf0bu,
			0xd9155ea3u,
			0xbb132f88u,
			0x515bad24,
			0x7b9479bf,
			0x763bd6eb,
			0x37392eb3,
			0xcc115979u,
			0x8026e297u,
			0xf42e312du,
			0x6842ada7,
			0xc66a2b3bu,
			0x12754ccc,
			0x782ef11c,
			0x6a124237,
			0xb79251e7u,
			0x6a1bbe6,
			0x4bfb6350,
			0x1a6b1018,
			0x11caedfa,
			0x3d25bdd8,
			0xe2e1c3c9u,
			0x44421659,
			0xa121386,
			0xd90cec6eu,
			0xd5abea2au,
			0x64af674e,
			0xda86a85fu,
			0xbebfe988u,
			0x64e4c3fe,
			0x9dbc8057u,
			0xf0f7c086u,
			0x60787bf8,
			0x6003604d,
			0xd1fd8346u,
			0xf6381fb0u,
			0x7745ae04,
			0xd736fcccu,
			0x83426b33u,
			0xf01eab71u,
			0xb0804187u,
			0x3c005e5f,
			0x77a057be,
			0xbde8ae24u,
			0x55464299,
			0xbf582e61u,
			0x4e58f48f,
			0xf2ddfda2u,
			0xf474ef38u,
			0x8789bdc2u,
			0x5366f9c3,
			0xc8b38e74u,
			0xb475f255u,
			0x46fcd9b9,
			0x7aeb2661,
			0x8b1ddf84u,
			0x846a0e79u,
			0x915f95e2u,
			0x466e598e,
			0x20b45770,
			0x8cd55591u,
			0xc902de4cu,
			0xb90bace1u,
			0xbb8205d0u,
			0x11a86248,
			0x7574a99e,
			0xb77f19b6u,
			0xe0a9dc09u,
			0x662d09a1,
			0xc4324633u,
			0xe85a1f02u,
			0x9f0be8c,
			0x4a99a025,
			0x1d6efe10,
			0x1ab93d1d,
			0xba5a4df,
			0xa186f20fu,
			0x2868f169,
			0xdcb7da83u,
			0x573906fe,
			0xa1e2ce9bu,
			0x4fcd7f52,
			0x50115e01,
			0xa70683fau,
			0xa002b5c4u,
			0xde6d027,
			0x9af88c27u,
			0x773f8641,
			0xc3604c06u,
			0x61a806b5,
			0xf0177a28u,
			0xc0f586e0u,
			0x6058aa,
			0x30dc7d62,
			0x11e69ed7,
			0x2338ea63,
			0x53c2dd94,
			0xc2c21634u,
			0xbbcbee56u,
			0x90bcb6deu,
			0xebfc7da1u,
			0xce591d76u,
			0x6f05e409,
			0x4b7c0188,
			0x39720a3d,
			0x7c927c24,
			0x86e3725fu,
			0x724d9db9,
			0x1ac15bb4,
			0xd39eb8fcu,
			0xed545578u,
			0x8fca5b5,
			0xd83d7cd3u,
			0x4dad0fc4,
			0x1e50ef5e,
			0xb161e6f8u,
			0xa28514d9u,
			0x6c51133c,
			0x6fd5c7e7,
			0x56e14ec4,
			0x362abfce,
			0xddc6c837u,
			0xd79a3234u,
			0x92638212u,
			0x670efa8e,
			0x406000e0
		};
        }

        private uint[] SetupS3()
        {
            return new uint[] {
			0x3a39ce37,
			0xd3faf5cfu,
			0xabc27737u,
			0x5ac52d1b,
			0x5cb0679e,
			0x4fa33742,
			0xd3822740u,
			0x99bc9bbeu,
			0xd5118e9du,
			0xbf0f7315u,
			0xd62d1c7eu,
			0xc700c47bu,
			0xb78c1b6bu,
			0x21a19045,
			0xb26eb1beu,
			0x6a366eb4,
			0x5748ab2f,
			0xbc946e79u,
			0xc6a376d2u,
			0x6549c2c8,
			0x530ff8ee,
			0x468dde7d,
			0xd5730a1du,
			0x4cd04dc6,
			0x2939bbdb,
			0xa9ba4650u,
			0xac9526e8u,
			0xbe5ee304u,
			0xa1fad5f0u,
			0x6a2d519a,
			0x63ef8ce2,
			0x9a86ee22u,
			0xc089c2b8u,
			0x43242ef6,
			0xa51e03aau,
			0x9cf2d0a4u,
			0x83c061bau,
			0x9be96a4du,
			0x8fe51550u,
			0xba645bd6u,
			0x2826a2f9,
			0xa73a3ae1u,
			0x4ba99586,
			0xef5562e9u,
			0xc72fefd3u,
			0xf752f7dau,
			0x3f046f69,
			0x77fa0a59,
			0x80e4a915u,
			0x87b08601u,
			0x9b09e6adu,
			0x3b3ee593,
			0xe990fd5au,
			0x9e34d797u,
			0x2cf0b7d9,
			0x22b8b51,
			0x96d5ac3au,
			0x17da67d,
			0xd1cf3ed6u,
			0x7c7d2d28,
			0x1f9f25cf,
			0xadf2b89bu,
			0x5ad6b472,
			0x5a88f54c,
			0xe029ac71u,
			0xe019a5e6u,
			0x47b0acfd,
			0xed93fa9bu,
			0xe8d3c48du,
			0x283b57cc,
			0xf8d56629u,
			0x79132e28,
			0x785f0191,
			0xed756055u,
			0xf7960e44u,
			0xe3d35e8cu,
			0x15056dd4,
			0x88f46dbau,
			0x3a16125,
			0x564f0bd,
			0xc3eb9e15u,
			0x3c9057a2,
			0x97271aecu,
			0xa93a072au,
			0x1b3f6d9b,
			0x1e6321f5,
			0xf59c66fbu,
			0x26dcf319,
			0x7533d928,
			0xb155fdf5u,
			0x3563482,
			0x8aba3cbbu,
			0x28517711,
			0xc20ad9f8u,
			0xabcc5167u,
			0xccad925fu,
			0x4de81751,
			0x3830dc8e,
			0x379d5862,
			0x9320f991u,
			0xea7a90c2u,
			0xfb3e7bceu,
			0x5121ce64,
			0x774fbe32,
			0xa8b6e37eu,
			0xc3293d46u,
			0x48de5369,
			0x6413e680,
			0xa2ae0810u,
			0xdd6db224u,
			0x69852dfd,
			0x9072166,
			0xb39a460au,
			0x6445c0dd,
			0x586cdecf,
			0x1c20c8ae,
			0x5bbef7dd,
			0x1b588d40,
			0xccd2017fu,
			0x6bb4e3bb,
			0xdda26a7eu,
			0x3a59ff45,
			0x3e350a44,
			0xbcb4cdd5u,
			0x72eacea8,
			0xfa6484bbu,
			0x8d6612aeu,
			0xbf3c6f47u,
			0xd29be463u,
			0x542f5d9e,
			0xaec2771bu,
			0xf64e6370u,
			0x740e0d8d,
			0xe75b1357u,
			0xf8721671u,
			0xaf537d5du,
			0x4040cb08,
			0x4eb4e2cc,
			0x34d2466a,
			0x115af84,
			0xe1b00428u,
			0x95983a1du,
			0x6b89fb4,
			0xce6ea048u,
			0x6f3f3b82,
			0x3520ab82,
			0x11a1d4b,
			0x277227f8,
			0x611560b1,
			0xe7933fdcu,
			0xbb3a792bu,
			0x344525bd,
			0xa08839e1u,
			0x51ce794b,
			0x2f32c9b7,
			0xa01fbac9u,
			0xe01cc87eu,
			0xbcc7d1f6u,
			0xcf0111c3u,
			0xa1e8aac7u,
			0x1a908749,
			0xd44fbd9au,
			0xd0dadecbu,
			0xd50ada38u,
			0x339c32a,
			0xc6913667u,
			0x8df9317cu,
			0xe0b12b4fu,
			0xf79e59b7u,
			0x43f5bb3a,
			0xf2d519ffu,
			0x27d9459c,
			0xbf97222cu,
			0x15e6fc2a,
			0xf91fc71,
			0x9b941525u,
			0xfae59361u,
			0xceb69cebu,
			0xc2a86459u,
			0x12baa8d1,
			0xb6c1075eu,
			0xe3056a0cu,
			0x10d25065,
			0xcb03a442u,
			0xe0ec6e0eu,
			0x1698db3b,
			0x4c98a0be,
			0x3278e964,
			0x9f1f9532u,
			0xe0d392dfu,
			0xd3a0342bu,
			0x8971f21eu,
			0x1b0a7441,
			0x4ba3348c,
			0xc5be7120u,
			0xc37632d8u,
			0xdf359f8du,
			0x9b992f2eu,
			0xe60b6f47u,
			0xfe3f11d,
			0xe54cda54u,
			0x1edad891,
			0xce6279cfu,
			0xcd3e7e6fu,
			0x1618b166,
			0xfd2c1d05u,
			0x848fd2c5u,
			0xf6fb2299u,
			0xf523f357u,
			0xa6327623u,
			0x93a83531u,
			0x56cccd02,
			0xacf08162u,
			0x5a75ebb5,
			0x6e163697,
			0x88d273ccu,
			0xde966292u,
			0x81b949d0u,
			0x4c50901b,
			0x71c65614,
			0xe6c6c7bdu,
			0x327a140a,
			0x45e1d006,
			0xc3f27b9au,
			0xc9aa53fdu,
			0x62a80f00,
			0xbb25bfe2u,
			0x35bdd2f6,
			0x71126905,
			0xb2040222u,
			0xb6cbcf7cu,
			0xcd769c2bu,
			0x53113ec0,
			0x1640e3d3,
			0x38abbd60,
			0x2547adf0,
			0xba38209cu,
			0xf746ce76u,
			0x77afa1c5,
			0x20756060,
			0x85cbfe4eu,
			0x8ae88dd8u,
			0x7aaaf9b0,
			0x4cf9aa7e,
			0x1948c25c,
			0x2fb8a8c,
			0x1c36ae4,
			0xd6ebe1f9u,
			0x90d4f869u,
			0xa65cdea0u,
			0x3f09252d,
			0xc208e69fu,
			0xb74e6132u,
			0xce77e25bu,
			0x578fdfe3,
			0x3ac372e6
		};
        }

        #endregion

        #region Some Conversions
        private byte wordByte0(uint w)
        {
            return Convert.ToByte(w / 256 / 256 / 256 % 256);
        }

        private byte wordByte1(uint w)
        {
            return Convert.ToByte(w / 256 / 256 % 256);
        }

        private byte wordByte2(uint w)
        {
            return Convert.ToByte(w / 256 % 256);
        }

        private byte wordByte3(uint w)
        {
            return Convert.ToByte(w % 256);
        }

        private string ByteToHex(byte[] bytes)
        {
            StringBuilder s = new StringBuilder();
            foreach (byte b in bytes)
            {
                s.Append(b.ToString("x2"));
            }
            return s.ToString();
        }

        private byte[] HexToBytes(string Value)
        {
            if (Value == null || Value.Length == 0)
                throw new Exception("Invalid hexadecimal input");
            byte[] Buffer = new byte[Value.Length / 2];
            for (int i = 0; i <= (Value.Length / 2) - 1; i++)
            {
                Buffer[i] = Convert.ToByte("&h" + Value.Substring((i * 2), 2));
            }
            return Buffer;
        }

        #endregion
    }
}
