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
        public bool IsSuccessSendMail { get; set; }

        /// <summary>
        /// Loads the Email information of the user and updates it to the UI.
        /// </summary>
        /// <param name="UseMail">Indicates if send using GMAIL or MAPUA MAIL</param>
        /// <param name="EmailAddress1">Gmail address</param>
        /// <param name="EmailPassword1">Gmail password</param>
        /// <param name="EmailAddress2">Office365 address</param>
        /// <param name="EmailPassword2">Office365 password</param>
        /// <param name="EmailFooter">Footer text to be added is email</param>
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
                                UseMail = Convert.IsDBNull(reader[1]) ? Globals.ERROR : reader.GetInt32(1);
                                EmailAddress1 = reader[2]?.ToString();
                                EmailPassword1 = reader[3]?.ToString();
                                EmailAddress2 = reader[4]?.ToString();
                                EmailPassword2 = reader[5]?.ToString();
                                EmailFooter = reader[6]?.ToString(); ;
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

        /// <summary>
        /// Saves the Email information of the user to database.
        /// </summary>
        /// <param name="UseMail">Indicates if send using GMAIL or MAPUA MAIL</param>
        /// <param name="EmailAddress1">Gmail address</param>
        /// <param name="EmailPassword1">Gmail password</param>
        /// <param name="EmailAddress2">Office365 address</param>
        /// <param name="EmailPassword2">Office365 password</param>
        /// <param name="EmailFooter">Footer text to be added is email</param>
        public void SaveEmailSettings(int UseMail, string EmailAddress1, string EmailPassword1, string EmailAddress2, string EmailPassword2, string EmailFooter)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "UPDATE tblSettings SET " +
                        "USE_EMAIL = @usemail, " +
                        "GMAIL_USER = @mail1, " +
                        "GMAIL_PASS = @pass1, " +
                        "MAPUA_USER = @mail2, " +
                        "MAPUA_PASS = @pass2, " +
                        "EMAIL_FOOTER = @footer " +
                        "WHERE ID = 1;";
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

        /// <summary>
        /// Sends mail to a single recipient.
        /// </summary>
        /// <param name="UseMail">Indicates if send using GMAIL or MAPUA MAIL</param>
        /// <param name="EmailFromAddr">Email address to be used by user</param>
        /// <param name="EmailFromPass">Password of the email address used</param>
        /// <param name="EmailToAddr">Recipient's email address</param>
        /// <param name="Subject">Subject of the email</param>
        /// <param name="Body">Body of the email</param>
        /// <returns></returns>
        public async Task SendMail(int UseMail, string EmailFromAddr, string EmailFromPass, string EmailToAddr, string Subject, string Body)
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

                using (client)
                {
                    await client.SendMailAsync(EmailFromAddr, EmailToAddr, Subject, Body);
                }

                IsSuccessSendMail = true;
            }
            catch
            {
                IsSuccessSendMail = false;
            }
        }

        /// <summary>
        /// Send email to many, single execution. Can be used for announcements. Not for grade verification. Not yet used, for future maybe.
        /// </summary>
        /// <param name="UseMail">Indicates if send using GMAIL or MAPUA MAIL</param>
        /// <param name="EmailFromAddr">Email address to be used by user</param>
        /// <param name="EmailFromPass">Password of the email address used</param>
        /// <param name="EmailToAddr">Recipient's email address</param>
        /// <param name="Subject">Subject of the email</param>
        /// <param name="Body">Body of the email</param>
        /// <returns></returns>
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
