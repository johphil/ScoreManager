namespace ScoreManager
{
    class Globals
    {
        /*ERROR INDICATOR*/
        public const int ERROR = -1;

        /*SQL*/
        public static string DbConString = "Data Source=dbScoreManager.db;Version=3;New=False;Compress=True;";

        /*LICENSE FILE NAME*/
        public static string LicenseFile = "license.txt";

        /**EMAIL**/
        public static string EmailSenderUsername = "smansender@gmail.com";
        public static string EmailSenderPassword = "mapuaccesc";
        public static string EmailSubject = "Score verification ";
        public const int USE_EMAIL_GMAIL = 1;
        public const int USE_EMAIL_MAPUA = 2;
        public static string SMTP_GMAIL = "smtp.gmail.com";
        public static string SMTP_OFFICE365 = "smtp.office365.com";

        /*DIRECTORIES*/
        public static string PATH_EMAIL_ERRORS = "EmailErrors/";
    }
}
