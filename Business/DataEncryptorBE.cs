using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Business;

internal class DataEncryptorBE
{
    // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
    // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
    private const string initVector = "Ali1982_namer315";
    // This constant is used to determine the keysize of the encryption algorithm.
    private const int keysize = 256;
    //Encrypt
    public static string EncryptString(string plainText)
    {
        byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
        byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        PasswordDeriveBytes password = new PasswordDeriveBytes("1" , null);
        byte[] keyBytes = password.GetBytes(keysize / 8);
        RijndaelManaged symmetricKey = new RijndaelManaged();
        symmetricKey.Mode = CipherMode.CBC;
        ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes , initVectorBytes);
        MemoryStream memoryStream = new MemoryStream();
        CryptoStream cryptoStream = new CryptoStream(memoryStream , encryptor , CryptoStreamMode.Write);
        cryptoStream.Write(plainTextBytes , 0 , plainTextBytes.Length);
        cryptoStream.FlushFinalBlock();
        byte[] cipherTextBytes = memoryStream.ToArray();
        memoryStream.Close();
        cryptoStream.Close();
        return Convert.ToBase64String(cipherTextBytes);
    }

    public static string DecryptString(string cipherText)
    {
        byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector); // Fixed: Changed from ASCII to UTF8
        byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
        PasswordDeriveBytes password = new PasswordDeriveBytes("1" , null);
        byte[] keyBytes = password.GetBytes(keysize / 8);
        RijndaelManaged symmetricKey = new RijndaelManaged();
        symmetricKey.Mode = CipherMode.CBC;
        ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes , initVectorBytes);
        MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
        CryptoStream cryptoStream = new CryptoStream(memoryStream , decryptor , CryptoStreamMode.Read);

        // Fixed: Use a list to collect all decrypted bytes
        List<byte> decryptedBytes = new List<byte>();
        byte[] buffer = new byte[1024];
        int bytesRead;

        while ((bytesRead = cryptoStream.Read(buffer , 0 , buffer.Length)) > 0)
        {
            for (int i = 0 ; i < bytesRead ; i++)
            {
                decryptedBytes.Add(buffer[i]);
            }
        }

        memoryStream.Close();
        cryptoStream.Close();

        return Encoding.UTF8.GetString(decryptedBytes.ToArray());
    }
}
