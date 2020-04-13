using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScoreManager.Class
{
    class Data
    {

        /// <summary>
        /// Creates a backup file of the database
        /// </summary>
        public void BackupData()
        {
            try
            {
                if (!Directory.Exists(Globals.PATH_DATA))
                    Directory.CreateDirectory(Globals.PATH_DATA);

                string BackupFName = Globals.DbBackupPathFile();
                File.Copy(Globals.DbPathFile, BackupFName, true);

                MessageBox.Show(BackupFName.Remove(0, 6) + " created.", "Backup Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public List<string> GetBackupFileNames()
        {
            try
            {
                if (!Directory.Exists(Globals.PATH_DATA))
                    Directory.CreateDirectory(Globals.PATH_DATA);

                string[] files = Directory.GetFiles(Globals.PATH_DATA);
                List<string> filenames = new List<string>();

                foreach (string file in files)
                    filenames.Add(Path.GetFileName(file));

                return filenames;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public void DeleteBackupFile(string FileName)
        {
            try
            {
                string file = String.Format(@"{0}\{1}", Globals.PATH_DATA, FileName);
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool RestoreBackup(string FileName)
        {
            try
            {
                string file = String.Format(@"{0}\{1}", Globals.PATH_DATA, FileName);
                
                File.Copy(file, Globals.DbPathFile, true);

                MessageBox.Show(FileName + " restored.", "Backup Success", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
