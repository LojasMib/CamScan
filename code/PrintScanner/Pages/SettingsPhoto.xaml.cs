using AForge.Video.DirectShow;
using CamScan.Services;
using IWshRuntimeLibrary;
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
        public SettingsPhoto()
        {
            InitializeComponent();
            Loaded += Loaded_SettingsPhoto;
            Unloaded += Unloaded_SettingsPhoto;
        }

        private void Loaded_SettingsPhoto(object sender, RoutedEventArgs e)
        {
            var config = xmlConnect.LoadConfigurations();
            if (config != null && config.Photos.Any())
            {
                var xml = config.Photos.Last();
                try
                {
                    DriverPhoto.Text = xml.ConfigDriverPhoto[0];
                }
                catch
                {
                    DriverPhoto.Text = "";
                }
                ImagemDeClientes.Text = xml.FolderImagemClientes ?? VerifyFoldersAreaOfWork("Imagens de Clientes");
                ImagemDeItens.Text = xml.FolderImagemItens ?? VerifyFoldersAreaOfWork("Imagens de Itens");
            }
            else
            {
                DriverPhoto.Text = "";
                ImagemDeClientes.Text = VerifyFoldersAreaOfWork("Imagens de Clientes");
                ImagemDeItens.Text = VerifyFoldersAreaOfWork("Imagens de Itens");
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


        private void Unloaded_SettingsPhoto(object sender, RoutedEventArgs e)
        {
            xmlConnect.SaveConfigPhoto("FolderImagemItens", ImagemDeClientes.Text);
            xmlConnect.SaveConfigPhoto("FolderImagemClientes", ImagemDeItens.Text);
        }

        private void DriverPhoto_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SelectDriver selectDriver = new SelectDriver();
            if(selectDriver.ShowDialog() == true)
            {
                FilterInfo selectDevice = selectDriver.SelectDeviceName;
                xmlConnect.SaveConfigPhoto("ConfigDriverPhoto", "", selectDevice);
                DriverPhoto.Text = selectDevice.Name;
            }
        }

        private void ImagemDeClientes_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string folder = xmlConnect.SelectFolder();
            xmlConnect.SaveConfigPhoto("FolderImagemClientes", folder);
            ImagemDeClientes.Text = folder;
        }

        private void ImagemDeItens_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string folder = xmlConnect.SelectFolder();
            xmlConnect.SaveConfigPhoto("FolderImagemItens", folder);
            ImagemDeItens.Text = folder;
        }
    }
}
