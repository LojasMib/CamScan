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
using CamScan.Router;

namespace CamScan.Pages
{
    public partial class SettingFranquia : Page
    {
        XMLConnect xmlConnect = new XMLConnect();
        private readonly Settings _settings;
        PathSavedXML _pathSavedXML;

        public class PathSavedXML()
        {
            public string Cidade { get; set; } = string.Empty;
            public string Franquia { get; set; } = string.Empty;
        }

        public SettingFranquia()
        {
            InitializeComponent();
            _pathSavedXML = new PathSavedXML();
            Loaded += Loades_SettingFranquia;
        }

        public SettingFranquia(Settings settings): this()
        {
            _settings = settings;
        }
        public SettingFranquia(Settings settings, string sucessMessage): this(settings)
        {
            InitializeWithSuccessMessage(sucessMessage);
        }

        public async Task Sucess_Message(string message, Settings setting)
        {
            setting.MessageText.Text = message;
            setting.MessageSucess.Visibility = Visibility.Visible;
            await Task.Delay(2000);// 1000 => 1 segundo
            setting.MessageSucess.Visibility = Visibility.Hidden;
        }

        private async void InitializeWithSuccessMessage(string successMessage)
        {
            await Sucess_Message(successMessage, _settings);
        }

        private void CompareTextFolders()
        {
            ConfirmFileSave(_pathSavedXML.Cidade, Cidade.Text, Cidade);
            ConfirmFileSave(_pathSavedXML.Franquia, Franquia.Text, Franquia);
        }

        private void Loades_SettingFranquia(object sender, RoutedEventArgs e)
        {
            var config = xmlConnect.LoadConfigurations();
            if (config != null && config.Franquias.Any())
            {
                var xml = config.Franquias.Last();

                _pathSavedXML.Cidade = xml.Cidade ?? "";
                _pathSavedXML.Franquia = xml.Franquia ?? "";

                Cidade.Text = _pathSavedXML.Cidade ?? "";
                Franquia.Text = _pathSavedXML.Franquia ?? "";
                CompareTextFolders();
            }
        }


        private void UpdateBorderBrushGreen(FolderSearch typePath)
        {
            typePath.BorderBrush = new SolidColorBrush(Colors.Green);
        }

        private void UpdateBorderBrushRed(FolderSearch typePath)
        {
            typePath.BorderBrush = new SolidColorBrush(Colors.Red);
        }


        private void ConfirmFileSave(string textInput, string pathSaved, FolderSearch typePath)
        {
            if (textInput != pathSaved || string.IsNullOrEmpty(textInput))
            {
                UpdateBorderBrushRed(typePath);
            }
            if (textInput == pathSaved && !string.IsNullOrEmpty(textInput))
            {
                UpdateBorderBrushGreen(typePath);
            }
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

        private void Save_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SaveConfigurations();
            string messageSucess = "Configurações Salvas com Sucesso!!";
            NavigationService?.Navigate(new SettingFranquia(_settings ,messageSucess));
        }
    }
}
