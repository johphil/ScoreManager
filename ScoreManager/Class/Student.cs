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
    class Student
    {
        /// <summary>
        /// Returns an observablecollection of Students in a selected exam and term
        /// </summary>
        /// <param name="ExamID">ID of the selected exam</param>
        /// <param name="TermID">ID of the selected term in the exam</param>
        /// <returns></returns>
        public ObservableCollection<Models._Student> GetStudentsInfoInExam(int ExamID, int TermID)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "SELECT tblStudents.ID,  tblStudents.NAME, tblStudents.PROGRAM, tblStudents.EMAIL FROM tblStudents " +
                    "INNER JOIN tblClass ON tblStudents.ID = tblClass.ID_STUDENT WHERE tblClass.ID_EXAM = @examid AND tblClass.ID_TERM = @termid ORDER BY tblStudents.ID ASC;";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.Add("@examid", DbType.Int32).Value = ExamID;
                        cmd.Parameters.Add("@termid", DbType.Int32).Value = TermID;

                        con.Open();

                        var col = new ObservableCollection<Models._Student>();

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                col.Add(new Models._Student
                                {
                                    ID = reader[0].ToString(),
                                    NAME = reader[1].ToString(),
                                    PROGRAM = reader[2].ToString(),
                                    EMAIL = reader[3].ToString()
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

        /// <summary>
        /// Using out keywords, gets the student's information (student number, name, program and email)
        /// </summary>
        /// <param name="StudentID">Student number/id as an input</param>
        /// <param name="Name">Name of the student</param>
        /// <param name="Program">Program of the student (e.g. ECE)</param>
        /// <param name="Email">Email address of the student</param>
        public void StudentInfo(string StudentID, out string Name, out string Program, out string Email)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "SELECT NAME, PROGRAM, EMAIL FROM tblStudents WHERE ID = @studentid;";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.Add("@studentid", DbType.String).Value = StudentID;

                        con.Open();

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Name = reader[0].ToString();
                                Program = reader[1].ToString();
                                Email = reader[2].ToString();
                            }
                            else
                            {
                                Name = "NOT FOUND";
                                Program = "";
                                Email = "";
                            }
                        }
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Name = "NOT FOUND";
                Program = "";
                Email = "";
            }
        }
        
        /// <summary>
        /// Returns the list of the Classes/Exams where the student has enrolled.
        /// </summary>
        /// <param name="StudentID">Student number/ID</param>
        /// <returns></returns>
        public List<Models._Exam> GetSingleStudentExams(string StudentID)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "SELECT DISTINCT tblExams.ID, tblExams.NAME FROM tblExams " +
                                    "JOIN tblClass ON tblClass.ID_EXAM = tblExams.ID WHERE tblClass.ID_STUDENT = @studentid;";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.Add("@studentid", DbType.String).Value = StudentID;

                        con.Open();

                        List<Models._Exam> lExams = new List<Models._Exam>();

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lExams.Add(new Models._Exam { ID = Convert.ToInt32(reader[0]), NAME = reader[1].ToString() });
                            }
                        }
                        return lExams;
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
        /// Returns the list of the terms of the selected class/exam where the student has enrolled
        /// </summary>
        /// <param name="StudentID">Student number/ID</param>
        /// <param name="ExamID">ID of the selected exam</param>
        /// <returns></returns>
        public List<Models._Term> GetSingleStudentTerms(string StudentID, int ExamID)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "SELECT tblTerms.ID, tblTerms.TERM FROM tblTerms JOIN tblClass ON tblClass.ID_TERM = tblTerms.ID " + 
                                    "WHERE tblClass.ID_STUDENT = @studentid AND tblClass.ID_EXAM = @examid;";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.Add("@studentid", DbType.String).Value = StudentID;
                        cmd.Parameters.Add("@examid", DbType.Int32).Value = ExamID;

                        con.Open();

                        List<Models._Term> lTerms = new List<Models._Term>();

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lTerms.Add(new Models._Term { ID = Convert.ToInt32(reader[0]), TERM = reader[1].ToString() });
                            }
                        }
                        return lTerms;
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
