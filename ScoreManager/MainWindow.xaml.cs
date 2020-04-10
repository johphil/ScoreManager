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

namespace ScoreManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            frame.Content = new Pages.HomePage(this);
        }

        public bool isLogout = false;
        public bool isWorking = false;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isWorking)
            {
                MessageBox.Show("Emails are being sent in the background process. Please wait for it to finish.", "Please Wait", MessageBoxButton.OK, MessageBoxImage.Information);
                e.Cancel = true;
                isLogout = false;
            }
            else
            {
                if (!isLogout && !isWorking)
                    Application.Current.Shutdown();
                else if (isLogout && !isWorking)
                    Logout();
            }
        }

        private void MenuEditExams_Click(object sender, RoutedEventArgs e)
        {
            SubWindows.ExamManageWindow emWin = new SubWindows.ExamManageWindow();
            emWin.ShowDialog();
        }

        private void MenuEditTerms_Click(object sender, RoutedEventArgs e)
        {
            SubWindows.TermWindow tWin = new SubWindows.TermWindow();
            tWin.ShowDialog();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            frame.Content = new Pages.HomePage(this);
        }

        private void MenuLogout_Click(object sender, RoutedEventArgs e)
        {
            isLogout = true;
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuDashboard_Click(object sender, RoutedEventArgs e)
        {
            frame.Content = new Pages.HomePage(this);
        }

        private void MenuScoreEdit_Click(object sender, RoutedEventArgs e)
        {
            frame.Content = new Pages.ScoresPage(this);
        }

        private void MenuVerification_Click(object sender, RoutedEventArgs e)
        {
            frame.Content = new Pages.VerifyPage(this);
        }

        private void MenuSettings_Click(object sender, RoutedEventArgs e)
        {
            frame.Content = new Pages.SettingsPage(this);
        }

        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            frame.Content = new Pages.SettingsPage(this);
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            frame.Content = new Pages.HomePage(this);
        }

        private void BtnScores_Click(object sender, RoutedEventArgs e)
        {
            frame.Content = new Pages.ScoresPage(this);
        }

        private void BtnVerification_Click(object sender, RoutedEventArgs e)
        {
            frame.Content = new Pages.VerifyPage(this);
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            frame.Content = new Pages.SettingsPage(this);
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            isLogout = true;
            this.Close();
        }

        private void MenuDeactivate_Click(object sender, RoutedEventArgs e)
        {
            SubWindows.DeactivateWindow dWin = new SubWindows.DeactivateWindow();
            dWin.ShowDialog();
            if (dWin.isDeactivate)
            {
                isLogout = true;
                Logout();
            }
        }

        public void Logout()
        {
            LoginWindow lWin = new LoginWindow();
            lWin.Show();
        }
    }
}
