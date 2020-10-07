using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using IWshRuntimeLibrary;

namespace InstallLockoutCreator2._0
{
    class Program
    {
        static void Main(string[] args)
        {
            // Copies directory, including subdirectories
            void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
            {
                // Get the subdirectories for the specified directory.
                DirectoryInfo dir = new DirectoryInfo(sourceDirName);

                if (!dir.Exists)
                {
                    throw new DirectoryNotFoundException( "Source directory does not exist or could not be found: " + sourceDirName);
                }

                DirectoryInfo[] dirs = dir.GetDirectories();
                // If the destination directory doesn't exist, create it.
                if (!Directory.Exists(destDirName))
                {
                    Directory.CreateDirectory(destDirName);
                }

                // Get the files in the directory and copy them to the new location.
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    string temppath = Path.Combine(destDirName, file.Name);

                    if (temppath.Contains("Install"))
                    {
                        Debug.WriteLine("install*.exe found, not copying itself to the folder.  Continuing...");
                    }
                    else
                    {
                        file.CopyTo(temppath, true);
                    }
                }

                // If copying subdirectories, copy them and their contents to new location.
                if (copySubDirs)
                {
                    foreach (DirectoryInfo subdir in dirs)
                    {
                        string temppath = Path.Combine(destDirName, subdir.Name);
                        DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                    }
                }
            }

            string programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\Lockout Creator 2.0\\Release";
            System.IO.Directory.CreateDirectory(programFilesPath);

            string lockoutCreatorNetworkPath = "\\\\ad.pch.local\\companyshares\\PPLW\\PPLWDP\\Public\\mbmenden\\Lockout Creator 2.0\\Release";

            DirectoryCopy(lockoutCreatorNetworkPath, programFilesPath, true);

            CreateShortcut("Lockout Creator 2.0", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), programFilesPath + "\\Lockout Creator.exe", "Shortcut to run Lockout Creator", programFilesPath + "\\Resources\\clwLogo_transparent.ico");

            Console.WriteLine("Lockout Creator 2.0 installed successfully!");
            Console.WriteLine("Press Enter to close this window...");
            Console.ReadLine();
        }

        public static void CreateShortcut(string shortcutName, string shortcutPath, string targetFileLocation, string shortcutDesc, string iconLocation)
        {
            string shortcutLocation = Path.Combine(shortcutPath, shortcutName + ".lnk");
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

            shortcut.Description = shortcutDesc;                // The description of the shortcut
            shortcut.IconLocation = iconLocation;               // The icon of the shortcut
            shortcut.TargetPath = targetFileLocation;           // The path of the file that will launch when the shortcut is run
            shortcut.Save();                                    // Save the shortcut
        }

    }
}
