using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreManager.Class
{
    class Logging
    {
        /// <summary>
        /// Writes the errors when sending an email for the whole class/exam. (e.g. [Student_ID] Email is invalid).
        /// </summary>
        /// <param name="FileName">Name of the error file</param>
        /// <param name="Content">Error logs</param>
        public void WriteToFileErrors(string FileName, string Content)
        {
            if (!Directory.Exists(Globals.PATH_EMAIL_ERRORS))
            {
                Directory.CreateDirectory(Globals.PATH_EMAIL_ERRORS);
            }

            using (StreamWriter file = new StreamWriter(Globals.PATH_EMAIL_ERRORS + FileName))
            {
                file.WriteLine(Content);
            }
        }
    }
}
