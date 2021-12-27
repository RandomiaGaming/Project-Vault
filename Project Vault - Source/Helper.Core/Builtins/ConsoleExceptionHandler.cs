using System;

namespace Helper.Core
{
    public sealed class ConsoleExceptionHandler : ExceptionHandler
    {
        public ConsoleExceptionHandler()
        {

        }
        public override void HandleException(Exception exception)
        {
            try
            {
                Console.WriteLine(GetExceptionMessage(exception));
            }
            catch
            {

            }
        }
        public static string GetExceptionMessage(Exception exception)
        {
            string exceptionType;
            try
            {
                exceptionType = exception.GetType().Name;
            }
            catch
            {
                exceptionType = "Exception";
            }
            string exceptionDateTime;
            try
            {
                exceptionDateTime = $"{DateTime.Now.ToString("MM/dd/yyyy HH:m:s")}";
            }
            catch
            {
                exceptionDateTime = "00/00/0000 00:00:00";
            }
            string exceptionMessage;
            try
            {
                exceptionMessage = exception.Message;
            }
            catch
            {
                exceptionMessage = "An unknown exception was thrown.";
            }
            return $"An exception of type \"{exceptionType}\" was thrown at \"{exceptionDateTime}\" with the message \"{exceptionMessage}\".";
        }
    }
}
