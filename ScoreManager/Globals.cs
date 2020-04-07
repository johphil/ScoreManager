using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreManager
{
    class Globals
    {
        /*SQL*/
        public static string DbConString = "Data Source=dbScoreManager.db;Version=3;New=False;Compress=True;";
        public const int SQL_ERROR = -1;

        /*LICENSE FILE NAME*/
        public static string LicenseFile = "license.txt";

        /**EMAIL**/
        public static string EmailSenderUsername = "smansender@gmail.com";
        public static string EmailSenderPassword = "mapuaccesc";
        public static string EmailSubject = "Score verification ";

    }
}
