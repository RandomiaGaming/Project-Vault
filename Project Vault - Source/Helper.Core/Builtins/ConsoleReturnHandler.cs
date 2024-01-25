using System;

namespace Helper.Core
{
    public sealed class ConsoleReturnHandler : ReturnHandler
    {
        public ConsoleReturnHandler()
        {

        }
        public override void HandleReturn(object returnValue)
        {
            Console.WriteLine(returnValue.ToString());
        }
    }
}
