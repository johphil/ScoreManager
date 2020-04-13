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
    /// Interaction logic for TermWindow.xaml
    /// </summary>
    public partial class TermWindow : Window
    {
        public TermWindow()
        {
            InitializeComponent();
            ToggleDetails(false);
        }
        /**************************************************************************/
        private bool isNew = false;
        public bool isRefresh = false;
        Class.Term tClass = new Class.Term();
        Models._Term _t = new Models._Term();
        ObservableCollection<Models._Term> _termTable = new ObservableCollection<Models._Term>();

        private void ToggleDetails(bool isShowDetails)
        {
            if (isShowDetails)
            {
                dgTerm.IsEnabled = false;
                dgTerm.Height = 350;
                gridDetails.Background = isNew ? Brushes.LightGreen : Brushes.LightSkyBlue;
                tbTermName.Focus();

                if (!isNew)
                    tbTermName.Text = _t.TERM;
            }
            else
            {
                dgTerm.Height = 435;
                tbTermName.Clear();
                isNew = false;
                dgTerm.IsEnabled = true;
                LoadTable();
            }
        }

        private void LoadTable()
        {
            _termTable = tClass.GetTerms();
            dgTerm.ItemsSource = _termTable;
        }
        /**************************************************************************/

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            isNew = true;
            ToggleDetails(true);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ToggleDetails(false);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string TermName = tbTermName.Text.Trim();
            if (!string.IsNullOrWhiteSpace(TermName))
            {
                if (isNew)
                {
                    List<Models._Term> lExist = null;
                    
                    //Check if exists
                    if (_termTable != null)
                        lExist = _termTable.Where(x => x.TERM == tbTermName.Text.Trim()).ToList();

                    if (lExist != null && lExist.Count() > 0)
                    {
                        MessageBox.Show("This term already exists.", "Already Exist", MessageBoxButton.OK, MessageBoxImage.Information);
                        tbTermName.SelectAll();
                        tbTermName.Focus();
                    }
                    else
                    {
                        tClass.AddTerm(TermName);
                        isRefresh = true;
                        ToggleDetails(false);
                    }
                }
                else
                {
                    tClass.UpdateTerm(_t.ID, TermName);
                    isRefresh = true;
                    ToggleDetails(false);
                }

            }
        }

        private void DgTerm_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender != null && dgTerm.SelectedIndex != -1)
            {
                _t = (Models._Term)dgTerm.SelectedItem;

                ToggleDetails(true);
            }
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (tClass.IsTermInUse(_t.ID))
                MessageBox.Show("THIS TERM IS IN USE AND CANNOT BE DELETED.", "IN USE", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                tClass.RemoveTerm(_t.ID);
                isRefresh = true;
                ToggleDetails(false);
            }
        }
    }
}
