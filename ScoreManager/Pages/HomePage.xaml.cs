using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
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
    public partial class HomePage : Page
    {
        MainWindow mWin;
        public HomePage(MainWindow mWin)
        {
            InitializeComponent();
            this.mWin = mWin;
            
            Refresh();
        }

        /******************************************************************************/
        Class.Term tClass = new Class.Term();
        Class.Exam eClass = new Class.Exam();
        Class.Grade gClass = new Class.Grade();
        Class.Student stClass = new Class.Student();
        Class.Subject sClass = new Class.Subject();
        Class.Logging log = new Class.Logging();
        Models._Exam _e = new Models._Exam();
        Models._Term _t = new Models._Term();
        Models._Grade _g = new Models._Grade();
        Models._Student _st = new Models._Student();
        Models._Subject _s = new Models._Subject();
        ObservableCollection<Models._Exam> _examTable = new ObservableCollection<Models._Exam>();
        ObservableCollection<Models._Term> _termTable = new ObservableCollection<Models._Term>();
        ObservableCollection<Models._Student> _studTable = new ObservableCollection<Models._Student>();
        ObservableCollection<Models._Subject> _subjTable = new ObservableCollection<Models._Subject>();
        DataTable dtResult, dtResultfromDb;
        List<string> lScores = new List<string>();

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
                        DataType = System.Type.GetType("System.String")
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
                                {
                                    row[_subjTable[j].CODE] = dtResultfromDb.Rows[j + offset][2];
                                    lScores.Add(dtResultfromDb.Rows[j + offset][2].ToString());
                                }
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

        private void SendEmailGroup()
        {
            Class.Email emClass = new Class.Email();
            emClass.LoadEmailSettings(out int UseMail, out string EmailAddr1, out string EmailPass1, out string EmailAddr2, out string EmailPass2, out string EmailFoot);

            string EmailAddr = "";
            string EmailPass = "";

            switch(UseMail)
            {
                case Globals.USE_EMAIL_GMAIL:
                    {
                        EmailAddr = EmailAddr1;
                        EmailPass = EmailPass1;
                        break;
                    }
                case Globals.USE_EMAIL_MAPUA:
                    {
                        EmailAddr = EmailAddr2;
                        EmailPass = EmailPass2;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            if (string.IsNullOrEmpty(EmailAddr) || string.IsNullOrWhiteSpace(EmailPass))
            {
                MessageBox.Show("Email Settings is not set. Please go to settings and update the email info.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                if (dgExams.SelectedIndex != -1 && dgTerms.SelectedIndex != -1 && _studTable != null)
                {
                    string confirmationmsg = "";
                    for(int i = 0; i < _studTable.Count; i++)
                    {
                        confirmationmsg += String.Format("{0} {1}\n", _studTable[i].ID, _studTable[i].NAME);
                    }
                    if (MessageBox.Show(String.Format("Email will be sent to {0} students. \n\n{1}\n\nDo you want to proceed?", _studTable.Count, confirmationmsg), "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                    {
                        string Result = "";
                        mWin.isWorking = true;
                        progressBar.IsIndeterminate = true;

                        if (lScores.Count != 0)
                        {
                            //Declare a new BackgroundWorker
                            BackgroundWorker worker = new BackgroundWorker();
                            List<Models._Student> lErrorMails = new List<Models._Student>();
                            List<string> lErrorInfo = new List<string>();

                            worker.DoWork += async (o, ea) =>
                            {
                                string subject = String.Format("{0} {1} {2}", Globals.EmailSubject, _e.NAME, _t.TERM);
                                string scores = "";

                                int offset = 0;
                                List<string> lStudentEmails = new List<string>();
                                bool isSuccess = false;

                                for (int x = 0; x < _studTable.Count; x++)
                                {
                                    scores = "";

                                    try
                                    {
                                        for (int i = 0; i < _subjTable.Count; i++)
                                        {
                                            scores += String.Format("{0} = {1}\n", _subjTable[i].CODE, lScores[i + offset].ToString());
                                        }
                                    }
                                    catch
                                    {
                                        lErrorMails.Add(_studTable[x]);
                                        lErrorInfo.Add("Score not set.");
                                    }

                                    offset += _subjTable.Count;

                                    string body = String.Format("{0} {1} \n\n{2}\n\n{3}\n\n{4}"
                                        , _e.NAME
                                        , _t.TERM
                                        , _studTable[x].NAME + "\n" + _studTable[x].ID + "\n" + _studTable[x].PROGRAM
                                        , scores
                                        , EmailFoot
                                        );

                                    await emClass.SendMail(UseMail, EmailAddr, EmailPass, _studTable[x].EMAIL, subject, body);

                                    isSuccess = emClass.isSuccessSendMail;

                                    if (!isSuccess)
                                    {
                                        lErrorMails.Add(_studTable[x]);
                                        lErrorInfo.Add("Invalid Email.");
                                    }
                                    
                                    int percent = (int)(((double)(x + 1) / _studTable.Count) * 1000);
                                    //worker.ReportProgress(percent, String.Format("{0} / {1}", x+1, _studTable.Count));
                                }

                                if (lErrorMails.Count > 0)
                                {
                                    string ErrorFile = String.Format("{0}_{1}_{2}.txt", _e.NAME, _t.TERM, DateTime.Now.ToShortDateString() + DateTime.Now.ToLongTimeString());
                                    ErrorFile = ErrorFile.Replace("/", "");
                                    ErrorFile = ErrorFile.Replace(":", "");
                                    ErrorFile = ErrorFile.Replace(" ", "");
                                    Result = "Errors Found:\n"; 
                                    for (int i = 0; i < lErrorMails.Count; i++)
                                    {
                                        Result += String.Format("{0}. [{1} {2}] {3} \n\n", i+1, lErrorMails[i].ID, lErrorMails[i].NAME, lErrorInfo[i]);
                                    }
                                    log.WriteToFileErrors(ErrorFile, Result);
                                    Result += String.Format("Error saved at {0}{1}",Globals.PATH_EMAIL_ERRORS, ErrorFile);
                                }
                                else
                                {
                                    Result = "Emails successfuly sent! No Errors.";
                                }

                                MessageBox.Show(Result, "Email Result", MessageBoxButton.OK, MessageBoxImage.Information);
                                mWin.isWorking = false;
                            };

                            /*worker.WorkerReportsProgress = true;

                            worker.ProgressChanged += (o, ea) =>
                            {
                                progressBar.Value = ea.ProgressPercentage;
                                txtSendStatus.Text = (string)ea.UserState;
                            };*/

                            worker.RunWorkerCompleted += (o, ea) =>
                            {
                                txtSendStatus.Text = "";
                                progressBar.Value = 0;
                                this.IsEnabled = true;
                            };

                            txtSendStatus.Text = "Sending...";
                            this.IsEnabled = false;
                            worker.RunWorkerAsync();

                            Result = "Email has been sent successfully!";
                        }
                        else
                            MessageBox.Show("Scores for this exam are not yet set. Kindly go to Scores Page and update this student's scores and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
                else
                    MessageBox.Show("Please select enter the student number, select the exam/class and term to be emailed and try again.", "Try Again", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        /******************************************************************************/
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            mWin.isLogout = true;
            mWin.Close();
            LoginWindow lWin = new LoginWindow();
            lWin.Show();
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

        private void BtnEnrollment_Click(object sender, RoutedEventArgs e)
        {
            if (dgExams.SelectedIndex != -1 && dgTerms.SelectedIndex != -1)
                mWin.frame.Content = new Pages.EnrollmentPage(mWin, _e.ID, _t.ID, _e.NAME, _t.TERM);
            else
                MessageBox.Show("Please Select an Exam/Class and Term and try again.", "Try Again", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            mWin.frame.Content = new Pages.SettingsPage(mWin);
        }

        private void BtnVerification_Click(object sender, RoutedEventArgs e)
        {
            mWin.frame.Content = new Pages.VerifyPage(mWin);
        }

        private void BtnEmailMultiple_Click(object sender, RoutedEventArgs e)
        {
            SendEmailGroup();
        }

        private void BtnScores_Click(object sender, RoutedEventArgs e)
        {
            mWin.frame.Content = new Pages.ScoresPage(mWin);
        }
    }
}
