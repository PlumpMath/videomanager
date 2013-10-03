using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.ComponentModel;

namespace VideoManager
{
    public class Video : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

		#region Fields
		private Int64? _id;
		private string _name;
		private Account _account;
		private string _path;
		private long _length;
		private DateTime _creationdate;
		private DateTime _lastplayed;
		private int _countplayed;
		#endregion

		#region Properties
		public Int64? ID
		{
			get { return _id; }
			set { _id = value; OnPropertyChanged("ID"); }
		}
		public string Name
		{
			get { return _name; }
			set { _name = value; OnPropertyChanged("Name"); }
		}
		public Account Account
		{
			get { return _account; }
			set { _account = value; OnPropertyChanged("Account"); }
		}
		public string AccountString
		{
			get { return _account.Name; }
			set { }
		}
		public string Path
		{
			get { return _path; }
			set { _path = value; OnPropertyChanged("Path"); }
		}
		public long Length
		{
			get { return _length; }
			set { _length = value; OnPropertyChanged("Length"); }
		}
		public string LengthString
		{
			get { return Utils.GetLongTimeAsString(_length); }
			set { }
		}
		public DateTime CreationDate
		{
			get { return _creationdate; }
			set { _creationdate = value; OnPropertyChanged("CreationDate"); }
		}
		public DateTime LastPlayed
		{
			get { return _lastplayed; }
			set { _lastplayed = value; OnPropertyChanged("LastPlayed"); }
		}
		public int CountPlayed
		{
			get { return _countplayed; }
			set { _countplayed = value; OnPropertyChanged("CountPlayed"); }
		}

        public List<Tag> Tags { get; set; }
        #endregion

        #region Constructors
        public Video(string title, Account acc, string path)
        {
            Name = title;
            Account = acc;
            Tags = new List<Tag>();
            Path = path;
        }

        public Video(Int64? id, string title, Account acc, string path)
        {
            ID = id;
            Name = title;
            Account = acc;
            Tags = new List<Tag>();
            Path = path;
        }

        public Video(Int64? id, string title, Account acc, string path, List<Tag> tags)
        {
            ID = id;
            Name = title;
            Account = acc;
            Tags = tags;
            Path = path;
        }
        #endregion

        #region Database Interaction
        public bool SaveToDatabase()
        {
			if (this.Account == null)
				return false;
            string conStr = Properties.Settings.Default.ConnectionString;
            using (SQLiteConnection con = new SQLiteConnection(conStr))
            {
                if (this.ID == null)
                {
                    // INSERT new video
                    string cmdStr = "INSERT INTO video (name, account, path, creationdate) VALUES " +
						"(@Name, @Account, @Path, @CreationDate);";
                    SQLiteCommand cmd = new SQLiteCommand(cmdStr, con);
                    cmd.Parameters.AddWithValue("@Name", this.Name);
					cmd.Parameters.AddWithValue("@Account", this.Account.ID);
					cmd.Parameters.AddWithValue("@Path", this.Path);
					cmd.Parameters.AddWithValue("@CreationDate", DateTime.Now);
                    con.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message);
                        return false;
                    }
                    try
                    {
						cmdStr = "SELECT last_insert_rowid()";
                        cmd = new SQLiteCommand(cmdStr, con);
                        this.ID = (int)cmd.ExecuteScalar();
						this.CreationDate = DateTime.Now;
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message);
                        return false;
                    }
                    con.Close();
                }
                else
                {
                    // UPDATE current video
                    string cmdStr = "UPDATE video SET name=@Name, account=@Account, path=@Path, " + 
						"length=@Length, lastplayed=@LastPlayed, countplayed=@CountPlayed WHERE ID=@Id;";
                    SQLiteCommand cmd = new SQLiteCommand(cmdStr, con);
                    cmd.Parameters.AddWithValue("@Name", this.Name);
                    cmd.Parameters.AddWithValue("@Account", this.Account.ID);
					cmd.Parameters.AddWithValue("@Path", this.Path);
					cmd.Parameters.AddWithValue("@Length", this.Length);
					cmd.Parameters.AddWithValue("@LastPlayed", this.LastPlayed);
					cmd.Parameters.AddWithValue("@CountPlayed", this.CountPlayed);
                    cmd.Parameters.AddWithValue("@Id", this.ID);
                    con.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message);
                        return false;
                    }
                }
            }
            return true;
        }


        public static Video LoadFromDatabase()
        {
            Video v = null;
            string conStr = Properties.Settings.Default.ConnectionString;
            using (SQLiteConnection con = new SQLiteConnection(conStr))
            {
                string cmdStr = "SELECT * FROM video " +
                    "JOIN account ON video.account = account.ID;";
                SQLiteCommand cmd = new SQLiteCommand(cmdStr, con);
                con.Open();
                try
                {
                    SQLiteDataReader reader = cmd.ExecuteReader();
					while (reader.Read())
					{
						v = new Video(
							Convert.ToInt32(reader["ID"].ToString()),
							reader["name"].ToString(),
							Account.GetById(Convert.ToInt32(reader["account"].ToString())),
							reader["path"].ToString());
						v.CreationDate = DateTime.Parse(reader["creationdate"].ToString());
						v.Length = Convert.ToInt64(reader["length"].ToString());
						v.LastPlayed = DateTime.Parse(reader["lastplayed"].ToString());
						v.CountPlayed = Convert.ToInt32(reader["countplayed"].ToString());
					}
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
            return v;
        }
        #endregion
        
        #region Factory Methods
        public static Video CreateFromFilepath(string filepath)
        {
			string filename = System.IO.Path.GetFileNameWithoutExtension(filepath);
			string title = filename;
			string accname = "Unknown";
			List<string> listParts = new List<string>(filename.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries));
			if (listParts.Count > 1)
			{
				title = listParts.Last().Trim();
				accname = listParts.First().Trim();
			}
			else
			{
				listParts = new List<string>(filename.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries));
				if (listParts.Count > 1)
				{
					title = listParts.Last().Trim();
					accname = listParts.First().Trim();
				}
			}
			Console.WriteLine(filepath + " => ACC: " + accname + " TITLE: " + title);
            return new Video(title, new Account(accname), filepath);
        }

        public static Video GetById(int id)
        {
            return Database.GetVideoById(id);
        }
        #endregion

        #region OnPropertyChanged Method
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
