using System;
using System.IO;
namespace ProjectVault
{
    public sealed class AESHeader
    {
        public const int KeySaltLength = 32;
        public const int HashedKeyLength = 32;
        public const int IVLength = 16;
        public static int HeaderLength
        {
            get
            {
                return KeySaltLength + HashedKeyLength + IVLength;
            }
        }
        private byte[] _keySalt = new byte[KeySaltLength];
        public byte[] keySalt
        {
            get
            {
                return ArrayHelper.CloneArray(_keySalt);
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
                _keySalt = ArrayHelper.CloneArray(value);
            }
        }
        private byte[] _hashedKey = new byte[HashedKeyLength];
        public byte[] hashedKey
        {
            get
            {
                return ArrayHelper.CloneArray(_hashedKey);
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
                _hashedKey = ArrayHelper.CloneArray(value);
            }
        }
        private byte[] _IV = new byte[IVLength];
        public byte[] IV
        {
            get
            {
                return ArrayHelper.CloneArray(_IV);
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
                _IV = ArrayHelper.CloneArray(value);
            }
        }
        public AESHeader()
        {
            _keySalt = new byte[KeySaltLength];
            _hashedKey = new byte[HashedKeyLength];
            _IV = new byte[IVLength];
        }
        public AESHeader(byte[] key)
        {
            if (key is null)
            {
                throw new Exception("Could not create AESHeader because key is null.");
            }
            byte[] lengthAdjustedKey = SHA256Helper.HashBytes(key);
            _keySalt = RandomnessHelper.NextBytes(KeySaltLength);
            _hashedKey = SHA256Helper.HashBytes(ArrayHelper.MergeArrays(lengthAdjustedKey, keySalt));
            _IV = RandomnessHelper.NextBytes(IVLength);
        }
        public AESHeader(byte[] keySalt, byte[] hashedKey, byte[] IV)
        {
            if (keySalt is null)
            {
                throw new Exception("Could not create AESHeader because keySalt is null.");
            }
            if (keySalt.Length != KeySaltLength)
            {
                throw new Exception("Could not create AESHeader because keySalt was the wrong length.");
            }
            this.keySalt = keySalt;
            if (hashedKey is null)
            {
                throw new Exception("Could not create AESHeader because hashedKey is null.");
            }
            if (hashedKey.Length != HashedKeyLength)
            {
                throw new Exception("Could not create AESHeader because hashedKey was the wrong length.");
            }
            this.hashedKey = hashedKey;
            if (IV is null)
            {
                throw new Exception("Could not create AESHeader because IV is null.");
            }
            if (IV.Length != IVLength)
            {
                throw new Exception("Could not create AESHeader because IV was the wrong length.");
            }
            this.IV = IV;
        }
        public byte[] Serialize()
        {
            return Serialize(this);
        }
        public static byte[] Serialize(AESHeader source)
        {
            if (source is null)
            {
                throw new Exception($"Could not serialize AESHeader because source was null.");
            }
            byte[] output = ArrayHelper.CloneArray(source.keySalt);
            output = ArrayHelper.MergeArrays(output, source.hashedKey);
            output = ArrayHelper.MergeArrays(output, source.IV);
            return output;
        }
        public static AESHeader Deserialize(byte[] source)
        {
            if (source is null)
            {
                throw new Exception("Could not deserialize AESHeader because source is null.");
            }
            else if (source.Length != HeaderLength)
            {
                throw new Exception("Could not deserialize AESHeader because keySalt was the wrong length.");
            }
            AESHeader output = new AESHeader();
            output.keySalt = ArrayHelper.GetRangeFromArray(source, 0, KeySaltLength);
            output.hashedKey = ArrayHelper.GetRangeFromArray(source, KeySaltLength, HashedKeyLength);
            output.IV = ArrayHelper.GetRangeFromArray(source, KeySaltLength + HashedKeyLength, IVLength);
            return output;
        }
        public bool VerifyKey(byte[] key)
        {
            return VerifyKey(this, key);
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
            byte[] lengthAdjustedKey = SHA256Helper.HashBytes(key);
            byte[] hashedKey = SHA256Helper.HashBytes(ArrayHelper.MergeArrays(lengthAdjustedKey, header.keySalt));
            return ArrayHelper.ArraysEqual(hashedKey, header.hashedKey);
        }
        public static AESHeader FromStream(Stream source)
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
        public static AESHeader FromBytes(byte[] source)
        {
            if (source is null)
            {
                throw new Exception("Could not get header from bytes beacuse source is null.");
            }
            else if (source.Length < HeaderLength)
            {
                throw new Exception("Could not get header from bytes beacuse source is too short.");
            }
            byte[] headerBytes = ArrayHelper.GetRangeFromArray(source, 0, HeaderLength);
            return Deserialize(headerBytes);
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
            AESHeader output = FromStream(fileStream);
            fileStream.Dispose();
            return output;
        }
    }
}
