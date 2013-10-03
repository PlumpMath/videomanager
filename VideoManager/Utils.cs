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


		public static string GetLongTimeAsString(long time)
		{
			long totalSeconds = time / 1000;
			long hours = totalSeconds / 3600;
			long minutes = (totalSeconds - hours * 3600) / 60;
			long seconds = totalSeconds - hours * 3600 - minutes * 60;
			return ((hours > 0) ? (hours.ToString() + ":") : "") +
				minutes.ToString("0#") + ":" + seconds.ToString("0#");
		}
    }	
}
