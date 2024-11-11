using CamScan.Pages;
using CamScan.Router;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CamScan.Pages
{
    /// <summary>
    /// Interação lógica para Landing.xaml
    /// </summary>
    public partial class Landing : Page
    {
        private const string AcessKey = "SENHA";
        public Landing()
        {
            InitializeComponent();
        }

        private void Grid_Click(object sender, RoutedEventArgs e)
        {
            var ClickButton = e.OriginalSource as Navigator;
            NavigationService.Navigate(ClickButton?.NavUri);
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            KeyAcess keyAcess = new KeyAcess(AcessKey);
            
            Window parentWindow = Window.GetWindow(this);
            
            if (parentWindow != null)
            {
                keyAcess.Owner = parentWindow;
            }
            keyAcess.ShowDialog();
            if (keyAcess.FreeAcess == true)
            {
                NavigationService.Navigate(new Uri("/Pages/Settings.xaml", UriKind.Relative));
            }
        }
    }
}
