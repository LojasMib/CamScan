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
        public bool FreeAcess {  get; private set;} = false;
        public KeyAcess(string AcessKey)
        {
            InitializeComponent();
            this.AcessKey = AcessKey;

        }

        private void KeyInput_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox != null && passwordBox.Password == AcessKey)
            {
                FreeAcess = true;
                this.Close();
            }
        }
    }
}
