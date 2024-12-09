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

namespace CamScan.Pages
{
    /// <summary>
    /// Lógica interna para KeyAcess.xaml
    /// </summary>
    public partial class KeyAcess : Window
    {
        private string AcessKey { get; set;}
        private string? SecondKey { get; set;}
        public bool FreeAcess {  get; private set;} = false;
        public KeyAcess(string AcessKey, string? secondKey = null)
        {
            InitializeComponent();
            this.AcessKey = AcessKey;
            this.SecondKey = secondKey;
        }

        private void KeyInput_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            var password = passwordBox?.Password;
            if (password != null && (password == AcessKey || (SecondKey != null && password == SecondKey)))
            {
                FreeAcess = true;
                this.Close();
            }
        }
    }
}
