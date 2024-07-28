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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.Xml.Serialization;
using ServiceStack.Text;
using Microsoft.VisualBasic;
using PrintScanner.Services;
using PrintScanner.Router;


namespace PrintScanner.Pages
{

    public partial class Settings : Page
    {
        
        public Settings()
        {
            InitializeComponent();
            MainSetting.Content = new SettingsScanner();
        } 

        private void SetScaner_Click(object sender, RoutedEventArgs e)
        {
            MainSetting.Content = new SettingsScanner();
        }

        private void SetPhoto_Click(object sender, RoutedEventArgs e)
        {
            MainSetting.Content = new SettingsPhoto();
        }

        private void Navigator_Click(object sender, RoutedEventArgs e)
        {
            var clickButton = e.OriginalSource as Navigator;
            NavigationService.Navigate(clickButton?.NavUri);
        }
    }
    
}
