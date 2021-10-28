using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO.Compression;
using System.Diagnostics;
using System.Collections.Generic;

namespace AESHelper.Installer
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            if (Directory.Exists(@"C:\Program Files\AESHelper"))
            {
                if (MessageBox.Show($"AESHelper has already been installed. Would you like to uninstall it?", $"Uninstall AESHelper?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Uninstall();
                    MessageBox.Show($"AESHelper was successfully uninstalled!", $"Uninstall Successful!", MessageBoxButtons.OK);
                }
                else
                {
                    Process.GetCurrentProcess().Kill();
                }
            }
            else
            {
                if (MessageBox.Show($"AESHelper has not been installed. Would you like to install it?", $"Install AESHelper?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        Install();
                        MessageBox.Show($"AESHelper was succesfully installed!", "Install Successful!", MessageBoxButtons.OK);
                        Process.GetCurrentProcess().Kill();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"AESHelper could not be installed due to exception: {ex.Message}!", "Install Failed!", MessageBoxButtons.OK);
                        MessageBox.Show($"Attempting to undo changes!", "Undoing Changes!", MessageBoxButtons.OK);
                        try
                        {
                            Uninstall();
                            MessageBox.Show($"Changes were successfully undone!", "Undo Successful!", MessageBoxButtons.OK);
                            Process.GetCurrentProcess().Kill();
                        }
                        catch (Exception ex2)
                        {
                            MessageBox.Show($"Changes could not be undone due to exception: {ex2.Message}!", "Undo Failed!", MessageBoxButtons.OK);
                        }
                    }
                }
                else
                {
                    Process.GetCurrentProcess().Kill();
                }
            }
            Process.GetCurrentProcess().Kill();
        }
        public static void Install()
        {
            Uninstall();

            Directory.CreateDirectory(@"C:\Program Files\AESHelper");
            Assembly assembly = Assembly.GetCallingAssembly();
            Stream payloadStream = assembly.GetManifestResourceStream("AESHelper.Installer.Payload.zip");
            byte[] payloadBytes = new byte[(int)payloadStream.Length];
            payloadStream.Read(payloadBytes, 0, (int)payloadStream.Length);
            payloadStream.Dispose();
            string payloadTempFilePath = Path.GetTempFileName();
            File.WriteAllBytes(payloadTempFilePath, payloadBytes);
            ZipFile.ExtractToDirectory(payloadTempFilePath, @"C:\Program Files\AESHelper");
            File.Delete(payloadTempFilePath);
            RegistryKey root = Registry.ClassesRoot;
            //Open .AES files
            RegistryKey DotAES = root.CreateSubKey(".aes", true);
            DotAES.SetValue(null, "AESHelper.OpenFile");
            DotAES.Dispose();
            RegistryKey AESHelperOpenFile = root.CreateSubKey("AESHelper.OpenFile", true);
            AESHelperOpenFile.SetValue("Icon", @"C:\Program Files\AESHelper\AESHelper.exe");
            RegistryKey AESHelperOpenFileShell = AESHelperOpenFile.CreateSubKey("shell", true);
            RegistryKey AESHelperOpenFileShellOpen = AESHelperOpenFileShell.CreateSubKey("Open", true);
            RegistryKey AESHelperOpenFileShellOpenCommand = AESHelperOpenFileShellOpen.CreateSubKey("command", true);
            AESHelperOpenFileShellOpenCommand.SetValue(null, @"""C:\Program Files\AESHelper\AESHelper.exe"" ""DecryptFile"" ""%1""");
            AESHelperOpenFileShellOpenCommand.Dispose();
            AESHelperOpenFileShellOpen.Dispose();
            AESHelperOpenFileShell.Dispose();
            AESHelperOpenFile.Dispose();
            //Encrypt File Context Menu
            RegistryKey Star = root.OpenSubKey("*", true);
            RegistryKey StarShell = Star.OpenSubKey("shell", true);
            RegistryKey StarShellAESHelperEncryptFile = StarShell.CreateSubKey("AESHelper.EncryptFile", true);
            StarShellAESHelperEncryptFile.SetValue(null, "AES Encrypt File");
            StarShellAESHelperEncryptFile.SetValue("Icon", @"C:\Program Files\AESHelper\AESHelper.exe");
            RegistryKey StarShellAESHelperEncryptFileCommand = StarShellAESHelperEncryptFile.CreateSubKey("command", true);
            StarShellAESHelperEncryptFileCommand.SetValue(null, @"""C:\Program Files\AESHelper\AESHelper.exe"" ""EncryptFile"" ""%1""");
            StarShellAESHelperEncryptFileCommand.Dispose();
            StarShellAESHelperEncryptFile.Dispose();
            //Decrypt File Context Menu
            RegistryKey StarShellAESHelperDecryptFile = StarShell.CreateSubKey("AESHelper.DecryptFile", true);
            StarShellAESHelperDecryptFile.SetValue(null, "AES Decrypt File");
            StarShellAESHelperDecryptFile.SetValue("Icon", @"C:\Program Files\AESHelper\AESHelper.exe");
            RegistryKey StarShellAESHelperDecryptFileCommand = StarShellAESHelperDecryptFile.CreateSubKey("command", true);
            StarShellAESHelperDecryptFileCommand.SetValue(null, @"""C:\Program Files\AESHelper\AESHelper.exe"" ""DecryptFile"" ""%1""");
            StarShellAESHelperDecryptFileCommand.Dispose();
            StarShellAESHelperDecryptFile.Dispose();
            //Shred File Context Menu
            RegistryKey StarShellAESHelperShredFile = StarShell.CreateSubKey("AESHelper.ShredFile", true);
            StarShellAESHelperShredFile.SetValue(null, "AES Shred File");
            StarShellAESHelperShredFile.SetValue("Icon", @"C:\Program Files\AESHelper\AESHelper.exe");
            RegistryKey StarShellAESHelperShredFileCommand = StarShellAESHelperShredFile.CreateSubKey("command", true);
            StarShellAESHelperShredFileCommand.SetValue(null, @"""C:\Program Files\AESHelper\AESHelper.exe"" ""ShredFile"" ""%1""");
            StarShellAESHelperShredFileCommand.Dispose();
            StarShellAESHelperShredFile.Dispose();
            StarShell.Dispose();
            Star.Dispose();
            //Encrypt Directory Context Menu
            RegistryKey DirectoryKey = root.OpenSubKey("Directory", true);
            RegistryKey DirectoryShell = DirectoryKey.OpenSubKey("shell", true);
            RegistryKey DirectoryShellAESHelperEncryptDirectory = DirectoryShell.CreateSubKey("AESHelper.EncryptDirectory", true);
            DirectoryShellAESHelperEncryptDirectory.SetValue(null, "AES Encrypt Directory");
            DirectoryShellAESHelperEncryptDirectory.SetValue("Icon", @"C:\Program Files\AESHelper\AESHelper.exe");
            RegistryKey DirectoryShellAESHelperEncryptDirectoryCommand = DirectoryShellAESHelperEncryptDirectory.CreateSubKey("command", true);
            DirectoryShellAESHelperEncryptDirectoryCommand.SetValue(null, @"""C:\Program Files\AESHelper\AESHelper.exe"" ""EncryptDirectory"" ""%1""");
            DirectoryShellAESHelperEncryptDirectoryCommand.Dispose();
            DirectoryShellAESHelperEncryptDirectory.Dispose();
            //Shred Directory Context Menu
            RegistryKey DirectoryShellAESHelperShredDirectory = DirectoryShell.CreateSubKey("AESHelper.ShredDirectory", true);
            DirectoryShellAESHelperShredDirectory.SetValue(null, "AES Shred Directory");
            DirectoryShellAESHelperShredDirectory.SetValue("Icon", @"C:\Program Files\AESHelper\AESHelper.exe");
            RegistryKey DirectoryShellAESHelperShredDirectoryCommand = DirectoryShellAESHelperShredDirectory.CreateSubKey("command", true);
            DirectoryShellAESHelperShredDirectoryCommand.SetValue(null, @"""C:\Program Files\AESHelper\AESHelper.exe"" ""ShredDirectory"" ""%1""");
            DirectoryShellAESHelperShredDirectoryCommand.Dispose();
            DirectoryShellAESHelperShredDirectory.Dispose();
            DirectoryShell.Dispose();
            DirectoryKey.Dispose();
            root.Dispose();
            //Register to CMD Path
            string pathValue = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
            List<string> pathSubValues = new List<string>(pathValue.Split(';'));
            pathSubValues.Add(@"C:\Program Files\AESHelper");
            string newPathValue = "";
            foreach (string pathSubValue in pathSubValues)
            {
                newPathValue += pathSubValue + ";";
            }
            Environment.SetEnvironmentVariable("PATH", newPathValue, EnvironmentVariableTarget.Machine);
        }
        public static void InstallRegistries()
        {

        }
        public static void Uninstall()
        {
            try
            {
                Directory.Delete(@"C:\Program Files\AESHelper", true);
            }
            catch
            {

            }
            try
            {
                RegistryKey root = Registry.ClassesRoot;
                try
                {
                    root.DeleteSubKeyTree(".aes");
                }
                catch
                {

                }
                try
                {
                    root.DeleteSubKeyTree("AESHelper.OpenFile");
                }
                catch
                {

                }
                try
                {
                    RegistryKey Star = root.OpenSubKey("*", true);
                    RegistryKey StarShell = Star.OpenSubKey("shell", true);
                    StarShell.DeleteSubKeyTree("AESHelper.EncryptFile", false);
                    StarShell.DeleteSubKeyTree("AESHelper.DecryptFile", false);
                    StarShell.DeleteSubKeyTree("AESHelper.ShredFile", false);
                    StarShell.Dispose();
                    Star.Dispose();
                }
                catch
                {

                }
                try
                {
                    RegistryKey DirectoryKey = root.OpenSubKey("Directory", true);
                    RegistryKey DirectoryShell = DirectoryKey.OpenSubKey("shell", true);
                    DirectoryShell.DeleteSubKeyTree("AESHelper.EncryptDirectory", false);
                    DirectoryShell.DeleteSubKeyTree("AESHelper.ShredDirectory", false);
                    DirectoryShell.Dispose();
                    DirectoryKey.Dispose();
                }
                catch
                {

                }
                root.Dispose();
            }
            catch
            {

            }
            try
            {
                string pathValue = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
                List<string> pathSubValues = new List<string>(pathValue.Split(';'));
                for (int i = 0; i < pathSubValues.Count; i++)
                {
                    if (pathSubValues[i].Replace("/", "\\").ToLower() == @"c:\program files\aeshelper")
                    {
                        pathSubValues.RemoveAt(i);
                        i--;
                    }
                }
                string newPathValue = "";
                foreach (string pathSubValue in pathSubValues)
                {
                    newPathValue += pathSubValue + ";";
                }
                Environment.SetEnvironmentVariable("PATH", newPathValue, EnvironmentVariableTarget.Machine);
            }
            catch
            {

            }
        }
        public static void UninstallRegistries()
        {

        }
    }
}