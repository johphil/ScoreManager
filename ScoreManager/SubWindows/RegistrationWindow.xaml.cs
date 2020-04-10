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
    /// Interaction logic for RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
        }

        /************************************************************************/
        Class.Email emClass = new Class.Email();

        /************************************************************************/
        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbUsername.Text) ||
                string.IsNullOrWhiteSpace(tbPassword.Text) ||
                string.IsNullOrWhiteSpace(tbName.Text) ||
                string.IsNullOrWhiteSpace(tbEmail.Text))
            {
                MessageBox.Show("Fill up all the fields and try again.", "Try Again", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                string body = String.Format("User's Info: \nNAME: {0}\nEMAIL: {1}\n\nLogin Info: \nUSERNAME: {2} \nPASSWORD: {3}", tbName.Text.Trim(), tbEmail.Text.Trim(), tbUsername.Text.Trim(), tbPassword.Password);

                if (emClass.SendMail(Globals.USE_EMAIL_GMAIL, Globals.EmailSenderUsername, Globals.EmailSenderPassword, Globals.EmailDev, "Score Manager Registration", body))
                {
                    MessageBox.Show("Registration Success! \nPlease wait for the developers to notice your request. Thank you.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Internet connection may not be available. If this problem persists, contact developer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
