using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace VideoManager
{
    public class Account
    {
        public static HashSet<Account> Accounts = new HashSet<Account>();

        #region Properties
        public int? ID { get; set; }
        public string Name { get; set; }
        public Page Page { get; set; }
        public Account MainAccount { get; set; }
        public int MainAccountId { get; set; }
        public List<Tag> Tags { get; set; }
        #endregion

        #region Constructors
        public Account(string name)
        {
            Name = name;
            Page = Page.GetDefaultPage();
            MainAccount = this;
            Tags = new List<Tag>();
        }

        public Account(int? id, string name)
        {
            ID = id;
            Name = name;
            Page = Page.GetDefaultPage();
            MainAccount = this;
            Tags = new List<Tag>();
        }

        public Account(int? id, string name, Page page)
        {
            ID = id;
            Name = name;
            Page = page;
            MainAccount = this;
            Tags = new List<Tag>();
        }

        public Account(int? id, string name, Page page, int mainAccId)
        {
            ID = id;
            Name = name;
            Page = page;
            MainAccountId = mainAccId;
            Tags = new List<Tag>();
        }

        public Account(int? id, string name, Page page, Account main)
        {
            ID = id;
            Name = name;
            Page = page;
            MainAccount = main;
            Tags = new List<Tag>();
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
                    string cmdStr = "INSERT INTO account (name, main_account, page) VALUES (@Name, @MainAccount, @Page);";
                    SQLiteCommand cmd = new SQLiteCommand(cmdStr, con);
                    cmd.Parameters.AddWithValue("@Name", this.Name);
                    cmd.Parameters.AddWithValue("@MainAccount", this.MainAccount.ID);
                    cmd.Parameters.AddWithValue("@Page", this.Page.ID);
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
                    string cmdStr = "UPDATE account SET name=@Name, main_account=@MainAccount, page=@Page WHERE ID=@Id;";
                    SQLiteCommand cmd = new SQLiteCommand(cmdStr, con);
                    cmd.Parameters.AddWithValue("@Name", this.Name);
                    cmd.Parameters.AddWithValue("@MainAccount", this.MainAccountId);
                    cmd.Parameters.AddWithValue("@Page", this.Page.ID);
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


        public static Account LoadFromDatabase()
        {
            Account a = null;
            string conStr = Properties.Settings.Default.ConnectionString;
            using (SQLiteConnection con = new SQLiteConnection(conStr))
            {
                string cmdStr = "SELECT * FROM account " +
                    "JOIN page ON account.page = page.ID;";
                SQLiteCommand cmd = new SQLiteCommand(cmdStr, con);
                con.Open();
                try
                {
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                        a = new Account(
                            Convert.ToInt32(reader["ID"].ToString()),
                            reader["name"].ToString(),
                            Page.GetById(Convert.ToInt32(reader["page"].ToString())),
                            Convert.ToInt32(reader["main_account"].ToString()));
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
            return a;
        }
        #endregion


        public static Account GetById(int id)
        {
            return Accounts.ToList().Find(a => a.ID == id);
        }

    }
}
