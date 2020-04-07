using System;
using System.Collections.Generic;
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
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        MainWindow mWin;
        public SettingsPage(MainWindow mWin)
        {
            InitializeComponent();
            this.mWin = mWin;

            License lClass = new License();
            lClass.GetLicenseInfo(lClass.GetLicenseKey(), out string name, out string email);

            txtAppName.Text = Application.ResourceAssembly.GetName().Name;
            txtAppVersion.Text = Application.ResourceAssembly.GetName().Version.ToString();
            txtLicense.Text = name;
            txtEmail.Text = email;
            emClass.LoadEmailSettings(out string EmailAddr, out string EmailPass, out string EmailFoot);
            tbEmailAddress.Text = EmailAddr;
            tbPassword.Password = EmailPass;
            tbFooter.AppendText(EmailFoot);
        }

        /*************************************************************/
        Class.Email emClass = new Class.Email();

        /*************************************************************/
        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            mWin.frame.Content = new Pages.HomePage(mWin);
        }

        private void BtnConfigExam_Click(object sender, RoutedEventArgs e)
        {
            SubWindows.ExamManageWindow emWin = new SubWindows.ExamManageWindow();
            emWin.ShowDialog();
        }

        private void BtnTermConfig_Click(object sender, RoutedEventArgs e)
        {
            SubWindows.TermWindow tWin = new SubWindows.TermWindow();
            tWin.ShowDialog();
        }

        private void BtnDeactivate_Click(object sender, RoutedEventArgs e)
        {
            SubWindows.DeactivateWindow dWin = new SubWindows.DeactivateWindow();
            dWin.ShowDialog();
            if (dWin.isDeactivate)
            {
                mWin.isLogout = true;
                mWin.Logout();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string footer = new TextRange(tbFooter.Document.ContentStart, tbFooter.Document.ContentEnd).Text;
            emClass.SaveEmailSettings(tbEmailAddress.Text, tbPassword.Password, footer);
            MessageBox.Show("Saved Successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
