using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using System.Windows.Forms;
using System.Text;
using System.Security.Cryptography;
using System.Threading;
using System.Drawing;

namespace AESHelper
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
                HandleException(ex);
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
            ShredFile(filePath, 3, ShreddingMethod.Random);
        }
        public static void ShredDirectoryCommand(string directoryPath)
        {
            ShredDirectory(directoryPath, 3, ShreddingMethod.Random);
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
            string password = PromptForPasswordWithConfirmation();
            byte[] passwordBytes = StringToBytes(password);
            EncryptFile(filePath, passwordBytes);
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
            byte[] passwordBytes = StringToBytes(password);
            EncryptDirectory(directoryPath, passwordBytes);
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
            AESHeader header = GetHeaderFromFile(filePath);
            bool correctPassword = false;
            byte[] passwordBytes = new byte[0];
            while (!correctPassword)
            {
                passwordBytes = StringToBytes(PromptForPassword());
                correctPassword = VerifyKey(header, passwordBytes);
                if (!correctPassword)
                {
                    if (MessageBox.Show("Password is incorrect. Please try again.", "Incorrect Password", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    {
                        KillProcess();
                    }
                }
            }
            DecryptFile(filePath, passwordBytes);
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
            byte[] passwordBytes = StringToBytes(password);
            DecryptDirectory(directoryPath, passwordBytes);
        }
        #endregion
        #region Shredding
        public enum ShreddingMethod { Random, Ones, Zeros };
        public static void ShredStream(Stream source, int passes = 3, ShreddingMethod method = ShreddingMethod.Random)
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
            for (int p = 0; p < passes; p++)
            {
                source.Position = 0;
                switch (method)
                {
                    case ShreddingMethod.Zeros:
                        for (long i = 0; i < source.Length; i++)
                        {
                            source.WriteByte(byte.MinValue);
                        }
                        break;
                    case ShreddingMethod.Ones:
                        for (long i = 0; i < source.Length; i++)
                        {
                            source.WriteByte(byte.MaxValue);
                        }
                        break;
                    case ShreddingMethod.Random:
                        for (long i = 0; i < source.Length; i++)
                        {
                            source.WriteByte(RandomByte());
                        }
                        break;
                    default:
                        for (long i = 0; i < source.Length; i++)
                        {
                            source.WriteByte(RandomByte());
                        }
                        break;
                }
                source.Flush();
            }
        }
        public static void ShredFile(string filePath, int passes = 3, ShreddingMethod method = ShreddingMethod.Random)
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
        public static void ShredDirectory(string directoryPath, int passes = 3, ShreddingMethod method = ShreddingMethod.Random)
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
            foreach (string subDirectoryPath in Directory.GetDirectories(directoryPath))
            {
                ShredDirectory(subDirectoryPath, passes, method);
            }
            foreach (string subFilePath in Directory.GetFiles(directoryPath))
            {
                ShredFile(subFilePath, passes, method);
            }
            Directory.Delete(directoryPath);
        }
        #endregion
        #region Hashing
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
        #endregion
        #region AESHeader
        public const int KeySaltLength = 32;
        public const int HashedKeyLength = 32;
        public const int IVLength = 16;
        public const int HeaderLength = 32 + 32 + 16;
        public sealed class AESHeader
        {
            private byte[] _keySalt = new byte[KeySaltLength];
            public byte[] keySalt
            {
                get
                {
                    return CloneArray(_keySalt);
                }
                set
                {
                    if (value is null)
                    {
                        throw new Exception("Could not set keySalt because value was null.");
                    }
                    if (value.Length != KeySaltLength)
                    {
                        throw new Exception("Could not set keySalt because value was the wrong length.");
                    }
                    _keySalt = value;
                }
            }
            private byte[] _hashedKey = new byte[HashedKeyLength];
            public byte[] hashedKey
            {
                get
                {
                    return CloneArray(_hashedKey);
                }
                set
                {
                    if (value is null)
                    {
                        throw new Exception("Could not set hashedKey because value was null.");
                    }
                    if (value.Length != HashedKeyLength)
                    {
                        throw new Exception("Could not set hashedKey because value was the wrong length.");
                    }
                    _hashedKey = value;
                }
            }
            private byte[] _IV = new byte[IVLength];
            public byte[] IV
            {
                get
                {
                    return CloneArray(_IV);
                }
                set
                {
                    if (value is null)
                    {
                        throw new Exception("Could not set IV because value was null.");
                    }
                    if (value.Length != IVLength)
                    {
                        throw new Exception("Could not set IV because value was the wrong length.");
                    }
                    _IV = value;
                }
            }
            public AESHeader()
            {
                _keySalt = new byte[KeySaltLength];
                _hashedKey = new byte[HashedKeyLength];
                _IV = new byte[IVLength];
            }
            public AESHeader(byte[] keySalt, byte[] hashedKey, byte[] IV)
            {
                if (keySalt is null)
                {
                    throw new Exception("Could not create data header because keySalt is null.");
                }
                if (keySalt.Length != KeySaltLength)
                {
                    throw new Exception("Could not create data header because keySalt was the wrong length.");
                }
                this.keySalt = keySalt;
                if (hashedKey is null)
                {
                    throw new Exception("Could not create data header because hashedKey is null.");
                }
                if (hashedKey.Length != HashedKeyLength)
                {
                    throw new Exception("Could not create data header because hashedKey was the wrong length.");
                }
                this.hashedKey = hashedKey;
                if (IV is null)
                {
                    throw new Exception("Could not create data header because IV is null.");
                }
                if (IV.Length != IVLength)
                {
                    throw new Exception("Could not create data header because IV was the wrong length.");
                }
                this.IV = IV;
            }
        }
        public static byte[] SerializeHeader(AESHeader source)
        {
            if (source is null)
            {
                throw new Exception($"Could not serialize header because source was null.");
            }
            byte[] output = CloneArray(source.keySalt);
            output = MergeArrays(output, source.hashedKey);
            output = MergeArrays(output, source.IV);
            return output;
        }
        public static AESHeader DeserializeHeader(byte[] source)
        {
            if (source is null)
            {
                throw new Exception("Could not deserialize header because source is null.");
            }
            else if (source.Length != HeaderLength)
            {
                throw new Exception("Could not deserialize header because keySalt was the wrong length.");
            }
            AESHeader output = new AESHeader();
            output.keySalt = GetRangeFromArray(source, 0, KeySaltLength);
            output.hashedKey = GetRangeFromArray(source, KeySaltLength, HashedKeyLength);
            output.IV = GetRangeFromArray(source, KeySaltLength + HashedKeyLength, IVLength);
            return output;
        }
        public static bool VerifyKey(AESHeader header, byte[] key)
        {
            if (header is null)
            {
                throw new Exception("Count not verify key because header is null.");
            }
            else if (key is null)
            {
                throw new Exception("Could not verify key because key is null.");
            }
            byte[] lengthAdjustedKey = HashBytes(key);
            byte[] hashedKey = HashBytes(MergeArrays(lengthAdjustedKey, header.keySalt));
            return ArraysEqual(hashedKey, header.hashedKey);
        }
        public static AESHeader GenerateHeader(byte[] key)
        {
            byte[] lengthAdjustedKey = HashBytes(key);
            byte[] keySalt = RandomBytes(KeySaltLength);
            byte[] hashedKey = HashBytes(MergeArrays(lengthAdjustedKey, keySalt));
            byte[] IV = RandomBytes(IVLength);
            AESHeader output = new AESHeader(keySalt, hashedKey, IV);
            return output;
        }
        public static AESHeader GetHeaderFromStream(Stream source)
        {
            if (source is null)
            {
                throw new Exception("Could not get header from stream beacuse source is null.");
            }
            else if (!source.CanRead)
            {
                throw new Exception("Could not get header from stream beacuse source is unreadable.");
            }
            else if (!source.CanSeek)
            {
                throw new Exception("Could not get header from stream beacuse source does not support seeking.");
            }
            else if (source.Length < HeaderLength)
            {
                throw new Exception("Could not get header from stream beacuse source is too short.");
            }
            byte[] headerBytes = new byte[HeaderLength];
            source.Position = 0;
            source.Read(headerBytes, 0, HeaderLength);
            return DeserializeHeader(headerBytes);
        }
        public static AESHeader GetHeaderFromBytes(byte[] source)
        {
            if (source is null)
            {
                throw new Exception("Could not get header from bytes beacuse source is null.");
            }
            else if (source.Length < HeaderLength)
            {
                throw new Exception("Could not get header from bytes beacuse source is too short.");
            }
            byte[] headerBytes = GetRangeFromArray(source, 0, HeaderLength);
            return DeserializeHeader(headerBytes);
        }
        public static AESHeader GetHeaderFromFile(string filePath)
        {
            if (filePath is null || filePath == "")
            {
                throw new Exception("Could not get header from file beacuse file path is null or empty.");
            }
            else if (!File.Exists(filePath))
            {
                throw new Exception($"Could not get header from file because file path does not exist.");
            }
            FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            if (fileStream.Length < HeaderLength)
            {
                throw new Exception("Could not get header from stream beacuse file is too small.");
            }
            AESHeader output = GetHeaderFromStream(fileStream);
            fileStream.Dispose();
            return output;
        }
        #endregion
        #region Encryption
        public static void EncryptStream(Stream source, Stream destination, byte[] key)
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
            byte[] lengthAdjustedKey = HashBytes(key);

            AESHeader header = GenerateHeader(key);

            source.Position = 0;
            destination.Position = 0;

            byte[] headerBytes = SerializeHeader(header);

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

            for (long i = 0; i < source.Length; i++)
            {
                cryptoStream.WriteByte((byte)source.ReadByte());
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
                    ShredFile(destinationFilePath);
                }
                catch
                {

                }
                throw ex;
            }
            try
            {
                ShredFile(filePath);
            }
            catch
            {

            }
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
            for (int i = 0; i < subFiles.Count; i++)
            {
                if (Path.GetExtension(subFiles[i]).ToUpper() == ".AES")
                {
                    subFiles.RemoveAt(i);
                    i--;
                }
            }
            foreach (string subFile in subFiles)
            {
                try
                {
                    EncryptFile(subFile, key);
                }
                catch
                {

                }
            }
        }
        #endregion
        #region Decryption
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
            byte[] lengthAdjustedKey = HashBytes(key);

            source.Position = 0;
            destination.Position = 0;

            AESHeader header = GetHeaderFromStream(source);

            if(!VerifyKey(header, key))
            {
                throw new Exception("Could not decrypt stream because key was incorrect.");
            }

            source.Position = HeaderLength;

            Aes aes = Aes.Create();

            aes.BlockSize = 128;
            aes.IV = header.IV;

            aes.KeySize = 256;
            aes.Key = lengthAdjustedKey;

            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.ISO10126;

            ICryptoTransform decryptor = aes.CreateDecryptor();

            CryptoStream cryptoStream = new CryptoStream(destination, decryptor, CryptoStreamMode.Write);

            for (long i = 0; i < source.Length - HeaderLength; i++)
            {
                cryptoStream.WriteByte((byte)source.ReadByte());
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
            string destinationFilePath = filePath;
            if (destinationFilePath.ToLower().EndsWith(".aes"))
            {
                destinationFilePath = destinationFilePath.Substring(0, filePath.Length - 4);
            }
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
                    ShredFile(destinationFilePath);
                }
                catch
                {

                }
                throw ex;
            }
            try
            {
                ShredFile(filePath);
            }
            catch
            {

            }
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
            for (int i = 0; i < subFiles.Count; i++)
            {
                if (Path.GetExtension(subFiles[i]).ToUpper() != ".AES")
                {
                    subFiles.RemoveAt(i);
                    i--;
                }
            }
            foreach (string subFile in subFiles)
            {
                try
                {
                    DecryptFile(subFile, key);
                }
                catch
                {

                }
            }
        }
        #endregion
        #region Exception Logging
        public static void HandleException(Exception ex)
        {
            LogException(ex);
            ShowErrorMessage(ex);
            Process.GetCurrentProcess().Kill();
        }
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
                exceptionDateTime = $"{DateTime.Now.ToString("MM/dd/yyyy HH:m:s")}";
            }
            catch
            {
                exceptionDateTime = "00/00/0000 00:00:00";
            }
            return $"An exception of type \"{exceptionType}\" was thrown at \"{exceptionDateTime}\" with the message \"{exceptionMessage}\".";
        }
        public static string GetExceptionMessage(Exception ex)
        {
            try
            {
                return ex.Message;
            }
            catch
            {
                return "An unknown exception was thrown.";
            }
        }
        #endregion
        #region PopUps
        public static void ShowErrorMessage(Exception ex)
        {
            try
            {
                MessageBox.Show($"{GetExceptionMessage(ex)}", "Exception Thrown", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch
            {

            }
        }
        public static string PromptForPassword()
        {
            string password = Interaction.InputBox("Please type your password below and press enter:", "Enter Password", "", -1, -1);
            if (password is null || password == "")
            {
                KillProcess();
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
                    KillProcess();
                }
                string passwordConfirmation = Interaction.InputBox("Please confirm your password below and press enter:", "Confirm Password", "", -1, -1);
                if (passwordConfirmation is null || passwordConfirmation == "")
                {
                    KillProcess();
                }
                else if (password == passwordConfirmation)
                {
                    return password;
                }
                else if (MessageBox.Show("Passwords do not match. Please try again.", "Non-matching Passwords", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                {
                    KillProcess();
                }
            }
        }
        #endregion
        #region Array Opperations
        public static T[] MergeArrays<T>(T[] arrayA, T[] arrayB)
        {
            if (arrayA is null)
            {
                throw new Exception("Could not merge arrays because arrayA is null.");
            }
            else if (arrayB is null)
            {
                throw new Exception("Could not merge arrays because arrayB is null.");
            }
            else if (arrayA.Length == 0)
            {
                return arrayB;
            }
            else if (arrayB.Length == 0)
            {
                return arrayA;
            }
            T[] output = new T[arrayA.Length + arrayB.Length];
            Array.Copy(arrayA, 0, output, 0, arrayA.Length);
            Array.Copy(arrayB, 0, output, arrayA.Length, arrayB.Length);
            return output;
        }
        public static T[] GetRangeFromArray<T>(T[] source, int startIndex, int length)
        {
            if (source is null)
            {
                throw new Exception("Could not get range from array because source is null.");
            }
            else if (startIndex < 0 || startIndex >= source.Length)
            {
                throw new Exception("Could not get range from array because startIndex was outside the bounds of the array.");
            }
            else if (length <= 0)
            {
                throw new Exception("Could not get range from array because length was less than or equal to 0.");
            }
            else if (source.Length < startIndex + length)
            {
                throw new Exception("Could not get range from array because the specified range was too large to fit within the bounds of the array.");
            }
            T[] output = new T[length];
            Array.Copy(source, startIndex, output, 0, length);
            return output;
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
            for (int i = 0; i < arrayA.Length; i++)
            {
                if (!arrayA[i].Equals(arrayB[i]))
                {
                    return false;
                }
            }
            return true;
        }
        public static T[] CloneArray<T>(T[] source)
        {
            if (source is null)
            {
                return null;
            }
            T[] output = new T[source.Length];
            Array.Copy(source, 0, output, 0, source.Length);
            return output;
        }
        #endregion
        #region String Encoding
        public static byte[] StringToBytes(string source)
        {
            if (source is null)
            {
                throw new Exception("Could not convert string to bytes because source is null.");
            }
            else if (source == "")
            {
                return new byte[0];
            }
            return Encoding.Unicode.GetBytes(source);
        }
        public static string BytesToString(byte[] source)
        {
            if (source is null)
            {
                throw new Exception("Could not convert bytes to string because source is null.");
            }
            else if (source.Length == 0)
            {
                return "";
            }
            return Encoding.Unicode.GetString(source);
        }
        #endregion
        #region Random Bytes
        public static readonly Random RNG = new Random();
        public static byte RandomByte()
        {
            byte[] buffer = new byte[1];
            RNG.NextBytes(buffer);
            return buffer[0];
        }
        public static byte[] RandomBytes(int bufferSize)
        {
            if (bufferSize < 0)
            {
                throw new Exception("Could not get random bytes because the bufferSize was less than zero.");
            }
            else if (bufferSize == 0)
            {
                return new byte[0];
            }
            byte[] buffer = new byte[bufferSize];
            RNG.NextBytes(buffer);
            return buffer;
        }
        #endregion
        public static void KillProcess()
        {
            //Kill the current process and then keep the thread busy waiting forever to ensure no further instructions are executed.
            Process.GetCurrentProcess().Kill();
            while (true)
            {
                Thread.Sleep(int.MaxValue);
            }
        }
    }
}