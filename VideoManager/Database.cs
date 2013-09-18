using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VideoManager
{
    public class Database
    {
        private static HashSet<Video> Videos { get; set; }

        public static Video GetVideoById(int id)
        {
            return Videos.First(v => v.ID == id);
        }
    }
}
