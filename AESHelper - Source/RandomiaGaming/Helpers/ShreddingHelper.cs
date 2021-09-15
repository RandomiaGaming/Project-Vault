using System;
using System.IO;
namespace RandomiaGaming.Helpers
{
    public enum ShreddingMethod { Zeros, Ones, Random };
    public static class ShreddingHelper
    {
        public static void ShredFolder(string folderPath, int passes = 1, ShreddingMethod method = ShreddingMethod.Random)
        {
            if (folderPath == null)
            {
                throw new ArgumentException("Shred operation was aborted because the given folder path was null.");
            }
            if (folderPath == "")
            {
                throw new ArgumentException("Shred operation was aborted because the given folder path was empty.");
            }
            if (!Directory.Exists(folderPath))
            {
                throw new ArgumentException("Shred operation was aborted because the given folder path did not exist or was inaccessible.");
            }
            try
            {
                foreach (string subFolderPath in Directory.GetDirectories(folderPath))
                {
                    ShredFolder(subFolderPath, passes, method);
                }
                foreach (string filePath in Directory.GetFiles(folderPath))
                {
                    ShredFile(filePath, passes, method);
                }
                Directory.Delete(folderPath);
            }
            catch (Exception ex)
            {
                Exception rethrowException;
                try
                {
                    rethrowException = new Exception($"Shred operation was aborted because a \"{ex.GetType().FullName}\" was thrown: \"{ex.Message}\".");
                }
                catch
                {
                    rethrowException = new Exception("Shred operation was aborted because an unknown exception was thrown.");
                }
                throw rethrowException;
            }
        }
        public static void ShredFile(string filePath, int passes = 1, ShreddingMethod method = ShreddingMethod.Random)
        {
            if (filePath == null)
            {
                throw new ArgumentException("Shred operation was aborted because the given file path was null.");
            }
            if (filePath == "")
            {
                throw new ArgumentException("Shred operation was aborted because the given file path was empty.");
            }
            if (!File.Exists(filePath))
            {
                throw new ArgumentException("Shred operation was aborted because the given file path did not exist or was inaccessible.");
            }
            try
            {
                FileStream fileStream = File.Open(filePath, FileMode.Open);
                for (int p = 1; p <= passes; p++)
                {
                    fileStream.Seek(0, SeekOrigin.Begin);
                    byte[] buffer = new byte[fileStream.Length];
                    switch (method)
                    {
                        case ShreddingMethod.Zeros:
                            for (int i = 0; i < buffer.Length; i++)
                            {
                                buffer[i] = byte.MinValue;
                            }
                            break;
                        case ShreddingMethod.Ones:
                            for (int i = 0; i < buffer.Length; i++)
                            {
                                buffer[i] = byte.MaxValue;
                            }
                            break;
                        case ShreddingMethod.Random:
                            RandomnessHelper.NextBytes(buffer);
                            break;
                        default:
                            RandomnessHelper.NextBytes(buffer);
                            break;
                    }
                    fileStream.Write(buffer, 0, buffer.Length);
                    fileStream.Flush();
                }
                fileStream.Close();
                fileStream.Dispose();
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                Exception rethrowException;
                try
                {
                    rethrowException = new Exception($"Shred operation was aborted because a \"{ex.GetType().FullName}\" was thrown: \"{ex.Message}\".");
                }
                catch
                {
                    rethrowException = new Exception("Shred operation was aborted because an unknown exception was thrown.");
                }
                throw rethrowException;
            }
        }
    }
}