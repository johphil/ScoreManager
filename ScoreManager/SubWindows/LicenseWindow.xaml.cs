using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for LicenseWindow.xaml
    /// </summary>
    public partial class LicenseWindow : Window
    {
        public LicenseWindow()
        {
            InitializeComponent();
            tbLicense.Focus();
        }

        License lClass = new License();
        /**********************************************************************/
        private void RequestLicenseKey()
        {
            if (MessageBox.Show("License not found!\n\nDo you want to request one?", "Invalid License", MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.Yes) == MessageBoxResult.Yes)
            {
                MessageBox.Show("Go to fb.com/johphil for to request license key. Thank you.");
            }
        }
        /**********************************************************************/

        private void BtnActivate_Click(object sender, RoutedEventArgs e)
        {
            if (lClass.ForActivation(tbLicense.Text.Trim()))
            {
                string pid = lClass.GetProcessorID();
                lClass.Activate(tbLicense.Text.Trim(), pid);
                lClass.GetLicenseInfo(lClass.GetLicenseKey(), out string name, out string email);
                MessageBox.Show(String.Format("License Key Accepted! \nInfo:\n\n{0} \n{1} \n", name, email), "License Activated Successfully", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                if (lClass.IsActivatedNotSameDevice(tbLicense.Text.Trim()))
                    MessageBox.Show("This license has already been activated with another device.", "Invalid Licent Key", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                else
                    MessageBox.Show("You have entered an invalid license.", "Invalid License Key", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                tbLicense.SelectAll();
                tbLicense.Focus();
            }
        }

        private void BtnRequest_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RequestLicenseKey();
        }
    }
}
