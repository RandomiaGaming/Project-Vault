using System.IO;

namespace ProjectVault
{
    public static class AESHeaderHelper
    {
        public static readonly string AESHeaderString = "Encrypted With Project Vault";
        public static readonly byte[] AESHeaderBytes = new byte[32] { 242, 29, 162, 135, 33, 117, 32, 215, 151, 197, 55, 74, 68, 232, 46, 214, 207, 71, 245, 1, 204, 219, 118, 208, 241, 75, 45, 183, 223, 250, 151, 250 };
        public static AESMeta GetAESMeta(Stream source)
        {

        }
        public bool ValidateHeader(Stream source)
        {

        }
    }
}
