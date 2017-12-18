using Common.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Data.Controller
{
    public class LocalDriveController
    {
        private const string TAG = "LocalDriveController";

        public LocalDriveController()
        {
            // Empty constructor, nothing needed here
        }

        public DriveInfo GetLibraryDrive()
        {
            return getDriveByLabel("Bibliothek");
        }

        public DriveInfo GetVideothekDrive()
        {
            return getDriveByLabel("Filme&Serien");
        }

        public DriveInfo GetRaspberryDrive()
        {
            return getDriveByLabel("raspberryStore");
        }

        public string[] ReadDirInDir(string directory)
        {
            return Directory.GetDirectories(directory);
        }

        public FileInfo[] ReadFilesInDir(DirectoryInfo directory, string[] extensionArray)
        {
            HashSet<string> searchExtensions = new HashSet<string>(extensionArray, StringComparer.OrdinalIgnoreCase);
            FileInfo[] foundFiles = new FileInfo[] { };

            try
            {
                FileInfo[] allFiles = directory.GetFiles();
                foundFiles = Array.FindAll(allFiles, files => searchExtensions.Contains(files.Extension));
            }
            catch (Exception exception)
            {
                Logger.Instance.Error(TAG, exception.Message);
            }

            return foundFiles;
        }

        public string[] ReadFileNamesInDir(string directory, string[] extensionArray)
        {
            HashSet<string> searchExtensions = new HashSet<string>(extensionArray, StringComparer.OrdinalIgnoreCase);
            string[] foundFiles = new string[] { };

            try
            {
                string[] allFiles = Directory.GetFiles(directory);
                foundFiles = allFiles.Where(file => extensionArray.Any(entry => file.Contains(entry))).Select(file => file).ToArray();
            }
            catch (Exception exception)
            {
                Logger.Instance.Error(TAG, exception.Message);
            }

            return foundFiles;
        }

        private DriveInfo getDriveByLabel(string label)
        {
            DriveInfo[] localDrives = DriveInfo.GetDrives();
            foreach (DriveInfo localDrive in localDrives)
            {
                try
                {
                    if (localDrive.VolumeLabel.Contains(label))
                    {
                        return localDrive;
                    }
                }
                catch (Exception exception)
                {
                    Logger.Instance.Error(TAG, exception.Message);
                    continue;
                }
            }

            return null;
        }
    }
}
