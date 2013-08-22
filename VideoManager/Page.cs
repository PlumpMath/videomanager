using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace VideoManager
{
    public class Page
    {
        public static HashSet<Page> Pages = new HashSet<Page>();

        #region Properties
        public int? ID { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public string Abbreviation { get; set; }
        private static Page Default { get; set; }
        #endregion

        #region Constructors
        public Page(string name, string url)
        {
            Name = name;
            URL = url;
        }

        public Page(int? id, string name, string url)
        {
            ID = id;
            Name = name;
            URL = url;
        }

        public Page(int? id, string name, string url, string abbrev)
        {
            ID = id;
            Name = name;
            URL = url;
            Abbreviation = abbrev;
        }
        #endregion


        public static Page GetDefaultPage()
        {
            if (Default == null)
                Default = Page.GetById(Properties.Settings.Default.DefaultPageId);
            return Default;
        }




        #region Database Interaction
        public bool SaveToDatabase()
        {
            string conStr = Properties.Settings.Default.ConnectionString;
            using (SQLiteConnection con = new SQLiteConnection(conStr))
            {
                if (this.ID == null)
                {
                    // INSERT new account
                    string cmdStr = "INSERT INTO page (name, url, abbreviation) VALUES (@Name, @Url, @Abbrev);";
                    SQLiteCommand cmd = new SQLiteCommand(cmdStr, con);
                    cmd.Parameters.AddWithValue("@Name", this.Name);
                    cmd.Parameters.AddWithValue("@Url", this.URL);
                    cmd.Parameters.AddWithValue("@Abbrev", this.Abbreviation);
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
                    string cmdStr = "UPDATE page SET name=@Name, url=@Url, abbreviation=@Abbrev WHERE ID=@Id;";
                    SQLiteCommand cmd = new SQLiteCommand(cmdStr, con);
                    cmd.Parameters.AddWithValue("@Name", this.Name);
                    cmd.Parameters.AddWithValue("@Url", this.URL);
                    cmd.Parameters.AddWithValue("@Abbrev", this.Abbreviation);
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


        public static Page LoadFromDatabase()
        {
            Page p = null;
            string conStr = Properties.Settings.Default.ConnectionString;
            using (SQLiteConnection con = new SQLiteConnection(conStr))
            {
                string cmdStr = "SELECT * FROM page;";
                SQLiteCommand cmd = new SQLiteCommand(cmdStr, con);
                con.Open();
                try
                {
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                        p = new Page(
                            Convert.ToInt32(reader["ID"].ToString()),
                            reader["name"].ToString(),
                            reader["url"].ToString(),
                            reader["abbreviation"].ToString());
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
            return p;
        }
        #endregion


        public static Page GetById(int id)
        {
            return Pages.ToList().Find(p => p.ID == id);
        }
    }
}
