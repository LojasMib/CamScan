﻿using PrintScanner.Services;
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
using System.Xml.Serialization;

namespace PrintScanner.Pages
{
    /// <summary>
    /// Interação lógica para SettingsScanner.xam
    /// </summary>
    public partial class SettingsScanner : Page
    {
        XMLConnect xmlConnect = new XMLConnect();
        public SettingsScanner()
        {
            InitializeComponent();
            LocalFolder();
        }

        private void LocalFolder()
        {
            var config = xmlConnect.LoadConfig();
            if (config != null && config.Any())
            {
                var xml = config.Last();
                DriverScanner.Text = xml.ConfigDriver ?? "";
                FolderDocClientes.Text = xml.FolderDocumentoCliente ?? "";
                ConfissaoDivida.Text = xml.FolderConfissaoDivida ?? "";
                Despesas.Text = xml.FolderDespesas ?? "";
                Outros.Text = xml.FolderOutros ?? "";
            }

        }

        //Interaction Buttons

        private void DriverScanner_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string folder = xmlConnect.SelectFolder();
            xmlConnect.SaveConfig("ConfigDriver", folder);
            DriverScanner.Text = folder;
        }

        private void FolderDocClientes_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string folder = xmlConnect.SelectFolder();
            xmlConnect.SaveConfig("FolderDocumentoCliente", folder);
            FolderDocClientes.Text = folder;
        }


        private void ConfissaoDivida_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string folder = xmlConnect.SelectFolder();
            xmlConnect.SaveConfig("FolderConfissaoDivida", folder);
            ConfissaoDivida.Text = folder;
        }

        private void Despesas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string folder = xmlConnect.SelectFolder();
            xmlConnect.SaveConfig("FolderDespesas", folder);
            Despesas.Text = folder;
        }
        private void Outros_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string folder = xmlConnect.SelectFolder();
            xmlConnect.SaveConfig("FolderOutros", folder);
            Outros.Text = folder;
        }
    }
}