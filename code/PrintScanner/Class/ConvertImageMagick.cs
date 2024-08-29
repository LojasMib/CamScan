using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;
using System.Drawing;
using System.IO;

namespace CamScan.Class
{
    internal class ConvertImageMagick
    {
        
        private string heicFolderPath {  get; set; }

        public ConvertImageMagick() => CreateTempPathJpeg();
        public void CreateTempPathHeic()
        {
            string tempFolderPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "ScamCam");

            if (!Directory.Exists(tempFolderPath))
            {
                Directory.CreateDirectory(tempFolderPath);
            }
            string heicFolderPath = System.IO.Path.Combine(tempFolderPath, "HEIC");
            if (!Directory.Exists(heicFolderPath))
            {
                Directory.CreateDirectory(heicFolderPath);
            }

            this.heicFolderPath = heicFolderPath;

        }
        public void CreateTempPathJpeg()
        {
            string tempFolderPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "ScamCam");

            if (!Directory.Exists(tempFolderPath))
            {
                Directory.CreateDirectory(tempFolderPath);
            }
            string heicFolderPath = System.IO.Path.Combine(tempFolderPath, "JPEG");
            if (!Directory.Exists(heicFolderPath))
            {
                Directory.CreateDirectory(heicFolderPath);
            }

            this.heicFolderPath = heicFolderPath;

        }

        public string ConvertBitmapToJpeg(string bitmap)
        {
            using (MagickImage magickImage = new MagickImage(bitmap))
            {
                magickImage.Format = MagickFormat.Jpeg;
                magickImage.Quality = 85;

                string jpegFileTemp = System.IO.Path.Combine(heicFolderPath, Guid.NewGuid().ToString() + ".jpg");

                magickImage.Write(jpegFileTemp);

                return (jpegFileTemp);
            }
        }

        public string ConvertBitmapToHeif(string bitmap)
        {
            using (MagickImage magickImage = new MagickImage(bitmap))
            {
                magickImage.Format = MagickFormat.Heic;
                magickImage.Quality = 85;

                string heicFileTemp = System.IO.Path.Combine(heicFolderPath, Guid.NewGuid().ToString() + ".heic");

                magickImage.Write(heicFileTemp);

                return (heicFileTemp);
            }
        }
    }
}
