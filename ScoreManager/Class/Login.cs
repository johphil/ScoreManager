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
    class Login : License
    {
        public int SignIn(string Username, string Password)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "SELECT ID FROM tblUsers WHERE USERNAME = @username AND PASSWORD = @password AND LICENSE = @license;";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.Add("@username", DbType.String).Value = Username;
                        cmd.Parameters.Add("@password", DbType.String).Value = Password;
                        cmd.Parameters.Add("@license", DbType.String).Value = GetLicenseKey();

                        con.Open();

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return Convert.ToInt32(reader[0]);
                            }
                            else
                                return 0;
                        }
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return Globals.SQL_ERROR;
            }
        }
    }
}
