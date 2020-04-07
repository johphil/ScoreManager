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
    class Grade
    {
        public DataTable GetStudentsGradeInExam(int ExamID, int TermID)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "SELECT STUDENT_ID, SUBJECT_ID, GRADE FROM tblGrades WHERE EXAM_ID = @examid AND TERM_ID = @termid ORDER BY STUDENT_ID ASC;";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.Add("@examid", DbType.Int32).Value = ExamID;
                        cmd.Parameters.Add("@termid", DbType.Int32).Value = TermID;

                        con.Open();
                        SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
                        DataTable dtItems = new DataTable();
                        sda.Fill(dtItems);
                        cmd.ExecuteNonQuery();
                        return dtItems;
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public DataTable GetSingleGradeInExam(string StudentID, int ExamID, int TermID)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "SELECT STUDENT_ID, SUBJECT_ID, GRADE FROM tblGrades WHERE EXAM_ID = @examid AND TERM_ID = @termid AND STUDENT_ID = @studentid;";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.Add("@examid", DbType.Int32).Value = ExamID;
                        cmd.Parameters.Add("@termid", DbType.Int32).Value = TermID;
                        cmd.Parameters.Add("@studentid", DbType.String).Value = StudentID;

                        con.Open();
                        SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
                        DataTable dtItems = new DataTable();
                        sda.Fill(dtItems);
                        cmd.ExecuteNonQuery();
                        return dtItems;
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public void SaveStudentScores(int ExamID, int TermID, string StudentID, int SubjectID, double Grade)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "INSERT OR REPLACE INTO tblGrades(EXAM_ID, TERM_ID, STUDENT_ID, SUBJECT_ID, GRADE) VALUES(@examid, @termid, @studentid, @subjectid, @grade);";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.Add("@examid", DbType.Int32).Value = ExamID;
                        cmd.Parameters.Add("@termid", DbType.Int32).Value = TermID;
                        cmd.Parameters.Add("@studentid", DbType.String).Value = StudentID;
                        cmd.Parameters.Add("@subjectid", DbType.Int32).Value = SubjectID;
                        cmd.Parameters.Add("@grade", DbType.Double).Value = Grade;

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
    }
}
