using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreManagerAdmin
{
    class Globals
    {
        /*FIREBASE*/
        public static string FIREBASE_SECRET = "qpKfDswZG4mb2OlAQxcWOpSmJMHX4Uymd9zpWFhl";
        public static string FIREBASE_PATH = "https://scoremanager-2e233.firebaseio.com/";
        public static string PATH_LICENSE = "License/";

        /*LICENSE*/
        public static int LENGTH = 16;

        /*EMAIL*/
        public static string EmailSenderUsername = "smansender@gmail.com";
        public static string EmailSenderPassword = "mapuaccesc";
        public static string EmailDev = "johphilencarnacion@gmail.com";
        public const int USE_EMAIL_GMAIL = 1;
        public static string SMTP_GMAIL = "smtp.gmail.com";
    }
}
