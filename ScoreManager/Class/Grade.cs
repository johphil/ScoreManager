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
        /// <summary>
        /// Returns a datatable of all students enrolled in a class/exam to be used in filling up the datagrid 
        /// </summary>
        /// <param name="ExamID">ID of the selected exam</param>
        /// <param name="TermID">ID of the selected term</param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the result of a student in a selected exam. Returns datatable.
        /// </summary>
        /// <param name="StudentID">Student Number/ID of the student</param>
        /// <param name="ExamID">ID of the selected exam</param>
        /// <param name="TermID">ID of the selected term in the exam</param>
        /// <returns></returns>
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

        /// <summary>
        /// Saves the scores of the students into database.
        /// </summary>
        /// <param name="ExamID">ID of the EXAM</param>
        /// <param name="TermID">ID of the term</param>
        /// <param name="StudentID">Student Number/ID</param>
        /// <param name="SubjectID">A subject in an exam (e.g. MATH, GEAS, EST, ELEC for ECE CORREL)</param>
        /// <param name="Grade">The grade/score of the student in that subject</param>
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
