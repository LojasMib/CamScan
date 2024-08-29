using AForge.Video.DirectShow;
using CamScan.Class;
using CamScan.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WinForms = System.Windows.Forms;

namespace CamScan.Services
{
    public class DriverScannerConfig
    {
        public string? Nome {  get; set; }
    }
    public class ConfigScanner
    {
        public ScannerDevice? ConfigDriver { get; set; }
        public string? FolderDocumentoCliente { get; set; }
        public string? FolderConfissaoDivida { get; set; }
        public string? FolderDespesas { get; set; }
        public string? FolderOutros { get; set; }
    }
    public class ConfigPhoto
    {
        public List<string>? ConfigDriverPhoto { get; set; }
        public string? FolderImagemClientes { get; set; }
        public string? FolderImagemItens { get;set; }
    }
    public class ConfigFranquia
    {
        public string? Cidade { get; set; }
        public string? Franquia { get; set; }

    }
    [XmlRoot("Configurations")]
    public class Configurations
    {
        public List<ConfigScanner> Scanner { get; set; } = new();
        public List<ConfigPhoto> Photos { get; set; } = new();
            public List<ConfigFranquia> Franquias { get; set; } = new();
    }
    public class XMLConnect
    {
        private string ConfigLocal => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.xml");

        public XMLConnect() { }

        public string SelectFolder()
        {
            using(WinForms.FolderBrowserDialog dialog = new WinForms.FolderBrowserDialog())
            {
                dialog.Description = "Selecione uma Pasta";
                if(dialog.ShowDialog() == DialogResult.OK)
                {
                    Console.WriteLine(dialog.SelectedPath);
                    return dialog.SelectedPath;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public Configurations LoadConfigurations()
        {
            if (File.Exists(ConfigLocal))
            {
                var serializer = new XmlSerializer(typeof(Configurations));
                using (var reader = new StreamReader(ConfigLocal))
                {
                    var configs = (Configurations?)serializer.Deserialize(reader) ?? new Configurations();
                    return configs;
                }
            }
            return new Configurations();
        }
        public void SaveConfigurations(Configurations configs)
        {
            var serializerSave = new XmlSerializer(typeof(Configurations));
            using (var writer = new StreamWriter(ConfigLocal))
            {
                serializerSave.Serialize(writer, configs);
            }
        }

        public void SaveConfigFranquia(string tipo, string folder)
        {
            var configs = LoadConfigurations();
            ConfigFranquia newFranquia = configs.Franquias.FirstOrDefault() ?? new ConfigFranquia();

            if(tipo == "ConfigCidade")
            {
                newFranquia.Cidade = folder;
            }
            else if(tipo == "ConfigFranquia")
            {
                newFranquia.Franquia = folder;
            }
            configs.Franquias.Clear();
            configs.Franquias.Add(newFranquia);
            SaveConfigurations(configs);

        }

        public void SaveConfigPhoto(string tipo, string folder, FilterInfo? driver = null)
        {
            var configs = LoadConfigurations();

            ConfigPhoto newConfig = configs.Photos.FirstOrDefault() ?? new ConfigPhoto();

            if(newConfig.ConfigDriverPhoto == null)
            {
                newConfig.ConfigDriverPhoto = new List<String>();
            }

            if (tipo == "ConfigDriverPhoto" && driver != null)
            {
                newConfig.ConfigDriverPhoto.Clear();
                newConfig.ConfigDriverPhoto.Add(driver.Name);
                newConfig.ConfigDriverPhoto.Add(driver.MonikerString);
            }
            else if (tipo == "FolderImagemClientes")
            {
                newConfig.FolderImagemClientes = folder;
            }
            else if (tipo == "FolderImagemItens")
            {
                newConfig.FolderImagemItens = folder;
            }
            configs.Photos.Clear();
            configs.Photos.Add(newConfig);

            SaveConfigurations(configs);
        }


        public void SaveConfigScanner(string tipo, string? folder, ScannerDevice? driver)
        {
            var configs = LoadConfigurations();
            ConfigScanner newConfig = configs.Scanner.FirstOrDefault() ?? new ConfigScanner();

            if (tipo == "ConfigDriver")
            {
                newConfig.ConfigDriver = driver;
            }
            else if (tipo == "FolderDocumentoCliente")
            {
                newConfig.FolderDocumentoCliente = folder;
            }
            else if (tipo == "FolderConfissaoDivida")
            {
                newConfig.FolderConfissaoDivida = folder;
            }
            else if (tipo == "FolderDespesas")
            {
                newConfig.FolderDespesas = folder;
            }
            else if (tipo == "FolderOutros")
            {
                newConfig.FolderOutros = folder;
            }

            configs.Scanner.Clear();
            configs.Scanner.Add(newConfig);

            SaveConfigurations(configs);
        }

    }
}
