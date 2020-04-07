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
    class Exam
    {
        public void AddExam(string Name)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "INSERT INTO tblExams(NAME) VALUES(@name);";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.Add("@name", DbType.String).Value = Name;
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

        public void AddExamTerm(int ExamID, int TermID)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "INSERT INTO tblExamTerms(ID_EXAM, ID_TERM) VALUES(@examid, @termid);";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.Add("@examid", DbType.String).Value = ExamID;
                        cmd.Parameters.Add("@termid", DbType.Int32).Value = TermID;
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

        public void RemoveExam(int ID)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "DELETE FROM tblExams WHERE ID = @id; " +
                        "DELETE FROM tblSubjects WHERE ID = @id";
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

        public void RemoveExamTerm(int ExamID, int TermID)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "DELETE FROM tblExamTerms WHERE ID_EXAM = @examid AND ID_TERM = @termid;";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.Add("@examid", DbType.Int32).Value = ExamID;
                        cmd.Parameters.Add("@termid", DbType.Int32).Value = TermID;

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

        public ObservableCollection<Models._Exam> GetExams()
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "SELECT * FROM tblExams ORDER BY ID DESC;";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        conn.Open();
                        var col = new ObservableCollection<Models._Exam>();

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                col.Add(new Models._Exam
                                {
                                    ID = Convert.ToInt32(reader[0]),
                                    NAME = reader[1].ToString()
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

        public Tuple<List<int>, List<string>> GetExams_List(bool ForSearch)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "SELECT ID, NAME FROM tblExams ORDER BY ID DESC;";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        conn.Open();

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            List<int> e = new List<int>();
                            List<string> e2 = new List<string>();

                            if (ForSearch)
                            {
                                e.Add(0);
                                e2.Add("ALL");
                            }

                            while (reader.Read())
                            {
                                e.Add(int.Parse(reader[0].ToString()));    //ID
                                e2.Add(reader[1].ToString());   //NAME
                            }

                            return Tuple.Create(e, e2);
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
    }
}
