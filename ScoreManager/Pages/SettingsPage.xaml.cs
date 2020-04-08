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
            emClass.LoadEmailSettings(out int UseMail, out string EmailAddr1, out string EmailPass1, out string EmailAddr2, out string EmailPass2, out string EmailFoot);

            switch (UseMail)
            {
                case Globals.USE_EMAIL_GMAIL:
                    {
                        rbGmail.IsChecked = true;
                        break;
                    }
                case Globals.USE_EMAIL_MAPUA:
                    {
                        rbMapuaMail.IsChecked = true;
                        break;
                    }
                default:
                    {
                        rbGmail.IsChecked = false;
                        rbMapuaMail.IsChecked = false;
                        break;
                    }
            }

            tbEmailAddress1.Text = EmailAddr1;
            tbPassword1.Password = EmailPass1;
            tbEmailAddress2.Text = EmailAddr2;
            tbPassword2.Password = EmailPass2;

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
            int useMail;

            if (rbGmail.IsChecked == true)
                useMail = Globals.USE_EMAIL_GMAIL;
            else if (rbMapuaMail.IsChecked == true)
                useMail = Globals.USE_EMAIL_MAPUA;
            else
                useMail = Globals.ERROR;

            emClass.SaveEmailSettings(useMail, tbEmailAddress1.Text, tbPassword1.Password, tbEmailAddress2.Text, tbPassword2.Password, footer);
            MessageBox.Show("Saved Successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
