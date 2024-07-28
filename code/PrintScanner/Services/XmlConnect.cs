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
    public class XMLConnect
    {
        private string? ConfigLocal { get; set; }
        private string? ConfigDriverPhoto { get; set; }
        private string? ConfigDriver { get; set; }
        private string? FolderDocumentoCliente { get; set; }
        private string? FolderConfissaoDivida { get; set; }
        private string? FolderDespesas { get; set; }
        private string? FolderOutros { get; set; }
        public string? FolderImagemClientes { get; set; }
        public string? FolderImagemItens { get; set; }

        public XMLConnect() { }

        private string LocalFolder()
        {
            string folderPath= AppDomain.CurrentDomain.BaseDirectory;
            return System.IO.Path.Combine(folderPath, "config.xml");
        }

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
                return "";
            }

        }

        public List<ConfigPhoto> LoadConfigPhoto()
        {

            ConfigLocal = LocalFolder();
            if (File.Exists(ConfigLocal))
            {
                var serializer = new XmlSerializer(typeof(List<ConfigPhoto>));
                using (var reader = new StreamReader(ConfigLocal))
                {
                    var configs = (List<ConfigPhoto>?)serializer.Deserialize(reader) ?? null;
                    if (configs != null && configs.Any())
                    {
                        var config = configs.Last();
                        ConfigDriverPhoto = config.ConfigDriverPhoto;
                        FolderImagemClientes = config.FolderImagemClientes;
                        FolderImagemItens = config.FolderImagemItens;
                        return configs;
                    }
                }
            }
            return new List<ConfigPhoto>();
        }
        public void SaveConfigPhoto(string tipo, string folder)
        {
            ConfigLocal = LocalFolder();
            List<ConfigPhoto> configs = new List<ConfigPhoto>();

            if (File.Exists(ConfigLocal))
            {
                var serializer = new XmlSerializer(typeof(List<ConfigPhoto>));
                using (var reader = new StreamReader(ConfigLocal))
                {
                    var existingConfigs = (List<ConfigPhoto>?)serializer.Deserialize(reader);
                    if (existingConfigs != null)
                    {
                        configs = existingConfigs;
                    }
                }
            }


            if (tipo == "ConfigDriverPhoto")
            {
                ConfigDriverPhoto = folder;
            }
            else if (tipo == "FolderImagemClientes")
            {
                FolderDocumentoCliente = folder;
            }
            else if (tipo == "FolderImagemItens")
            {
                FolderConfissaoDivida = folder;
            }
            // Add or update configuration
            ConfigPhoto newConfig = new ConfigPhoto
            {
                ConfigDriverPhoto = ConfigDriverPhoto,
                FolderImagemClientes = FolderImagemClientes,
                FolderImagemItens = FolderImagemItens
            };
            configs.Clear();
            configs.Add(newConfig);

            var serializerSave = new XmlSerializer(typeof(List<ConfigPhoto>));
            using (var writer = new StreamWriter(ConfigLocal))
            {
                serializerSave.Serialize(writer, configs);
            }
        }

        public List<ConfigScanner> LoadConfig()
        {

            ConfigLocal = LocalFolder();
            if (File.Exists(ConfigLocal))
            {
                var serializer = new XmlSerializer(typeof(List<ConfigScanner>));
                using (var reader = new StreamReader(ConfigLocal))
                {
                    var configs = (List<ConfigScanner>?)serializer.Deserialize(reader);
                    if (configs != null && configs.Any())
                    {
                        var config = configs.Last();
                        ConfigDriver = config.ConfigDriver;
                        FolderDocumentoCliente = config.FolderDocumentoCliente;
                        FolderConfissaoDivida = config.FolderConfissaoDivida;
                        FolderDespesas = config.FolderDespesas;
                        FolderOutros = config.FolderOutros;
                        return configs;
                    }
                }
            }
            return new List<ConfigScanner>();
        }

        

        public void SaveConfig(string tipo, string folder)
        {
            ConfigLocal = LocalFolder();
            List<ConfigScanner> configs = new List<ConfigScanner>();

            if (File.Exists(ConfigLocal))
            {
                var serializer = new XmlSerializer(typeof(List<ConfigScanner>));
                using (var reader = new StreamReader(ConfigLocal))
                {
                    var existingConfigs = (List<ConfigScanner>?)serializer.Deserialize(reader);
                    if (existingConfigs != null)
                    {
                        configs = existingConfigs;
                    }
                }
            }


            if (tipo == "ConfigDriver")
            {
                ConfigDriver = folder;
            }
            else if (tipo == "FolderDocumentoCliente")
            {
                FolderDocumentoCliente = folder;
            }
            else if (tipo == "FolderConfissaoDivida")
            {
                FolderConfissaoDivida = folder;
            }
            else if (tipo == "FolderDespesas")
            {
                FolderDespesas = folder;
            }
            else if (tipo == "FolderOutros")
            {
                FolderOutros = folder;
            }
            // Add or update configuration
            ConfigScanner newConfig = new ConfigScanner
            {
                ConfigDriver = ConfigDriver,
                FolderDocumentoCliente = FolderDocumentoCliente,
                FolderConfissaoDivida = FolderConfissaoDivida,
                FolderDespesas = FolderDespesas,
                FolderOutros = FolderOutros
            };
            configs.Clear();
            configs.Add(newConfig);

            var serializerSave = new XmlSerializer(typeof(List<ConfigScanner>));
            using (var writer = new StreamWriter(ConfigLocal))
            {
                serializerSave.Serialize(writer, configs);
            }
        }



    }
}
