using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CamScan.Services;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static CamScan.Services.ConfigFranquia;

namespace CamScan.Pages
{
    public partial class SettingFranquia : Page
    {
        XMLConnect xmlConnect = new XMLConnect();
        public SettingFranquia()
        {
            InitializeComponent();
            Loaded += LocalFolder;
            Unloaded += OnUnloaded;
        }

        private void LocalFolder(object sender, RoutedEventArgs e)
        {
            var config = xmlConnect.LoadConfigurations();
            if (config != null && config.Franquias.Any())
            {
                var xml = config.Franquias.Last();
                Cidade.Text = xml.Cidade ?? "";
                Franquia.Text = xml.Franquia ?? "";
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            SaveConfigurations();
        }

        private void SaveConfigurations()
        {
            if(!string.IsNullOrEmpty(Cidade.Text))
            {
                xmlConnect.SaveConfigFranquia("ConfigCidade", Cidade.Text);
            }
            if(!string.IsNullOrEmpty(Franquia.Text))
            {
                xmlConnect.SaveConfigFranquia("ConfigFranquia", Franquia.Text);
            }
        }
    }
}
