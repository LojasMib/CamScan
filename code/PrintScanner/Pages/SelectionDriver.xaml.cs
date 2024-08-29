using AForge.Video.DirectShow;
using CamScan.Router;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfSystem = System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows;
using CamScan.Class;

namespace CamScan.Pages
{
    /// <summary>
    /// Lógica interna para SelectionDriver.xaml
    /// </summary>
    public partial class SelectionDriver : Window
    {
        private ScannerDevice? ScannerDevice { get; set; }
        private List<ScannerDevice> Devices {  get; set; }
        public ScannerDevice? SelectDeviceName { get; private set; }

        public event Action<ScannerDevice>? SelectionMade;
        public SelectionDriver(List<ScannerDevice> Devices)
        {
            InitializeComponent();
            this.Devices = Devices;
            Loaded += Driver_Options;
        }

        public void Driver_Options(object sender, RoutedEventArgs e)
        {
            if (Devices.Count == 0)
            {
                WpfSystem.MessageBox.Show("No device found", "Error", WpfSystem.MessageBoxButton.OK, WpfSystem.MessageBoxImage.Error);
                return;
            }

            int rowIndex = 0;
            foreach (var device in Devices)
            {
                DriversOptions.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                var navigator = new Navigator
                {
                    Style = (Style)FindResource("SettingDrivers"),
                    Text = $"{device.Name} - {device.Type}",
                    Tag= device
                };

                navigator.Click += Navigator_Click;

                DriversOptions.Children.Add(navigator);
                Grid.SetRow(navigator, rowIndex);
                rowIndex++;

            }
        }


        private void Navigator_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Navigator button && button.Tag is ScannerDevice device)
            {
                SelectDeviceName = device;
                SelectionMade?.Invoke(SelectDeviceName);
                DialogResult = true;
                Close();
            }
            
        }
    }
}
