using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace VideoManager
{
    class Utils
    {
        public static string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            string[] searchPatterns = searchPattern.Split('|');
            List<string> files = new List<string>();
            foreach (string sp in searchPatterns)
                files.AddRange(Directory.GetFiles(path, sp, searchOption));
            //files.Sort();
            return files.ToArray();
        }


        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern void SetDllDirectory(string lpPathName);
    }
}
