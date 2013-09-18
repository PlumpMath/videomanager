using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VideoManager
{
    public class VideoMgr
    {
        #region DLL Path
        public static bool InitLibraryPath()
        {
            if (IsValidLibraryPath(Properties.Settings.Default.LibraryPath))
                return true;
            else
            {
                string currentDir = AppDomain.CurrentDomain.BaseDirectory;
                string curDir3Up = Directory.GetParent(Directory.GetParent(Directory.GetParent(currentDir).FullName).FullName).FullName;
                string[] possibleDirs = new string[] { 
                    currentDir, 
                    curDir3Up
                };
                foreach (string dir in possibleDirs)
                {
                    string dllPath = (dir.EndsWith(Path.DirectorySeparatorChar.ToString())) 
                        ? dir + "libs"
                        : dir + Path.DirectorySeparatorChar + "libs";
                    if (IsValidLibraryPath(dllPath))
                    {
                        Properties.Settings.Default.LibraryPath = dllPath;
                        return true;
                    }
                }
            }
            return false;
        }


        private static bool IsValidLibraryPath(string dllPath)
        {
            return Directory.Exists(dllPath) 
                && File.Exists(dllPath + Path.DirectorySeparatorChar + "libvlc.dll")
                && Directory.Exists(dllPath + Path.DirectorySeparatorChar + "plugins");
        }
        #endregion


        #region Filetypes
        public static string GetAllowedFiletypesSelector()  // read from file or sth for better descriptions
        {
            string allowed = Properties.Settings.Default.AllowedFiletypes;
            string[] arrAllowed = allowed.Split(new char[] { ',' });
            List<string> types = new List<string>();
            types.Add("All|*.*");
            foreach (string type in arrAllowed)
            {
                types.Add(type.ToUpper() + "|*." + type);
            }
            return String.Join("|", types.ToArray());
        }
        #endregion


        #region Filesystem
        public static string[] GetVideofilesFromDirectory(string dirPath, bool recursive)
        {
            string allowedFiletypes = Properties.Settings.Default.AllowedFiletypes;
            List<string> listFiletypes = new List<string>(allowedFiletypes.Split(new Char[] { ',' }));
            for (int i = 0; i < listFiletypes.Count; i++)
                listFiletypes[i] = "*." + listFiletypes[i];
            string searchPattern = String.Join("|", listFiletypes.ToArray());
            SearchOption searchOption = (recursive) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            string[] files = Utils.GetFiles(dirPath, searchPattern, searchOption);
            return files;
        }

        public static List<Video> GetVideosFromDirectory(string dirPath, bool recursive)
        {
            // get all video file paths from the specified folder
            string[] files = GetVideofilesFromDirectory(dirPath, recursive);

            // get videos
            List<Video> list = new List<Video>();
            foreach (string file in files)
                list.Add(Video.CreateFromFilepath(file));
            return list;
        }
        #endregion
    }
}
