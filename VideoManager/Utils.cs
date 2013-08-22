using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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
    }
}
