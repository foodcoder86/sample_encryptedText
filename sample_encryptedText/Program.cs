// See https://aka.ms/new-console-template for more information
//using System.Security.Cryptography;
//using System.Text;

//Console.WriteLine("Hello, World!");


﻿// See https://aka.ms/new-console-template for more information
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace ConsoleApp
{
    class Program
    {
        //secure string to apply to the cipher of 


        static void Main(string[] args)
        {
            string SharedKey = null;
            string messageToEncrypt = null;
            string salt = null;
            Program program = new Program();

            foreach (string arg in args)
            {
                //EncryptThis="noOnewillGuessThis!" SecretKey=superSecret 
                Console.WriteLine(arg);
                if (arg.ToLowerInvariant().Split("=")[0] == "encryptthis")
                {
                    messageToEncrypt = arg.Split("=")[1].Trim('"');
                }
                if (arg.ToLowerInvariant().Split("=")[0] == "secretkey")
                {
                    SharedKey = arg.Split("=")[1].Trim('"');
                }
                if (arg.ToLowerInvariant().Split("=")[0] == "salt")
                {
                    salt = arg.Split("=")[1].Trim('"');
                }
            }
            if (messageToEncrypt == null)
            {
                Environment.Exit(10);
            }
            if (SharedKey == null)
            {
                Environment.Exit(10);
            }
            if (salt == null)
            {
                Environment.Exit(10);
            }



            string decrypted = Decrypt("QctZLOoeHwfI8kxJAIPjESdCIQhHEIqf6+3niWxj4WQSoSm4VQFEI9bYqLTmkQvX", SharedKey, salt);
            Console.WriteLine(decrypted);

            //string encrypted = Encrypt(messageToEncrypt, SharedKey, salt);
            //Console.WriteLine($"Encrypted: {encrypted}");

            //string decrypted = Decrypt(encrypted, SharedKey, salt);
            //Console.WriteLine($"Decrypted: {decrypted}");
        }

        public static string Encrypt(string plainText, string secureStringphrase, string salt)
        {
            using (Aes aesAlg = Aes.Create())
            {
                byte[] key = GenerateKey(secureStringphrase, aesAlg.KeySize / 8, salt);
                aesAlg.Key = key;

                byte[] iv = aesAlg.IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, iv);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(iv, 0, iv.Length);

                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText, string secureStringphrase, string salt = null)
        {
            byte[] fullCipher = Convert.FromBase64String(cipherText);

            using (Aes aesAlg = Aes.Create())
            {
                byte[] key = GenerateKey(secureStringphrase, aesAlg.KeySize / 8, salt);

                using (MemoryStream msDecrypt = new MemoryStream(fullCipher))
                {
                    byte[] iv = new byte[aesAlg.BlockSize / 8];
                    msDecrypt.Read(iv, 0, iv.Length);

                    aesAlg.Key = key;
                    aesAlg.IV = iv;

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        //private static byte[] GenerateKey(string password, int keySize, string salt)
        //{
        //    const int iterations = 1000;
        //    using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt ?? string.Empty), iterations))
        //    {
        //        return rfc2898DeriveBytes.GetBytes(keySize);
        //    }
        //}

        private static byte[] GenerateKey(string passphrase, int keySize, string salt)
        {
            const int iterations = 1000;
            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(passphrase, Encoding.UTF8.GetBytes(salt ?? string.Empty), iterations))
            {
                return rfc2898DeriveBytes.GetBytes(keySize);
            }
        }
    }
}