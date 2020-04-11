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

namespace ScoreManagerAdmin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        Admin aClass = new Admin();

        private void BtnSubmitRegister_Click(object sender, RoutedEventArgs e)
        {
            string License = aClass.GenerateLicenseKey();

            _User _user = new _User
            {
                NAME = tbName.Text.Trim(),
                EMAIL = tbEmail.Text.Trim(),
                USERNAME = tbUsername.Text.Trim(),
                PASSWORD = tbPassword.Text.Trim(),
                ISACTIVATED = 0
            };

            string body = String.Format("Hi! We have noticed your request. Here's your license key and the details of your registration: \n\n" +
                "LICENSE KEY: {0}\n" +
                "LICENSED TO: {1}\n" +
                "EMAIL: {2}\n" +
                "USERNAME: {3}\n" +
                "PASSWORD {4}\n\n" +
                "Please do not reply. This is an automated e-mail. If you have any suggestions, questions, or concerns you can send us your feedbacks which is available in the Score Manager App under Preferences. We'd love to hear from you ♥. \n\nThank you. \n\nMicrosoft Bagnet Team",
                License, _user.NAME, _user.EMAIL, _user.USERNAME, _user.PASSWORD);

            aClass.RegisterUser(License, _user);
            aClass.SendMail(tbEmail.Text.Trim(), "Score Manager Registration", body);
        }
    }
}
