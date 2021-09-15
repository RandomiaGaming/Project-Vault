using System;
using System.Diagnostics;
using RandomiaGaming.Helpers;
using System.IO;
using Microsoft.VisualBasic;
using System.Windows.Forms;

namespace AESHelper
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                if (args is null)
                {
                    throw new NullReferenceException("Command line arguments were null which should be impossible.");
                }
                else if (args.Length == 0)
                {
                    throw new ArgumentException("No command line arguments were given.");
                }
                else if (args.Length > 2)
                {
                    throw new ArgumentException("Too many command line arguments were given.");
                }
                else
                {
                    string commandName = args[0];

                    if (commandName is null)
                    {
                        throw new NullReferenceException("Command name was null which should be impossible.");
                    }
                    else if (commandName.ToLower() == "encrypt")
                    {
                        if (args.Length < 2)
                        {
                            throw new ArgumentException("No file or folder was specified to encrypt.");
                        }
                        else
                        {
                            string targetPath = args[1];
                            EncryptFileOrFolder(targetPath);
                        }
                    }
                    else if (commandName.ToLower() == "decrypt")
                    {
                        if (args.Length < 2)
                        {
                            throw new ArgumentException("No file or folder was specified to decrypt.");
                        }
                        else
                        {
                            string targetPath = args[1];
                            DecryptFile(targetPath);
                        }
                    }
                    else
                    {
                        throw new Exception($"\"{commandName}\" is not a valid command.");
                    }
                }

            }
            catch (Exception ex)
            {
                try
                {
                    MessageBox.Show(ex.Message, "Exception Thrown", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch
                {
                    try
                    {
                        Console.WriteLine("An unknown exception was thrown.");
                    }
                    catch
                    {

                    }
                }
            }
            Process.GetCurrentProcess().Kill();
        }
        public static void EncryptFile(string targetFilePath)
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
        public static void EncryptFolder(string targetFolderPath)
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
        public static void DecryptFile(string targetPath)
        {
            if (targetPath is null || targetPath == "")
            {
                Console.WriteLine("Could not lock file beacuse target file path was null.");
            }
            else
            {
                if (File.Exists(targetPath))
                {
                    Console.WriteLine($"Locking \"{targetPath}\".");

                    byte[] fileContents = File.ReadAllBytes(targetPath);
                    if (fileContents is null || fileContents.Length == 0)
                    {
                        Console.WriteLine($"Could not lock file \"{targetPath}\" because the file was empty.");
                    }
                    else
                    {
                        try
                        {
                            string password = PromptForPasswordWithConfirmation();
                            File.WriteAllBytes(targetPath, Encrypt(File.ReadAllBytes(targetPath), Hash(EncodeString(password))));
                            File.Move(targetPath, targetPath + ".aes");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Could not lock file \"{targetPath}\" due to exception: {ex.Message}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Could not lock file file \"{targetPath}\" because target file path was invalid.");
                }
            }
        }

        public static string PromptForPassword()
        {
            try
            {
                while (true)
                {
                    string password = Interaction.InputBox("Please enter your password below.", "Enter Password", "", -1, -1);
                    if (password is null || password == "")
                    {
                        MessageBox.Show("Password cannot be null or empty. Please try again.", "Invalid Password", MessageBoxButtons.OK);
                    }
                    else
                    {
                        return password;
                    }
                }
            }
            catch (Exception ex)
            {
                Exception rethrowException;
                try
                {
                    rethrowException = new Exception($"Could not get password due to exception \"{ex.Message}\".");
                }
                catch
                {
                    rethrowException = new Exception("Could not get password due to an unknown exception.");
                }
                throw rethrowException;
            }
        }
        private static string PromptForPasswordWithConfirmation()
        {
            try
            {
                while (true)
                {
                    string password = Interaction.InputBox("Please enter your password below.", "Enter Password", "", -1, -1);
                    if (password is null || password == "")
                    {
                        MessageBox.Show("Password cannot be null or empty. Please try again.", "Invalid Password", MessageBoxButtons.OK);
                    }
                    else
                    {
                        string passwordConfirmation = Interaction.InputBox("Please confirm your password below.", "Confirm Password", "", -1, -1);
                        if (passwordConfirmation == password)
                        {
                            return password;
                        }
                        else
                        {
                            MessageBox.Show("Passwords did not match. Please try again.", "Non-matching Passwords", MessageBoxButtons.OK);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Exception rethrowException;
                try
                {
                    rethrowException = new Exception($"Could not get password due to exception \"{ex.Message}\".");
                }
                catch
                {
                    rethrowException = new Exception("Could not get password due to an unknown exception.");
                }
                throw rethrowException;
            }
        }
    }
}