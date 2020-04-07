using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScoreManager.Class
{
    class Email
    {
        public void LoadEmailSettings(out string EmailAddress, out string EmailPassword, out string EmailFooter)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "SELECT * FROM tblSettings;";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        con.Open();

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                EmailAddress = reader[1].ToString();
                                EmailPassword = reader[2].ToString();
                                EmailFooter = reader[3].ToString();
                            }
                            else
                            {
                                EmailAddress = null;
                                EmailPassword = null;
                                EmailFooter = "";
                            }
                        }
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                EmailAddress = null;
                EmailPassword = null;
                EmailFooter = "";
            }
        }

        public void SaveEmailSettings(string EmailAddress, string EmailPassword, string EmailFooter)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "INSERT OR REPLACE INTO tblSettings " +
                        "VALUES(1, @gmail, @pass, @footer);";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        con.Open();

                        cmd.Parameters.Add("@gmail", DbType.String).Value = EmailAddress.Trim();
                        cmd.Parameters.Add("@pass", DbType.String).Value = EmailPassword;
                        cmd.Parameters.Add("@footer", DbType.String).Value = EmailFooter;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
