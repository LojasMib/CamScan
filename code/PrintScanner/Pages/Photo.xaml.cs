using PrintScanner.Router;
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
using System.IO;
using System.Windows.Threading;

namespace PrintScanner
{
    public partial class Photo : Page
    {
        private FilterInfoCollection? videoDevices;
        private VideoCaptureDevice? videoSource;
        private Bitmap? latestFrame;
        private DispatcherTimer? timer;
        private readonly object frameLock = new object();
        
        public Photo()
        {
            InitializeComponent();
            Loaded += PhotoPage_Loaded;
            Unloaded += PhotoPage_Unloaded;
        }

        private void PhotoPage_Loaded(object sender, RoutedEventArgs e)
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count == 0)
            {
                MessageBox.Show("No video source found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
            videoSource.Start();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(30); // Adjust the interval as needed
            timer.Tick += Timer_Tick;
            timer.Start();

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
                    return bitmapImage;
                }
            }
            
        }

        private void PhotoPage_Unloaded(object sender, EventArgs e)
        {
            if(videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
                videoSource.NewFrame -= video_NewFrame;
            }
            if (timer != null)
            {
                timer.Stop();
                timer.Tick -= Timer_Tick;
            }
            if(latestFrame != null)
            {
                latestFrame.Dispose();
                latestFrame = null;
            }
        }
        public void StopCamera()
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
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
            if(timer != null)
            {
                timer.Stop();
            }
        }
    }
}
