using System;
using Helper.Core;
namespace ProjectVault
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            CommandProcessor commandProcessor = new CommandProcessor();
        }
    }
}