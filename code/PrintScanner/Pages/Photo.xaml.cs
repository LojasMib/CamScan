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
using System.Windows.Navigation;
using System.Windows.Shapes;
using AForge.Video.DirectShow;
using AForge.Video;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Threading;
using CamScan.Services;
using System.Diagnostics.Tracing;
using CamScan.Pages;
using System.Windows.Forms;

namespace CamScan.Pages
{
    public partial class Photo : Page
    {
        private string? ImagemCliente {  get; set; }
        private string? ImagemItem { get; set; }
        private Bitmap? Captura { get; set; }

        private VideoCaptureDevice? videoSource;
        private Bitmap? latestFrame;
        private DispatcherTimer? timer;
        private readonly object frameLock = new object();
        private readonly XMLConnect xmlConnect = new XMLConnect();
        
        public Photo()
        {
            InitializeComponent();
            Loaded += PhotoPage_Loaded;
            Unloaded += PhotoPage_Unloaded;
        }

        private void Navigator_Click(object sender, RoutedEventArgs e)
        {
            var clickButton = e.OriginalSource as Navigator;
            NavigationService.Navigate(clickButton?.NavUri);
        }

        private void PhotoPage_Loaded(object sender, RoutedEventArgs e)
        {
            var config = xmlConnect.LoadConfigurations();
            RdBtn_ImagemCliente.IsChecked = true;
            if(config != null && config.Photos.Any() )
            {
                var xml = config.Photos.Last();
                if(xml.ConfigDriverPhoto != null && xml.ConfigDriverPhoto.Count > 1)
                {
                    videoSource = new VideoCaptureDevice(xml.ConfigDriverPhoto[1]);
                    ImagemCliente = xml.FolderImagemClientes;
                    ImagemItem = xml.FolderImagemItens;
                    videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
                    videoSource.Start();

                    timer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromMilliseconds(30)
                    };
                    timer.Tick += Timer_Tick;
                    timer.Start();
                }
                
            }

            if(ImagemCliente == null || ImagemCliente == "")
            {
                Error error = new Error("Pasta para salvamento de imagem de cliente não condigurada");
                Window parentWindow = Window.GetWindow(this);
                if (parentWindow != null)
                {
                    error.Owner = parentWindow;
                }
                error.ShowDialog();
                NavigationService.Navigate(new Uri("Pages/Landing.xaml", UriKind.Relative));
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            lock(frameLock)
            {
                if (latestFrame != null)
                {
                    BitmapImage bitmapImage = BitmapToBitmapImage(latestFrame);
                    webCamImage.Source = bitmapImage;
                }
            }
        }

        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            lock (frameLock)
            {
                if (latestFrame != null)
                {
                    latestFrame.Dispose();
                }
                latestFrame = (Bitmap)eventArgs.Frame.Clone();
            }
            
        }

        private BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            lock (frameLock)
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                    memory.Position = 0;
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                    return bitmapImage;
                }
            }
            
        }

        private void PhotoPage_Unloaded(object sender, EventArgs e)
        {
            StopCamera();
        }
        public void StopCamera()
        {
            if (videoSource != null)
            {
                if (videoSource.IsRunning)
                {
                    videoSource.SignalToStop();
                    videoSource.WaitForStop();
                }
                videoSource.NewFrame -= video_NewFrame;
            }
            if (timer != null)
            {
                timer.Stop();
                timer.Tick -= Timer_Tick;
            }
            if (latestFrame != null)
            {
                latestFrame.Dispose();
                latestFrame = null;
            }
        }

        private void webCamImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            lock (frameLock)
            {
                if(latestFrame != null)
                {
                    Captura = (Bitmap)latestFrame.Clone();
                    timer?.Stop();
                }
            }
        }

        private bool Error_Input(string input, string Message)
        {
            if(string.IsNullOrEmpty(input) || input == "")
            {
                CodigoInput.BorderBrush = System.Windows.Media.Brushes.Red;
                CodigoInput.BorderThickness = new Thickness(2);
                ErrorInput.Text = Message;
                ErrorInput.Visibility = Visibility.Visible;
                return true;
            }
            return false;
        }

        private void Error_SetFolder(string Message)
        {
            string message = Message;
            Console.WriteLine(message);
            Error error = new Error(message);
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                error.Owner = parentWindow;
            }
            error.ShowDialog();
        }
        private void Error_Frame(string Message)
        {
            string message = Message;
            Console.WriteLine(message);
            Error error = new Error(message);
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                error.Owner = parentWindow;
            }
            error.ShowDialog();
        }

        private void Salvar_Imagem(int option)
        {
            string CodigoCliente = CodigoInput.Text + ".jpg";
            string ImagemLocal = "";
            if(option == 0)
            {
                ImagemLocal = System.IO.Path.Combine(ImagemCliente, CodigoCliente);
            }
            if(option == 1)
            {
                ImagemLocal = System.IO.Path.Combine(ImagemItem, CodigoCliente);
            }
            
            try
            {
                lock (frameLock)
                {
                    if (Captura != null)
                    {
                        Captura.Save(ImagemLocal, ImageFormat.Jpeg);
                        Captura = null;
                        timer?.Start();
                    }
                    else
                    {
                        Console.WriteLine("Errro: Captura é null");
                    }
                }
                Console.WriteLine("Imagem salva com sucesso");
                CodigoInput.BorderBrush = System.Windows.Media.Brushes.Black;
                CodigoInput.BorderThickness = new Thickness(1);
                CodigoInput.Text = "";
                ErrorInput.Text = "";
                ErrorInput.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"erro ao salvar a imagem: {ex.Message}");
            }
        }

        private void Salvar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if(Captura == null)
                {
                    ErrorImage.Visibility = Visibility.Visible;
                    ErrorImage.Text = "Tire uma foto antes de tentar salvar!";
                    return;
                }
                if (RdBtn_ImagemCliente.IsChecked == true)
                {
                    bool filter_input = Error_Input(CodigoInput.Text, "Insira o código do cliente antes de salvar!");
                    if (filter_input)
                    {
                        return;
                    }
                    if (ImagemCliente == "" || ImagemCliente == null)
                    {
                        Error_SetFolder("Pasta de imagens de clientes não configurada.");
                        return;
                    }
                    Salvar_Imagem(0);
                    if (Captura != null)
                    {
                        Cancel_Process();
                    }
                    
                }
                else if (RdBtn_ImagemItens.IsChecked == true)
                {
                    bool filter_input = Error_Input(CodigoInput.Text, "Insira a descição do item antes de salvar!");
                    if (filter_input)
                    {
                        return;
                    }
                    if (ImagemCliente == "" || ImagemCliente == null)
                    {
                        Error_SetFolder("Pasta de imagem de itens não configurada.");
                        return;
                    }
                    Salvar_Imagem(1);
                    if (Captura != null)
                    {
                        Cancel_Process();
                    }
                    
                }
            }catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Erro ao salvar Imagem => {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            

        }

        private void Cancel_Process()
        {
            CodigoInput.Text = "";
            ErrorInput.Visibility = Visibility.Hidden;
            ErrorImage.Visibility = Visibility.Hidden;
        }

        private void Cancelar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Cancel_Process();
            timer?.Start();
        }

        private void RdBtn_ImagemCliente_Checked(object sender, RoutedEventArgs e)
        {
            if(CodigoInput.Text == null || CodigoInput.Text == "")
            {
                ErrorInput.Visibility = Visibility.Collapsed;
                ErrorInput.Text = "Insira o codigo do cliente antes de salvar!";
            }
            LabelFolderPhoto.Content = "Código do Cliente";
        }

        private void RdBtn_ImagemItens_Checked(object sender, RoutedEventArgs e)
        {
            if (CodigoInput.Text == null || CodigoInput.Text == "")
            {
                ErrorInput.Visibility = Visibility.Collapsed;
                ErrorInput.Text = "Insira a descriçao do item antes de salvar!";
            }
            LabelFolderPhoto.Content = "Descrição do Item";
        }
    }
}
