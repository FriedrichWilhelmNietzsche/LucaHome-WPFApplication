using Common.Tools;
using System;
using System.Collections.Generic;
using System.IO;

namespace Data.Controller
{
    public class LocalDriveController
    {
        private const string TAG = "LocalDriveController";
        private readonly Logger _logger;

        public LocalDriveController()
        {
            _logger = new Logger(TAG);
        }

        public DriveInfo GetMovieDrive()
        {
            _logger.Debug("GetMovieDrive");

            DriveInfo[] localDrives = DriveInfo.GetDrives();
            foreach (DriveInfo localDrive in localDrives)
            {
                try
                {
                    if (localDrive.VolumeLabel.Contains("Filme&Serien"))
                    {
                        return localDrive;
                    }
                }
                catch (Exception exception)
                {
                    _logger.Error(exception.Message);
                }
            }

            return null;
        }

        public FileInfo[] ReadFilesInDir(DirectoryInfo directory)
        {
            string[] extensionArray = new string[] { ".mkv", ".avi", ".mp4" };
            HashSet<string> videExtensions = new HashSet<string>(extensionArray, StringComparer.OrdinalIgnoreCase);

            FileInfo[] movieFiles = new FileInfo[] { };

            try
            {
                FileInfo[] allFiles = directory.GetFiles();
                movieFiles = Array.FindAll(allFiles, f => videExtensions.Contains(f.Extension));
            }
            catch (Exception exception)
            {
                _logger.Error(exception.Message);
            }

            return movieFiles;
        }
    }
}
