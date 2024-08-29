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
    /// Lógica interna para Error.xaml
    /// </summary>
    public partial class Error : Window
    {
        private string TextError;
        public Error(string errorMessage)
        {
            InitializeComponent();
            TextError = errorMessage;
            Loaded += DisplayError;
        }

        public void DisplayError(object sender, RoutedEventArgs e)
        {
            TitleError.Text = TextError;
        }

    }
}
