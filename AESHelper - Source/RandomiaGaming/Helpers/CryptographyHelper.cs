using System;
using System.IO;
using System.Security.Cryptography;
namespace RandomiaGaming.Helpers
{
    public static class CryptographyHelper
    {
        #region SHA256
        public static byte[] SHA256Hash(byte[] data)
        {
            if (data is null)
            {
                throw new Exception("SHA256 hash operation was aborted because the given data was null.");
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
                Exception rethrowException;
                try
                {
                    rethrowException = new Exception($"SHA256 hash operation was aborted because a \"{ex.GetType().FullName}\" was thrown: \"{ex.Message}\".");
                }
                catch
                {
                    rethrowException = new Exception("SHA256 hash operation was aborted because an unknown exception was thrown.");
                }
                throw rethrowException;
            }
        }
        public static byte[] SHA256HashWithSalt(byte[] data, byte[] salt)
        {
            if (data is null)
            {
                throw new NullReferenceException("SHA256 hash operation was aborted because the given data was null.");
            }

            if (salt is null)
            {
                throw new NullReferenceException("SHA256 hash operation was aborted because the given salt was null.");
            }

            try
            {
                return SHA256Hash(ArrayHelper.MergeArrays(data, salt));
            }
            catch (Exception ex)
            {
                Exception rethrowException;
                try
                {
                    rethrowException = new Exception($"SHA256 hash operation was aborted because a \"{ex.GetType().FullName}\" was thrown: \"{ex.Message}\".");
                }
                catch
                {
                    rethrowException = new Exception("SHA256 hash operation was aborted because an unknown exception was thrown.");
                }
                throw rethrowException;
            }
        }
        #endregion
        #region AES
        public static byte[] AESSimpleEncrypt(byte[] data, byte[] key)
        {
            if (data is null)
            {
                throw new NullReferenceException("Encryption operation was aborted because the given data was null.");
            }

            if (key is null)
            {
                throw new NullReferenceException("AES encryption operation was aborted because the given key was null.");
            }

            if (key.Length != 32)
            {
                throw new NullReferenceException("AES encryption operation was aborted because the given key was not a valid length. It is recommended that you SHA256 hash all keys before using them for AES encryption.");
            }

            try
            {
                Aes aes = Aes.Create();

                aes.BlockSize = 128;
                aes.IV = new byte[16];
                aes.KeySize = 256;
                aes.Key = key;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.ISO10126;

                ICryptoTransform decryptor = aes.CreateEncryptor();
                MemoryStream outputStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(outputStream, decryptor, CryptoStreamMode.Write);
                cryptoStream.Write(data, 0, data.Length);
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
                Exception rethrowException;
                try
                {
                    rethrowException = new Exception($"AES encryption operation was aborted because a \"{ex.GetType().FullName}\" was thrown: \"{ex.Message}\".");
                }
                catch
                {
                    rethrowException = new Exception("AES encryption operation was aborted because an unknown exception was thrown.");
                }
                throw rethrowException;
            }
        }
        public static AESEncryptedMessage AESEncrypt(byte[] data, byte[] key)
        {
            if (data is null)
            {
                throw new NullReferenceException("AES encryption operation was aborted because the given data was null.");
            }

            if (key is null)
            {
                throw new NullReferenceException("AES encryption operation was aborted because the given key was null.");
            }


            try
            {
                byte[] lengthAdjustedKey = SHA256Hash(key);

                byte[] keySalt = RandomnessHelper.NextBytes(32);

                byte[] hashedKey = SHA256HashWithSalt(lengthAdjustedKey, keySalt);

                byte[] encryptedData = AESSimpleEncrypt(data, lengthAdjustedKey);

                AESEncryptedMessage output = new AESEncryptedMessage(keySalt, hashedKey, encryptedData);

                return output;
            }
            catch (Exception ex)
            {
                Exception rethrowException;
                try
                {
                    rethrowException = new Exception($"Encryption operation was aborted because a \"{ex.GetType().FullName}\" was thrown: \"{ex.Message}\".");
                }
                catch
                {
                    rethrowException = new Exception("Encryption operation was aborted because an unknown exception was thrown.");
                }
                throw rethrowException;
            }
        }
        public static byte[] AESSimpleDecrypt(byte[] data, byte[] key)
        {
            if (data is null)
            {
                throw new NullReferenceException("AES decryption operation was aborted because the given data was null.");
            }

            if (key is null)
            {
                throw new NullReferenceException("AES decryption operation was aborted because the given key was null.");
            }

            if (key.Length != 32)
            {
                throw new NullReferenceException("AES decryption operation was aborted because the given key was not a valid length.");
            }

            try
            {
                Aes aes = Aes.Create();

                aes.BlockSize = 128;
                aes.IV = new byte[16];
                aes.KeySize = 256;
                aes.Key = key;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.ISO10126;

                ICryptoTransform decryptor = aes.CreateDecryptor();
                MemoryStream outputStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(outputStream, decryptor, CryptoStreamMode.Write);
                cryptoStream.Write(data, 0, data.Length);
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
                Exception rethrowException;
                try
                {
                    rethrowException = new Exception($"AES decryption operation was aborted because a \"{ex.GetType().FullName}\" was thrown: \"{ex.Message}\".");
                }
                catch
                {
                    rethrowException = new Exception("AES decryption operation was aborted because an unknown exception was thrown.");
                }
                throw rethrowException;
            }
        }
        public static byte[] AESDecrypt(AESEncryptedMessage encryptedMessage, byte[] key)
        {
            if (encryptedMessage is null)
            {
                throw new NullReferenceException("AES decryption operation was aborted because the given encrypted message was null.");
            }

            if (key is null)
            {
                throw new NullReferenceException("AES decryption operation was aborted because the given key was null.");
            }

            byte[] lengthAdjustedKey;
            byte[] hashedKey;

            try
            {
                lengthAdjustedKey = SHA256Hash(key);
                hashedKey = SHA256HashWithSalt(lengthAdjustedKey, encryptedMessage.keySalt);
            }
            catch (Exception ex)
            {
                Exception rethrowException;
                try
                {
                    rethrowException = new Exception($"AES decryption operation was aborted because a \"{ex.GetType().FullName}\" was thrown: \"{ex.Message}\".");
                }
                catch
                {
                    rethrowException = new Exception("AES decryption operation was aborted because an unknown exception was thrown.");
                }
                throw rethrowException;
            }

            if (!ArrayHelper.ArraysEqual(hashedKey, encryptedMessage.hashedKey))
            {
                throw new IncorrectKeyException();
            }

            try
            {
                byte[] output = AESSimpleDecrypt(encryptedMessage.encryptedData, lengthAdjustedKey);

                return output;
            }
            catch (Exception ex)
            {
                Exception rethrowException;
                try
                {
                    rethrowException = new Exception($"AES decryption operation was aborted because a \"{ex.GetType().FullName}\" was thrown: \"{ex.Message}\".");
                }
                catch
                {
                    rethrowException = new Exception("AES decryption operation was aborted because an unknown exception was thrown.");
                }
                throw rethrowException;
            }
        }
        #endregion
        #region RSA
        public static RSAKeyPair RSAGenerateKeyPair()
        {
            return new RSAKeyPair();
        }
        public static byte[] RSAPrivateToPublicKey(byte[] privateKey)
        {
            if (privateKey is null)
            {
                throw new Exception("RSA convertion operation was aborted because the given private key was null.");
            }

            if (privateKey.Length == 0)
            {
                throw new Exception("RSA convertion operation was aborted because the given private key was empty.");
            }

            try
            {
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSA.FromXmlString(SerializationHelper.BytesToString(privateKey));
                return SerializationHelper.StringToBytes(RSA.ToXmlString(false));
            }
            catch (Exception ex)
            {
                Exception rethrowException;
                try
                {
                    rethrowException = new Exception($"RSA convertion operation was aborted because a \"{ex.GetType().FullName}\" was thrown: \"{ex.Message}\".");
                }
                catch
                {
                    rethrowException = new Exception("RSA convertion operation was aborted because an unknown exception was thrown.");
                }
                throw rethrowException;
            }
        }
        public static RSAEncryptedMessage RSAEncrypt(byte[] data, byte[] publicKey)
        {
            if (data is null)
            {
                throw new Exception("RSA encryption operation was aborted because the given data was null.");
            }

            if (publicKey.Length == 0)
            {
                throw new Exception("RSA encryption operation was aborted because the given key was empty.");
            }

            try
            {
                byte[] AESKey = RandomnessHelper.NextBytes(32);

                AESEncryptedMessage encryptedData = AESEncrypt(data, AESKey);

                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSA.FromXmlString(SerializationHelper.BytesToString(publicKey));

                byte[] encryptedAESKey = RSA.Encrypt(AESKey, false);

                return new RSAEncryptedMessage(encryptedAESKey, encryptedData);
            }
            catch (Exception ex)
            {
                Exception rethrowException;
                try
                {
                    rethrowException = new Exception($"RSA encryption operation was aborted because a \"{ex.GetType().FullName}\" was thrown: \"{ex.Message}\".");
                }
                catch
                {
                    rethrowException = new Exception("RSA encryption operation was aborted because an unknown exception was thrown.");
                }
                throw rethrowException;
            }
        }
        public static byte[] RSADecrypt(RSAEncryptedMessage encryptedMessage, byte[] privateKey)
        {
            if (encryptedMessage is null)
            {
                throw new Exception("RSA encryption operation was aborted because the given data was null.");
            }

            if (privateKey.Length == 0)
            {
                throw new Exception("RSA encryption operation was aborted because the given key was empty.");
            }

            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(SerializationHelper.BytesToString(privateKey));
                byte[] AESKey = rsa.Decrypt(encryptedMessage.encryptedKey, false);
                return AESDecrypt(encryptedMessage.encryptedData, AESKey);
            }
            catch (Exception ex)
            {
                Exception rethrowException;
                try
                {
                    rethrowException = new Exception($"RSA encryption operation was aborted because a \"{ex.GetType().FullName}\" was thrown: \"{ex.Message}\".");
                }
                catch
                {
                    rethrowException = new Exception("RSA encryption operation was aborted because an unknown exception was thrown.");
                }
                throw rethrowException;
            }
        }
        #endregion
    }
    public sealed class AESEncryptedMessage
    {
        private byte[] _keySalt = new byte[0];
        public byte[] keySalt
        {
            get
            {
                return ArrayHelper.CloneArray(_keySalt);
            }
        }
        private byte[] _hashedKey = new byte[0];
        public byte[] hashedKey
        {
            get
            {
                return ArrayHelper.CloneArray(_hashedKey);
            }
        }
        private byte[] _encryptedData = new byte[0];
        public byte[] encryptedData
        {
            get
            {
                return ArrayHelper.CloneArray(_encryptedData);
            }
        }
        private AESEncryptedMessage()
        {
            _keySalt = new byte[0];
            _hashedKey = new byte[0];
            _encryptedData = new byte[0];
        }
        public AESEncryptedMessage(byte[] keySalt, byte[] hashedKey, byte[] encryptedData)
        {
            if (keySalt is null)
            {
                throw new NullReferenceException("Could not create AESMessage because key salt was null.");
            }

            if (keySalt.Length == 0)
            {
                throw new NullReferenceException("Could not create AESMessage because key salt was empty.");
            }

            if (hashedKey is null)
            {
                throw new NullReferenceException("Could not create AESMessage because hashed key was null.");
            }

            if (hashedKey.Length == 0)
            {
                throw new NullReferenceException("Could not create AESMessage because hashed key was empty.");
            }

            if (encryptedData is null)
            {
                throw new NullReferenceException("Could not create AESMessage because encrypted data was null.");
            }

            if (encryptedData.Length == 0)
            {
                throw new NullReferenceException("Could not create AESMessage because encrypted data was empty.");
            }

            try
            {
                _keySalt = ArrayHelper.CloneArray(keySalt);
                _hashedKey = ArrayHelper.CloneArray(hashedKey);
                _encryptedData = ArrayHelper.CloneArray(encryptedData);
            }
            catch (Exception ex)
            {
                Exception rethrowException;
                try
                {
                    rethrowException = new Exception($"Could not create AESMessage because a \"{ex.GetType().FullName}\" was thrown: \"{ex.Message}\".");
                }
                catch
                {
                    rethrowException = new Exception("Could not create AESMessage because an unknown exception was thrown.");
                }
                throw rethrowException;
            }
        }
        public AESEncryptedMessage Clone()
        {
            return new AESEncryptedMessage(ArrayHelper.CloneArray(keySalt), ArrayHelper.CloneArray(hashedKey), ArrayHelper.CloneArray(encryptedData));
        }
    }
    public sealed class RSAKeyPair
    {
        private byte[] _privateKey = new byte[0];
        public byte[] privateKey
        {
            get
            {
                return ArrayHelper.CloneArray(_privateKey);
            }
        }
        private byte[] _publicKey = new byte[0];
        public byte[] publicKey
        {
            get
            {
                return ArrayHelper.CloneArray(_publicKey);
            }
        }
        public RSAKeyPair()
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                _privateKey = SerializationHelper.StringToBytes(rsa.ToXmlString(true));
                _publicKey = SerializationHelper.StringToBytes(rsa.ToXmlString(false));
            }
            catch (Exception ex)
            {
                Exception rethrowException;
                try
                {
                    rethrowException = new Exception($"Could not create RSAKeyPair because a \"{ex.GetType().FullName}\" was thrown: \"{ex.Message}\".");
                }
                catch
                {
                    rethrowException = new Exception("Could not create RSAKeyPair because an unknown exception was thrown.");
                }
                throw rethrowException;
            }
        }
        public RSAKeyPair(byte[] privateKey)
        {
            if (privateKey is null)
            {
                throw new NullReferenceException("Could not create RSAKeyPair because given private key was null.");
            }

            if (privateKey.Length == 0)
            {
                throw new NullReferenceException("Could not create RSAKeyPair because given private key was empty.");
            }

            try
            {
                _privateKey = ArrayHelper.CloneArray(privateKey);
                _publicKey = CryptographyHelper.RSAPrivateToPublicKey(_privateKey);
            }
            catch (Exception ex)
            {
                Exception rethrowException;
                try
                {
                    rethrowException = new Exception($"Could not create RSAKeyPair because a \"{ex.GetType().FullName}\" was thrown: \"{ex.Message}\".");
                }
                catch
                {
                    rethrowException = new Exception("Could not create RSAKeyPair because an unknown exception was thrown.");
                }
                throw rethrowException;
            }
        }
        public RSAKeyPair Clone()
        {
            return new RSAKeyPair(ArrayHelper.CloneArray(_privateKey));
        }
    }
    public sealed class RSAEncryptedMessage
    {
        public byte[] _encryptedKey = new byte[0];
        public byte[] encryptedKey
        {
            get
            {
                return ArrayHelper.CloneArray(_encryptedKey);
            }
        }
        public AESEncryptedMessage _encryptedData = null;
        public AESEncryptedMessage encryptedData
        {
            get
            {
                return _encryptedData.Clone();
            }
        }
        private RSAEncryptedMessage()
        {
            _encryptedKey = new byte[0];
            _encryptedData = null;
        }
        public RSAEncryptedMessage(byte[] encryptedKey, AESEncryptedMessage encryptedData)
        {
            if (encryptedKey is null)
            {
                throw new NullReferenceException("Could not create RSAMessage because encrypted key was null.");
            }
            if (encryptedKey.Length == 0)
            {
                throw new NullReferenceException("Could not create RSAMessage because encrypted key was empty.");
            }
            if (encryptedData is null)
            {
                throw new NullReferenceException("Could not create RSAMessage because encrypted data was null.");
            }

            try
            {
                _encryptedKey = ArrayHelper.CloneArray(encryptedKey);
                _encryptedData = encryptedData.Clone();
            }
            catch (Exception ex)
            {
                Exception rethrowException;
                try
                {
                    rethrowException = new Exception($"Could not create RSAMessage because a \"{ex.GetType().FullName}\" was thrown: \"{ex.Message}\".");
                }
                catch
                {
                    rethrowException = new Exception("Could not create RSAMessage because an unknown exception was thrown.");
                }
                throw rethrowException;
            }
        }
        public RSAEncryptedMessage Clone()
        {
            return new RSAEncryptedMessage(ArrayHelper.CloneArray(_encryptedKey), encryptedData.Clone());
        }
    }
    public sealed class IncorrectKeyException : Exception
    {
        public IncorrectKeyException() : base("The given key was incorrect.") { }
    }
}