using System;
namespace RandomiaGaming.Helpers
{
    public static class RandomnessHelper
    {
        private static readonly Random rng = new Random();
        public static int Next()
        {
            return rng.Next();
        }
        public static int Next(int min, int max)
        {
            return rng.Next(min, max + 1);
        }
        public static int Next(int max)
        {
            return rng.Next(max + 1);
        }
        public static byte[] NextBytes(int bufferSize)
        {
            byte[] buffer = new byte[bufferSize];
            rng.NextBytes(buffer);
            return buffer;
        }
        public static void NextBytes(byte[] buffer)
        {
            rng.NextBytes(buffer);
        }
        public static double NextDouble()
        {
            return rng.NextDouble();
        }
        public static string NextString(int length)
        {
            if (length < 0)
            {
                throw new ArgumentException("Length must be greater than or equal to zero.");
            }
            if (length == 0)
            {
                return "";
            }
            string charSet = "";
            for (int i = char.MinValue; i <= char.MaxValue; i++)
            {
                char c = Convert.ToChar(i);
                if (!char.IsControl(c) && !char.IsSurrogate(c) && !char.IsHighSurrogate(c) && !char.IsLowSurrogate(c) && !char.IsWhiteSpace(c) && !char.IsSeparator(c))
                {
                    charSet += c;
                }
            }
            string output = "";
            for (int i = 0; i < length; i++)
            {
                output += charSet[Next(0, charSet.Length - 1)];
            }
            return output;
        }
    }
}