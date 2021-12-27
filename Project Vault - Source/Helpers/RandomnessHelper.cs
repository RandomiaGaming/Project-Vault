using System;
namespace ProjectVault
{
    public static class RandomnessHelper
    {
        private static readonly Random RNG = new Random();
        public static int Next()
        {
            return RNG.Next(int.MinValue, int.MaxValue);
        }
        public static int Next(int min, int max)
        {
            return RNG.Next(min, max + 1);
        }
        public static int Next(int max)
        {
            return RNG.Next(max + 1);
        }
        public static byte NextByte()
        {
            byte[] buffer = new byte[1];
            RNG.NextBytes(buffer);
            return buffer[0];
        }
        public static byte[] NextBytes(int bufferSize)
        {
            if (bufferSize < 0)
            {
                throw new Exception("bufferSize must be greater than or equal to zero.");
            }
            else if (bufferSize == 0)
            {
                return new byte[0];
            }
            byte[] buffer = new byte[bufferSize];
            RNG.NextBytes(buffer);
            return buffer;
        }
        public static void NextBytes(byte[] buffer)
        {
            RNG.NextBytes(buffer);
        }
        public static double NextDouble()
        {
            return RNG.NextDouble();
        }
    }
}