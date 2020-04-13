using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

namespace ScoreManager.SubWindows
{
    /// <summary>
    /// Interaction logic for LicenseWindow.xaml
    /// </summary>
    public partial class LicenseWindow : Window
    {
        public LicenseWindow()
        {
            InitializeComponent();
            tbLicense.Focus();
        }

        /*****************************************************************/
        License lClass = new License();
        Class.Email emClass = new Class.Email();

        private async void ActivateLicense()
        {
            await lClass.GetRegistrationFirebase(tbLicense.Text.Trim());

            if (lClass._user != null && !string.IsNullOrWhiteSpace(tbLicense.Text))
            {
                if (lClass._user.ISACTIVATED == 0)
                {
                    await emClass.SendMail(Globals.USE_EMAIL_GMAIL, Globals.EmailSenderUsername, Globals.EmailSenderPassword, lClass._user.EMAIL, "Score Manager License Activated", Globals.MSG_ACTIVATE);
                    if (emClass.IsSuccessSendMail)
                    {
                        string pid = lClass.GetProcessorID();
                        await lClass.Activate(tbLicense.Text.Trim(), pid, lClass._user);
                        lClass.GetLicenseNameEmail(lClass.GetLicenseKey(), out string name, out string email);

                        MessageBox.Show(String.Format("License Key Accepted! \nInfo:\n\n{0} \n{1} \n", lClass._user.NAME, lClass._user.EMAIL), "License Activated Successfully", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Cannot proceed. Please check Internet connection and try again.", "Try Again", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
                else
                {
                    MessageBox.Show("This license has already been activated with another device.", "Invalid Licence Key", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    this.IsEnabled = true;
                    tbLicense.SelectAll();
                    tbLicense.Focus();
                }
            }
            else
            {
                MessageBox.Show("License maybe invalid or Internet connection is lost. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                this.IsEnabled = true;
                tbLicense.SelectAll();
                tbLicense.Focus();
            }
        }
        /*****************************************************************/
        

        private void BtnRequest_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SubWindows.RegistrationWindow rWin = new SubWindows.RegistrationWindow();
            rWin.ShowDialog();
        }

        private void BtnActivate_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            ActivateLicense();
        }
    }
}
