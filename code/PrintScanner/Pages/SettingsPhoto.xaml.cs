using AForge.Video.DirectShow;
using CamScan.Router;
using CamScan.Services;
using IWshRuntimeLibrary;
using ServiceStack;
using Shell32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace CamScan.Pages
{
    /// <summary>
    /// Interação lógica para SettingsPhoto.xam
    /// </summary>
    public partial class SettingsPhoto : Page
    {
        XMLConnect xmlConnect = new XMLConnect();
        PathSavedXML _pathSavedXML;
        FilterInfo _selectedDevice;
        private readonly Settings _settings;

        public class PathSavedXML()
        {
            public string DriverPhotoDevice { get; set; } = string.Empty;
            public string ImagemDeClientes { get; set; } = string.Empty;
            public string ImagemDeItens { get; set; } = string.Empty;
        }


        public SettingsPhoto()
        {
            InitializeComponent();
            _pathSavedXML = new PathSavedXML();
            Loaded += Loaded_SettingsPhoto;
        }

        public SettingsPhoto(Settings settings): this()
        {
            _settings = settings;
        }

        public SettingsPhoto(Settings settings, string sucessMessage) : this(settings)
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
            ConfirmFileSave(_pathSavedXML.DriverPhotoDevice, DriverPhoto.Text, DriverPhoto);
            ConfirmFileSave(_pathSavedXML.ImagemDeClientes, ImagemDeClientes.Text, ImagemDeClientes);
            ConfirmFileSave(_pathSavedXML.ImagemDeItens, ImagemDeItens.Text, ImagemDeItens);
        }

        private void Loaded_SettingsPhoto(object sender, RoutedEventArgs e)
        {
            var config = xmlConnect.LoadConfigurations();
            if (config != null && config.Photos.Any())
            {
                var xml = config.Photos.Last();
                if(xml.ConfigDriverPhoto != null && xml.ConfigDriverPhoto.Any())
                {
                    _pathSavedXML.DriverPhotoDevice = xml.ConfigDriverPhoto[0] ?? "";
                    DriverPhoto.Text = _pathSavedXML.DriverPhotoDevice ?? "";
                }

                _pathSavedXML.ImagemDeClientes = xml.FolderImagemClientes ?? "";
                _pathSavedXML.ImagemDeItens = xml.FolderImagemItens ?? "";


                ImagemDeClientes.Text = _pathSavedXML.ImagemDeClientes ?? VerifyFoldersAreaOfWork("Imagens de Clientes");
                ImagemDeItens.Text = _pathSavedXML.ImagemDeItens ?? VerifyFoldersAreaOfWork("Imagens de Itens");
                CompareTextFolders();
            }
            else
            {
                DriverPhoto.Text = "";
                ImagemDeClientes.Text = VerifyFoldersAreaOfWork("Imagens de Clientes");
                ImagemDeItens.Text = VerifyFoldersAreaOfWork("Imagens de Itens");
                CompareTextFolders();
            }
        }
        
        private string VerifyFoldersAreaOfWork(string folder)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string[] shortcutFiles = Directory.GetFiles(desktopPath, "*.lnk");

            foreach (var shortcutFile in shortcutFiles)
            {
                string fileNameWithouthExtension = System.IO.Path.GetFileNameWithoutExtension(shortcutFile);
                fileNameWithouthExtension = fileNameWithouthExtension.Replace("- Atalho", "");

                if (string.Equals(fileNameWithouthExtension.Replace(" ", ""), folder.Replace(" ", ""), StringComparison.OrdinalIgnoreCase))
                {
                    string targetPath = ResolveShortcut(shortcutFile);
                    if (Directory.Exists(targetPath))
                    {
                        return targetPath;
                    }
                }
            }
            return "";
        }


        private string ResolveShortcut(string shortcutPath)
        {
            try
            {
                WshShell shell = new WshShell();
                IWshShortcut link = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                return link.TargetPath;
            }
            catch (Exception)
            {
                return "";
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

        private void DriverPhoto_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SelectDriver selectDriver = new SelectDriver();
            if(selectDriver.ShowDialog() == true && selectDriver.SelectDeviceName != null)
            {
                _selectedDevice = selectDriver.SelectDeviceName;
                DriverPhoto.Text = _selectedDevice.Name ?? "";
                ConfirmFileSave(DriverPhoto.Text, _pathSavedXML.DriverPhotoDevice, DriverPhoto);
            }
        }

        private void ImagemDeClientes_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string folder = xmlConnect.SelectFolder();
            ImagemDeClientes.Text = folder;
            ConfirmFileSave(ImagemDeClientes.Text, _pathSavedXML.ImagemDeClientes, ImagemDeClientes);
        }

        private void ImagemDeItens_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string folder = xmlConnect.SelectFolder();
            ImagemDeItens.Text = folder;
            ConfirmFileSave(ImagemDeItens.Text, _pathSavedXML.ImagemDeItens, ImagemDeItens);
        }

        private void SaveConfigurationsinXML(string folder, FolderSearch folderSearch)
        {
            if (!folder.IsNullOrEmpty())
            {
                if (folderSearch.Name == "DriverPhoto")
                {
                    xmlConnect.SaveConfigPhoto(null, folderSearch, _selectedDevice);
                }
                else
                {
                    xmlConnect.SaveConfigPhoto(folder, folderSearch, null);
                }
            }
        }


        private void Save_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SaveConfigurationsinXML(DriverPhoto.Text, DriverPhoto);
            SaveConfigurationsinXML(ImagemDeItens.Text, ImagemDeItens);
            SaveConfigurationsinXML(ImagemDeClientes.Text, ImagemDeClientes);
            string messageSucess = "Configurações Salvas com Sucesso!!";
            NavigationService?.Navigate(new SettingsPhoto(_settings, messageSucess));
        }
    }
}
