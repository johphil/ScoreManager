using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScoreManager.Class
{
    class Term
    {
        public void AddTerm(string Term)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "INSERT INTO tblTerms(TERM) VALUES(@term);";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.Add("@term", DbType.String).Value = Term;
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateTerm(int ID, string Term)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "UPDATE tblTerms " +
                        "SET TERM = @term " +
                        "WHERE ID = @id;";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.Add("@id", DbType.Int32).Value = ID;
                        cmd.Parameters.Add("@term", DbType.String).Value = Term;
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void RemoveTerm(int ID)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "DELETE FROM tblTerms WHERE ID = @id";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.Add("@id", DbType.Int32).Value = ID;
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public ObservableCollection<Models._Term> GetTerms(int ExamID)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "SELECT E.ID_TERM, T.TERM FROM tblExamTerms E JOIN tblTerms T ON T.ID = E.ID_TERM WHERE E.ID_EXAM = @id ORDER BY E.ID_TERM DESC";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.Add("@id", DbType.Int32).Value = ExamID;
                        conn.Open();

                        SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
                        DataTable dtItems = new DataTable();
                        sda.Fill(dtItems);
                        cmd.ExecuteNonQuery();

                        var col = new ObservableCollection<Models._Term>();

                        if (dtItems.Rows.Count != 0)
                        {
                            foreach (DataRow row in dtItems.Rows)
                            {
                                var obj = new Models._Term()
                                {
                                    ID = Convert.ToInt32(row[0]),
                                    TERM = row[1].ToString()
                                };
                                col.Add(obj);
                            }
                            return col;
                        }
                        return null;
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public ObservableCollection<Models._Term> GetTerms()
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "SELECT ID, TERM FROM tblTerms ORDER BY ID DESC;";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        conn.Open();
                        var col = new ObservableCollection<Models._Term>();

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                col.Add(new Models._Term
                                {
                                    ID = Convert.ToInt32(reader[0]),
                                    TERM = reader[1].ToString()
                                });
                            }
                        }
                        return col;
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public Tuple<List<int>, List<string>> GetTerms_List(bool ForSearch)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "SELECT * FROM tblTerms ORDER BY ID DESC;";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        conn.Open();

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            List<int> t = new List<int>();
                            List<string> t2 = new List<string>();

                            if (ForSearch)
                            {
                                t.Add(0);
                                t2.Add("ALL");
                            }

                            while (reader.Read())
                            {
                                t.Add(int.Parse(reader[0].ToString()));    //ID
                                t2.Add(reader[1].ToString());   //TERM
                            }

                            return Tuple.Create(t, t2);
                        }
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return Tuple.Create<List<int>, List<string>>(null, null);
            }
        }

        public bool IsInUse(int ID)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "SELECT COUNT(*) FROM tblExamTerms WHERE ID_TERM = @id;";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.Add("@id", DbType.Int32).Value = ID;
                        conn.Open();

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (Convert.ToInt32(reader[0]) > 0)
                                    return true;
                                else
                                    return false;
                            }
                            return false;
                        }
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
