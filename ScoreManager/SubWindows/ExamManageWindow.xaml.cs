using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace ScoreManager.SubWindows
{
    /// <summary>
    /// Interaction logic for ExamManageWindow.xaml
    /// </summary>
    public partial class ExamManageWindow : Window
    {
        public ExamManageWindow()
        {
            InitializeComponent();
            LoadExamTable();
            LoadComboBoxTerms();
        }

        /**************************************************************************/
        Class.Term tClass = new Class.Term();
        Class.Exam eClass = new Class.Exam();
        Class.Subject sClass = new Class.Subject();

        Models._Exam _e = new Models._Exam();
        Models._Term _t = new Models._Term();
        Models._Subject _s = new Models._Subject();

        ObservableCollection<Models._Exam> _examTable = new ObservableCollection<Models._Exam>();
        ObservableCollection<Models._Term> _termTable = new ObservableCollection<Models._Term>();
        ObservableCollection<Models._Subject> _subjTable = new ObservableCollection<Models._Subject>();

        List<int> lTermID = new List<int>();
        List<string> lTerm = new List<string>();

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

        private void LoadSubjectTable()
        {
            _e = (Models._Exam)dgExams.SelectedItem;

            _subjTable = sClass.GetSubjects(_e.ID);
            dgSubjects.ItemsSource = _subjTable;
        }

        private void LoadComboBoxTerms()
        {
            var tupleTerms = tClass.GetTerms_List();
            lTermID = tupleTerms.Item1;
            lTerm = tupleTerms.Item2;

            cbTerm.ItemsSource = null;
            cbTerm.ItemsSource = lTerm;
        }
        /**************************************************************************/
        
        private void BtnTerms_Click(object sender, RoutedEventArgs e)
        {
            SubWindows.TermWindow tWin = new SubWindows.TermWindow();
            tWin.ShowDialog();

            if (tWin.isRefresh)
                LoadComboBoxTerms();
        }

        private void DgExams_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgExams.SelectedIndex != -1)
            {
                LoadTermTable();
                LoadSubjectTable();
            }
        }

        private void BtnRemoveTerm_Click(object sender, RoutedEventArgs e)
        {
            if (dgTerms.SelectedIndex != -1)
            {
                _t = (Models._Term)dgTerms.SelectedItem;
                eClass.RemoveExamTerm(_e.ID, _t.ID);
                LoadTermTable();
            }
            else
                MessageBox.Show("Please select a term.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private void BtnRemoveExam_Click(object sender, RoutedEventArgs e)
        {
            if (dgExams.SelectedIndex != -1)
            {
                _e = (Models._Exam)dgExams.SelectedItem;

                if (MessageBox.Show("Do you want to remove this exam?\n\n" + _e.NAME, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    eClass.RemoveExam(_e.ID);
                    LoadExamTable();
                    dgTerms.ItemsSource = null;
                    dgSubjects.ItemsSource = null;
                }
            }
            else
                MessageBox.Show("Please select an exam/class.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private void BtnOkExam_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbExamName.Text.ToString().Trim()))
            {
                if (_examTable != null && _examTable.Any(i => i.NAME == tbExamName.Text.ToString().Trim()))
                {
                    MessageBox.Show("This exam already exists.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    tbExamName.SelectAll();
                    tbExamName.Focus();
                }
                else
                {
                    eClass.AddExam(tbExamName.Text.ToString().Trim());
                    tbExamName.Clear();
                    LoadExamTable();
                    dgTerms.ItemsSource = null;
                    dgSubjects.ItemsSource = null;
                }
            }
        }

        private void BtnOkTerm_Click(object sender, RoutedEventArgs e)
        {
            if (cbTerm.SelectedIndex != -1 && dgExams.SelectedIndex != -1)
            {
                if (_termTable != null && _termTable.Any(i => i.TERM == cbTerm.SelectedItem.ToString()))
                {
                    MessageBox.Show("This term already exists.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    cbTerm.SelectedIndex = -1;
                }
                else
                {
                    eClass.AddExamTerm(_e.ID, lTermID[cbTerm.SelectedIndex]);
                    cbTerm.SelectedIndex = -1;
                    LoadTermTable();
                }
            }
            else if (dgExams.SelectedIndex == -1)
                MessageBox.Show("Please select an exam/class.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            else if (cbTerm.SelectedIndex == -1)
                MessageBox.Show("Please select a term.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);

        }

        private void BtnOkSubject_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbSubject.Text.ToString().Trim()) && dgExams.SelectedIndex != -1)
            {
                if (_subjTable != null && _subjTable.Any(i => i.CODE == tbSubject.Text.ToString().Trim()))
                {
                    MessageBox.Show("This subject already exists.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    tbSubject.SelectAll();
                    tbSubject.Focus();
                }
                else
                {
                    sClass.AddSubject(_e.ID, tbSubject.Text.ToString().Trim());
                    tbSubject.Clear();
                    LoadSubjectTable();
                }
            }
            else if (dgExams.SelectedIndex == -1)
                MessageBox.Show("Please select an exam/class.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            else if (string.IsNullOrWhiteSpace(tbSubject.Text.ToString().Trim()))
                MessageBox.Show("Please enter a subject.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private void BtnRemoveSubject_Click(object sender, RoutedEventArgs e)
        {
            if (dgSubjects.SelectedIndex != -1)
            {
                _s = (Models._Subject)dgSubjects.SelectedItem;

                if (MessageBox.Show("Do you want to remove this subject?\n\n" + _s.CODE, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    sClass.RemoveSubject(_s.ID);
                    LoadSubjectTable();
                }
            }
            else
                MessageBox.Show("Please select a subject.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
    }
}
