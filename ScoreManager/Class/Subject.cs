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
    class Subject
    {
        /// <summary>
        /// Adds subject to selected exam
        /// </summary>
        /// <param name="ExamID">ID of the selected exam</param>
        /// <param name="Subject">Name/Title of the subject (e.g. Math)</param>
        public void AddSubject(int ExamID, string Subject)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "INSERT INTO tblSubjects(ID_EXAM, CODE) VALUES(@examid, @subject);";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.Add("@examid", DbType.Int32).Value = ExamID;
                        cmd.Parameters.Add("@subject", DbType.String).Value = Subject;
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

        /// <summary>
        /// Update the information of the subject.
        /// </summary>
        /// <param name="ID">ID of the subject</param>
        /// <param name="Subject">Name/Title of the subject</param>
        public void UpdateSubject(int ID, string Subject)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "UPDATE tblSubjects " +
                        "SET CODE = @subject " +
                        "WHERE ID = @id;";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.Add("@id", DbType.Int32).Value = ID;
                        cmd.Parameters.Add("@subject", DbType.String).Value = Subject;
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

        /// <summary>
        /// Remove/delete the selected subject
        /// </summary>
        /// <param name="ID">ID of the subject</param>
        public void RemoveSubject(int ID)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "DELETE FROM tblSubjects WHERE ID = @id;";
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
        
        /// <summary>
        /// Returns an observablecollection of subjects of the selected exam
        /// </summary>
        /// <param name="ExamID">ID of the selected exam</param>
        /// <returns></returns>
        public ObservableCollection<Models._Subject> GetSubjects(int ExamID)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "SELECT ID, CODE FROM tblSubjects WHERE ID_EXAM = @examid;";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.Add("@examid", DbType.String).Value = ExamID;
                        conn.Open();
                        var col = new ObservableCollection<Models._Subject>();

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                col.Add(new Models._Subject
                                {
                                    ID = Convert.ToInt32(reader[0]),
                                    CODE = reader[1].ToString()
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
    }
}
