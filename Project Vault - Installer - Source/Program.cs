using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO.Compression;
using System.Diagnostics;
using System.Collections.Generic;

namespace ProjectVault.Installer
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            if (Directory.Exists(@"C:\Program Files\Project Vault"))
            {
                if (MessageBox.Show($"Project Vault has already been installed. Would you like to uninstall it?", $"Uninstall Project Vault?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Uninstall();
                    MessageBox.Show($"Project Vault was successfully uninstalled!", $"Uninstall Successful!", MessageBoxButtons.OK);
                }
                else
                {
                    Process.GetCurrentProcess().Kill();
                }
            }
            else
            {
                if (MessageBox.Show($"Project Vault has not been installed. Would you like to install it?", $"Install Project Vault?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        Install();
                        MessageBox.Show($"Project Vault was succesfully installed!", "Install Successful!", MessageBoxButtons.OK);
                        Process.GetCurrentProcess().Kill();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Project Vault could not be installed due to exception: {ex.Message}!", "Install Failed!", MessageBoxButtons.OK);
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

            Directory.CreateDirectory(@"C:\Program Files\Project Vault");
            Assembly assembly = Assembly.GetCallingAssembly();
            Stream payloadStream = assembly.GetManifestResourceStream("ProjectVault.Installer.Payload.zip");
            byte[] payloadBytes = new byte[(int)payloadStream.Length];
            payloadStream.Read(payloadBytes, 0, (int)payloadStream.Length);
            payloadStream.Dispose();
            string payloadTempFilePath = Path.GetTempFileName();
            File.WriteAllBytes(payloadTempFilePath, payloadBytes);
            ZipFile.ExtractToDirectory(payloadTempFilePath, @"C:\Program Files\Project Vault");
            File.Delete(payloadTempFilePath);
            RegistryKey root = Registry.ClassesRoot;
            //Open .AES files
            RegistryKey DotAES = root.CreateSubKey(".aes", true);
            DotAES.SetValue(null, "ProjectVault.OpenFile");
            DotAES.Dispose();
            RegistryKey ProjectVaultOpenFile = root.CreateSubKey("ProjectVault.OpenFile", true);
            ProjectVaultOpenFile.SetValue("Icon", @"""C:\Program Files\Project Vault\Project Vault.exe""");
            RegistryKey ProjectVaultOpenFileShell = ProjectVaultOpenFile.CreateSubKey("shell", true);
            RegistryKey ProjectVaultOpenFileShellOpen = ProjectVaultOpenFileShell.CreateSubKey("Open", true);
            RegistryKey ProjectVaultOpenFileShellOpenCommand = ProjectVaultOpenFileShellOpen.CreateSubKey("command", true);
            ProjectVaultOpenFileShellOpenCommand.SetValue(null, @"""C:\Program Files\Project Vault\Project Vault.exe"" ""DecryptFile"" ""%1""");
            ProjectVaultOpenFileShellOpenCommand.Dispose();
            ProjectVaultOpenFileShellOpen.Dispose();
            ProjectVaultOpenFileShell.Dispose();
            ProjectVaultOpenFile.Dispose();
            //Encrypt File Context Menu
            RegistryKey Star = root.OpenSubKey("*", true);
            RegistryKey StarShell = Star.OpenSubKey("shell", true);
            RegistryKey StarShellProjectVaultEncryptFile = StarShell.CreateSubKey("ProjectVault.EncryptFile", true);
            StarShellProjectVaultEncryptFile.SetValue(null, "PV Encrypt");
            StarShellProjectVaultEncryptFile.SetValue("Icon", @"""C:\Program Files\Project Vault\Project Vault.exe""");
            RegistryKey StarShellProjectVaultEncryptFileCommand = StarShellProjectVaultEncryptFile.CreateSubKey("command", true);
            StarShellProjectVaultEncryptFileCommand.SetValue(null, @"""C:\Program Files\Project Vault\Project Vault.exe"" ""EncryptFile"" ""%1""");
            StarShellProjectVaultEncryptFileCommand.Dispose();
            StarShellProjectVaultEncryptFile.Dispose();
            //Decrypt File Context Menu
            RegistryKey StarShellProjectVaultDecryptFile = StarShell.CreateSubKey("ProjectVault.DecryptFile", true);
            StarShellProjectVaultDecryptFile.SetValue(null, "PV Decrypt");
            StarShellProjectVaultDecryptFile.SetValue("Icon", @"""C:\Program Files\Project Vault\Project Vault.exe""");
            RegistryKey StarShellProjectVaultDecryptFileCommand = StarShellProjectVaultDecryptFile.CreateSubKey("command", true);
            StarShellProjectVaultDecryptFileCommand.SetValue(null, @"""C:\Program Files\Project Vault\Project Vault.exe"" ""DecryptFile"" ""%1""");
            StarShellProjectVaultDecryptFileCommand.Dispose();
            StarShellProjectVaultDecryptFile.Dispose();
            //Shred File Context Menu
            RegistryKey StarShellProjectVaultShredFile = StarShell.CreateSubKey("ProjectVault.ShredFile", true);
            StarShellProjectVaultShredFile.SetValue(null, "PV Shred");
            StarShellProjectVaultShredFile.SetValue("Icon", @"""C:\Program Files\Project Vault\Project Vault.exe""");
            RegistryKey StarShellProjectVaultShredFileCommand = StarShellProjectVaultShredFile.CreateSubKey("command", true);
            StarShellProjectVaultShredFileCommand.SetValue(null, @"""C:\Program Files\Project Vault\Project Vault.exe"" ""ShredFile"" ""%1""");
            StarShellProjectVaultShredFileCommand.Dispose();
            StarShellProjectVaultShredFile.Dispose();
            StarShell.Dispose();
            Star.Dispose();
            //Encrypt Directory Context Menu
            RegistryKey DirectoryKey = root.OpenSubKey("Directory", true);
            RegistryKey DirectoryShell = DirectoryKey.OpenSubKey("shell", true);
            RegistryKey DirectoryShellProjectVaultEncryptDirectory = DirectoryShell.CreateSubKey("ProjectVault.EncryptDirectory", true);
            DirectoryShellProjectVaultEncryptDirectory.SetValue(null, "PV Encrypt");
            DirectoryShellProjectVaultEncryptDirectory.SetValue("Icon", @"""C:\Program Files\Project Vault\Project Vault.exe""");
            RegistryKey DirectoryShellProjectVaultEncryptDirectoryCommand = DirectoryShellProjectVaultEncryptDirectory.CreateSubKey("command", true);
            DirectoryShellProjectVaultEncryptDirectoryCommand.SetValue(null, @"""C:\Program Files\Project Vault\Project Vault.exe"" ""EncryptDirectory"" ""%1""");
            DirectoryShellProjectVaultEncryptDirectoryCommand.Dispose();
            DirectoryShellProjectVaultEncryptDirectory.Dispose();
            //Decrypt Directory Context Menu
            RegistryKey DirectoryShellProjectVaultDecryptDirectory = DirectoryShell.CreateSubKey("ProjectVault.DecryptDirectory", true);
            DirectoryShellProjectVaultDecryptDirectory.SetValue(null, "PV Decrypt");
            DirectoryShellProjectVaultDecryptDirectory.SetValue("Icon", @"""C:\Program Files\Project Vault\Project Vault.exe""");
            RegistryKey DirectoryShellProjectVaultDecryptDirectoryCommand = DirectoryShellProjectVaultDecryptDirectory.CreateSubKey("command", true);
            DirectoryShellProjectVaultDecryptDirectoryCommand.SetValue(null, @"""C:\Program Files\Project Vault\Project Vault.exe"" ""DecryptDirectory"" ""%1""");
            DirectoryShellProjectVaultDecryptDirectoryCommand.Dispose();
            DirectoryShellProjectVaultDecryptDirectory.Dispose();
            //Shred Directory Context Menu
            RegistryKey DirectoryShellProjectVaultShredDirectory = DirectoryShell.CreateSubKey("ProjectVault.ShredDirectory", true);
            DirectoryShellProjectVaultShredDirectory.SetValue(null, "PV Shred");
            DirectoryShellProjectVaultShredDirectory.SetValue("Icon", @"""C:\Program Files\Project Vault\Project Vault.exe""");
            RegistryKey DirectoryShellProjectVaultShredDirectoryCommand = DirectoryShellProjectVaultShredDirectory.CreateSubKey("command", true);
            DirectoryShellProjectVaultShredDirectoryCommand.SetValue(null, @"""C:\Program Files\Project Vault\Project Vault.exe"" ""ShredDirectory"" ""%1""");
            DirectoryShellProjectVaultShredDirectoryCommand.Dispose();
            DirectoryShellProjectVaultShredDirectory.Dispose();
            DirectoryShell.Dispose();
            DirectoryKey.Dispose();
            root.Dispose();
            //Register to CMD Path
            string pathValue = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
            List<string> pathSubValues = new List<string>(pathValue.Split(';'));
            pathSubValues.Add(@"""C:\Program Files\Project Vault""");
            string newPathValue = "";
            foreach (string pathSubValue in pathSubValues)
            {
                newPathValue += pathSubValue + ";";
            }
            Environment.SetEnvironmentVariable("PATH", newPathValue, EnvironmentVariableTarget.Machine);
        }
        public static void Uninstall()
        {
            try
            {
                Directory.Delete(@"C:\Program Files\Project Vault", true);
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
                    root.DeleteSubKeyTree("ProjectVault.OpenFile");
                }
                catch
                {

                }
                try
                {
                    RegistryKey Star = root.OpenSubKey("*", true);
                    RegistryKey StarShell = Star.OpenSubKey("shell", true);
                    try
                    {
                        StarShell.DeleteSubKeyTree("ProjectVault.EncryptFile", false);
                    }
                    catch
                    {

                    }
                    try
                    {
                        StarShell.DeleteSubKeyTree("ProjectVault.DecryptFile", false);
                    }
                    catch
                    {

                    }
                    try
                    {
                        StarShell.DeleteSubKeyTree("ProjectVault.ShredFile", false);
                    }
                    catch
                    {

                    }
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
                    try
                    {
                        DirectoryShell.DeleteSubKeyTree("ProjectVault.EncryptDirectory", false);
                    }
                    catch
                    {

                    }
                    try
                    {
                        DirectoryShell.DeleteSubKeyTree("ProjectVault.ShredDirectory", false);

                    }
                    catch
                    {

                    }
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
                    if (pathSubValues[i].Replace("/", "\\").Replace("\"", "").ToLower() == @"c:\program files\project vault")
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
    }
}