using System;
using System.IO;
namespace ProjectVault
{
    public sealed class AESMeta
    {
        public string sourceFileExtension = ".bin";
        public string sourceFileName = "UnnamedFile";
        public string version = "Unknown Version";
        public ushort versionCode = 0;
        private AESMeta()
        {
            sourceFileExtension = null;
            sourceFileName = null;
        }
        internal AESMeta(string sourceFileName, string sourceFileExtension)
        {
            if (sourceFileName is null || sourceFileName == "")
            {
                throw new Exception("SourceFileName may not be null or empty.");
            }
            this.sourceFileName = sourceFileName;
            if (sourceFileExtension is null || sourceFileExtension == "")
            {
                throw new Exception("SourceFileExtension may not be null or empty.");
            }
            this.sourceFileExtension = sourceFileExtension;
        }
        internal AESMeta(string sourceFileName, string sourceFileExtension, string version, ushort versionCode)
        {
            if (sourceFileName is null || sourceFileName == "")
            {
                throw new Exception("SourceFileName may not be null or empty.");
            }
            this.sourceFileName = sourceFileName;
            if (sourceFileExtension is null || sourceFileExtension == "")
            {
                throw new Exception("SourceFileExtension may not be null or empty.");
            }
            this.sourceFileExtension = sourceFileExtension;
            if(version is null || version == "")
            {
                throw new Exception("Version may not be null or empty.");
            }
            this.version = version;
            if(versionCode == 0)
            {
                throw new Exception("VersionCode must be greater than 0.");
            }
            this.versionCode = versionCode;
        }
        public byte[] Serialize()
        {
            return Serialize(this);
        }
        public static byte[] Serialize(AESMeta source)
        {
            if (source is null)
            {
                throw new Exception($"Could not serialize AESHeader because source was null.");
            }
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            MemoryStream serializationStream = new MemoryStream();
            binaryFormatter.Serialize(serializationStream, source);
            serializationStream.Position = 0;
            byte[] output = new byte[serializationStream.Length];
            serializationStream.Read(output, 0, (int)serializationStream.Length);
            serializationStream.Close();
            serializationStream.Dispose();
            return output;
        }
        public static AESMeta Deserialize(byte[] source)
        {
            if (source is null)
            {
                throw new Exception("Could not deserialize AESHeader because source is null.");
            }
            else if (source.Length != HeaderLength)
            {
                throw new Exception("Could not deserialize AESHeader because keySalt was the wrong length.");
            }
            AESMeta output = new AESMeta();
            output.keySalt = ArrayHelper.SubArray(source, 0, KeySaltLength);
            output.hashedKey = ArrayHelper.SubArray(source, KeySaltLength, HashedKeyLength);
            output.IV = ArrayHelper.SubArray(source, KeySaltLength + HashedKeyLength, IVLength);
            return output;
        }
        public bool VerifyKey(byte[] key)
        {
            return VerifyKey(this, key);
        }
        public static bool VerifyKey(AESMeta header, byte[] key)
        {
            if (header is null)
            {
                throw new Exception("Count not verify key because header is null.");
            }
            else if (key is null)
            {
                throw new Exception("Could not verify key because key is null.");
            }
            byte[] lengthAdjustedKey = SHA256Helper.HashBytes(key);
            byte[] hashedKey = SHA256Helper.HashBytes(ArrayHelper.MergeArrays(lengthAdjustedKey, header.keySalt));
            return ArrayHelper.ArraysEqual(hashedKey, header.hashedKey);
        }
        public static AESMeta FromStream(Stream source)
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
            return Deserialize(headerBytes);
        }
        public static AESMeta FromBytes(byte[] source)
        {
            if (source is null)
            {
                throw new Exception("Could not get header from bytes beacuse source is null.");
            }
            else if (source.Length < HeaderLength)
            {
                throw new Exception("Could not get header from bytes beacuse source is too short.");
            }
            byte[] headerBytes = ArrayHelper.SubArray(source, 0, HeaderLength);
            return Deserialize(headerBytes);
        }
        public static AESMeta GetHeaderFromFile(string filePath)
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
            AESMeta output = FromStream(fileStream);
            fileStream.Dispose();
            return output;
        }
    }
}
