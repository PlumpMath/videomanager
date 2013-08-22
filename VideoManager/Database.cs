using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VideoManager
{
    public class Database
    {
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

        public static void FillFromDirectory(string dirPath, bool recursive)
        {
            // get all video file paths from the specified folder
            string[] files = GetVideofilesFromDirectory(dirPath, recursive);

            // fill database
            foreach (string file in files)
            {
                Console.WriteLine(file);
                Video v = Video.CreateFromFilepath(file);
            }
        }
    }
}
