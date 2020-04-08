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
        public void WriteToFileErrors(string FileName, string Content)
        {
            if (!Directory.Exists(Globals.PATH_EMAIL_ERRORS))
            {
                Directory.CreateDirectory(Globals.PATH_EMAIL_ERRORS);
            }

            using (StreamWriter file = new StreamWriter(Globals.PATH_EMAIL_ERRORS + FileName))
            {
                //hash muna
                file.WriteLine(Content);
            }
        }
    }
}
