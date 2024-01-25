using System;
using System.Collections.Generic;
using System.IO;

namespace ProjectVault
{
    public enum ShreddingMethod { Zeros, Random };
    public static class ShreddingHelper
    {
        private static void ShredStreamZeros(Stream source, int passes = 1)
        {
            if (passes <= 0)
            {
                throw new Exception("Could not shred stream because passes was less than or equal to 0.");
            }
            else if (source is null)
            {
                throw new Exception("Could not shred stream because source is null.");
            }
            else if (!source.CanWrite)
            {
                throw new Exception("Could not shred stream because source is not writable.");
            }
            else if (!source.CanSeek)
            {
                throw new Exception("Could not shred stream because stream does not support seeking.");
            }
            else if (source.Length == 0)
            {
                return;
            }
            ulong totalMemory = new ComputerInfo().TotalPhysicalMemory;
            long chunkSize = (long)Math.Ceiling(totalMemory / 100.0);
            if (chunkSize > int.MaxValue)
            {
                chunkSize = int.MaxValue;
            }
            for (int p = 0; p < passes; p++)
            {
                source.Position = 0;
                long currentIndex = 0;
                while (currentIndex < source.Length)
                {
                    long bufferSize = source.Length - currentIndex;
                    if (bufferSize > chunkSize)
                    {
                        bufferSize = chunkSize;
                    }
                    byte[] buffer = new byte[(int)bufferSize];
                    source.Write(buffer, 0, buffer.Length);
                    currentIndex += bufferSize;
                }
                source.Flush();
            }
        }
        private static void ShredStreamRandom(Stream source, int passes = 1)
        {
            if (passes <= 0)
            {
                throw new Exception("Could not shred stream because passes was less than or equal to 0.");
            }
            else if (source is null)
            {
                throw new Exception("Could not shred stream because source is null.");
            }
            else if (!source.CanWrite)
            {
                throw new Exception("Could not shred stream because source is not writable.");
            }
            else if (!source.CanSeek)
            {
                throw new Exception("Could not shred stream because stream does not support seeking.");
            }
            else if (source.Length == 0)
            {
                return;
            }
            ulong totalMemory = new ComputerInfo().TotalPhysicalMemory;
            long chunkSize = (long)Math.Ceiling(totalMemory / 100.0);
            if (chunkSize > int.MaxValue)
            {
                chunkSize = int.MaxValue;
            }
            for (int p = 0; p < passes; p++)
            {
                source.Position = 0;
                long currentIndex = 0;
                while (currentIndex < source.Length)
                {
                    long bufferSize = source.Length - currentIndex;
                    if (bufferSize > chunkSize)
                    {
                        bufferSize = chunkSize;
                    }
                    byte[] buffer = RandomnessHelper.NextBytes((int)bufferSize);
                    source.Write(buffer, 0, buffer.Length);
                    currentIndex += bufferSize;
                }
                source.Flush();
            }
        }
        public static void ShredStream(Stream source, int passes = 1, ShreddingMethod method = ShreddingMethod.Zeros)
        {
            if (method == ShreddingMethod.Random)
            {
                ShredStreamRandom(source, passes);
            }
            else if (method == ShreddingMethod.Zeros)
            {
                ShredStreamZeros(source, passes);
            }
            else
            {
                ShredStreamRandom(source, passes);
            }
        }
        public static void ShredFile(string filePath, int passes = 1, ShreddingMethod method = ShreddingMethod.Zeros)
        {
            if (passes <= 0)
            {
                throw new Exception("Could not shred stream because passes was less than or equal to 0.");
            }
            else if (filePath is null || filePath == "")
            {
                throw new Exception("Could not shred file because file path is null or empty.");
            }
            if (!File.Exists(filePath))
            {
                throw new Exception("Could not shred file because file path does not exist.");
            }
            FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.Write);
            ShredStream(fileStream, passes, method);
            fileStream.Dispose();
            File.Delete(filePath);
        }
        public static void ShredDirectory(string directoryPath, int passes = 1, ShreddingMethod method = ShreddingMethod.Zeros)
        {
            if (passes <= 0)
            {
                throw new Exception("Could not shred directory because passes was less than or equal to 0.");
            }
            else if (directoryPath is null || directoryPath == "")
            {
                throw new Exception("Could not shred directory because directory path is null or empty.");
            }
            if (!Directory.Exists(directoryPath))
            {
                throw new Exception("Could not shred directory because directory path does not exist.");
            }
            List<string> subFiles = new List<string>(Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories));
            foreach (string subFile in subFiles)
            {
                ShredFile(subFile, passes, method);
            }
            Directory.Delete(directoryPath, true);
        }
    }
}
