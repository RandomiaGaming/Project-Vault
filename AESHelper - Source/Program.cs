using System;
using System.IO;
using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using System.Security.Cryptography;

namespace AESHelper
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            string[] arguments = Environment.GetCommandLineArgs();
            if (arguments is null || arguments.Length < 2)
            {
                Console.WriteLine("Could not process arguments because no arguments were given.");
            }
            else if (arguments.Length > 2)
            {
                Console.WriteLine("Could not process arguments because too many arguments were given.");
            }
            else
            {
                string targetPath = arguments[1];
                LockOrUnlockFileOrFolder(targetPath);
            }
            Console.WriteLine("Press any key to close this window.");
            Console.ReadKey();
            Process.GetCurrentProcess().Kill();
        }
        public static void LockOrUnlockFileOrFolder(string targetPath)
        {
            if (targetPath is null || targetPath == "")
            {
                Console.WriteLine("Could not lock or unlock file or folder because target path was null.");
            }
            else
            {
                if (File.Exists(targetPath))
                {
                    if (targetPath.ToUpper().EndsWith(".AES"))
                    {
                        UnlockFile(targetPath);
                    }
                    else
                    {
                        LockFile(targetPath);
                    }
                }
                else if (Directory.Exists(targetPath))
                {
                    LockFolder(targetPath);
                }
                else
                {
                    Console.WriteLine($"Could not lock or unlock file or folder \"{targetPath}\" because target path was invalid.");
                }
            }
        }
        public static void UnlockFile(string targetFilePath)
        {
            if (targetFilePath is null || targetFilePath == "")
            {
                Console.WriteLine("Could not unlock file because target file path was null.");
            }
            else
            {
                if (File.Exists(targetFilePath))
                {
                    Console.WriteLine($"Unlocking \"{targetFilePath}\".");

                    byte[] fileContents = File.ReadAllBytes(targetFilePath);
                    if (fileContents is null || fileContents.Length == 0)
                    {
                        Console.WriteLine($"Could not unlock file \"{targetFilePath}\" because the file was empty.");
                    }
                    else
                    {
                        bool decryptionSuccessful = false;
                        while (!decryptionSuccessful)
                        {
                            try
                            {
                                string password = PromptForPassword(false);
                                File.WriteAllBytes(targetFilePath, Decrypt(File.ReadAllBytes(targetFilePath), Hash(EncodeString(password))));
                                decryptionSuccessful = true;
                            }
                            catch
                            {
                                Console.WriteLine($"Something went wrong when unlocking file \"{targetFilePath}\". Most likely the given password was incorrect. Please try again.");
                                decryptionSuccessful = false;
                            }
                        }
                        Console.WriteLine($"Successfully unlocked \"{targetFilePath}\".");

                        if (targetFilePath.ToUpper().EndsWith(".AES"))
                        {
                            string newfilePath = targetFilePath.Substring(0, targetFilePath.Length - 4);
                            File.Move(targetFilePath, newfilePath);
                            targetFilePath = newfilePath;
                        }

                        if (targetFilePath.ToUpper().EndsWith(".ZIP"))
                        {
                            try
                            {
                                ZipFile.ExtractToDirectory(targetFilePath, targetFilePath.Substring(0, targetFilePath.Length - 4));
                                File.Delete(targetFilePath);
                            }
                            catch
                            {
                                Console.WriteLine($"Something went wrong when trying to extract \"{targetFilePath}\".");
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Could not unlock file \"{targetFilePath}\" because target file path was invalid.");
                }
            }
        }
        public static void LockFile(string targetFilePath)
        {
            if (targetFilePath is null || targetFilePath == "")
            {
                Console.WriteLine("Could not lock file beacuse target file path was null.");
            }
            else
            {
                if (File.Exists(targetFilePath))
                {
                    Console.WriteLine($"Locking \"{targetFilePath}\".");

                    byte[] fileContents = File.ReadAllBytes(targetFilePath);
                    if (fileContents is null || fileContents.Length == 0)
                    {
                        Console.WriteLine($"Could not lock file \"{targetFilePath}\" because the file was empty.");
                    }
                    else
                    {
                        try
                        {
                            string password = PromptForPassword(true);
                            File.WriteAllBytes(targetFilePath, Encrypt(File.ReadAllBytes(targetFilePath), Hash(EncodeString(password))));
                            File.Move(targetFilePath, targetFilePath + ".aes");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Could not lock file \"{targetFilePath}\" due to exception: {ex.Message}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Could not lock file file \"{targetFilePath}\" because target file path was invalid.");
                }
            }
        }
        public static void LockFolder(string targetFolderPath)
        {
            if (targetFolderPath is null || targetFolderPath == "")
            {
                Console.WriteLine("Could not lock folder beacuse folder path was null.");
            }
            else
            {
                if (Directory.Exists(targetFolderPath))
                {
                    try
                    {
                        string password = PromptForPassword(true);
                        string zipFilePath = $"{targetFolderPath}.zip";
                        ZipFile.CreateFromDirectory(targetFolderPath, zipFilePath);
                        Directory.Delete(targetFolderPath, true);
                        File.WriteAllBytes(zipFilePath, Encrypt(File.ReadAllBytes(zipFilePath), Hash(EncodeString(password))));
                        File.Move(zipFilePath, zipFilePath + ".aes");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Could not lock file \"{targetFolderPath}\" due to exception: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"Could not lock folder \"{targetFolderPath}\" because target folder path was invalid.");
                }
            }
        }

        private static string PromptForPassword(bool requireConfirmation)
        {
            bool validPassword = false;
            string password = null;
            while (!validPassword)
            {
                Console.WriteLine($"Please enter your password below.");
                password = Console.ReadLine();
                if (password == "mydick")
                {
                    Console.WriteLine("Password was too short. Please try again.");
                }
                else if (!(password is null) && password != "")
                {
                    if (requireConfirmation)
                    {
                        Console.WriteLine("Please confirm your password by typing it again.");
                        string passwordConfirmation = Console.ReadLine();
                        if (passwordConfirmation != password)
                        {
                            Console.WriteLine("Passwords did not match. Please try again.");
                            validPassword = false;
                        }
                        else
                        {
                            validPassword = true;
                        }
                    }
                    else
                    {
                        validPassword = true;
                    }
                }
                else
                {
                    Console.WriteLine("Password cannot be empty.");
                    validPassword = false;
                }
            }
            return password;
        }

        public static byte[] Decrypt(byte[] data, byte[] key)
        {
            if (data is null)
            {
                throw new NullReferenceException("Decrypting operation was aborted because the target data was null.");
            }
            if (data.Length <= 0)
            {
                throw new ArgumentException("Decrypting operation was aborted because the target data had a length of zero.");
            }
            if (key is null)
            {
                throw new NullReferenceException("Decrypting operation was aborted because the given key was null.");
            }
            if (key.Length <= 0)
            {
                throw new ArgumentException("Decrypting operation was aborted because the given key had a length of zero.");
            }

            Aes aes = Aes.Create();

            aes.KeySize = key.Length * 8;
            aes.IV = new byte[aes.BlockSize / 8];
            aes.Key = key;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.ISO10126;

            ICryptoTransform decryptor = aes.CreateDecryptor();
            MemoryStream ms = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Write);
            cryptoStream.Write(data, 0, data.Length);
            cryptoStream.FlushFinalBlock();
            return ms.ToArray();
        }
        public static byte[] Encrypt(byte[] data, byte[] key)
        {
            if (data is null)
            {
                throw new NullReferenceException("Encrypting operation was aborted because the target data was null.");
            }
            if (data.Length <= 0)
            {
                throw new ArgumentException("Encrypting operation was aborted because the target data had a length of zero.");
            }
            if (key is null)
            {
                throw new NullReferenceException("Encrypting operation was aborted because the given key was null.");
            }
            if (key.Length <= 0)
            {
                throw new ArgumentException("Encrypting operation was aborted because the given key had a length of zero.");
            }
            Aes aes = Aes.Create();

            aes.KeySize = key.Length * 8;
            aes.IV = new byte[aes.BlockSize / 8];
            aes.Key = key;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.ISO10126;

            ICryptoTransform decryptor = aes.CreateEncryptor();
            MemoryStream ms = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Write);
            cryptoStream.Write(data, 0, data.Length);
            cryptoStream.FlushFinalBlock();
            return ms.ToArray();
        }
        public static byte[] Hash(byte[] data)
        {
            if (data is null)
            {
                throw new NullReferenceException("Hashing operation was aborted because the target data was null.");
            }
            if (data.Length <= 0)
            {
                throw new ArgumentException("Hashing operation was aborted because the target data had a length of zero.");
            }
            SHA256 hash = SHA256.Create();
            return hash.ComputeHash(data);
        }

        public static byte[] EncodeString(string source)
        {
            if (source is null)
            {
                return new byte[0];
            }
            if (source.Length <= 0)
            {
                return new byte[0];
            }
            return Encoding.Unicode.GetBytes(source);
        }
        public static string DecodeString(byte[] data)
        {
            if (data is null)
            {
                return "";
            }
            if (data.Length <= 0)
            {
                return "";
            }
            return Encoding.Unicode.GetString(data);
        }
    }
}