using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
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

        }

        private void BtnScores_Click(object sender, RoutedEventArgs e)
        {
            mWin.frame.Content = new Pages.ScoresPage(mWin);
        }
    }
}
