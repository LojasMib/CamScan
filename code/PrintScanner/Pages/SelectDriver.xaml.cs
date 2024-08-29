using AForge.Video.DirectShow;
using CamScan.Pages;
using CamScan.Router;
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
using System.Windows.Shapes;

using WpfSystem = System.Windows;

namespace CamScan.Pages
{
    /// <summary>
    /// Lógica interna para SelectDriver.xaml
    /// </summary>
    public partial class SelectDriver : Window
    {
        private FilterInfoCollection? videoDevices;

        public FilterInfo? SelectDeviceName { get; private set; }

        public event Action<FilterInfo>? SelectionMade;
        public SelectDriver()
        {
            InitializeComponent();
            Loaded += Driver_Options;
        }

        private void Driver_Options(object sender, RoutedEventArgs e)
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count == 0)
            {
                WpfSystem.MessageBox.Show("No video source found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DriversOptions.RowDefinitions.Clear();
            DriversOptions.Children.Clear();

            int rowIndex = 0;
            foreach (FilterInfo device in videoDevices)
            {
                DriversOptions.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                var navigator = new Navigator
                {
                    Style = (Style)FindResource("SettingDrivers"),
                    Text = device.Name
                };

                navigator.Click += (s, args) => Navigator_Click(s, args, device);

                DriversOptions.Children.Add(navigator);
                Grid.SetRow(navigator, rowIndex);
                rowIndex++;

            }
        }

        private void Navigator_Click(object s, RoutedEventArgs args, FilterInfo device)
        {
            SelectDeviceName = device;
            SelectionMade?.Invoke(SelectDeviceName);
            this.DialogResult = true;
            this.Close();
        }


    }
}
