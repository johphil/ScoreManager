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


        /*****************************************************************/


        private void BtnActivate_Click(object sender, RoutedEventArgs e)
        {
            _User _user = lClass.GetRegistrationFirebase(tbLicense.Text.Trim());
            
            if (_user != null && !string.IsNullOrWhiteSpace(tbLicense.Text))
            {
                if (_user.ISACTIVATED == 0)
                {
                    string pid = lClass.GetProcessorID();
                    lClass.Activate(tbLicense.Text.Trim(), pid, _user);
                    lClass.GetLicenseInfo(lClass.GetLicenseKey(), out string name, out string email);
                    emClass.SendMail(Globals.USE_EMAIL_GMAIL, Globals.EmailSenderUsername, Globals.EmailSenderPassword, _user.EMAIL, "Score Manager License Activated", Globals.MSG_ACTIVATE);
                    MessageBox.Show(String.Format("License Key Accepted! \nInfo:\n\n{0} \n{1} \n", _user.NAME, _user.EMAIL), "License Activated Successfully", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("This license has already been activated with another device.", "Invalid Licence Key", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    tbLicense.SelectAll();
                    tbLicense.Focus();
                }
            }
            else
            {
                MessageBox.Show("License maybe invalid or Internet connection is lost. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                
                tbLicense.SelectAll();
                tbLicense.Focus();
            }
        }

        private void BtnRequest_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SubWindows.RegistrationWindow rWin = new SubWindows.RegistrationWindow();
            rWin.ShowDialog();
        }
    }
}
