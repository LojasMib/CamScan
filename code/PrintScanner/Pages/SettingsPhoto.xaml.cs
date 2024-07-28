using PrintScanner.Services;
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

namespace PrintScanner.Pages
{
    /// <summary>
    /// Interação lógica para SettingsPhoto.xam
    /// </summary>
    public partial class SettingsPhoto : Page
    {
        XMLConnect xmlConnect = new XMLConnect();
        public SettingsPhoto()
        {
            InitializeComponent();
            LocalFolder();
        }

        private void LocalFolder()
        {
            var config = xmlConnect.LoadConfigPhoto();
            if (config != null && config.Any())
            {
                var xml = config.Last();
                DriverPhoto.Text = xml.ConfigDriverPhoto ?? "";
                ImagemDeClientes.Text = xml.FolderImagemClientes ?? "";
                ImagemDeItens.Text = xml.FolderImagemItens ?? "";
            }

        }

        private void DriverPhoto_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string folder = xmlConnect.SelectFolder();
            xmlConnect.SaveConfigPhoto("ConfigDriverPhoto", folder);
            DriverPhoto.Text = folder;
        }

        private void ImagemDeClientes_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string folder = xmlConnect.SelectFolder();
            xmlConnect.SaveConfigPhoto("FolderImagemClientes", folder);
            DriverPhoto.Text = folder;
        }

        private void ImagemDeItens_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string folder = xmlConnect.SelectFolder();
            xmlConnect.SaveConfigPhoto("FolderImagemItens", folder);
            DriverPhoto.Text = folder;
        }
    }
}
