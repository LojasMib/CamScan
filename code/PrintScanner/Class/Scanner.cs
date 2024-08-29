using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.IO;

namespace CamScan.Class
{
    internal class Scanner
    {

        
        public Scanner()
        {
            
        }

        public void SelectSource()
        {
            
            
        }

        public void Acquire()
        {
            
            
        }

        private void AcquireCompleted(object sender, EventArgs e)
        {
            //if(e.Image != null)
            //{
            //    using(MemoryStream memoryStream = new MemoryStream())
            //    {
            //        e.Image.Save(memoryStream, ImageFormat.Bmp);
            //        memoryStream.Seek(0, SeekOrigin.Begin);
            //        BitmapImage bitmapImage = new BitmapImage();
            //        bitmapImage.BeginInit();
            //        bitmapImage.StreamSource = memoryStream;
            //        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            //        bitmapImage.EndInit();
            //        ScanCompleted?.Invoke(this, bitmapImage);
            //    }
            //}
        }
    }
}
