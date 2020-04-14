using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ScoreManager.SubWindows
{
    /// <summary>
    /// Interaction logic for BackupWindow.xaml
    /// </summary>
    public partial class BackupWindow : Window
    {
        public BackupWindow()
        {
            InitializeComponent();
            LoadBackups();
            LoadAutoBackupSetting();
        }

        /**************************************************************************/
        public bool isRestore = false;
        Class.Data dClass = new Class.Data();

        private void LoadBackups()
        {
            lbBackups.ItemsSource = null;
            List<string> backupFiles = dClass.GetBackupFileNames();

            if (backupFiles != null)
                lbBackups.ItemsSource = backupFiles;
        }

        private void LoadAutoBackupSetting()
        {
            if (dClass.LoadAutoBackup() == Globals.AUTO_BACKUP_CHECK)
                rbAutoBackup.IsChecked = true;
            else
                rbAutoBackup.IsChecked = false;
        }
        /**************************************************************************/
        private void BtnCreateBackup_Click(object sender, RoutedEventArgs e)
        {
            dClass.BackupData(false);
            LoadBackups();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lbBackups.SelectedIndex != -1)
            {
                dClass.DeleteBackupFile(lbBackups.SelectedItem.ToString());
                LoadBackups();
            }
            else
                MessageBox.Show("Please select a backup to be deleted.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private void BtnRestore_Click(object sender, RoutedEventArgs e)
        {
            if (lbBackups.SelectedIndex != -1)
            {
                if (MessageBox.Show("Do you want to restore data?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    if (dClass.RestoreBackup(lbBackups.SelectedItem.ToString()))
                    {
                        isRestore = true;
                        MessageBox.Show("Success! Going back to login.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                }
            }
            else
                MessageBox.Show("Please select a backup to be deleted.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private void RbAutoBackup_Click(object sender, RoutedEventArgs e)
        {
            if (rbAutoBackup.IsChecked == true)
                dClass.SaveAutoBackup(Globals.AUTO_BACKUP_CHECK);
            else
                dClass.SaveAutoBackup(Globals.AUTO_BACKUP_UNCHECK);
        }
    }
}
