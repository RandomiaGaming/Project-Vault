using System;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Newtonsoft.Json;

namespace RandomiaGaming.Helpers
{
    public static class SerializationHelper
    {
        public static byte[] StringToBytes(string source)
        {
            if (source is null)
            {
                throw new Exception("Conversion operation was aborted because the source was null.");
            }
            try
            {
                return Encoding.Unicode.GetBytes(source);
            }
            catch (Exception ex)
            {
                Exception rethrowException;
                try
                {
                    rethrowException = new Exception($"Conversion operation was aborted because a \"{ex.GetType().FullName}\" was thrown: \"{ex.Message}\".");
                }
                catch
                {
                    rethrowException = new Exception("Conversion operation was aborted because an unknown exception was thrown.");
                }
                throw rethrowException;
            }
        }
        public static string BytesToString(byte[] source)
        {
            if (source is null)
            {
                throw new Exception("Conversion operation was aborted because the source was null.");
            }
            try
            {
                return Encoding.Unicode.GetString(source);
            }
            catch (Exception ex)
            {
                Exception rethrowException;
                try
                {
                    rethrowException = new Exception($"Conversion operation was aborted because a \"{ex.GetType().FullName}\" was thrown: \"{ex.Message}\".");
                }
                catch
                {
                    rethrowException = new Exception("Conversion operation was aborted because an unknown exception was thrown.");
                }
                throw rethrowException;
            }
        }

        public static byte[] BinarySerialize(object source)
        {
            if (source is null)
            {
                throw new Exception("Serialization operation was aborted because the source was null.");
            }
            try
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                MemoryStream outputStream = new MemoryStream();
                binaryFormatter.Serialize(outputStream, source);
                byte[] output = new byte[outputStream.Length];
                outputStream.Read(output, 0, (int)outputStream.Length);
                outputStream.Close();
                outputStream.Dispose();
                return output;
            }
            catch (Exception ex)
            {
                Exception rethrowException;
                try
                {
                    rethrowException = new Exception($"Serialization operation was aborted because a \"{ex.GetType().FullName}\" was thrown: \"{ex.Message}\".");
                }
                catch
                {
                    rethrowException = new Exception("Serialization operation was aborted because an unknown exception was thrown.");
                }
                throw rethrowException;
            }
        }
        public static object BinaryDeserialize(byte[] source)
        {
            if (source is null)
            {
                throw new Exception("Serialization operation was aborted because the source was null.");
            }
            if (source.Length == 0)
            {
                throw new Exception("Serialization operation was aborted because the source was empty.");
            }
            try
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                MemoryStream sourceStream = new MemoryStream();
                sourceStream.Write(source, 0, source.Length);
                object output = binaryFormatter.Deserialize(sourceStream);
                sourceStream.Close();
                sourceStream.Dispose();
                return output;
            }
            catch (Exception ex)
            {
                Exception rethrowException;
                try
                {
                    rethrowException = new Exception($"Deserialization operation was aborted because a \"{ex.GetType().FullName}\" was thrown: \"{ex.Message}\".");
                }
                catch
                {
                    rethrowException = new Exception("Deserialization operation was aborted because an unknown exception was thrown.");
                }
                throw rethrowException;
            }
        }

        public static string JSONSerialize(object source)
        {
            return JsonConvert.SerializeObject(source);
        }
        public static T JSONDeserialize<T>(string source)
        {
            return JsonConvert.DeserializeObject<T>(source);
        }
    }
}
