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

        #region Properties
        public int? ID { get; set; }
        public string Name { get; set; }
        public Account Account { get; set; }
        public string Path { get; set; }
        public int Length { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastPlayed { get; set; }
        public int CountPlayed { get; set; }

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

        public Video(int? id, string title, Account acc, string path)
        {
            ID = id;
            Name = title;
            Account = acc;
            Tags = new List<Tag>();
            Path = path;
        }

        public Video(int? id, string title, Account acc, string path, List<Tag> tags)
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
            string conStr = Properties.Settings.Default.ConnectionString;
            using (SQLiteConnection con = new SQLiteConnection(conStr))
            {
                if (this.ID == null)
                {
                    // INSERT new video
                    string cmdStr = "INSERT INTO video (name, account) VALUES (@Name, @Account);";
                    SQLiteCommand cmd = new SQLiteCommand(cmdStr, con);
                    cmd.Parameters.AddWithValue("@Name", this.Name);
                    cmd.Parameters.AddWithValue("@Account", this.Account);
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
                        cmdStr = "last_insert_rowid()";
                        cmd = new SQLiteCommand(cmdStr, con);
                        this.ID = (int)cmd.ExecuteScalar();
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
                    string cmdStr = "UPDATE video SET name=@Name, account=@Account WHERE ID=@Id;";
                    SQLiteCommand cmd = new SQLiteCommand(cmdStr, con);
                    cmd.Parameters.AddWithValue("@Name", this.Name);
                    cmd.Parameters.AddWithValue("@Account", this.Account);
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
                        v = new Video(
                            Convert.ToInt32(reader["ID"].ToString()),
                            reader["name"].ToString(),
                            Account.GetById(Convert.ToInt32(reader["account"].ToString())),
                            reader["path"].ToString());
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
            List<string> listParts = new List<string>(filename.Split(new string[] {" - "}, StringSplitOptions.RemoveEmptyEntries));
            string title = listParts.Last();
            Console.WriteLine(filepath + " => " + title);
            string accname = listParts.First();
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
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
    }
}
