using CamScan.Pages;
using CamScan.Router;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CamScan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Process atualizateProcess;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += VerifyUpdate;
        }
        private void VerifyUpdate(object sender, EventArgs e)
        {
            try
            {
                this.IsEnabled = false;

                string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

                atualizateProcess = new Process();
                atualizateProcess.StartInfo.FileName = $"{currentDirectory}/Atualizate/net8.0-windows/Atualizate.exe";
                atualizateProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                atualizateProcess.StartInfo.Arguments = Process.GetCurrentProcess().Id.ToString();
                atualizateProcess.Start();

                atualizateProcess.WaitForExit();
                this.IsEnabled = true;
            }
            catch (Exception err)
            {
                System.Windows.MessageBox.Show($"Erro na localização do arquivo de atualização: {err.Message}", "Erro de Atualização", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.IsEnabled = true;
            }

        }
        private void Window_Closed(object sender, EventArgs e)
        {
            if(atualizateProcess != null && !atualizateProcess.HasExited)
            {
                atualizateProcess.Kill();
                atualizateProcess.Dispose();
            }

            if(MainFrame.Content is Photo photo)
            {
                photo.StopCamera();
            }
        }
    }
}