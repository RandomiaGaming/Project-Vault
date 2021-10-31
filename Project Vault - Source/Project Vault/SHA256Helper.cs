using System;
using System.IO;
using System.Security.Cryptography;

namespace ProjectVault
{
    public static class SHA256Helper
    {
        public static byte[] HashStream(Stream source)
        {
            if (source is null)
            {
                throw new Exception("Could not hash stream because source is null.");
            }
            else if (!source.CanRead)
            {
                throw new Exception("Could not hash stream because source is not readable.");
            }
            else if (!source.CanSeek)
            {
                throw new Exception("Could not hash stream because source does not support seeking.");
            }
            SHA256 hash = SHA256.Create();
            source.Position = 0;
            byte[] output = hash.ComputeHash(source);
            hash.Dispose();
            return output;
        }
        public static byte[] HashBytes(byte[] source)
        {
            if (source is null)
            {
                throw new Exception("Could not hash bytes because source is null.");
            }
            MemoryStream sourceStream = new MemoryStream(source);
            byte[] output = HashStream(sourceStream);
            sourceStream.Dispose();
            return output;
        }
        public static byte[] HashFile(string filePath)
        {
            if (filePath is null || filePath == "")
            {
                throw new Exception("Could not hash file beacuse file path is null or empty.");
            }
            if (!File.Exists(filePath))
            {
                throw new Exception($"Could not hash file because file path does not exist.");
            }
            FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite);
            byte[] output = HashStream(fileStream);
            fileStream.Dispose();
            return output;
        }
        public static byte[] HashDirectory(string directoryPath)
        {
            if (directoryPath is null || directoryPath == "")
            {
                throw new Exception("Could not hash directory beacuse directory path is null or empty.");
            }
            else if (!Directory.Exists(directoryPath))
            {
                throw new Exception($"Could not hash directory because directory path does not exist.");
            }
            string[] subFiles = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
            byte[] fileHashes = new byte[0];
            foreach (string subFile in subFiles)
            {
                fileHashes = ArrayHelper.MergeArrays(fileHashes, HashFile(subFile));
            }
            return HashBytes(fileHashes);
        }
    }
}
