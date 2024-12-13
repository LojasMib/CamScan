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
using System.Runtime.InteropServices; 
///retirar biblioteca
using IWshRuntimeLibrary;
using PdfSharp.Drawing;
using ServiceStack.Text.Support;
using CamScan.Router;
using static CamScan.Pages.SettingsScanner;

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
        ScannerDevice _selectedDevice;
        PathSavedXML _pathSavedXML;
        private readonly Settings _settings;
        public class PathSavedXML()
        {
            public string DriverScannerDevice { get; set; } = string.Empty;
            public string FolderDocClientes { get; set; } = string.Empty;
            public string ConfissaoDivida { get; set; } = string.Empty;
            public string Despesas { get; set; } = string.Empty;
            public string Outros { get; set; } = string.Empty;
        }
        

        private bool DriverType = true; 

        ScannerDevice ScannerDevice { get; set; }
        public SettingsScanner()
        {
            InitializeComponent();
            _pathSavedXML = new PathSavedXML();
            Loaded += Loaded_SettingScanner;
        }

        public SettingsScanner(Settings settings): this()
        {
            _settings = settings;
        }
        public SettingsScanner(Settings settings, string sucessMessage) : this(settings)
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

        private ConfigScanner? Load_XML()
        {
            var config = xmlConnect.LoadConfigurations();
            if (config == null || config.Scanner.Any()) return config.Scanner.Last();
            else return null;
        }

        private void CompareTextFolders()
        {
            ConfirmFileSave(_pathSavedXML.DriverScannerDevice, DriverScanner.Text, DriverScanner);
            ConfirmFileSave(_pathSavedXML.FolderDocClientes, FolderDocClientes.Text, FolderDocClientes);
            ConfirmFileSave(_pathSavedXML.ConfissaoDivida, ConfissaoDivida.Text, ConfissaoDivida);
            ConfirmFileSave(_pathSavedXML.Despesas, Despesas.Text, Despesas);
            ConfirmFileSave(_pathSavedXML.Outros, Outros.Text, Outros);
        }

        private void Private_DriverType(string driverType)
        {
            if(driverType == "WIA")
            {
                SelectDriverType(true);
            }
            else if(driverType == "TWAIN")
            {
                SelectDriverType(false);
            }
            else
            {
                SelectDriverType(true);
            }
        }

        private void Loaded_SettingScanner(object sender, RoutedEventArgs e)
        {
            var xml = Load_XML();
            if (xml != null)
            {
                if (xml.ConfigDriver != null)
                {
                    _pathSavedXML.DriverScannerDevice = xml.ConfigDriver.Name ?? "";
                    _selectedDevice = xml.ConfigDriver;
                    DriverScanner.Text = _pathSavedXML.DriverScannerDevice ?? "";
                    Private_DriverType(xml.ConfigDriver.Type);
                }

                _pathSavedXML.FolderDocClientes = xml.FolderDocumentoCliente ?? "";
                _pathSavedXML.ConfissaoDivida = xml.FolderConfissaoDivida ?? "";
                _pathSavedXML.Despesas = xml.FolderDespesas ?? "";
                _pathSavedXML.Outros = xml.FolderOutros ?? "";

                FolderDocClientes.Text = _pathSavedXML.FolderDocClientes ?? VerifyFolders_AreaOfWork("Documentos de Clientes");
                ConfissaoDivida.Text = _pathSavedXML.ConfissaoDivida ?? VerifyFolders_AreaOfWork("Confissao de Divida");
                Despesas.Text = _pathSavedXML.Despesas ?? VerifyFolders_AreaOfWork("Despesas");
                Outros.Text = _pathSavedXML.Outros ?? "";
                CompareTextFolders();
            }
            else
            {
                DriverScanner.Text = "";
                FolderDocClientes.Text = VerifyFolders_AreaOfWork("Documentos de Clientes");
                ConfissaoDivida.Text = VerifyFolders_AreaOfWork("Confissao de Divida");
                Despesas.Text = VerifyFolders_AreaOfWork("Despesas");
                Outros.Text = "";
                CompareTextFolders();
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
            if(textInput != pathSaved || string.IsNullOrEmpty(textInput))
            {
                UpdateBorderBrushRed(typePath);
            }
            else if(textInput == pathSaved && !string.IsNullOrEmpty(textInput))
            {
                UpdateBorderBrushGreen(typePath);
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

            if (DriverType)
            {
                foreach (var device in WiaDevices)
                {
                    scannerDevices.Add(new ScannerDevice { Name = device, Type = "WIA" });
                }
            }
            else
            {
                foreach (var device in TwainDevices)
                {
                    scannerDevices.Add(new ScannerDevice { Name = device, Type = "TWAIN" });
                }
            }
            
            SelectionDriver selectionDriver = new SelectionDriver(scannerDevices);
            if(selectionDriver.ShowDialog() == true && selectionDriver.SelectDeviceName != null)
            {
                _selectedDevice = selectionDriver.SelectDeviceName;
                DriverScanner.Text = _selectedDevice.Name ?? "";
                ConfirmFileSave(DriverScanner.Text, _pathSavedXML.DriverScannerDevice, DriverScanner);
            }

        }

        private void FolderDocClientes_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string folder = xmlConnect.SelectFolder();
            FolderDocClientes.Text = folder;
            ConfirmFileSave(FolderDocClientes.Text, _pathSavedXML.FolderDocClientes, FolderDocClientes);
        }


        private void ConfissaoDivida_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string folder = xmlConnect.SelectFolder();
            ConfissaoDivida.Text = folder;
            ConfirmFileSave(FolderDocClientes.Text, _pathSavedXML.ConfissaoDivida, ConfissaoDivida);
        }

        private void Despesas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string folder = xmlConnect.SelectFolder();
            Despesas.Text = folder;
            ConfirmFileSave(FolderDocClientes.Text, _pathSavedXML.Despesas, Despesas);
        }
        private void Outros_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string folder = xmlConnect.SelectFolder();
            Outros.Text = folder;
            ConfirmFileSave(FolderDocClientes.Text, _pathSavedXML.Outros, Outros);
        }


        private void SaveConfigurationsinXML(string folder, FolderSearch folderSearch)
        {
            if (!folder.IsNullOrEmpty())
            {
                if (folderSearch.Name ==  "DriverScanner")
                {
                    xmlConnect.SaveConfigScanner(null, folderSearch, _selectedDevice);
                }
                else
                {
                    xmlConnect.SaveConfigScanner(folder, folderSearch, null);
                }
            }
        }

        private void Save_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SaveConfigurationsinXML(DriverScanner.Text, DriverScanner);
            SaveConfigurationsinXML(FolderDocClientes.Text, FolderDocClientes);
            SaveConfigurationsinXML(ConfissaoDivida.Text, ConfissaoDivida);
            SaveConfigurationsinXML(Despesas.Text, Despesas);
            SaveConfigurationsinXML(Outros.Text, Outros);

            string messageSucess = "Configurações Salvas com Sucesso!!";
            NavigationService?.Navigate(new SettingsScanner(_settings, messageSucess));
        }

        private void SelectDriverType(bool typeDrive)
        {
            string colorGrayHexa = "#708090";
            string colorOrangeHexa = "#FF7F50";
            DriverType = typeDrive;

            wpfSystem.Media.Color colorGray = (wpfSystem.Media.Color)wpfSystem.Media.ColorConverter.ConvertFromString(colorGrayHexa);
            wpfSystem.Media.Color colorOrange = (wpfSystem.Media.Color)wpfSystem.Media.ColorConverter.ConvertFromString(colorOrangeHexa);
            if (DriverType)
            {
                Btn_Rede.BorderBrush = new SolidColorBrush(colorGray);
                Btn_USB.BorderBrush = new SolidColorBrush(colorOrange);
            }
            else
            {
                Btn_Rede.BorderBrush = new SolidColorBrush(colorOrange);
                Btn_USB.BorderBrush = new SolidColorBrush(colorGray);
            }
        }

        private void Btn_USB_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SelectDriverType(true);
        }

        private void Btn_Rede_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SelectDriverType(false);
        }
    }
}
