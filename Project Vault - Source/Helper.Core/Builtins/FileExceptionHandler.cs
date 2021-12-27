using System;
using System.IO;

namespace Helper.Core
{
    public sealed class FileExceptionHandler : ExceptionHandler
    {
        public readonly string exceptionLogFilePath = null;
        public FileExceptionHandler(string exceptionLogFilePath)
        {
            if (!File.Exists(exceptionLogFilePath))
            {
                throw new Exception($"exceptionLogFilePath was invalid because no file exists at path \"{exceptionLogFilePath}\".");
            }
            this.exceptionLogFilePath = exceptionLogFilePath;
        }
        public override void HandleException(Exception exception)
        {
            try
            {
                File.WriteAllText(exceptionLogFilePath, GetExceptionMessage(exception) + "\n" + File.ReadAllText(exceptionLogFilePath));
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
