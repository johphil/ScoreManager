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
    /// Interaction logic for FeedbackWindow.xaml
    /// </summary>
    public partial class FeedbackWindow : Window
    {
        public FeedbackWindow()
        {
            InitializeComponent();
        }

        /*******************************************************************/
        Class.Email emClass = new Class.Email();
        License lClass = new License();

        public async void Send(string Message)
        {
            string License = lClass.GetLicenseKey();
            lClass.GetLicenseInfo(License, out string Name, out string Email);
            string body = String.Format("Sender: \n" +
                "LICENSE = {0} \n" +
                "NAME = {1} \n" +
                "EMAIL = {2} \n\n" +
                "Message: \n\n" +
                "{3}", License, Name, Email, Message);
            await emClass.SendMail(Globals.USE_EMAIL_GMAIL, Globals.EmailSenderUsername, Globals.EmailSenderPassword, Globals.EmailDev, "Score Manager Concerns & Feedbacks", body);

            if (emClass.IsSuccessSendMail)
            {
                MessageBox.Show("Thank you for sending us your concerns and/or feedbacks!", "Message Sent", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
                MessageBox.Show("There is an error delivering your message. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            this.IsEnabled = true;
        }
        /*******************************************************************/

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            string msg = new TextRange(rtbMessage.Document.ContentStart, rtbMessage.Document.ContentEnd).Text;
            this.IsEnabled = false;
            Send(msg);
        }
    }
}
