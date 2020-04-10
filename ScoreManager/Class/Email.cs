using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScoreManager.Class
{
    class Email
    {
        public void LoadEmailSettings(out int UseMail, out string EmailAddress1, out string EmailPassword1, out string EmailAddress2, out string EmailPassword2, out string EmailFooter)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "SELECT * FROM tblSettings;";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        con.Open();

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                UseMail = Convert.ToInt32(reader[1]);
                                EmailAddress1 = reader[2].ToString();
                                EmailPassword1 = reader[3].ToString();
                                EmailAddress2 = reader[4].ToString();
                                EmailPassword2 = reader[5].ToString();
                                EmailFooter = reader[6].ToString();
                            }
                            else
                            {
                                UseMail = Globals.ERROR;
                                EmailAddress1 = null;
                                EmailPassword1 = null;
                                EmailAddress2 = null;
                                EmailPassword2 = null;
                                EmailFooter = "";
                            }
                        }
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                UseMail = Globals.ERROR;
                EmailAddress1 = null;
                EmailPassword1 = null;
                EmailAddress2 = null;
                EmailPassword2 = null;
                EmailFooter = "";
            }
        }

        public void SaveEmailSettings(int UseMail, string EmailAddress1, string EmailPassword1, string EmailAddress2, string EmailPassword2, string EmailFooter)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "INSERT OR REPLACE INTO tblSettings " +
                        "VALUES(1, @usemail, @mail1, @pass1, @mail2, @pass2, @footer);";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        con.Open();

                        cmd.Parameters.Add("@usemail", DbType.Int32).Value = UseMail;
                        cmd.Parameters.Add("@mail1", DbType.String).Value = EmailAddress1.Trim();
                        cmd.Parameters.Add("@pass1", DbType.String).Value = EmailPassword1;
                        cmd.Parameters.Add("@mail2", DbType.String).Value = EmailAddress2.Trim();
                        cmd.Parameters.Add("@pass2", DbType.String).Value = EmailPassword2;
                        cmd.Parameters.Add("@footer", DbType.String).Value = EmailFooter;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool SendMail(int UseMail, string EmailFromAddr, string EmailFromPass, string EmailToAddr, string Subject, string Body)
        {
            string smtp;
            switch(UseMail)
            {
                case Globals.USE_EMAIL_GMAIL:
                    {
                        smtp = Globals.SMTP_GMAIL;
                        break;
                    }
                case Globals.USE_EMAIL_MAPUA:
                    {
                        smtp = Globals.SMTP_OFFICE365;
                        break;
                    }
                default:
                    {
                        smtp = "";
                        break;
                    }
            }

            try
            {
                var client = new System.Net.Mail.SmtpClient(smtp, 587)
                {
                    UseDefaultCredentials = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(EmailFromAddr, EmailFromPass),
                    EnableSsl = true
                };

                client.Send(EmailFromAddr, EmailToAddr, Subject, Body);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SendMailMultiple(int UseMail, string EmailFromAddr, string EmailFromPass, List<string> EmailToAddr, string Subject, string Body)
        {
            string smtp;
            switch (UseMail)
            {
                case Globals.USE_EMAIL_GMAIL:
                    {
                        smtp = Globals.SMTP_GMAIL;
                        break;
                    }
                case Globals.USE_EMAIL_MAPUA:
                    {
                        smtp = Globals.SMTP_OFFICE365;
                        break;
                    }
                default:
                    {
                        smtp = "";
                        break;
                    }
            }

            try
            {
                var client = new System.Net.Mail.SmtpClient(smtp, 587)
                {
                    UseDefaultCredentials = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(EmailFromAddr, EmailFromPass),
                    EnableSsl = true
                };

                MailMessage msg = new MailMessage
                {
                    From = new MailAddress(EmailFromAddr)
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
