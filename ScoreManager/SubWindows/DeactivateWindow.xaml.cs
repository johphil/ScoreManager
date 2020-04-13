using System;
using System.Collections.Generic;
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
    /// Interaction logic for DeactivateWindow.xaml
    /// </summary>
    public partial class DeactivateWindow : Window
    {
        License lClass = new License();
        string License = "";
        public bool isDeactivate = false;
        string Email = "";

        public DeactivateWindow()
        {
            InitializeComponent();
            License = lClass.GetLicenseKey();


            lClass.GetLicenseNameEmail(License, out string name, out string email);

            txtLicense.Text = name;
            txtEmail.Text = email;
            Email = email;

            tbLicense.Focus();
        }

        /*******************************************************************************************/
        private async void Deactivate()
        {
            Class.Email emClass = new Class.Email();

            if (MessageBox.Show("Do you really want to deactivate this license?\nContact the developer for assistance if needed.", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
            {
                if (License != tbLicense.Text.Trim())
                {
                    MessageBox.Show("The License you entered is incorrect.", "Incorrect License Key", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    this.IsEnabled = true;
                }
                else if (License == tbLicense.Text.Trim())
                {
                    if (lClass.IsFirebaseConnected())
                    {
                        await emClass.SendMail(Globals.USE_EMAIL_GMAIL, Globals.EmailSenderUsername, Globals.EmailSenderPassword, Email, "Score Manager License Deactivated", Globals.MSG_DEACTIVATE);
                        if (emClass.IsSuccessSendMail)
                        {
                            await lClass.DeactivateFirebase(License);
                            lClass.Deactivate(License);

                            MessageBox.Show(String.Format("Your License: {0}, has been deactivated. \nYou will be redirected to login screen.", License), "Deactivate Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            isDeactivate = true;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Cannot proceed. Please check Internet connection and try again.", "Try Again", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                    }
                }
            }
            else
                this.IsEnabled = false;
        }
        /*******************************************************************************************/

        private void BtnDeactivate_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            Deactivate();
        }

    }
}
