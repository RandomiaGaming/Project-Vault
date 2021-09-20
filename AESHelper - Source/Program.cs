using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using System.Windows.Forms;
using System.IO.Compression;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace AESHelper
{
    public static class Program
    {
        private static readonly Random RNG = new Random();
        [STAThread]
        public static void Main()
        {
            try
            {
                RunCommand(GetArgumentCommand());
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }
        #region Command Processing
        public static string GetArgumentCommand()
        {
            try
            {
                List<string> arguments = new List<string>(Environment.GetCommandLineArgs());
                if (arguments.Count >= 1)
                {
                    arguments.RemoveAt(0);
                }
                else
                {
                    return "";
                }
                string output = "";
                foreach (string arg in arguments)
                {
                    if (arg.Contains(" "))
                    {
                        output += $" \"{arg}\"";
                    }
                    else
                    {
                        output += $" {arg}";
                    }
                }
                return output;
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not get command from arguments because of exception: {GetExceptionMessage(ex)}");
            }
        }
        public static void RunCommand(string command)
        {
            if (command is null)
            {
                throw new Exception("Could not run command because command is null.");
            }
            if (command == "")
            {
                return;
            }
            List<string> splitCommand = SplitCommand(command);
            string commandName = splitCommand[0];
            string commandNameToUpper = commandName.ToUpper();

            switch (commandNameToUpper)
            {
                case "ENCRYPTFILE":
                    if (splitCommand.Count < 2)
                    {
                        throw new Exception("Could not run command EncryptFile because no argument was provided for file path.");
                    }
                    else if (splitCommand.Count > 2)
                    {
                        throw new Exception("Could not run command EncryptFile because too many arguments were provided.");
                    }
                    EncryptFile(splitCommand[1]);
                    break;
                case "ENCRYPTDIRECTORY":
                    if (splitCommand.Count < 2)
                    {
                        throw new Exception("Could not run command EncryptDirectory because no argument was provided for directory path.");
                    }
                    else if (splitCommand.Count > 2)
                    {
                        throw new Exception("Could not run command EncryptDirectory because too many arguments were provided.");
                    }
                    EncryptDirectory(splitCommand[1]);
                    break;
                case "DECRYPTFILE":
                    if (splitCommand.Count < 2)
                    {
                        throw new Exception("Could not run command DecryptFile because no argument was provided for file path.");
                    }
                    else if (splitCommand.Count > 2)
                    {
                        throw new Exception("Could not run command DecryptFile because too many arguments were provided.");
                    }
                    DecryptFile(splitCommand[1]);
                    break;
                case "SHREDFILE":
                    if (splitCommand.Count < 2)
                    {
                        throw new Exception("Could not run command ShredFile because no argument was provided for file path.");
                    }
                    else if (splitCommand.Count > 2)
                    {
                        throw new Exception("Could not run command ShredFile because too many arguments were provided.");
                    }
                    ShredFile(splitCommand[1]);
                    break;
                case "SHREDDIRECTORY":
                    if (splitCommand.Count < 2)
                    {
                        throw new Exception("Could not run command ShredDirectory because no argument was provided for directory path.");
                    }
                    else if (splitCommand.Count > 2)
                    {
                        throw new Exception("Could not run command ShredDirectory because too many arguments were provided.");
                    }
                    ShredDirectory(splitCommand[1]);
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
            if (source == "")
            {
                return new List<string>();
            }
            try
            {
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
                if (currentStatement != "" && !(currentStatement is null))
                {
                    output.Add(currentStatement);
                }

                return output;
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not split command because of exception: {GetExceptionMessage(ex)}");
            }
        }
        #endregion
        #region Shredding
        public enum ShreddingMethod { Random, Ones, Zeros };
        public static void ShredDirectory(string directoryPath, int passes = 3, ShreddingMethod method = ShreddingMethod.Random)
        {
            if (directoryPath == null)
            {
                throw new ArgumentException("Could not shred directory because directory path is null.");
            }
            if (!Directory.Exists(directoryPath))
            {
                throw new ArgumentException("Could not shred directory because directory path does not exist.");
            }
            try
            {
                foreach (string subFolderPath in Directory.GetDirectories(directoryPath))
                {
                    ShredDirectory(subFolderPath, passes, method);
                }
                foreach (string filePath in Directory.GetFiles(directoryPath))
                {
                    ShredFile(filePath, passes, method);
                }
                Directory.Delete(directoryPath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not shred directory because of exception: {GetExceptionMessage(ex)}");
            }
        }
        public static void ShredFile(string filePath, int passes = 3, ShreddingMethod method = ShreddingMethod.Random)
        {
            if (filePath == null)
            {
                throw new ArgumentException("Could not shred file because file path is null.");
            }
            if (!File.Exists(filePath))
            {
                throw new ArgumentException("Could not shred file because file path does not exist.");
            }
            try
            {
                int fileSize = File.ReadAllBytes(filePath).Length;
                for (int p = 1; p <= passes; p++)
                {
                    byte[] buffer = new byte[fileSize];
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
                            buffer = RandomBytes(buffer.Length);
                            break;
                        default:
                            buffer = RandomBytes(buffer.Length);
                            break;
                    }
                    File.WriteAllBytes(filePath, buffer);
                }
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not shred file because of exception: {GetExceptionMessage(ex)}");
            }
        }
        #endregion
        #region Hashing
        public static byte[] Hash(byte[] data)
        {
            if (data is null)
            {
                throw new Exception("Could not hash data because data is null.");
            }
            try
            {
                SHA256 hash = SHA256.Create();
                byte[] output = hash.ComputeHash(data);
                hash.Dispose();
                return output;
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not hash data because of exception: {GetExceptionMessage(ex)}");
            }
        }
        public static byte[] HashAndSalt(byte[] data, byte[] salt)
        {
            if (data is null)
            {
                throw new Exception($"Could not hash and salt data because data is null.");
            }
            if (salt is null)
            {
                throw new Exception($"Could not hash and salt data because salt is null.");
            }
            try
            {
                return Hash(MergeArrays(data, salt));
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not hash and salt data because of exception: {GetExceptionMessage(ex)}");
            }
        }
        #endregion
        #region General AES
        public enum DataType { File, Directory, Unknown };
        public sealed class EncryptedData
        {
            public DataType dataType = DataType.Unknown;
            public byte[] keySalt = new byte[32];
            public byte[] hashedKey = new byte[32];
            public byte[] IV = new byte[32];
            public byte[] encryptedData = new byte[0];
            private EncryptedData()
            {
                keySalt = new byte[32];
                hashedKey = new byte[32];
                IV = new byte[32];
                encryptedData = new byte[0];
            }
            public EncryptedData(DataType dataType, byte[] keySalt, byte[] hashedKey, byte[] IV, byte[] encryptedData)
            {
                this.dataType = dataType;
                this.keySalt = keySalt;
                this.hashedKey = hashedKey;
                this.IV = IV;
                this.encryptedData = encryptedData;
            }
        }
        public static bool VerifyKey(EncryptedData data, byte[] key)
        {
            if (data is null)
            {
                throw new NullReferenceException("Could not verify key because data is null.");
            }
            if (key is null)
            {
                throw new NullReferenceException("Could not verify key because key is null.");
            }
            try
            {
                byte[] givenLengthAdjustedKey = Hash(key);
                byte[] givenHashedKey = HashAndSalt(givenLengthAdjustedKey, data.keySalt);
                bool givenKeyCorrect = ArraysEqual(givenHashedKey, data.hashedKey);
                return givenKeyCorrect;
            }
            catch (Exception ex)
            {
                throw new Exception($"Could verify key because of exception: {GetExceptionMessage(ex)}");
            }
        }
        #endregion
        #region Encryption
        public static void EncryptFile(string filePath)
        {
            if (filePath is null || filePath == "")
            {
                throw new NullReferenceException("Could not encrypt file beacuse file path is null.");
            }
            if (!File.Exists(filePath))
            {
                throw new Exception($"Could not encrypt file because file does not exist.");
            }
            string newFilePath = filePath + ".aes";
            if (File.Exists(newFilePath))
            {
                throw new Exception($"Could not encrypt file because file already exists at new file path.");
            }
            try
            {
                byte[] decryptedData = File.ReadAllBytes(filePath);
                string password = PromptForPasswordWithConfirmation();
                byte[] passwordBytes = BinarySerializeString(password);
                EncryptedData encryptedData = Encrypt(decryptedData, passwordBytes);
                encryptedData.dataType = DataType.File;
                string encryptedDataJson = JsonSerialize(encryptedData);
                File.WriteAllText(newFilePath, encryptedDataJson);
                ShredFile(filePath, 3, ShreddingMethod.Random);
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not encrypt file because of exception: {GetExceptionMessage(ex)}");
            }
        }
        public static void EncryptDirectory(string directoryPath)
        {
            if (directoryPath is null || directoryPath == "")
            {
                throw new NullReferenceException("Could not encrypt directory beacuse directory path is null.");
            }
            if (!Directory.Exists(directoryPath))
            {
                throw new Exception($"Could not encrypt directory because directory path does not exist.");
            }
            string newFilePath = directoryPath + ".aes";
            if (File.Exists(newFilePath))
            {
                throw new Exception($"Could not encrypt directory because file already exists at new file path.");
            }
            string tempArchiveLocation = Path.GetTempFileName();
            try
            {
                File.Delete(tempArchiveLocation);
                ZipFile.CreateFromDirectory(directoryPath, tempArchiveLocation);
                byte[] decryptedData = File.ReadAllBytes(tempArchiveLocation);
                ShredFile(tempArchiveLocation);
                string password = PromptForPasswordWithConfirmation();
                byte[] passwordBytes = BinarySerializeString(password);
                EncryptedData encryptedData = Encrypt(decryptedData, passwordBytes);
                encryptedData.dataType = DataType.Directory;
                string encryptedDataJson = JsonSerialize(encryptedData);
                File.WriteAllText(newFilePath, encryptedDataJson);
                ShredDirectory(directoryPath, 3, ShreddingMethod.Random);
            }
            catch (Exception ex)
            {
                try
                {
                    ShredFile(tempArchiveLocation);
                }
                catch
                {

                }
                throw new Exception($"Could not encrypt directory because of exception: {GetExceptionMessage(ex)}");
            }
        }
        public static EncryptedData Encrypt(byte[] data, byte[] key)
        {
            if (data is null)
            {
                throw new NullReferenceException("Could not encrypt data because the data is null.");
            }
            if (key is null)
            {
                throw new NullReferenceException("Could not encrypt data because the key is null.");
            }
            try
            {
                byte[] lengthAdjustedKey = Hash(key);

                byte[] keySalt = RandomBytes(32);

                byte[] hashedKey = HashAndSalt(lengthAdjustedKey, keySalt);

                byte[] IV = RandomBytes(16);

                Aes aes = Aes.Create();

                aes.BlockSize = 128;
                aes.IV = IV;

                aes.KeySize = 256;
                aes.Key = lengthAdjustedKey;

                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.ISO10126;

                ICryptoTransform decryptor = aes.CreateEncryptor();
                MemoryStream outputStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(outputStream, decryptor, CryptoStreamMode.Write);
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();

                byte[] encryptedData = outputStream.ToArray();

                cryptoStream.Close();
                cryptoStream.Dispose();
                outputStream.Close();
                outputStream.Dispose();
                decryptor.Dispose();
                aes.Dispose();

                return new EncryptedData(DataType.Unknown, keySalt, hashedKey, IV, encryptedData);
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not encrypt data because of exception: {GetExceptionMessage(ex)}");
            }
        }
        #endregion
        #region Decryption
        public static void DecryptFile(string filePath)
        {
            if (filePath is null || filePath == "")
            {
                throw new Exception("Could not decrypt file because file path is null.");
            }
            if (!File.Exists(filePath))
            {
                throw new Exception($"Could not decrypt file because file path does not exist.");
            }
            string fileContents;
            try
            {
                fileContents = File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not decrypt file because of exception: {GetExceptionMessage(ex)}");
            }
            EncryptedData encryptedData;
            try
            {
                encryptedData = JsonDeserialize<EncryptedData>(fileContents);
            }
            catch
            {
                throw new Exception("Could not decrypt file because file does not follow the .aes format.");
            }
            byte[] decryptedData;
            try
            {
                bool correctKey = false;
                byte[] password = new byte[0];
                while (!correctKey)
                {
                    password = BinarySerializeString(PromptForPassword());
                    correctKey = VerifyKey(encryptedData, password);
                    if (!correctKey)
                    {
                        if (MessageBox.Show("The given password was incorrect. Please try again.", "Incorrect Password", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                        {
                            throw new Exception("User chose to cancel the operation.");
                        }
                    }
                }

                decryptedData = DecryptData(encryptedData, password);
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not decrypt file because of exception: {GetExceptionMessage(ex)}");
            }
            if (encryptedData.dataType == DataType.Directory)
            {
                string tempFilePath = Path.GetTempFileName();
                string newDirectoryPath;
                try
                {
                    File.WriteAllBytes(tempFilePath, decryptedData);
                    newDirectoryPath = Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath);
                }
                catch (Exception ex)
                {
                    try
                    {
                        ShredFile(tempFilePath);
                    }
                    catch
                    {

                    }
                    throw new Exception($"Could not decrypt file because of exception: {GetExceptionMessage(ex)}");
                }
                if (Directory.Exists(newDirectoryPath))
                {
                    throw new Exception("Could not decrypt file because directory already exists at new directory path.");
                }
                try
                {
                    ZipFile.ExtractToDirectory(tempFilePath, newDirectoryPath);
                    ShredFile(tempFilePath);
                    ShredFile(filePath);
                }
                catch (Exception ex)
                {
                    try
                    {
                        ShredFile(tempFilePath);
                    }
                    catch
                    {

                    }
                    throw new Exception($"Could not decrypt file because of exception: {GetExceptionMessage(ex)}");
                }
            }
            else
            {
                string newFilePath = filePath;
                try
                {
                    if (newFilePath.ToUpper().EndsWith(".AES"))
                    {
                        newFilePath = newFilePath.Substring(0, newFilePath.Length - 4);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Could not decrypt file because of exception: {GetExceptionMessage(ex)}");
                }
                if (File.Exists(newFilePath))
                {
                    throw new Exception("Could not decrypt file because file already exists at new file path.");
                }
                try
                {
                    File.WriteAllBytes(newFilePath, decryptedData);
                    ShredFile(filePath, 3, ShreddingMethod.Random);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Could not decrypt file because of exception: {GetExceptionMessage(ex)}");
                }
            }
        }
        public static byte[] DecryptData(EncryptedData data, byte[] key)
        {
            if (data is null)
            {
                throw new NullReferenceException("Could not decrypt data because the data is null.");
            }
            if (key is null)
            {
                throw new NullReferenceException("Could not decrypt data because the key is null.");
            }
            if (!VerifyKey(data, key))
            {
                throw new Exception("Could not decrypt data because the key was incorrect.");
            }
            try
            {
                byte[] lengthAdjustedKey = Hash(key);

                Aes aes = Aes.Create();

                aes.BlockSize = 128;
                aes.IV = data.IV;

                aes.KeySize = 256;
                aes.Key = lengthAdjustedKey;

                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.ISO10126;

                ICryptoTransform decryptor = aes.CreateDecryptor();
                MemoryStream outputStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(outputStream, decryptor, CryptoStreamMode.Write);
                cryptoStream.Write(data.encryptedData, 0, data.encryptedData.Length);
                cryptoStream.FlushFinalBlock();

                byte[] output = outputStream.ToArray();

                cryptoStream.Close();
                cryptoStream.Dispose();
                outputStream.Close();
                outputStream.Dispose();
                decryptor.Dispose();
                aes.Dispose();

                return output;
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not decrypt data because of exception: {GetExceptionMessage(ex)}");
            }
        }
        #endregion
        #region Exception Logging
        public static void LogException(Exception ex)
        {
            string exceptionLogEntry = GetExceptionLogEntry(ex);
            try
            {
                string exceptionLogLocation = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) + "\\ExceptionLog.txt";
                string exceptionLogContents;
                if (!File.Exists(exceptionLogLocation))
                {
                    File.WriteAllText(exceptionLogLocation, "");
                    exceptionLogContents = "";
                }
                else
                {
                    exceptionLogContents = File.ReadAllText(exceptionLogLocation);
                }
                File.WriteAllText(exceptionLogLocation, $"{exceptionLogEntry}\n{exceptionLogContents}");
            }
            catch
            {

            }
            try
            {
                MessageBox.Show($"{GetExceptionMessage(ex)}", "Exception Thrown", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch
            {

            }
            Process.GetCurrentProcess().Kill();
        }
        public static string GetExceptionLogEntry(Exception ex)
        {
            string exceptionType;
            try
            {
                exceptionType = ex.GetType().Name;
            }
            catch
            {
                exceptionType = "Exception";
            }
            string exceptionMessage = GetExceptionMessage(ex);
            string exceptionDateTime;
            try
            {
                exceptionDateTime = $"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}";
            }
            catch
            {
                exceptionDateTime = "00/00/0000 00:00:00";
            }
            return $"An exception of type \"{exceptionType}\" was thrown at \"{exceptionDateTime}\" with the message \"{exceptionMessage}\".";
        }
        public static string GetExceptionMessage(Exception ex)
        {
            string output;
            try
            {
                output = ex.Message;
                output = output.Replace("\n", "");
                output = output.Replace("\r", "");
                output = output.Replace("\t", "");
            }
            catch
            {
                output = "An unknown exception was thrown.";
            }
            return output;
        }
        #endregion
        #region Password Prompts
        public static string PromptForPassword()
        {
            string password;
            try
            {
                password = Interaction.InputBox("Please enter your password below.", "Enter Password", "", -1, -1);
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not get password from user because of exception: {GetExceptionMessage(ex)}");
            }
            if (password is null || password == "")
            {
                throw new Exception($"Could not get password from user because user refused.");
            }
            return password;
        }
        public static string PromptForPasswordWithConfirmation()
        {
            string password;
            try
            {
                while (true)
                {
                    password = Interaction.InputBox("Please enter your password below.", "Enter Password", "", -1, -1);
                    if (password is null || password == "")
                    {
                        break;
                    }
                    string passwordConfirmation = Interaction.InputBox("Please confirm your password below.", "Confirm Password", "", -1, -1);
                    if (passwordConfirmation == password)
                    {
                        break;
                    }
                    else
                    {
                        if (MessageBox.Show("Passwords does not match. Please try again.", "Non-matching Passwords", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                        {
                            throw new Exception($"Could not get password from user because user refused.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not get password from user because of exception: {GetExceptionMessage(ex)}");
            }
            if (password is null || password == "")
            {
                throw new Exception($"Could not get password from user because user refused.");
            }
            return password;
        }
        #endregion
        #region Array Opperations
        public static T[] MergeArrays<T>(T[] arrayA, T[] arrayB)
        {
            if (arrayA is null)
            {
                throw new NullReferenceException("Could not merge arrays because array A is null.");
            }
            if (arrayB is null)
            {
                throw new NullReferenceException("Could not merge arrays because array B is null.");
            }
            try
            {
                if (arrayA.Length == 0)
                {
                    return arrayB;
                }
                else if (arrayB.Length == 0)
                {
                    return arrayA;
                }
                else
                {
                    T[] output = new T[arrayA.Length + arrayB.Length];
                    Array.Copy(arrayA, 0, output, 0, arrayA.Length);
                    Array.Copy(arrayB, 0, output, arrayA.Length, arrayB.Length);
                    return output;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not merge arrays because of exception: {GetExceptionMessage(ex)}");
            }
        }
        public static T[] GetRangeFromArray<T>(T[] array, int startIndex, int length)
        {
            if (array is null)
            {
                throw new NullReferenceException("Could not get range from array because array is null.");
            }
            if (startIndex < 0 || startIndex >= array.Length)
            {
                throw new Exception("Could not get range from array because start index was outside the bounds of the array.");
            }
            if (length < 0)
            {
                throw new ArgumentException("Could not get range from array because length was negative.");
            }
            if (array.Length < startIndex + length)
            {
                throw new Exception("Could not get range from array because the specified range was too large to fit within the bounds of the array.");
            }
            try
            {
                T[] output = new T[length];
                Array.Copy(array, startIndex, output, 0, length);
                return output;
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not get range from array because of exception: {GetExceptionMessage(ex)}");
            }
        }
        public static bool ArraysEqual<T>(T[] arrayA, T[] arrayB)
        {
            if (arrayA is null && arrayB is null)
            {
                return true;
            }
            else if (arrayA is null || arrayB is null)
            {
                return false;
            }
            else if (arrayA.Length != arrayB.Length)
            {
                return false;
            }
            try
            {
                for (int i = 0; i < arrayA.Length; i++)
                {
                    if (!arrayA[i].Equals(arrayB[i]))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not determine if arrays were equal because of exception: {GetExceptionMessage(ex)}");
            }
        }
        public static T[] CloneArray<T>(T[] array)
        {
            if (array is null)
            {
                throw new NullReferenceException("Could not clone array because the array is null.");
            }
            try
            {
                T[] output = new T[array.Length];
                for (int i = 0; i < array.Length; i++)
                {
                    output[i] = array[i];
                }
                return output;
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not clone array because of exception: {GetExceptionMessage(ex)}");
            }
        }
        #endregion
        #region Serialization
        public static byte[] BinarySerializeString(string source)
        {
            if (source is null)
            {
                throw new Exception("Could not binary serialize string because the source string is null.");
            }
            try
            {
                return Encoding.Unicode.GetBytes(source);
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not binary serialize string because of exception: {GetExceptionMessage(ex)}");
            }
        }
        public static string BinaryDeserializeString(byte[] source)
        {
            if (source is null)
            {
                throw new Exception("Could not binary deserialize string because the source byte array is null.");
            }
            try
            {
                return Encoding.Unicode.GetString(source);
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not binary deserialize string because of exception: {GetExceptionMessage(ex)}");
            }
        }
        public static string JsonSerialize(object source)
        {
            try
            {
                return JsonConvert.SerializeObject(source);
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not json serialize object because of exception: {GetExceptionMessage(ex)}");
            }
        }
        public static T JsonDeserialize<T>(string source)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(source);
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not json deserialize string because of exception: {GetExceptionMessage(ex)}");
            }
        }
        #endregion
        public static byte[] RandomBytes(int bufferSize)
        {
            if (bufferSize < 0)
            {
                throw new Exception("Could not get random bytes because the buffer size was less than zero.");
            }
            try
            {
                byte[] buffer = new byte[bufferSize];
                RNG.NextBytes(buffer); ;
                return buffer;
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not get random bytes because of exception: {GetExceptionMessage(ex)}");
            }
        }
    }
}