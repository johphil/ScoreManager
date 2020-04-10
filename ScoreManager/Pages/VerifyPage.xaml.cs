using System;
using System.Collections.Generic;
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
    /// Interaction logic for VerifyPage.xaml
    /// </summary>
    public partial class VerifyPage : Page
    {
        MainWindow mWin;
        public VerifyPage(MainWindow mWin)
        {
            InitializeComponent();
            this.mWin = mWin;
            tbStudentNo.Focus();
        }

        /***************************************************************************/
        List<Models._Exam> _examTable = new List<Models._Exam>();
        List<Models._Term> _termTable = new List<Models._Term>();
        List<Models._Subject> _subjTable = new List<Models._Subject>();
        Models._Exam _e = new Models._Exam();
        Models._Term _t = new Models._Term();
        Models._Student _st = new Models._Student();
        Class.Student stClass = new Class.Student();
        Class.Grade gClass = new Class.Grade();
        Class.Subject sClass = new Class.Subject();
        List<string> lScores = new List<string>();
        string StudentID = "";

        private void LoadExams()
        {
            _examTable = stClass.GetSingleStudentExams(StudentID);
            dgExams.ItemsSource = null;
            dgExams.ItemsSource = _examTable;
        }

        private void LoadTerms(int ExamID)
        {
            _termTable = stClass.GetSingleStudentTerms(StudentID, ExamID);
            dgTerms.ItemsSource = _termTable;
        }

        private void LoadResult()
        {
            if (dgExams.SelectedIndex != -1 && dgTerms.SelectedIndex != -1)
            {
                DataTable dtResultfromDb = gClass.GetSingleGradeInExam(StudentID, _e.ID, _t.ID);

                _subjTable = sClass.GetSubjects(_e.ID).ToList();

                DataTable dtResult = new DataTable();

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

                if (_subjTable.Count > 0 && _subjTable != null && dtResultfromDb != null && dtResultfromDb.Rows.Count > 0)
                {
                    DataRow row = dtResult.NewRow();
                    row["STUDENT NO"] = StudentID;
                    lScores.Clear();
                    for (int i = 0; i < _subjTable.Count; i++)
                    {
                        row[_subjTable[i].CODE] = dtResultfromDb.Rows[i][2];
                        lScores.Add(dtResultfromDb.Rows[i][2].ToString());
                    }
                    dtResult.Rows.Add(row);
                }

                dgResults.ItemsSource = dtResult.DefaultView;
            }
            else
            {
                dgResults.Columns.Clear();
                dgResults.ItemsSource = null;
            }
        }

        private void SendEmail()
        {
            Class.Email emClass = new Class.Email();
            emClass.LoadEmailSettings(out int UseMail, out string EmailAddr1, out string EmailPass1, out string EmailAddr2, out string EmailPass2, out string EmailFoot);

            string EmailAddr = "";
            string EmailPass = "";

            switch (UseMail)
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
                if (dgExams.SelectedIndex != -1 && dgTerms.SelectedIndex != -1 && _st != null)
                {
                    if (MessageBox.Show(String.Format("Email will be sent to:\n{0}\n{1}\n\nDo you want to proceed?", _st.NAME, _st.EMAIL), "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                    {
                        string Result = "";
                        if (lScores.Count != 0)
                        {
                            //Declare a new BackgroundWorker
                            BackgroundWorker worker = new BackgroundWorker();

                            worker.DoWork += async (o, ea) =>
                            {
                                string subject = String.Format("{0} {1} {2}", Globals.EmailSubject, _e.NAME, _t.TERM);
                                string scores = "";

                                for (int i = 0; i < _subjTable.Count; i++)
                                {
                                    scores += String.Format("{0} = {1}\n", _subjTable[i].CODE, lScores[i].ToString());
                                }
                                string body = String.Format("{0} {1} \n\n{2}\n\n{3}\n\n{4}"
                                    , _e.NAME
                                    , _t.TERM
                                    , _st.NAME + "\n" + _st.ID + "\n" + _st.PROGRAM
                                    , scores
                                    , EmailFoot
                                    );
                                
                                bool isSuccess = false;
                                await emClass.SendMail(UseMail, EmailAddr, EmailPass, _st.EMAIL, subject, body);
                                isSuccess = emClass.isSuccessSendMail;

                                if (isSuccess)
                                    Result = "Email has been sent successfully!";
                                else
                                    Result = "Email has failed!";


                                MessageBox.Show(Result, "Email Status", MessageBoxButton.OK, MessageBoxImage.Information); 
                            };
                                
                            worker.RunWorkerCompleted += (o, ea) =>
                            {
                                txtSendStatus.Text = "";
                                this.IsEnabled = true;
                            };

                            txtSendStatus.Text = "Sending...";
                            this.IsEnabled = false;
                            worker.RunWorkerAsync();
                        }
                        else
                            MessageBox.Show("Scores for this exam are not yet set. Kindly go to Scores Page and update this student's scores and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
                else
                    MessageBox.Show("Please select enter the student number, select the exam/class and term to be emailed and try again.", "Try Again", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void Refresh()
        {
            dgExams.ItemsSource = null;
            dgTerms.ItemsSource = null;
            dgResults.ItemsSource = null;
            txtName.Text = "";
            lScores.Clear();
            _st = null;
        }
        /***************************************************************************/

        private void DgExams_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgExams.SelectedIndex != -1)
            {
                _e = (Models._Exam)dgExams.SelectedItem;
                LoadTerms(_e.ID);
            }
            else
                dgExams.ItemsSource = null;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
            StudentID = tbStudentNo.Text.Trim();
            stClass.StudentInfo(StudentID, out string Name, out string Program, out string Email);
            _st = new Models._Student { ID = StudentID, NAME = Name, PROGRAM = Program, EMAIL = Email };
            txtName.Text = String.Format("{0} {1} {2}", Name, Program, Email);
            dgTerms.ItemsSource = null;
            LoadExams();
        }

        private void DgTerms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgTerms.SelectedIndex != -1)
            {
                _t = (Models._Term)dgTerms.SelectedItem;
                LoadResult();
            }
        }

        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            mWin.frame.Content = new Pages.HomePage(mWin);
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
            tbStudentNo.Clear();
            tbStudentNo.Focus();
        }

        private void BtnEmail_ClickAsync(object sender, RoutedEventArgs e)
        {
            SendEmail();
            //SendMailOffice365();
        }
    }
}
