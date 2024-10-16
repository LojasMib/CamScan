using CamScan.Class;
using CamScan.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wpfSystem = System.Windows;
using interopSystem = System.Windows.Interop;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using System.Windows;
using System.IO;
using System.Diagnostics.Eventing.Reader;
using ServiceStack;
using System.Runtime.InteropServices; ///retirar biblioteca
using IWshRuntimeLibrary;

namespace CamScan.Pages
{
    /// <summary>
    /// Interação lógica para SettingsScanner.xaml
    /// </summary>
    public partial class SettingsScanner : Page
    {
        XMLConnect xmlConnect = new XMLConnect();
        WiaScanner wiaScanner = new WiaScanner();
        TwainScanner twainScanner = new TwainScanner();

        ScannerDevice ScannerDevice { get; set; }
        public SettingsScanner()
        {
            InitializeComponent();
            Loaded += Loaded_SettingScanner;
        }

        private ConfigScanner? Load_XML()
        {
            var config = xmlConnect.LoadConfigurations();
            if (config == null || config.Scanner.Any()) return config.Scanner.Last();
            else return null;
        }

        private void Loaded_SettingScanner(object sender, RoutedEventArgs e)
        {
            var xml = Load_XML();
            if (xml != null)
            {
                if (xml.ConfigDriver != null)
                {
                    DriverScanner.Text = xml.ConfigDriver.Name ?? "";
                }
                FolderDocClientes.Text = xml.FolderDocumentoCliente ?? VerifyFolders_AreaOfWork("Documentos de Clientes");
                ConfissaoDivida.Text = xml.FolderConfissaoDivida ?? VerifyFolders_AreaOfWork("Confissao de Divida");
                Despesas.Text = xml.FolderDespesas ?? VerifyFolders_AreaOfWork("Despesas");
                Outros.Text = xml.FolderOutros ?? "";
            }
            else
            {
                DriverScanner.Text = "";
                FolderDocClientes.Text = VerifyFolders_AreaOfWork("Documentos de Clientes");
                ConfissaoDivida.Text = VerifyFolders_AreaOfWork("Confissao de Divida");
                Despesas.Text = VerifyFolders_AreaOfWork("Despesas");
                Outros.Text = "";
            }
            
        }

        private string VerifyFolders_AreaOfWork(string folder)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string[] shortcutFiles = Directory.GetFiles(desktopPath, "*.lnk");

            foreach(var shortcutFile in shortcutFiles)
            {
                string fileNameWithouthExtension = System.IO.Path.GetFileNameWithoutExtension(shortcutFile);
                fileNameWithouthExtension = fileNameWithouthExtension.Replace("- Atalho", "");

                if (string.Equals(fileNameWithouthExtension.Replace(" ",""), folder.Replace(" ",""), StringComparison.OrdinalIgnoreCase))
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
        private void DriverScanner_MouseDown(object sender, MouseButtonEventArgs e)
        {

            //Lista dispositivos Twain
            List<string>? TwainDevices = twainScanner.ListDevices();
            if (TwainDevices == null)
            {
                return;
            }



            //Lista dispositivos WIA
            List<string> WiaDevices = wiaScanner.ListScanners();

            List<ScannerDevice> scannerDevices = new List<ScannerDevice>();

            foreach (var device in WiaDevices)
            {
                scannerDevices.Add(new ScannerDevice { Name = device, Type = "WIA"});
            }

            foreach (var device in TwainDevices)
            {
                scannerDevices.Add(new ScannerDevice { Name = device, Type = "TWAIN" });
            }

            SelectionDriver selectionDriver = new SelectionDriver(scannerDevices);
            if(selectionDriver.ShowDialog() == true && selectionDriver.SelectDeviceName != null)
            {
                var selectedDevice = selectionDriver.SelectDeviceName;
                xmlConnect.SaveConfigScanner("ConfigDriver", null , selectedDevice);
                DriverScanner.Text = selectedDevice.Name;
            }

        }

        private void FolderDocClientes_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string folder = xmlConnect.SelectFolder();
            xmlConnect.SaveConfigScanner("FolderDocumentoCliente", folder, null);
            FolderDocClientes.Text = folder;
        }


        private void ConfissaoDivida_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string folder = xmlConnect.SelectFolder();
            xmlConnect.SaveConfigScanner("FolderConfissaoDivida", folder, null);
            ConfissaoDivida.Text = folder;
        }

        private void Despesas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string folder = xmlConnect.SelectFolder();
            xmlConnect.SaveConfigScanner("FolderDespesas", folder, null);
            Despesas.Text = folder;
        }
        private void Outros_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string folder = xmlConnect.SelectFolder();
            xmlConnect.SaveConfigScanner("FolderOutros", folder, null);
            Outros.Text = folder;
        }

        private void SaveConfigurationsinXML(string folder, string xmlFolder)
        {
            if (!folder.IsNullOrEmpty())
            {
                xmlConnect.SaveConfigScanner(xmlFolder, folder, null);
            }
        }

        private void Save_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SaveConfigurationsinXML(FolderDocClientes.Text, "FolderDocumentoCliente");
            SaveConfigurationsinXML(ConfissaoDivida.Text, "FolderConfissaoDivida");
            SaveConfigurationsinXML(Despesas.Text, "FolderDespesas");
            SaveConfigurationsinXML(Outros.Text, "FolderOutros");
        }
    }
}
