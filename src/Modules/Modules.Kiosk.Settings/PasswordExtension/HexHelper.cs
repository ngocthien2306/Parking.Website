using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Modules.Kiosk.Settings.PasswordExtension
{
    public class HexHelper
    {
        private static string EncryptionKey = "Thanhtin@01282252279";
        private const string ProtectedKey = @"qazwsxedcRFV";
        //**-------------------------------------------------------------------------------------------
        public static string CalculateMd5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
        public static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            byte[] result = md5.Hash;
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }
            return strBuilder.ToString();
        }
        public static string CalculateMd5(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
        //**-------------------------------------------------------------------------------------------
        public static string Encrypt(string toEncrypt, bool useHashing = true, string key = "")
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
            // Get the key from config file            
            if (string.IsNullOrEmpty(key))
                /////key = settingsReader.GetValue("SecurityKey", typeof(String)).ToString();
                key = "UA5jbxx29qKzRc2M8PXF6g==";
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        /// <summary>
        /// DeCrypt a string using dual encryption method. Return a DeCrypted clear string
        /// </summary>
        /// <param name="cipherString">encrypted string</param>
        /// <param name="useHashing">Did you use hashing to encrypt this data? pass true is yes</param>
        /// <returns></returns>
        public static string Decrypt(string cipherString, bool useHashing, string key = "")
        {
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(cipherString);
            //Get your key from config file to open the lock!
            if (string.IsNullOrEmpty(key))
                key = "UA5jbxx29qKzRc2M8PXF6g==";
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            tdes.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        //**-------------------------------------------------------------------------------------------
        public static string EncryptRfc(string clearText)
        {
            try
            {
                byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        clearText = Convert.ToBase64String(ms.ToArray());
                    }
                }
                return clearText;
            }
            catch (Exception)
            {
                return clearText;
            }

        }
        public static string DecryptRfc(string cipherText)
        {
            try
            {
                cipherText = cipherText.Replace(" ", "+");
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
                return cipherText;
            }
            catch (Exception ex)
            {
                return "";
            }

        }
        //**-------------------------------------------------------------------------------------------
        public static string EncryptPassword(string password)
        {
            SHA256 sha = SHA256.Create();
            byte[] rs = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(rs);
        }
        //**-------------------------------------------------------------------------------------------
        // This constant is used to determine the keysize of the encryption algorithm
        private const int keysize = 256;
        public static string EncryptString(string plainText, string passPhrase)
        {
            if (string.IsNullOrEmpty(plainText)) return "";
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(EncryptionKey);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(cipherTextBytes);
        }
        public static string DecryptString(string cipherText, string passPhrase)
        {
            if (string.IsNullOrEmpty(cipherText)) return "";
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(EncryptionKey);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }
        //**-------------------------------------------------------------------------------------------
        public static string GenerateSHA256String(string inputString)
        {
            SHA256 sha256 = SHA256Managed.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hash = sha256.ComputeHash(bytes);
            return GetStringFromHash(hash);
        }
        public static string GenerateSHA512String(string inputString)
        {
            SHA512 sha512 = SHA512Managed.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hash = sha512.ComputeHash(bytes);
            return GetStringFromHash(hash);
        }
        public static string CalculateSHA1Hash(string input)
        {
            using (var sha1 = SHA1.Create())
            {
                byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);

                byte[] hash = sha1.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < hash.Length; i++)
                    sb.Append(hash[i].ToString("x2"));

                return sb.ToString();
            }
        }
        public static string CalculateSHA256Hash(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);

                byte[] hash = sha256.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < hash.Length; i++)
                    sb.Append(hash[i].ToString("x2"));

                return sb.ToString();
            }
        }
        public static string CalculateSHA384Hash(string input)
        {
            using (var sha384 = SHA384.Create())
            {
                byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);

                byte[] hash = sha384.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < hash.Length; i++)
                    sb.Append(hash[i].ToString("x2"));

                return sb.ToString();
            }
        }
        public static string CalculateSHA512Hash(string input)
        {
            using (var sha512 = SHA512.Create())
            {
                byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);

                byte[] hash = sha512.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < hash.Length; i++)
                    sb.Append(hash[i].ToString("x2"));

                return sb.ToString();
            }
        }
        private static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }
        //**-------------------------------------------------------------------------------------------
        public static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            byte[] saltBytes = passwordBytes;
            // Example:
            //saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (CryptoStream cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        public static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;
            // Set your salt here to meet your flavor:
            byte[] saltBytes = passwordBytes;
            // Example:
            //saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (CryptoStream cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }

        public static string AES_Encrypt1(string text, byte[] passwordBytes)
        {
            byte[] originalBytes = Encoding.UTF8.GetBytes(text);
            byte[] encryptedBytes = null;

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            // Getting the salt size
            int saltSize = GetSaltSize(passwordBytes);
            // Generating salt bytes
            byte[] saltBytes = GetRandomBytes(saltSize);

            // Appending salt bytes to original bytes
            byte[] bytesToBeEncrypted = new byte[saltBytes.Length + originalBytes.Length];
            for (int i = 0; i < saltBytes.Length; i++)
            {
                bytesToBeEncrypted[i] = saltBytes[i];
            }
            for (int i = 0; i < originalBytes.Length; i++)
            {
                bytesToBeEncrypted[i + saltBytes.Length] = originalBytes[i];
            }

            encryptedBytes = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

            return Convert.ToBase64String(encryptedBytes);
        }

        public static string AES_Decrypt(string decryptedText, byte[] passwordBytes)
        {
            byte[] bytesToBeDecrypted = Convert.FromBase64String(decryptedText);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] decryptedBytes = AES_Decrypt(bytesToBeDecrypted, passwordBytes);

            // Getting the size of salt
            int saltSize = GetSaltSize(passwordBytes);

            // Removing salt bytes, retrieving original bytes
            byte[] originalBytes = new byte[decryptedBytes.Length - saltSize];
            for (int i = saltSize; i < decryptedBytes.Length; i++)
            {
                originalBytes[i - saltSize] = decryptedBytes[i];
            }

            return Encoding.UTF8.GetString(originalBytes);
        }
        //**-------------------------------------------------------------------------------------------
        public static void EncryptFile(string inputFile, string outputFile, string skey)
        {
            try
            {
                using (RijndaelManaged aes = new RijndaelManaged())
                {
                    byte[] key = ASCIIEncoding.UTF8.GetBytes(skey);

                    /* This is for demostrating purposes only. 
                     * Ideally you will want the IV key to be different from your key and you should always generate a new one for each encryption in other to achieve maximum security*/
                    byte[] IV = ASCIIEncoding.UTF8.GetBytes(skey);

                    using (FileStream fsCrypt = new FileStream(outputFile, FileMode.Create))
                    {
                        using (ICryptoTransform encryptor = aes.CreateEncryptor(key, IV))
                        {
                            using (CryptoStream cs = new CryptoStream(fsCrypt, encryptor, CryptoStreamMode.Write))
                            {
                                using (FileStream fsIn = new FileStream(inputFile, FileMode.Open))
                                {
                                    int data;
                                    while ((data = fsIn.ReadByte()) != -1)
                                    {
                                        cs.WriteByte((byte)data);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // failed to encrypt file
            }
        }
        public static void DecryptFile(string inputFile, string outputFile, string skey)
        {
            try
            {
                using (RijndaelManaged aes = new RijndaelManaged())
                {
                    byte[] key = ASCIIEncoding.UTF8.GetBytes(skey);

                    /* This is for demostrating purposes only. 
                     * Ideally you will want the IV key to be different from your key and you should always generate a new one for each encryption in other to achieve maximum security*/
                    byte[] IV = ASCIIEncoding.UTF8.GetBytes(skey);

                    using (FileStream fsCrypt = new FileStream(inputFile, FileMode.Open))
                    {
                        using (FileStream fsOut = new FileStream(outputFile, FileMode.Create))
                        {
                            using (ICryptoTransform decryptor = aes.CreateDecryptor(key, IV))
                            {
                                using (CryptoStream cs = new CryptoStream(fsCrypt, decryptor, CryptoStreamMode.Read))
                                {
                                    int data;
                                    while ((data = cs.ReadByte()) != -1)
                                    {
                                        fsOut.WriteByte((byte)data);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // failed to decrypt file
            }
        }
        //**---------------------------------------------------------------------------------------------
        public static int GetSaltSize(byte[] passwordBytes)
        {
            var key = new Rfc2898DeriveBytes(passwordBytes, passwordBytes, 1000);
            byte[] ba = key.GetBytes(2);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < ba.Length; i++)
            {
                sb.Append(Convert.ToInt32(ba[i]).ToString());
            }
            int saltSize = 0;
            string s = sb.ToString();
            foreach (char c in s)
            {
                int intc = Convert.ToInt32(c.ToString());
                saltSize = saltSize + intc;
            }

            return saltSize;
        }

        public static byte[] GetRandomBytes(int length)
        {
            byte[] ba = new byte[length];
            RNGCryptoServiceProvider.Create().GetBytes(ba);
            return ba;
        }
        public static String ToHexString(byte value)
        {
            return (Convert.ToString(value / 16, 16).ToUpper() + Convert.ToString(value % 16, 16).ToUpper());
        }

        public static String ToHexString(UInt16 value)
        {
            var barrValue = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                return (ToHexString(barrValue[1]) + ToHexString(barrValue[0]));
            }
            return (ToHexString(barrValue[0]) + ToHexString(barrValue[1]));
        }

        public static String ToHexString(int value)
        {
            var barrValue = BitConverter.GetBytes(value);
            return (ToHexString(barrValue[0]) + ToHexString(barrValue[1]) + ToHexString(barrValue[2]) + ToHexString(barrValue[3]));
        }

        public static String ToHexString(UInt32 value)
        {
            var barrValue = BitConverter.GetBytes(value);
            return (ToHexString(barrValue[0]) + ToHexString(barrValue[1]) + ToHexString(barrValue[2]) + ToHexString(barrValue[3]));
        }

        public static String ToHexString(Byte[] value)
        {
            if (value == null) return null;
            //* Address
            var sb = new StringBuilder();
            foreach (var v in value)
            {
                sb.Append(ToHexString(v));
            }
            return sb.ToString();
        }

        public static bool IsEqual(Byte[] s1, Byte[] s2)
        {
            if (s1 == null && s2 == null) return true;
            if (s1 == null) return false;
            if (s2 == null) return false;
            if (s1.Length != s2.Length) return false;
            for (int i = 0; i < s1.Length; i++) if (s1[i] != s2[i]) return false;
            return true;
        }

        public static bool IsHexChar(char c)
        {
            if (Char.IsNumber(c)) return true;
            var upper = Char.ToUpper(c);
            if (upper >= 'A' && upper <= 'F') return true;
            return false;
        }

        public static bool IsHexString(String st)
        {
            foreach (var c in st)
            {
                if (!IsHexChar(c)) return false;
            }
            return true;
        }

        public static Byte ToByte(Char c)
        {
            if (c >= '0' && c <= '9') return (Byte)(c - '0');
            var upper = Char.ToUpper(c);
            if (upper >= 'A' && upper <= 'F') return (Byte)(upper - 'A' + 10);
            throw new ArgumentOutOfRangeException();
        }

        public static Byte ToByte(String st)
        {
            if (st.Length > 2) throw new ArgumentOutOfRangeException();
            Byte ret = 0;
            for (int i = 0; i < st.Length; i++)
            {
                ret *= 16;
                ret += ToByte(st[i]);
            }
            return ret;
        }

        public static Byte[] ToBytes(String st)
        {
            var builder = new StringBuilder(st);
            if (st.Length % 2 != 0) builder.Insert(0, '0');
            var ret = new byte[builder.Length / 2];
            for (int i = 0; i < st.Length; i++)
            {
                ret[i / 2] += ToByte(st[i]);
                if (i % 2 == 0)
                {
                    ret[i / 2] *= 16;
                }
            }
            return ret;
        }

        public static UInt16 ToUInt16(String st)
        {
            if (st.Length > 4) throw new ArgumentOutOfRangeException();
            UInt16 ret = 0;
            for (int i = 0; i < st.Length; i++)
            {
                ret *= 16;
                ret += ToByte(st[i]);
            }
            return ret;
        }

        public static int InsertToBytes(byte[] array, int start, int val)
        {
            var bytes = BitConverter.GetBytes(val);
            for (int i = 0; i < bytes.Length; i++) array[start + i] = bytes[i];
            return start + 4;
        }

        public static int InsertToBytes(byte[] array, int start, UInt32 val)
        {
            var bytes = BitConverter.GetBytes(val);
            for (int i = 0; i < bytes.Length; i++) array[start + i] = bytes[i];
            return start + 4;
        }

        public static int InsertToBytes(byte[] array, int start, UInt16 val)
        {
            var bytes = BitConverter.GetBytes(val);
            for (int i = 0; i < bytes.Length; i++) array[start + i] = bytes[i];
            return start + 2;
        }

        public static int InsertToBytes(byte[] array, int start, byte[] val)
        {
            for (int i = 0; i < val.Length; i++) array[start + i] = val[i];
            return start + val.Length;
        }
        public static string Base642String(string value)
        {
            var EncodedBytes = System.Convert.FromBase64String(value);
            return System.Text.Encoding.UTF8.GetString(EncodedBytes);
        }
        public static string String2Base64(string value)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(value);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string ComputeStringToSha256Hash(string plainText)
        {
            // Create a SHA256 hash from string   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Computing Hash - returns here byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(plainText));

                // now convert byte array to a string   
                StringBuilder stringbuilder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    stringbuilder.Append(bytes[i].ToString("x2"));
                }
                return stringbuilder.ToString();
            }
        }
        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return password;
            byte[] salt;
            byte[] bytes;
            using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, 16, 1000))
            {
                salt = rfc2898DeriveBytes.Salt;
                bytes = rfc2898DeriveBytes.GetBytes(32);
            }
            byte[] array = new byte[49];
            Buffer.BlockCopy(salt, 0, array, 1, 16);
            Buffer.BlockCopy(bytes, 0, array, 17, 32);
            return Convert.ToBase64String(array);
        }
        public static string Encryptxx(string plainText)
        {
            byte[] bytes = Encoding.UTF8.GetBytes("2646294A404E635266556A586E3272357538782F413F4428472D4B6150645367566B5970337336763979244226452948404D6251655468576D5A713474377721");
            string result;
            using (Aes aes = Aes.Create())
            {
                using (ICryptoTransform cryptoTransform = aes.CreateEncryptor(bytes, aes.IV))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
                        {
                            using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                            {
                                streamWriter.Write(plainText);
                            }
                        }
                        byte[] iV = aes.IV;
                        byte[] array = memoryStream.ToArray();
                        byte[] array2 = new byte[iV.Length + array.Length];
                        Buffer.BlockCopy(iV, 0, array2, 0, iV.Length);
                        Buffer.BlockCopy(array, 0, array2, iV.Length, array.Length);
                        result = Convert.ToBase64String(array2);
                    }
                }
            }
            return result;
        }
        public string Decrypt(string encryptedText)
        {
            byte[] array = Convert.FromBase64String(encryptedText);
            byte[] array2 = new byte[16];
            byte[] array3 = new byte[array.Length - array2.Length];
            Buffer.BlockCopy(array, 0, array2, 0, array2.Length);
            Buffer.BlockCopy(array, array2.Length, array3, 0, array.Length - array2.Length);
            byte[] bytes = Encoding.UTF8.GetBytes("2646294A404E635266556A586E3272357538782F413F4428472D4B6150645367566B5970337336763979244226452948404D6251655468576D5A713474377721");
            string result;
            using (Aes aes = Aes.Create())
            {
                using (ICryptoTransform cryptoTransform = aes.CreateDecryptor(bytes, array2))
                {
                    string text;
                    using (MemoryStream memoryStream = new MemoryStream(array3))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, 0))
                        {
                            using (StreamReader streamReader = new StreamReader(cryptoStream))
                            {
                                text = streamReader.ReadToEnd();
                            }
                        }
                    }
                    result = text;
                }
            }
            return result;
        }




    }
}
