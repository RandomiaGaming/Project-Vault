using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;
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
                    try
                    {
                        Uninstall();
                        MessageBox.Show($"AESHelper was successfully uninstalled!", $"Uninstall Successful!", MessageBoxButtons.OK);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"AESHelper could not be uninstalled due to exception: {ex.Message}!", "Uninstall Failed!", MessageBoxButtons.OK);
                    }
                }
                else
                {
                    return;
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
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"AESHelper could not be installed due to exception: {ex.Message}!", "Install Failed!", MessageBoxButtons.OK);
                        MessageBox.Show($"Attempting to undo changes!", "Undoing Changes!", MessageBoxButtons.OK);
                        try
                        {
                            Uninstall();
                            MessageBox.Show($"Changes were successfully undone!", "Undo Successful!", MessageBoxButtons.OK);
                        }
                        catch (Exception ex2)
                        {
                            MessageBox.Show($"Changes could not be undone due to exception: {ex2.Message}!", "Undo Failed!", MessageBoxButtons.OK);
                        }
                    }
                }
                else
                {
                    return;
                }
            }
        }
        public static void Install()
        {
            Directory.CreateDirectory(@"C:\Program Files\AESHelper");
            Assembly assembly = Assembly.GetCallingAssembly();
            Stream payloadStream = assembly.GetManifestResourceStream("AESHelper.Installer.AESHelper.exe");
            byte[] payloadBytes = new byte[(int)payloadStream.Length];
            payloadStream.Read(payloadBytes, 0, (int)payloadStream.Length);
            payloadStream.Dispose();
            File.WriteAllBytes(@"C:\Program Files\AESHelper\AESHelper.exe", payloadBytes);

            RegistryKey root = Registry.ClassesRoot;

            RegistryKey fileContextMenuRoot = root.OpenSubKey("*", true);
            RegistryKey fileContextMenuShell = fileContextMenuRoot.OpenSubKey("shell", true);

            fileContextMenuShell.DeleteSubKeyTree("AESHelper", false);

            RegistryKey fileContextMenu = fileContextMenuShell.CreateSubKey("AESHelper", true);
            fileContextMenu.SetValue("", "AESHelper Lock/Unlock");
            fileContextMenu.SetValue("Icon", "AESHelper.Installer.AESHelper.exe");

            RegistryKey fileContextMenuCommand = fileContextMenu.CreateSubKey("command", true);
            fileContextMenuCommand.SetValue("", "\"C:\\Program Files\\AESHelper\\AESHelper.exe\" \"%1\"");

            fileContextMenuCommand.Close();
            fileContextMenu.Close();
            fileContextMenuShell.Close();
            fileContextMenuRoot.Close();

            RegistryKey folderContextMenuRoot = root.OpenSubKey("Directory", true);
            RegistryKey folderContextMenuShell = folderContextMenuRoot.OpenSubKey("shell", true);

            folderContextMenuShell.DeleteSubKeyTree("AESHelper", false);

            RegistryKey folderContextMenu = folderContextMenuShell.CreateSubKey("AESHelper", true);
            folderContextMenu.SetValue("", "AESHelper Lock/Unlock");
            folderContextMenu.SetValue("Icon", "AESHelper.Installer.AESHelper.exe");

            RegistryKey folderContextMenuCommand = folderContextMenu.CreateSubKey("command", true);
            folderContextMenuCommand.SetValue("", "\"C:\\Program Files\\AESHelper\\AESHelper.exe\" \"%1\"");

            folderContextMenuCommand.Close();
            folderContextMenu.Close();
            folderContextMenuShell.Close();
            folderContextMenuRoot.Close();

            root.Close();
        }
        public static void Uninstall()
        {
            if (Directory.Exists(@"C:\Program Files\AESHelper"))
            {
                Directory.Delete(@"C:\Program Files\AESHelper", true);
            }

            RegistryKey root = Registry.ClassesRoot;

            RegistryKey fileContextMenuRoot = root.OpenSubKey("*", true);
            RegistryKey fileContextMenuShell = fileContextMenuRoot.OpenSubKey("shell", true);

            fileContextMenuShell.DeleteSubKeyTree("AESHelper", false);

            fileContextMenuShell.Close();
            fileContextMenuRoot.Close();

            RegistryKey folderContextMenuRoot = root.OpenSubKey("Directory", true);
            RegistryKey folderContextMenuShell = folderContextMenuRoot.OpenSubKey("shell", true);

            folderContextMenuShell.DeleteSubKeyTree("AESHelper", false);

            folderContextMenuShell.Close();
            folderContextMenuRoot.Close();

            root.Close();
        }
    }
}