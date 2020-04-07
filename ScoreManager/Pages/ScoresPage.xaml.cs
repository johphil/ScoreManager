using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
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

namespace ScoreManager.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class ScoresPage : Page
    {
        MainWindow mWin;
        public ScoresPage(MainWindow mWin)
        {
            InitializeComponent();
            this.mWin = mWin;
            
            Refresh();
        }

        /******************************************************************************/
        Class.Term tClass = new Class.Term();
        Class.Exam eClass = new Class.Exam();
        Class.Student stClass = new Class.Student();
        Class.Subject sClass = new Class.Subject();
        Class.Grade gClass = new Class.Grade();
        Models._Exam _e = new Models._Exam();
        Models._Term _t = new Models._Term();
        Models._Student _st = new Models._Student();
        Models._Subject _s = new Models._Subject();
        Models._Grade _g = new Models._Grade();
        ObservableCollection<Models._Exam> _examTable = new ObservableCollection<Models._Exam>();
        ObservableCollection<Models._Term> _termTable = new ObservableCollection<Models._Term>();
        ObservableCollection<Models._Student> _studTable = new ObservableCollection<Models._Student>();
        ObservableCollection<Models._Subject> _subjTable = new ObservableCollection<Models._Subject>();
        ObservableCollection<Models._Grade> _gradTable = new ObservableCollection<Models._Grade>();
        DataTable dtResult, dtResultfromDb;
        bool hasError = false;

        private void LoadExamTable()
        {
            _examTable = eClass.GetExams();
            dgExams.ItemsSource = _examTable;
        }

        private void LoadTermTable()
        {
            _e = (Models._Exam)dgExams.SelectedItem;

            _termTable = tClass.GetTerms(_e.ID);
            dgTerms.ItemsSource = _termTable;
        }

        private void Refresh()
        {
            LoadExamTable();
            dgTerms.ItemsSource = null;
            dgResults.ItemsSource = null;
        }

        private void LoadClassResult()
        {
            if (dgExams.SelectedIndex != -1 && dgTerms.SelectedIndex != -1)
            {
                _studTable = stClass.GetStudentsInfoInExam(_e.ID, _t.ID);
                _subjTable = sClass.GetSubjects(_e.ID);
                dtResultfromDb = gClass.GetStudentsGradeInExam(_e.ID, _t.ID);

                dtResult = new DataTable();
                //CREATE COLUMNS
                if (_subjTable != null)
                {
                    DataColumn col;
                    col = new DataColumn("STUDENT NO")
                    {
                        DataType = System.Type.GetType("System.String"),
                        ReadOnly = true
                    };
                    dtResult.Columns.Add(col);

                    for (int i = 0; i < _subjTable.Count; i++)
                    {
                        col = new DataColumn(_subjTable[i].CODE)
                        {
                            DataType = System.Type.GetType("System.Double"),
                            DefaultValue = 0
                        };
                        dtResult.Columns.Add(col);
                    }
                }

                //ROWS
                if (_studTable != null)
                {
                    DataRow row;
                    int offset = 0;
                    for (int i = 0; i < _studTable.Count; i++)
                    {
                        row = dtResult.NewRow();
                        row["STUDENT NO"] = _studTable[i].ID;
                        if (dtResultfromDb != null)
                        {
                            for (int j = 0; j < _subjTable.Count; j++)
                            {
                                if (dtResultfromDb.Rows.Count > j + offset)
                                    row[_subjTable[j].CODE] = dtResultfromDb.Rows[j + offset][2];
                            }
                            offset += _subjTable.Count;
                        }
                        dtResult.Rows.Add(row);
                    }
                }

                dgResults.ItemsSource = dtResult.DefaultView;
            }
            else
            {
                dgResults.Columns.Clear();
                dgResults.ItemsSource = null;
            }
        }

        //WORKER
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            //worker.ReportProgress(0, String.Format("Processing Iteration 1."));
            int TotalItems = _subjTable.Count * dtResult.Rows.Count;
            int x = 1;
            int percent = 0;

            try
            {
                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "INSERT OR REPLACE INTO tblGrades(EXAM_ID, TERM_ID, STUDENT_ID, SUBJECT_ID, GRADE) VALUES(@examid, @termid, @studentid, @subjectid, @grade);";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        con.Open();
                        using (var transaction = con.BeginTransaction())
                        {
                            for (int i = 0; i < dtResult.Rows.Count; i++) //rows
                            {
                                for (int j = 0; j < _subjTable.Count; j++) //columns
                                {
                                    cmd.Parameters.Add("@examid", DbType.Int32).Value = _e.ID;
                                    cmd.Parameters.Add("@termid", DbType.Int32).Value = _t.ID;
                                    cmd.Parameters.Add("@studentid", DbType.String).Value = _studTable[i].ID;
                                    cmd.Parameters.Add("@subjectid", DbType.Int32).Value = _subjTable[j].ID;
                                    cmd.Parameters.Add("@grade", DbType.Double).Value = Convert.ToDouble(dtResult.Rows[i][_subjTable[j].CODE]);
                                    cmd.ExecuteNonQuery();

                                    percent = (int)((double)x / TotalItems * 1000);
                                    worker.ReportProgress(percent, String.Format("{0}%", percent/10));
                                    x++;

                                    //ANIMATE
                                    Thread.Sleep(15);
                                }
                            }
                            transaction.Commit();
                        }
                    }
                    con.Close();
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                
                btnRefresh.IsEnabled = true;
                btnSave.IsEnabled = true;
            }
            else
            {
                System.Windows.MessageBox.Show("Save Completed!", "Complete", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                progressBar.Value = 0;
                txtStatus.Text = "";

                btnRefresh.IsEnabled = true;
                btnSave.IsEnabled = true;
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            txtStatus.Text = (string)e.UserState;
        }

        //SAVE GRADE
        private void StartSave()
        {
            btnRefresh.IsEnabled = false;
            btnSave.IsEnabled = false;

            BackgroundWorker worker = new BackgroundWorker();
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerAsync();
        }
        /******************************************************************************/
        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            mWin.frame.Content = new Pages.HomePage(mWin);
        }
       
        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void DgExams_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgExams.SelectedIndex != -1)
                LoadTermTable();
        }

        private void DgTerms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _t = (Models._Term)dgTerms.SelectedItem;
            LoadClassResult();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (dgExams.SelectedIndex == -1 || dgTerms.SelectedIndex == -1)
            {
                MessageBox.Show("Nothing to save.", "Empty", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (dtResult != null)
            {
                if (dtResult.Rows.Count == 0)
                    MessageBox.Show("Nothing to save.", "Empty", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                else
                {
                    hasError = false;
                    StartSave();
                }
            }
        }
    }
}
