using ExcelDataReader;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.OleDb;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.Office.Interop.Excel;

namespace ScoreManager.Pages
{
    /// <summary>
    /// Interaction logic for ImportPage.xaml
    /// </summary>
    public partial class EnrollmentPage : System.Windows.Controls.Page
    {
        MainWindow mWin;
        public EnrollmentPage(MainWindow mWin, int ExamID, int TermID, string Exam, string Term)
        {
            InitializeComponent();
            this.mWin = mWin;
            this.ExamID = ExamID;
            this.TermID = TermID;
            txtExamTerm.Text = Exam + " " + Term;
            btnCancelEdit.Visibility = Visibility.Hidden;
            Refresh();
        }

        /******************************************************************************/
        int ExamID = 0, TermID = 0;
        bool hasError = false;
        bool isEdit = false;
        Class.Student stClass = new Class.Student();
        Models._Student _s = new Models._Student();
        ObservableCollection<Models._Student> _studTable = new ObservableCollection<Models._Student>();
        ObservableCollection<Models._Student> ExcelData;
        List<_DeleteStudents> lDeleteStudents = new List<_DeleteStudents>();

        struct _DeleteStudents
        {
            public int EXAM_ID { get; set; }
            public int TERM_ID { get; set; }
            public string STUDENT_ID { get; set; }
        }

        private void Refresh()
        {
            _studTable = stClass.GetStudentsInfoInExam(ExamID, TermID);

            if (_studTable == null || _studTable.Count == 0)
                _studTable = new ObservableCollection<Models._Student>();

            dgStudents.ItemsSource = null;
            dgStudents.ItemsSource = _studTable;
        }

        private void AddStudent()
        {
            //Check if student number exists
            List<Models._Student> lExist = null;

            if (_studTable != null)
                lExist = _studTable.Where(x => x.ID == tbStudentNo.Text.Trim()).ToList();

            if (lExist != null && lExist.Count() > 0)
                MessageBox.Show("This Student No. already exists.", "Already Exist", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                _studTable.Add(new Models._Student { ID = tbStudentNo.Text.Trim(), NAME = tbStudentName.Text.Trim(), PROGRAM = tbProgram.Text.Trim(), EMAIL = tbEmail.Text.Trim() });
                tbStudentNo.Clear();
                tbStudentName.Clear();
                tbProgram.Clear();
                tbEmail.Clear();
                tbStudentNo.Focus();
            }
        }

        private ObservableCollection<Models._Student> GetExcelData(string filePath)
        {
            this.IsEnabled = false;
            txtStatus.Text = "Loading Please Wait...";

            try
            {
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    // Auto-detect format, supports:
                    //  - Binary Excel files (2.0-2003 format; *.xls)
                    //  - OpenXml Excel files (2007 format; *.xlsx)
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var idm = new ObservableCollection<Models._Student>();
                            
                        // 1. Use the reader methods
                        reader.Read(); //skip 1 line (header)
                        txtSheetName.Text = reader.Name;
                        
                        //start 2nd row
                        while (reader.Read())
                        {
                            idm.Add(new Models._Student()
                            {
                                ID = string.IsNullOrWhiteSpace(reader.GetValue(0).ToString()) ? "ERROR PLS TRY AGAIN" : reader.GetValue(0).ToString(),
                                NAME = string.IsNullOrWhiteSpace(reader.GetString(1)) ? "ERROR PLS TRY AGAIN" : reader.GetString(1),
                                PROGRAM = string.IsNullOrWhiteSpace(reader.GetString(2)) ? "ERROR PLS TRY AGAIN" : reader.GetString(2),
                                EMAIL = string.IsNullOrWhiteSpace(reader.GetString(3)) ? "" : reader.GetString(3),
                            });
                        }

                        this.IsEnabled = true;
                        txtStatus.Text = "";

                        return idm;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message.ToString(), "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.IsEnabled = true;
                txtStatus.Text = "Error Loading";
                hasError = true;
                return null;
            }
        }

        //WORKER
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;

            int totalDelete = lDeleteStudents.Count;
            int totalStudent = _studTable.Count;
            int TotalItems = totalStudent + totalDelete;
            int i = 1;
            int ii = 0;

            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(Globals.DbConString))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();
                        using (var transaction = conn.BeginTransaction())
                        {

                            if (lDeleteStudents.Count > 0)
                            {
                                for (int z = 0; z < lDeleteStudents.Count; z++)
                                {
                                    cmd.CommandText = "DELETE FROM tblClass WHERE ID_STUDENT = @studentid AND ID_EXAM = @examid AND ID_TERM = @termid;" +
                                                        "DELETE FROM tblGrades WHERE STUDENT_ID = @studentid AND EXAM_ID = @examid AND TERM_ID = @termid;";
                                    cmd.Parameters.Add("@examid", DbType.Int32).Value = lDeleteStudents[z].EXAM_ID;
                                    cmd.Parameters.Add("@termid", DbType.Int32).Value = lDeleteStudents[z].TERM_ID;
                                    cmd.Parameters.Add("@studentid", DbType.String).Value = lDeleteStudents[z].STUDENT_ID;
                                    cmd.ExecuteNonQuery();

                                    //stClass.RemoveStudent(lDeleteStudents[z].EXAM_ID, lDeleteStudents[z].TERM_ID, lDeleteStudents[z].STUDENT_ID);
                                    ii = (int)((double)i / TotalItems * 1000);
                                    worker.ReportProgress(ii, String.Format("Removing {0} / {1} Students", i, totalDelete));
                                    i++;
                                    //ANIMATE
                                    Thread.Sleep(15);
                                }
                            }

                            foreach (var element in _studTable)
                            {
                                cmd.CommandText = "INSERT INTO tblClass(ID_EXAM,ID_TERM,ID_STUDENT) SELECT @examid, @termid, @studentid WHERE NOT EXISTS(SELECT 1 FROM tblClass WHERE ID_EXAM = @examid AND ID_TERM = @termid AND ID_STUDENT = @studentid); " +
                                   "INSERT OR REPLACE INTO tblStudents(ID, NAME, PROGRAM, EMAIL) VALUES(@studentid, @name, @program, @email); ";
                                cmd.Parameters.Add("@examid", DbType.Int32).Value = ExamID;
                                cmd.Parameters.Add("@termid", DbType.Int32).Value = TermID;
                                cmd.Parameters.Add("@studentid", DbType.String).Value = element.ID;
                                cmd.Parameters.Add("@name", DbType.String).Value = element.NAME;
                                cmd.Parameters.Add("@program", DbType.String).Value = element.PROGRAM;
                                cmd.Parameters.Add("@email", DbType.String).Value = element.EMAIL;
                                cmd.ExecuteNonQuery();

                                ii = (int)((double)i / TotalItems * 1000);
                                worker.ReportProgress(ii, String.Format("Saving {0} / {1} Students", i, totalStudent));
                                i++;
                                //ANIMATE
                                Thread.Sleep(15);
                            }

                            transaction.Commit();
                        }

                        conn.Close();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                System.Windows.MessageBox.Show(ex.Message.ToString(), "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //System.Windows.MessageBox.Show("The student has already enrolled in this exam/class.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                hasError = true;
            }

            if (!hasError)
                worker.ReportProgress(1000, "Done Processing.");
            else
                worker.ReportProgress(0, "Error Processing.");
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (hasError)
            {
                System.Windows.MessageBox.Show("Error! Could not continue.", "Operation Failed", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                btnOk.IsEnabled = true;
                btnBrowse.IsEnabled = true;
                btnSave.IsEnabled = true;
                btnRemove.IsEnabled = true;
                //_studTable.Clear();
                //ExcelData.Clear();
            }
            else
            {
                System.Windows.MessageBox.Show("Save Completed!", "Complete", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                progressBar.Value = 0;
                txtStatus.Text = "";
                txtSheetName.Text = "";

                btnOk.IsEnabled = true;
                btnBrowse.IsEnabled = true;
                btnSave.IsEnabled = true;
                btnRemove.IsEnabled = true;
                if (ExcelData != null)
                    ExcelData.Clear();
                //_studTable.Clear();
                //dgStudents.ItemsSource = null;
                //dgStudents.ItemsSource = _studTable;
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            txtStatus.Text = (string)e.UserState;
        }

        //START UPLOAD
        private void StartUpload()
        {
            btnSave.IsEnabled = false;
            btnBrowse.IsEnabled = false;
            btnOk.IsEnabled = false;
            btnRemove.IsEnabled = false;

            BackgroundWorker worker = new BackgroundWorker();
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerAsync();
        }

        private void NotEdit()
        {
            isEdit = false;
            btnOk.Content = "ENROLL";
            tbStudentNo.IsReadOnly = false;
            tbStudentNo.Clear();
            tbStudentName.Clear();
            tbProgram.Clear();
            tbEmail.Clear();
            btnCancelEdit.Visibility = Visibility.Hidden;
        }
        /******************************************************************************/
        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbStudentNo.Text.Trim())
                || string.IsNullOrWhiteSpace(tbStudentName.Text.Trim())
                || string.IsNullOrWhiteSpace(tbProgram.Text.Trim()))
            {
                MessageBox.Show("Invalid! Student Number, Name, or Program cannot be empty.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                if (isEdit)
                {
                    for (int i = 0; i < _studTable.Count; i++)
                    {
                        if (_studTable[i].ID == tbStudentNo.Text.Trim())
                        {
                            _studTable[i].NAME = tbStudentName.Text.Trim();
                            _studTable[i].PROGRAM = tbProgram.Text.Trim();
                            _studTable[i].EMAIL = tbEmail.Text.Trim();
                            break;
                        }
                    }
                    dgStudents.ItemsSource = null;
                    dgStudents.ItemsSource = _studTable;
                    NotEdit();
                }
                else
                    AddStudent();
            }
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (dgStudents.SelectedIndex != -1 && _studTable.Count > 0)
            {
                Models._Student _s = (Models._Student)dgStudents.SelectedItem;
                lDeleteStudents.Add(new _DeleteStudents { EXAM_ID = ExamID, TERM_ID = TermID, STUDENT_ID = _s.ID });
                _studTable.RemoveAt(dgStudents.SelectedIndex);
            }
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            switch (MessageBox.Show("Do you want to create a sample file for import?", "Question", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No))
            {
                case MessageBoxResult.Yes:
                    {
                        var dlg = new CommonOpenFileDialog
                        {
                            Title = "Select Directory to Save File",
                            IsFolderPicker = true,
                            //dlg.InitialDirectory = currentDirectory;

                            AddToMostRecentlyUsedList = false,
                            AllowNonFileSystemItems = false,
                            //dlg.DefaultDirectory = currentDirectory;
                            EnsureFileExists = true,
                            EnsurePathExists = true,
                            EnsureReadOnly = false,
                            EnsureValidNames = true,
                            Multiselect = false,
                            ShowPlacesList = true
                        };

                        try
                        {

                            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
                            {
                                var folder = dlg.FileName;
                                // Do something with selected folder string
                                Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application
                                {
                                    Visible = true,
                                    WindowState = XlWindowState.xlMaximized
                                };

                                Workbook wb = app.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
                                Worksheet ws = wb.Worksheets[1];
                                ws.Name = "Student List";

                                ws.Range["A:A"].NumberFormat = "@";
                                ws.Range["B:B"].NumberFormat = "@";
                                ws.Range["C:C"].NumberFormat = "@";
                                ws.Range["D:D"].NumberFormat = "@";

                                ws.Range["A:A"].ColumnWidth = 20;
                                ws.Range["B:B"].ColumnWidth = 50;
                                ws.Range["C:C"].ColumnWidth = 15;
                                ws.Range["D:D"].ColumnWidth = 50;

                                ws.Range["A1"].Value = "STUDENT NO";
                                ws.Range["B1"].Value = "NAME (LN, FN, MI)";
                                ws.Range["C1"].Value = "PROGRAM";
                                ws.Range["D1"].Value = "EMAIL";


                                wb.SaveAs(folder + "\\SM_Import_Students.xlsx");
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Windows.MessageBox.Show(ex.Message.ToString() + "\nPlease Contact Software Developer", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        break;
                    }
                case MessageBoxResult.No:
                    {
                        OpenFileDialog openFileDialog = new OpenFileDialog
                        {
                            Filter = "Excel Files|*.csv;*.xls;*.xlsx;",
                            ValidateNames = false
                        };
                        Nullable<bool> result = openFileDialog.ShowDialog();
                        if (result == true)
                        {
                            ExcelData = GetExcelData(openFileDialog.FileName);
                            if (ExcelData != null)
                            {
                                _studTable = new ObservableCollection<Models._Student>(_studTable.Union(ExcelData).ToList());
                                dgStudents.ItemsSource = _studTable;
                            }
                        }
                        //openFileDialog.Dispose();
                        break;
                    }
                default:
                    break;
            }
        }

        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            mWin.frame.Content = new Pages.HomePage(mWin);
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void DgStudents_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgStudents.SelectedIndex != -1 && _studTable.Count > 0)
            {
                _s = (Models._Student)dgStudents.SelectedItem;
                isEdit = true;

                tbStudentNo.Text = _s.ID;
                tbStudentName.Text = _s.NAME;
                tbProgram.Text = _s.PROGRAM;
                tbEmail.Text = _s.EMAIL;

                btnOk.Content = "SAVE";
                tbStudentNo.IsReadOnly = true;
                btnCancelEdit.Visibility = Visibility.Visible;
            }
        }

        private void BtnCancelEdit_Click(object sender, RoutedEventArgs e)
        {
            NotEdit();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_studTable == null)
                MessageBox.Show("Cannot save without entries.", "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            else if (MessageBox.Show("All entries will be saved. Do you want to continue?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                hasError = false;
                StartUpload();
            }
        }
    }
}
