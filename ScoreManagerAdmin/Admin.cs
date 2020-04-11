using FireSharp.Config;
using FireSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScoreManagerAdmin
{
    class Admin
    {
        private IFirebaseClient fbClient;

        private readonly IFirebaseConfig fbConfig = new FirebaseConfig
        {
            AuthSecret = Globals.FIREBASE_SECRET,
            BasePath = Globals.FIREBASE_PATH
        };

        public bool IsFirebaseConnected()
        {
            fbClient = new FireSharp.FirebaseClient(fbConfig);

            if (fbClient != null)
                return true;
            else
                return false;
        }

        public async void RegisterUser(string License, _User _user)
        {
            await RegisterUserTask(License, _user);
        }

        private async Task RegisterUserTask(string License, _User _user)
        {
            try
            {
                if (IsFirebaseConnected())
                {
                    await fbClient.SetTaskAsync(Globals.PATH_LICENSE + License, _user);
                    MessageBox.Show("Register Success", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                    MessageBox.Show("Cannot connect to firebase.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n\nError Registration", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public string GenerateLicenseKey()
        {
            StringBuilder sb = new StringBuilder();
            int numGuidsToConcat = (((Globals.LENGTH - 1) / 32) + 1);
            for (int i = 1; i <= numGuidsToConcat; i++)
            {
                sb.Append(Guid.NewGuid().ToString("N").ToUpper());
            }

            return sb.ToString(0, Globals.LENGTH);
        }

        public async void SendMail(string EmailToAddr, string Subject, string Body)
        {
            await SendMailTask(EmailToAddr, Subject, Body);
        }

        public async Task SendMailTask(string EmailToAddr, string Subject, string Body)
        {
            try
            {
                var client = new System.Net.Mail.SmtpClient(Globals.SMTP_GMAIL, 587)
                {
                    UseDefaultCredentials = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(Globals.EmailSenderUsername, Globals.EmailSenderPassword),
                    EnableSsl = true
                };

                using (client)
                {
                    await client.SendMailAsync(Globals.EmailSenderUsername, EmailToAddr, Subject, Body);
                }

                MessageBox.Show("Email Sent.");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + " Email not sent.");
            }
        }

        public bool SendMailMultiple(List<string> EmailToAddr, string Subject, string Body)
        {
            try
            {
                var client = new System.Net.Mail.SmtpClient(Globals.SMTP_GMAIL, 587)
                {
                    UseDefaultCredentials = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(Globals.EmailSenderUsername, Globals.EmailSenderPassword),
                    EnableSsl = true
                };

                MailMessage msg = new MailMessage
                {
                    From = new MailAddress(Globals.EmailSenderUsername)
                };

                foreach (string addr in EmailToAddr)
                {
                    msg.To.Add(new MailAddress(addr));
                }

                msg.Subject = Subject;
                msg.Body = Body;

                client.Send(msg);

                msg.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
