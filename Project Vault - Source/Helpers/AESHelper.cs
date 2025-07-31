using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace ProjectVault
{
    public static class AESHelper
    {
        public const string AESHeaderString = "Encrypted With Project Vault";
        public static byte[] AESHeaderBytes
        {
            get
            {
                return StringEncodingHelper.StringToBytesUnicode(AESHeaderString);
            }
        }
        public static int AESHeaderLength
        {
            get
            {
                return AESHeaderBytes.Length;
            }
        }
        public static void EncryptStream(byte[] key, Stream source, Stream destination = null)
        {
            if (source is null)
            {
                throw new Exception("Could not encrypt stream beacuse source is null.");
            }
            else if (!source.CanRead)
            {
                throw new Exception("Could not encrypt stream beacuse source is unreadable.");
            }
            if (destination is null)
            {
                throw new Exception("Could not encrypt stream beacuse destination is null.");
            }
            else if (!destination.CanWrite)
            {
                throw new Exception("Could not encrypt stream beacuse destination is unwriteable.");
            }
            else if (key is null)
            {
                throw new Exception("Could not encrypt stream beacuse key is null.");
            }
            byte[] lengthAdjustedKey = SHA256Helper.HashBytes(key);

            AESMeta header = new AESMeta(key);

            source.Position = 0;
            destination.Position = 0;

            byte[] headerBytes = header.Serialize();

            destination.Write(headerBytes, 0, headerBytes.Length);

            Aes aes = Aes.Create();

            aes.BlockSize = 128;
            aes.IV = header.IV;

            aes.KeySize = 256;
            aes.Key = lengthAdjustedKey;

            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.ISO10126;

            ICryptoTransform decryptor = aes.CreateEncryptor();

            CryptoStream cryptoStream = new CryptoStream(destination, decryptor, CryptoStreamMode.Write);

            ulong totalMemory = new ComputerInfo().TotalPhysicalMemory;
            long chunkSize = (long)Math.Ceiling(totalMemory / 100.0);
            if (chunkSize > int.MaxValue)
            {
                chunkSize = int.MaxValue;
            }

            long currentIndex = 0;

            while (currentIndex < source.Length)
            {
                long bufferSize = source.Length - currentIndex;
                if (bufferSize > chunkSize)
                {
                    bufferSize = chunkSize;
                }
                byte[] buffer = new byte[bufferSize];
                source.Read(buffer, 0, buffer.Length);
                cryptoStream.Write(buffer, 0, buffer.Length);
                currentIndex += bufferSize;
            }

            cryptoStream.FlushFinalBlock();
            cryptoStream.Flush();
            cryptoStream.Dispose();
            decryptor.Dispose();
            aes.Dispose();
        }
        public static byte[] EncryptBytes(byte[] source, byte[] key)
        {
            if (source is null)
            {
                throw new Exception("Could not encrypt bytes beacuse source is null.");
            }
            else if (key is null)
            {
                throw new Exception("Could not encrypt bytes beacuse key is null.");
            }
            MemoryStream sourceStream = new MemoryStream(source);
            MemoryStream outputStream = new MemoryStream();
            EncryptStream(sourceStream, outputStream, key);
            sourceStream.Dispose();
            byte[] output = outputStream.ToArray();
            outputStream.Dispose();
            return output;
        }
        public static void EncryptFile(string filePath, byte[] key)
        {
            if (filePath is null || filePath == "")
            {
                throw new Exception("Could not encrypt file beacuse file path is null or empty.");
            }
            else if (!File.Exists(filePath))
            {
                throw new Exception($"Could not encrypt file because file path does not exist.");
            }
            else if (key is null)
            {
                throw new Exception("Could not encrypt file because the key is null.");
            }
            else if (Path.GetExtension(filePath).ToUpper() == ".AES")
            {
                throw new Exception("Could not encrypt file because the file was already encrypted.");
            }
            string destinationFilePath = filePath + ".aes";
            if (File.Exists(destinationFilePath))
            {
                throw new Exception($"Could not encrypt file because file already exists at destinationFilePath.");
            }
            try
            {
                FileStream sourceFileStream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                FileStream destinationFileStream = File.Open(destinationFilePath, FileMode.Create, FileAccess.Write);
                EncryptStream(sourceFileStream, destinationFileStream, key);
                sourceFileStream.Dispose();
                destinationFileStream.Dispose();
            }
            catch (Exception ex)
            {
                try
                {
                    File.Delete(destinationFilePath);
                }
                catch
                {

                }
                throw ex;
            }
            ShreddingHelper.ShredFile(filePath);
        }
        public static void EncryptDirectory(string directoryPath, byte[] key)
        {
            if (directoryPath is null || directoryPath == "")
            {
                throw new Exception("Could not encrypt directory beacuse directory path is null or empty.");
            }
            else if (!Directory.Exists(directoryPath))
            {
                throw new Exception($"Could not encrypt directory because directory path does not exist.");
            }
            List<string> subFiles = new List<string>(Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories));
            foreach (string subFile in subFiles)
            {
                if (Path.GetExtension(subFile).ToUpper() == ".AES")
                {
                    throw new Exception("Could not encrypt directory because one or more files were already encrypted.");
                }
            }
            foreach (string subFile in subFiles)
            {
                EncryptFile(subFile, key);
            }
        }
        public static void DecryptStream(Stream source, Stream destination, byte[] key)
        {
            if (source is null)
            {
                throw new Exception("Could not decrypt stream beacuse source is null.");
            }
            else if (!source.CanRead)
            {
                throw new Exception("Could not decrypt stream beacuse source is unreadable.");
            }
            else if (!source.CanSeek)
            {
                throw new Exception("Could not decrypt stream beacuse source does not support seeking.");
            }
            else if (destination is null)
            {
                throw new Exception("Could not decrypt stream beacuse destination is null.");
            }
            else if (!destination.CanWrite)
            {
                throw new Exception("Could not decrypt stream beacuse destination is unwriteable.");
            }
            else if (!destination.CanSeek)
            {
                throw new Exception("Could not decrypt stream beacuse destination does not support seeking.");
            }
            else if (key is null)
            {
                throw new Exception("Could not decrypt stream beacuse key is null.");
            }
            byte[] lengthAdjustedKey = SHA256Helper.HashBytes(key);

            source.Position = 0;
            destination.Position = 0;

            AESMeta header = AESMeta.FromStream(source);

            if (!header.VerifyKey(key))
            {
                throw new Exception("Could not decrypt stream because key was incorrect.");
            }

            source.Position = AESMeta.HeaderLength;

            Aes aes = Aes.Create();

            aes.BlockSize = 128;
            aes.IV = header.IV;

            aes.KeySize = 256;
            aes.Key = lengthAdjustedKey;

            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.ISO10126;

            ICryptoTransform decryptor = aes.CreateDecryptor();

            CryptoStream cryptoStream = new CryptoStream(destination, decryptor, CryptoStreamMode.Write);

            ulong totalMemory = new ComputerInfo().TotalPhysicalMemory;
            long chunkSize = (long)Math.Ceiling(totalMemory / 100.0);
            if (chunkSize > int.MaxValue)
            {
                chunkSize = int.MaxValue;
            }

            long currentIndex = AESMeta.HeaderLength;

            while (currentIndex < source.Length)
            {
                long bufferSize = source.Length - currentIndex;
                if (bufferSize > chunkSize)
                {
                    bufferSize = chunkSize;
                }
                byte[] buffer = new byte[bufferSize];
                source.Read(buffer, 0, buffer.Length);
                cryptoStream.Write(buffer, 0, buffer.Length);
                currentIndex += bufferSize;
            }

            cryptoStream.FlushFinalBlock();
            cryptoStream.Flush();
            cryptoStream.Dispose();
            decryptor.Dispose();
            aes.Dispose();
        }
        public static byte[] DecryptBytes(byte[] source, byte[] key)
        {
            if (source is null)
            {
                throw new Exception("Could not decrypt bytes beacuse source is null.");
            }
            else if (key is null)
            {
                throw new Exception("Could not decrypt bytes beacuse key is null.");
            }
            MemoryStream sourceStream = new MemoryStream(source);
            MemoryStream outputStream = new MemoryStream();
            DecryptStream(sourceStream, outputStream, key);
            sourceStream.Dispose();
            byte[] output = outputStream.ToArray();
            outputStream.Dispose();
            return output;
        }
        public static void DecryptFile(string filePath, byte[] key)
        {
            if (filePath is null || filePath == "")
            {
                throw new Exception("Could not decrypt file beacuse file path is null or empty.");
            }
            else if (!File.Exists(filePath))
            {
                throw new Exception($"Could not decrypt file because file path does not exist.");
            }
            else if (key is null)
            {
                throw new Exception("Could not decrypt file because the key is null.");
            }
            else if (!filePath.ToUpper().EndsWith(".AES"))
            {
                throw new Exception("Could not decrypt file because it was not encrypted.");
            }
            string destinationFilePath = filePath.Substring(0, filePath.Length - 4);
            if (File.Exists(destinationFilePath))
            {
                throw new Exception($"Could not decrypt file because file already exists at destinationFilePath.");
            }
            try
            {
                FileStream sourceFileStream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                FileStream destinationFileStream = File.Open(destinationFilePath, FileMode.Create, FileAccess.Write);
                DecryptStream(sourceFileStream, destinationFileStream, key);
                sourceFileStream.Dispose();
                destinationFileStream.Dispose();
            }
            catch (Exception ex)
            {
                try
                {
                    ShreddingHelper.ShredFile(destinationFilePath);
                }
                catch
                {

                }
                throw ex;
            }
            File.Delete(filePath);
        }
        public static void DecryptDirectory(string directoryPath, byte[] key)
        {
            if (directoryPath is null || directoryPath == "")
            {
                throw new Exception("Could not decrypt directory beacuse directory path is null or empty.");
            }
            else if (!Directory.Exists(directoryPath))
            {
                throw new Exception($"Could not decrypt directory because directory path does not exist.");
            }
            List<string> subFiles = new List<string>(Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories));
            foreach (string subFile in subFiles)
            {
                if (Path.GetExtension(subFile).ToUpper() != ".AES")
                {
                    throw new Exception("Could not decrypt directory because one or more files were not encrypted.");
                }
            }
            foreach (string subFile in subFiles)
            {
                AESMeta subFileHeader = AESMeta.GetHeaderFromFile(subFile);
                if (!subFileHeader.VerifyKey(key))
                {
                    throw new Exception("Could not decrypt directory because the given key was incorrect for one or more files.");
                }
            }
            foreach (string subFile in subFiles)
            {
                DecryptFile(subFile, key);
            }
        }
    }
}
