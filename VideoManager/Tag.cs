using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace VideoManager
{
    public class Tag
    {
        public static HashSet<Tag> Tags = new HashSet<Tag>();

        #region Properties
        public int ID { get; set; }
        public string Name { get; set; }
        #endregion

        #region Constructors
        public Tag(string name)
        {
            Name = name;
        }

        public Tag(int id, string name)
        {
            ID = id;
            Name = name;
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
                    // INSERT new account
                    string cmdStr = "INSERT INTO tag (name) VALUES (@Name);";
                    SQLiteCommand cmd = new SQLiteCommand(cmdStr, con);
                    cmd.Parameters.AddWithValue("@Name", this.Name);
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
                    string cmdStr = "UPDATE tag SET name=@Name WHERE ID=@Id;";
                    SQLiteCommand cmd = new SQLiteCommand(cmdStr, con);
                    cmd.Parameters.AddWithValue("@Name", this.Name);
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


        public static Tag LoadFromDatabase()
        {
            Tag t = null;
            string conStr = Properties.Settings.Default.ConnectionString;
            using (SQLiteConnection con = new SQLiteConnection(conStr))
            {
                string cmdStr = "SELECT * FROM tag;";
                SQLiteCommand cmd = new SQLiteCommand(cmdStr, con);
                con.Open();
                try
                {
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                        t = new Tag(
                            Convert.ToInt32(reader["ID"].ToString()),
                            reader["name"].ToString());
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
            return t;
        }
        #endregion


        public static Tag GetById(int id)
        {
            return Tags.ToList().Find(t => t.ID == id);
        }
    }
}
