using PrintScanner.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PrintScanner.Services
{
    public class ConfigScanner
    {
        public string? ConfigDriver { get; set; }
        public string? FolderDocumentoCliente { get; set; }
        public string? FolderConfissaoDivida { get; set; }
        public string? FolderDespesas { get; set; }
        public string? FolderOutros { get; set; }
    }
    public class ConfigPhoto
    {
        public string? ConfigDriverPhoto { get; set; }
        public string? FolderImagemClientes { get; set; }
        public string? FolderImagemItens { get;set; }
    }

    [XmlRoot("Configurations")]
    public class Configurations
    {
        public List<ConfigScanner> Scanner { get; set; } = new();
        public List<ConfigPhoto> Photos { get; set; } = new();
    }
    public class XMLConnect
    {

        private string ConfigLocal => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.xml");

        public XMLConnect() { }

        public string SelectFolder()
        {
            Microsoft.Win32.OpenFolderDialog dialog = new();
            dialog.Multiselect = false;
            dialog.Title = "Select a Folder";

            bool? result = dialog.ShowDialog();

            if (result != null)
            {
                Console.WriteLine(dialog.FolderName);
                return dialog.FolderName.ToString();

            }
            else
            {
                Console.WriteLine("Error in select folder");
                return string.Empty;
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
        public void SaveConfigPhoto(string tipo, string folder)
        {
            var configs = LoadConfigurations();

            ConfigPhoto newConfig = configs.Photos.FirstOrDefault() ?? new ConfigPhoto();

            if (tipo == "ConfigDriverPhoto")
            {
                newConfig.ConfigDriverPhoto = folder;
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


        public void SaveConfigScanner(string tipo, string folder)
        {
            var configs = LoadConfigurations();
            ConfigScanner newConfig = configs.Scanner.FirstOrDefault() ?? new ConfigScanner();

            if (tipo == "ConfigDriver")
            {
                newConfig.ConfigDriver = folder;
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
