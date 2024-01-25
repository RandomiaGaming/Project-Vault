using System;
using System.Collections.Generic;
using System.IO;
using Android.OS.Storage;
using Android.Content;
using Android.App;

namespace ProjectVault
{
    public static class PathHelper
    {
        public enum PathType { Directory, File, Nonexistant };
        public static PathType GetPathType(string path)
        {
            if (File.Exists(path))
            {
                return PathType.File;
            }
            else if (Directory.Exists(path))
            {
                return PathType.Directory;
            }
            else
            {
                return PathType.Nonexistant;
            }
        }
        public static bool IsPathDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool IsPathFile(string path)
        {
            if (File.Exists(path))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static string GetFileName(string fullPath)
        {
            string output = Path.GetFileName(fullPath);
            if (output is null || output == "")
            {
                return "";
            }
            else
            {
                return output;
            }
        }
        public static string GetDirectoryName(string fullPath)
        {
            if (fullPath is null || fullPath == "" || !IsPathDirectory(fullPath))
            {
                return "";
            }
            else
            {
                if (fullPath[fullPath.Length - 1] == Path.DirectorySeparatorChar)
                {
                    fullPath = fullPath.Substring(0, fullPath.Length - 1);
                }
                int substringLength = 0;
                while (substringLength <= fullPath.Length)
                {
                    if (fullPath[fullPath.Length - 1 - substringLength] == Path.DirectorySeparatorChar)
                    {
                        break;
                    }
                    else
                    {
                        substringLength++;
                    }
                }
                return fullPath.Substring(fullPath.Length - substringLength, substringLength);
            }
        }
        public static bool IsFileAccessable(string filePath)
        {
            if (!IsPathFile(filePath))
            {
                throw new Exception("filePath was invalid.");
            }
            try
            {
                FileStream fileStream = File.OpenWrite(filePath);
                bool output = fileStream.CanWrite;
                fileStream.Dispose();
                if (output == false)
                {

                }
                return output;
            }
            catch
            {
                return false;
            }
        }
        public static bool IsDirectoryAccessable(string directoryPath)
        {
            try
            {
                _ = Directory.GetFiles(directoryPath);
                return true;
            }
            catch
            {
                Console.WriteLine($"Directory \"{directoryPath}\" is inaccessable.");
                return false;
            }
        }
        public static string GetParentDirectory(string path)
        {
            PathType pathType = GetPathType(path);
            if (pathType == PathType.Nonexistant)
            {
                return "/";
            }
            else if (pathType == PathType.File)
            {
                return Path.GetDirectoryName(path);
            }
            else
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                string output = directoryInfo.Parent.FullName;
                if (!IsPathDirectory(output) || !IsDirectoryAccessable(output))
                {
                    return "/";
                }
                else
                {
                    return output;
                }
            }
        }
        public static string[] GetDirectoryContents(string directoryPath)
        {
            if (!IsPathDirectory(directoryPath))
            {
                throw new Exception("directoryPath was nonexistant.");
            }
            if (!IsDirectoryAccessable(directoryPath))
            {
                throw new Exception("directoryPath is not accessable.");
            }
            List<string> output = new List<string>();
            output.AddRange(GetSubdirectories(directoryPath));
            output.AddRange(GetFilesInDirectory(directoryPath));
            return output.ToArray();
        }
        public static string[] GetSubdirectories(string directoryPath)
        {
            if (!IsPathDirectory(directoryPath))
            {
                throw new Exception("directoryPath was nonexistant.");
            }
            if (!IsDirectoryAccessable(directoryPath))
            {
                throw new Exception("directoryPath is not accessable.");
            }
            List<string> output = new List<string>();
            foreach (string filePath in Directory.GetDirectories(directoryPath))
            {
                if (IsDirectoryAccessable(filePath))
                {
                    output.Add(filePath);
                }
            }
            return output.ToArray();
        }
        public static string[] GetFilesInDirectory(string directoryPath)
        {
            if (!IsPathDirectory(directoryPath))
            {
                throw new Exception("directoryPath was nonexistant.");
            }
            if (!IsDirectoryAccessable(directoryPath))
            {
                throw new Exception("directoryPath is not accessable.");
            }
            List<string> output = new List<string>();
            foreach (string filePath in Directory.GetFiles(directoryPath))
            {
                if (IsFileAccessable(filePath))
                {
                    output.Add(filePath);
                }
            }
            return output.ToArray();
        }
        public static string[] GetDrives(Context context)
        {
            StorageManager storageManager = StorageManager.FromContext(context);
            List<StorageVolume> storageVolumes = new List<StorageVolume>(storageManager.StorageVolumes);
            List<string> output = new List<string>();
            foreach (StorageVolume storageVolume in storageVolumes)
            { 
                output.Add(storageVolume.Directory.AbsolutePath);
            }
            return output.ToArray();
        }
    }
}
