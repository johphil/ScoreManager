﻿using System;

namespace ScoreManager
{
    class Globals
    {
        /*ERROR INDICATOR*/
        public const int ERROR = -1;

        /*SQL*/
        public static string DbConString = "Data Source=dbScoreManager.db;Version=3;New=False;Compress=True;";
        public static string DbPathFile = "dbScoreManager.db";
        /*SQL BACKUP*/
        public static string DbBackupPathFile()
        {
            string datetimeformat = DateTime.Now.ToShortDateString() + DateTime.Now.ToLongTimeString();
            datetimeformat = datetimeformat.Replace("/", "");
            datetimeformat = datetimeformat.Replace(":", "");
            datetimeformat = datetimeformat.Replace(" ", "");
            return string.Format(@"{0}\dbScoreManager_{1}.db",PATH_DATA, datetimeformat);
        }
        public static string DbBackupPathFile2()
        {
            string datetimeformat = DateTime.Now.ToShortDateString() + DateTime.Now.ToLongTimeString();
            datetimeformat = datetimeformat.Replace("/", "");
            datetimeformat = datetimeformat.Replace(":", "");
            datetimeformat = datetimeformat.Replace(" ", "");
            return string.Format(@"{0}\dbScoreManager_AutoBackup_{1}.db", PATH_DATA, datetimeformat);
        }

        /*LICENSE FILE NAME*/
        public static string LicenseFile = "license.txt";

        /**EMAIL**/
        public static string EmailSenderUsername = "smansender@gmail.com";
        public static string EmailSenderPassword = "mapuaccesc";
        public static string EmailDev = "johphilencarnacion@gmail.com";
        public static string EmailSubject = "Score verification ";
        public const int USE_EMAIL_GMAIL = 1;
        public const int USE_EMAIL_MAPUA = 2;
        public static string SMTP_GMAIL = "smtp.gmail.com";
        public static string SMTP_OFFICE365 = "smtp.office365.com";

        /*DIRECTORIES*/
        public static string PATH_EMAIL_ERRORS = "EmailErrors\\";
        public static string PATH_USER_MANUAL = "userman.pdf";
        public static string PATH_DATA = "Data\\";

        /*FIREBASE*/
        public static string FIREBASE_SECRET = "qpKfDswZG4mb2OlAQxcWOpSmJMHX4Uymd9zpWFhl";
        public static string FIREBASE_PATH = "https://scoremanager-2e233.firebaseio.com/";
        public static string FIREBASE_PATH_LICENSE = "License/";

        /*MESSAGE*/
        public static string MSG_ACTIVATE = "You have successfully activated your license! Enjoy! =)";
        public static string MSG_DEACTIVATE = "You have successfully deactivated your license! Thank you for using Score Manager!";

        /*AUTO BACKUP*/
        public static int AUTO_BACKUP_CHECK = 1;
        public static int AUTO_BACKUP_UNCHECK = 0;
    }
}
