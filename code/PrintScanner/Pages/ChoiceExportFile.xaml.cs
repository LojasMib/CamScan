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
    /// Lógica interna para ChoiceExportFile.xaml
    /// </summary>

    

    public partial class ChoiceExportFile : Window
    {
        public string? TypeFile { get; private set; } = null;

        public ChoiceExportFile()
        {
            InitializeComponent();
        }

        private void ButtonPDF_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TypeFile = "PDF";
            this.Close();
        }

        private void ButtonJPG_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TypeFile = "JPG";
            this.Close();
        }
    }
}
