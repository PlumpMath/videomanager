using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;

namespace VideoManager
{
    public class Playlist
    {
		public static Playlist ActivePlaylist { get; set; }
        public ObservableCollection<Video> Videos { get; set; }
        public int Count { get { return Videos.Count; } private set {} }

        #region Constructors
        public Playlist()
        {
			Videos = new ObservableCollection<Video>();
        }

        public Playlist(IEnumerable<Video> videos)
        {
            if (videos != null)
				Videos = new ObservableCollection<Video>(videos);
            else
				Videos = new ObservableCollection<Video>();
        }
        #endregion

        #region Management Methods
        public void AddVideo(Video video)
        {
            if (video != null)
                Videos.Add(video);
        }

        public void AddVideos(IEnumerable<Video> videos)
        {
            if (videos != null)
				foreach (Video v in videos)
					Videos.Add(v);
        }

		public void AddVideos(IEnumerable<string> paths)
		{
			List<Video> listVideos = new List<Video>();
			foreach (string path in paths)
				listVideos.Add(Video.CreateFromFilepath(path));
		}

		public void AddVideosFromDirectory(string path, bool recursive = true)
		{
			foreach (Video v in VideoMgr.GetVideosFromDirectory(path, recursive))
				Videos.Add(v);
		}

        public bool RemoveVideo(Video video)
        {
            return Videos.Remove(video);
        }

        public void RemoveVideos(IEnumerable<Video> videos)
        {
            foreach (Video v in videos)
                Videos.Remove(v);
        }

        public void RemoveIndex(int index)
        {
            Videos.RemoveAt(index);
        }

        public void RemoveIndexRange(int start, int end)
        {
			for (; end > start; end--)
				Videos.RemoveAt(end);
        }
		/*
        public void RemoveMatches(Predicate<Video> match)
        {
			foreach (Video v in this.Videos
            Videos.Where(match);
        }*/

        public void Clear()
        {
            Videos.Clear();
        }
        #endregion

        #region Import/Export
        public bool ExportToIdListFile(string filepath)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach (Video v in this.Videos)
                    sb.Append(v.ID + ",");
                string text = sb.ToString().TrimEnd(new char[] { ',' });
                File.WriteAllText(filepath, text);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error writing playlist file ("+filepath+"): " + ex.Message);
                return false;
            }
        }

        public bool ImportFromIdListFile(string filepath)
        {
            try
            {
                string text = File.ReadAllText(filepath);
                foreach (string idStr in text.Split(new char[] { ',' }))
                {
                    int id;
                    if (!Int32.TryParse(idStr, out id))
                        return false;
                    this.Videos.Add(Video.GetById(id));
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading playlist file (" + filepath + "): " + ex.Message);
                return false;
            }
        }
        #endregion
    }
}
