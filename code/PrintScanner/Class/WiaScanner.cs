using CamScan.Pages;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WIA;
using WpfSystem = System.Windows;

namespace CamScan.Class
{
    

    internal class WiaScanner
    {
        private const int DPI = 200;
        ConverToPdf convertToPdf = new ConverToPdf();

        private const string WiaFormatJPEG = "{B96B3CAF-0728-11D3-9D7B-0000F81EF32E}"; //GUID (Globally Unique Identifier). Especifiva o formato JPEG
        private const string WiaFormatBMP = "{B96B3CAB-0728-11D3-9D7B-0000F81EF32E}";
        public Item SelectedScan { get; set; }
        private DeviceManager _deviceManager;
        public ImageFile _imageFile;

        public WiaScanner()
        {
            _deviceManager = new DeviceManager();
            _imageFile = new ImageFile();
        }

        ///<summary>
        /// Lista os scanners WIA conectados ao sistema
        ///</summary>
        ///<returns>Lista dos nomes de scanners conectados.</returns>
        
        public List<string> ListScanners()
        {

            List<string> deviceNames = new List<string>();
            foreach(DeviceInfo info in _deviceManager.DeviceInfos)
            {
                if(info.Type == WiaDeviceType.ScannerDeviceType)
                {
                    deviceNames.Add(info.Properties["Name"].get_Value().ToString());
                }
            }
            return deviceNames;
        }

        public void Scan()
        {
            
            if (SelectedScan == null)
            {
                WpfSystem.MessageBox.Show("Scanner não encontrado");
                return;
            }

            SetScannerSettings(SelectedScan);

            _imageFile = (ImageFile)SelectedScan.Transfer(WiaFormatBMP);

        }

        public string? CreateTempPath()
        {
            try
            {
                string tempFolderPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "ScamCam");

                if (!Directory.Exists(tempFolderPath))
                {
                    Directory.CreateDirectory(tempFolderPath);
                }
                string tempFilePath = System.IO.Path.Combine(tempFolderPath, Guid.NewGuid().ToString() + ".bmp");

                return tempFilePath;
            }
            catch(Exception ex)
            {
                WpfSystem.MessageBox.Show($"Erro na criacao de pastas temporarias WIA {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public void DeleteTempFolderPath()
        {
            string tempFolderPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "ScamCam");

            if (Directory.Exists(tempFolderPath))
            {
                Directory.Delete(tempFolderPath);
            }
        }
        //public void SaveJpegAsPdf(string jpegPath)
        //{
        //    using (PdfDocument document = new PdfDocument())
        //    {
        //        PdfPage pdfPage = document.AddPage();
        //        pdfPage.Size = PdfSharp.PageSize.A4;

        //        using (XGraphics gfx = XGraphics.FromPdfPage(pdfPage))
        //        {
        //            using (XImage xImage = XImage.FromFile(jpegPath))
        //            {
        //                double imageRatio = (double)xImage.PixelWidth / xImage.PixelHeight;
        //                double pageWidth = pdfPage.Width;
        //                double pageHeight = pdfPage.Height;
        //                double pageRatio = pageWidth / pageHeight;

        //                double drawWidth, drawHeight;

        //                if (imageRatio > pageRatio)
        //                {
        //                    drawWidth = pageWidth;
        //                    drawHeight = drawWidth / imageRatio;
        //                }
        //                else
        //                {
        //                    drawHeight = pageHeight;
        //                    drawWidth = drawHeight * imageRatio;
        //                }

        //                // Centraliza a imagem na página
        //                double xOffset = (pageWidth - drawWidth) / 2;
        //                double yOffset = (pageHeight - drawHeight) / 2;

        //                gfx.DrawImage(xImage, xOffset, yOffset, drawWidth, drawHeight);
        //            }
        //        }
        //    }
        //}

        //public void SaveJpeg(string path, Bitmap bmp, int quality)
        //{
        //    ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

        //    System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
        //    EncoderParameters myEncoderParameters = new EncoderParameters(1);

        //    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, quality);
        //}

        //private ImageCodecInfo? GetEncoder(ImageFormat format)
        //{
        //    ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
        //    foreach (ImageCodecInfo codec in codecs)
        //    {
        //        if (codec.FormatID == format.Guid)
        //        {
        //            return codec;
        //        }
        //    }
        //    return null;
        //}
        public Device? ConnectScan(string scannerName)
        {
            DeviceInfo deviceInfo = FindDeviceByName(scannerName);
            if (deviceInfo == null)
            {
                WpfSystem.MessageBox.Show("Scanner não encontrado");
                return null;
            }
            try
            {
                Device device = deviceInfo.Connect();
                return device;
            }
            catch (Exception ex)
            {
                WpfSystem.MessageBox.Show($"Não foi possível conectar no Scanner, confira as pastas para salvamento e a conexão com o dispositivo: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private DeviceInfo? FindDeviceByName(string scannerName)
        {
            foreach(DeviceInfo deviceInfo in _deviceManager.DeviceInfos)
            {
                if (deviceInfo.Properties["Name"].get_Value().ToString().Equals(scannerName, StringComparison.OrdinalIgnoreCase))
                {
                    return deviceInfo;
                }
            }
            return null;
        }

        private void SetScannerSettings(Item scannerItem)
        {
            try{
                // Configurações típicas que podem ser ajustadas
                SetItemProperty(scannerItem.Properties, "6147", DPI); // DPI Horizontal
                SetItemProperty(scannerItem.Properties, "6148", DPI); // DPI Vertical
                SetItemProperty(scannerItem.Properties, "6149", 0);   // Horizontal Start Position
                SetItemProperty(scannerItem.Properties, "6150", 0);   // Vertical Start Position
                SetItemProperty(scannerItem.Properties, "6151", (int)(8.5 * DPI)); // Horizontal Extent
                SetItemProperty(scannerItem.Properties, "6152", (int)(11.7 * DPI)); // Vertical Extent
                SetItemProperty(scannerItem.Properties, "6154", 1);    // Bits per Pixel (1 - Black & White, 8 - Grayscale, 24 - Color)

            }
            catch
            {
                foreach (Property prop in scannerItem.Properties)
                {
                    WpfSystem.MessageBox.Show($" Erro nas configurações do Scanner, Propriedades do Scanner {prop.Name}: {prop.get_Value()}");
                }
            }
            
        }

        private void SetItemProperty(IProperties properties, object propertyId, object value)
        {
            Property property = properties.get_Item(ref propertyId);
            property.set_Value(ref value);
        }

        public void SaveScanned(ImageFile image, string outputFilePath)
        {
            if (_imageFile == null)
            {
                image.SaveFile(outputFilePath);
            }

        }

    }
}
