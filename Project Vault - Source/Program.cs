using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using System.Windows.Forms;

namespace ProjectVault
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            try
            {
                RunCommand(GetArgumentCommand());
            }
            catch (Exception ex)
            {
                ExceptionHelper.HandleException(ex);
            }
        }
        #region Command Processing
        public static string GetArgumentCommand()
        {
            List<string> arguments = new List<string>(Environment.GetCommandLineArgs());
            if (arguments.Count <= 0)
            {
                return "";
            }
            arguments.RemoveAt(0);
            string output = "";
            for (int i = 0; i < arguments.Count; i++)
            {
                if (arguments[i].Contains(" "))
                {
                    if (i == 0)
                    {
                        output += $"\"{arguments[i]}\"";
                    }
                    else
                    {
                        output += $" \"{arguments[i]}\"";
                    }
                }
                else
                {
                    if (i == 0)
                    {
                        output += $"{arguments[i]}";
                    }
                    else
                    {
                        output += $" {arguments[i]}";
                    }
                }
            }
            return output;
        }
        public static void RunCommand(string command)
        {
            if (command is null)
            {
                throw new Exception("Could not run command because command is null.");
            }
            List<string> splitCommand = SplitCommand(command);
            if (splitCommand is null || splitCommand.Count < 1)
            {
                return;
            }
            string commandName = splitCommand[0];
            splitCommand.RemoveAt(0);
            string commandNameToUpper = commandName.ToUpper();
            List<string> arguments = splitCommand;
            switch (commandNameToUpper)
            {
                case "SHRED":
                    if (arguments.Count < 1)
                    {
                        throw new Exception("Could not run Shred command because no argument was provided for targetPath.");
                    }
                    else if (arguments.Count > 1)
                    {
                        throw new Exception("Could not run Shred command because too many arguments were provided.");
                    }
                    ShredCommand(arguments[0]);
                    break;
                case "SHREDFILE":
                    if (arguments.Count < 1)
                    {
                        throw new Exception("Could not run ShredFile command command because no argument was provided for filePath.");
                    }
                    else if (arguments.Count > 1)
                    {
                        throw new Exception("Could not run ShredFile command because too many arguments were provided.");
                    }
                    ShredFileCommand(arguments[0]);
                    break;
                case "SHREDDIRECTORY":
                    if (arguments.Count < 1)
                    {
                        throw new Exception("Could not run ShredDirectory command because no argument was provided for directoryPath.");
                    }
                    else if (arguments.Count > 1)
                    {
                        throw new Exception("Could not run ShredDirectory command because too many arguments were provided.");
                    }
                    ShredDirectoryCommand(arguments[0]);
                    break;
                case "ENCRYPT":
                    if (arguments.Count < 1)
                    {
                        throw new Exception("Could not run Encrypt command because no argument was provided for targetPath.");
                    }
                    else if (arguments.Count > 1)
                    {
                        throw new Exception("Could not run Encrypt command because too many arguments were provided.");
                    }
                    EncryptCommand(arguments[0]);
                    break;
                case "ENCRYPTFILE":
                    if (arguments.Count < 1)
                    {
                        throw new Exception("Could not run EncryptFile command because no argument was provided for filePath.");
                    }
                    else if (arguments.Count > 1)
                    {
                        throw new Exception("Could not run EncryptFile command because too many arguments were provided.");
                    }
                    EncryptFileCommand(arguments[0]);
                    break;
                case "ENCRYPTDIRECTORY":
                    if (arguments.Count < 1)
                    {
                        throw new Exception("Could not run EncryptDirectory command because no argument was provided for directoryPath.");
                    }
                    else if (arguments.Count > 1)
                    {
                        throw new Exception("Could not run EncryptDirectory command because too many arguments were provided.");
                    }
                    EncryptDirectoryCommand(arguments[0]);
                    break;
                case "DECRYPT":
                    if (arguments.Count < 1)
                    {
                        throw new Exception("Could not run Decrypt command because no argument was provided for targetPath.");
                    }
                    else if (arguments.Count > 1)
                    {
                        throw new Exception("Could not run Decrypt command because too many arguments were provided.");
                    }
                    DecryptCommand(arguments[0]);
                    break;
                case "DECRYPTFILE":
                    if (arguments.Count < 1)
                    {
                        throw new Exception("Could not run DecryptFile command because no argument was provided for filePath.");
                    }
                    else if (arguments.Count > 1)
                    {
                        throw new Exception("Could not run DecryptFile command because too many arguments were provided.");
                    }
                    DecryptFileCommand(arguments[0]);
                    break;
                case "DECRYPTDIRECTORY":
                    if (arguments.Count < 1)
                    {
                        throw new Exception("Could not run DecryptDirectory command because no argument was provided for directoryPath.");
                    }
                    else if (arguments.Count > 1)
                    {
                        throw new Exception("Could not run DecryptDirectory command because too many arguments were provided.");
                    }
                    DecryptDirectoryCommand(arguments[0]);
                    break;
                default:
                    throw new Exception($"No command exists with the name {commandName}");
            }
        }
        public static List<string> SplitCommand(string source)
        {
            if (source is null)
            {
                throw new Exception($"Could not split command because source was null.");
            }
            else if (source == "")
            {
                return new List<string>();
            }
            List<string> output = new List<string>();

            string currentStatement = "";
            bool inQuotes = false;
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] == '"')
                {
                    if (currentStatement != "")
                    {
                        output.Add(currentStatement);
                    }
                    currentStatement = "";
                    inQuotes = !inQuotes;
                }
                else if (source[i] == ' ' && !inQuotes)
                {
                    if (currentStatement != "")
                    {
                        output.Add(currentStatement);
                    }
                    currentStatement = "";
                }
                else
                {
                    currentStatement += source[i];
                }
            }
            if (inQuotes)
            {
                throw new Exception("Could not split command because quotes were imbalanced.");
            }
            else if (currentStatement != "")
            {
                output.Add(currentStatement);
            }
            return output;
        }
        #endregion
        #region Commands
        public static void ShredCommand(string targetPath)
        {
            if (targetPath is null || targetPath == "")
            {
                throw new Exception("Could not run Shred command because targetPath is null or empty.");
            }
            else if (File.Exists(targetPath))
            {
                ShredFileCommand(targetPath);
            }
            else if (Directory.Exists(targetPath))
            {
                ShredDirectoryCommand(targetPath);
            }
            else
            {
                throw new Exception("Could not run Shred command because targetPath does not exist.");
            }
        }
        public static void ShredFileCommand(string filePath)
        {
            ShreddingHelper.ShredFile(filePath);
        }
        public static void ShredDirectoryCommand(string directoryPath)
        {
            ShreddingHelper.ShredDirectory(directoryPath);
        }
        public static void EncryptCommand(string targetPath)
        {
            if (targetPath is null || targetPath == "")
            {
                throw new Exception("Could not run Encrypt command because targetPath is null or empty.");
            }
            else if (File.Exists(targetPath))
            {
                EncryptFileCommand(targetPath);
            }
            else if (Directory.Exists(targetPath))
            {
                EncryptDirectoryCommand(targetPath);
            }
            else
            {
                throw new Exception("Could not run Encrypt command because targetPath does not exist.");
            }
        }
        public static void EncryptFileCommand(string filePath)
        {
            if (filePath is null || filePath == "")
            {
                throw new Exception("Could not run EncryptFile command because filePath is null or empty.");
            }
            else if (!File.Exists(filePath))
            {
                throw new Exception("Could not run EncryptFile command because filePath does not exist.");
            }
            else if (Path.GetExtension(filePath).ToUpper() == ".AES")
            {
                throw new Exception("Could not run EncryptFile command because the file was already encrypted.");
            }
            string password = PromptForPasswordWithConfirmation();
            byte[] passwordBytes = StringEncodingHelper.StringToBytes(password);
            AESHelper.EncryptFile(filePath, passwordBytes);
        }
        public static void EncryptDirectoryCommand(string directoryPath)
        {
            if (directoryPath is null || directoryPath == "")
            {
                throw new Exception("Could not run EncryptDirectory command because directoryPath is null or empty.");
            }
            else if (!Directory.Exists(directoryPath))
            {
                throw new Exception("Could not run EncryptDirectory command because directoryPath does not exist.");
            }
            string password = PromptForPasswordWithConfirmation();
            byte[] passwordBytes = StringEncodingHelper.StringToBytes(password);
            AESHelper.EncryptDirectory(directoryPath, passwordBytes);
        }
        public static void DecryptCommand(string targetPath)
        {
            if (targetPath is null || targetPath == "")
            {
                throw new Exception("Could not run Decrypt command because targetPath is null or empty.");
            }
            else if (File.Exists(targetPath))
            {
                DecryptFileCommand(targetPath);
            }
            else if (Directory.Exists(targetPath))
            {
                DecryptDirectoryCommand(targetPath);
            }
            else
            {
                throw new Exception("Could not run Decrypt command because targetPath does not exist.");
            }
        }
        public static void DecryptFileCommand(string filePath)
        {
            if (filePath is null || filePath == "")
            {
                throw new Exception("Could not run DecryptFile command because filePath is null or empty.");
            }
            else if (!File.Exists(filePath))
            {
                throw new Exception("Could not run DecryptFile command because filePath does not exist.");
            }
            else if (Path.GetExtension(filePath).ToUpper() != ".AES")
            {
                throw new Exception("Could not run DecryptFile command because the file was not encrypted.");
            }
            string password = PromptForPassword();
            byte[] passwordBytes = StringEncodingHelper.StringToBytes(password);
            AESHelper.DecryptFile(filePath, passwordBytes);
        }
        public static void DecryptDirectoryCommand(string directoryPath)
        {
            if (directoryPath is null || directoryPath == "")
            {
                throw new Exception("Could not run DecryptDirectory command because directoryPath is null or empty.");
            }
            else if (!Directory.Exists(directoryPath))
            {
                throw new Exception("Could not run DecryptDirectory command because directoryPath does not exist.");
            }
            string password = PromptForPassword();
            byte[] passwordBytes = StringEncodingHelper.StringToBytes(password);
            AESHelper.DecryptDirectory(directoryPath, passwordBytes);
        }
        #endregion
        #region Prompts
        public static string PromptForPassword()
        {
            string password = Interaction.InputBox("Please type your password below and press enter:", "Enter Password", "", -1, -1);
            if (password is null || password == "")
            {
                ExceptionHelper.KillAndWait();
                return "";
            }
            else
            {
                return password;
            }
        }
        public static string PromptForPasswordWithConfirmation()
        {
            while (true)
            {
                string password = Interaction.InputBox("Please type your password below and press enter:", "Enter Password", "", -1, -1);
                if (password is null || password == "")
                {
                    ExceptionHelper.KillAndWait();
                }
                string passwordConfirmation = Interaction.InputBox("Please confirm your password below and press enter:", "Confirm Password", "", -1, -1);
                if (passwordConfirmation is null || passwordConfirmation == "")
                {
                    ExceptionHelper.KillAndWait();
                }
                else if (password == passwordConfirmation)
                {
                    return password;
                }
                else if (MessageBox.Show("Passwords do not match. Please try again.", "Non-matching Passwords", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                {
                    ExceptionHelper.KillAndWait();
                }
            }
        }
        #endregion
    }
}