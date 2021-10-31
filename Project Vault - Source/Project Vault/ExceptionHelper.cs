using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace ProjectVault
{
    public static class ExceptionHelper
    {
        public static void HandleException(Exception ex)
        {
            LogException(ex);
            ShowErrorMessage(ex);
            Process.GetCurrentProcess().Kill();
        }
        public static void ShowErrorMessage(Exception ex)
        {
            try
            {
                MessageBox.Show($"{GetExceptionMessage(ex)}", "Exception Thrown", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch
            {

            }
        }
        public static void LogException(Exception ex)
        {
            string exceptionLogEntry = GetExceptionLogEntry(ex);
            try
            {
                string exceptionLogLocation = @"C:\Program Files\Project Vault\Log.txt";
                string exceptionLogContents;
                if (!File.Exists(exceptionLogLocation))
                {
                    File.WriteAllText(exceptionLogLocation, "");
                    exceptionLogContents = "";
                }
                else
                {
                    exceptionLogContents = File.ReadAllText(exceptionLogLocation);
                }
                File.WriteAllText(exceptionLogLocation, $"{exceptionLogEntry}\n{exceptionLogContents}");
            }
            catch
            {

            }
        }
        public static string GetExceptionLogEntry(Exception ex)
        {
            string exceptionType;
            try
            {
                exceptionType = ex.GetType().Name;
            }
            catch
            {
                exceptionType = "Exception";
            }
            string exceptionMessage = GetExceptionMessage(ex);
            string exceptionDateTime;
            try
            {
                exceptionDateTime = $"{DateTime.Now.ToString("MM/dd/yyyy HH:m:s")}";
            }
            catch
            {
                exceptionDateTime = "00/00/0000 00:00:00";
            }
            return $"An exception of type \"{exceptionType}\" was thrown at \"{exceptionDateTime}\" with the message \"{exceptionMessage}\".";
        }
        public static string GetExceptionMessage(Exception ex)
        {
            try
            {
                return ex.Message;
            }
            catch
            {
                return "An unknown exception was thrown.";
            }
        }
        public static void KillAndWait()
        {
            try
            {
                Process.GetCurrentProcess().Kill();
                while (true)
                {
                    Thread.Sleep(int.MaxValue);
                }
            }
            catch
            {
                return;
            }
        }
    }
}
