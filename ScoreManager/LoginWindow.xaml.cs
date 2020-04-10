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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScoreManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            tbUsername.Focus();
        }

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        Class.Login l = new Class.Login();
        Class.Email emClass = new Class.Email();
        License lClass = new License();

        private void InvalidLicense()
        {
            MessageBox.Show("Software is not activated with a valid license key. Please provide a license key.", "License not found.", MessageBoxButton.OK, MessageBoxImage.Information);
            SubWindows.LicenseWindow lWin = new SubWindows.LicenseWindow();
            lWin.ShowDialog();
        }

        private async void ForgotUserPass()
        {
            if (lClass.IsLicenseFileExists())
            {
                string License = lClass.GetLicenseKey();
                lClass.GetLicenseInfo(License, out string Name, out string Email);
                lClass.GetUsernamePassword(License, out string Username, out string Password);
                string body = String.Format("Your account details: \n" +
                    "NAME: {0} \n" +
                    "USERNAME: {1} \n" +
                    "PASSWORD: {2} \n\n\n" +
                    "Please do not reply. This is an automated e-mail. Thank you for using Score Manager!", Name, Username, Password);
                await emClass.SendMail(1, Globals.EmailSenderUsername, Globals.EmailSenderPassword, Email, "Score Manager Forgot Password", body);

                if (emClass.isSuccessSendMail)
                    MessageBox.Show("Please check your email.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show("Please try again.", "Try Again", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            this.IsEnabled = true;
        }
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (lClass.IsLicenseFileExists())
            {
                string pid = lClass.GetProcessorID();
                if (lClass.IsActivated(lClass.GetLicenseKey(), pid))
                {
                    int isExist = l.SignIn(tbUsername.Text.Trim(), tbPassword.Password);
                    switch (isExist)
                    {
                        case 0:
                            {
                                MessageBox.Show("Incorrect Username or Password.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                tbPassword.Password = null;
                                tbPassword.Focus();
                                break;
                            }
                        case Globals.ERROR:
                            {
                                MessageBox.Show("Program is corrupted please contact developer.", "Corrupted", MessageBoxButton.OK, MessageBoxImage.Error);
                                Application.Current.Shutdown();
                                break;
                            }
                        default:
                            {
                                MainWindow mWin = new MainWindow();
                                mWin.Show();
                                this.Hide();
                                break;
                            }
                    }
                }
                else
                {
                    InvalidLicense();
                }
            }
            else
            {
                InvalidLicense();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnForgotUserPass_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.IsEnabled = false;
            ForgotUserPass();
        }
    }
}
