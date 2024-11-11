using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace CamScan.Class
{
    class SaveFolderScan
    {
        //Create Files in JPG
        public void CreateFileJPG(string bmpFilePath, string FileName, string Folder)
        {
            using(Bitmap bitmap = new Bitmap(bmpFilePath))
            {
                BitmapSource bitmapSource = ConvertBitmapToBitmapSource(bitmap);

                BitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                string jpgFilePath = System.IO.Path.Combine(Folder, FileName + ".jpg");
                int fileIndex = 1;

                while (File.Exists(jpgFilePath))
                {
                    string tempFileName = $"{FileName}({fileIndex}).jpg";
                    jpgFilePath = System.IO.Path.Combine(Folder, tempFileName);
                    fileIndex++;
                }
                using (var fileStream = new FileStream(jpgFilePath, FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
            }
            
        }

        public void CreateFilePDF(List<string> ScannedList, string Folder, string FileName)
        {
            ConverToPdf converToPdf = new ConverToPdf();
            converToPdf.AddListHEIC(ScannedList);
            converToPdf.ConvertListJPEGinDocumetPdf();

            string FileNameConcat = FileName + ".pdf";
            string pdfFilePath = System.IO.Path.Combine(Folder, FileNameConcat);

            int fileIndex = 1;
            while (File.Exists(pdfFilePath))
            {
                string tempFileName = $"{FileName}({fileIndex}).pdf";
                pdfFilePath = System.IO.Path.Combine(Folder, tempFileName);
                fileIndex++;
            }

            converToPdf.SaveDocumentinPdf(pdfFilePath);
        }

        public void SaveDocumentoCliente(List<string> ScannedList, string FolderDocumentoCliente, string CodigoCliente)
        {
            string bmpFilePath = ScannedList[0];

            CreateFileJPG(bmpFilePath, CodigoCliente, FolderDocumentoCliente);

        }
        public void SaveDespesas(List<string> ScannedList, string FolderDespesas, string FileName, string TypeFile)
        {
            BitmapEncoder encoder = new JpegBitmapEncoder();
            string bmpFilePath = ScannedList[0];
            if(TypeFile == "JPG")
            {
                CreateFileJPG(bmpFilePath, FileName, FolderDespesas);
            }
            else if(TypeFile == "PDF")
            {
                CreateFilePDF(ScannedList, FolderDespesas, FileName);
            }
            
        }
        public void SaveOutros(List<string> ScannedList, string FolderOutros, string FileName, string TypeFile)
        {
            BitmapEncoder encoder = new JpegBitmapEncoder();
            string bmpFilePath = ScannedList[0];
            if (TypeFile == "JPG")
            {
                CreateFileJPG(bmpFilePath, FileName, FolderOutros);
            }
            else if (TypeFile == "PDF")
            {
                CreateFilePDF(ScannedList, FileName, FolderOutros);
            }
        }



        //Create Files in PDF
        public void SaveConfissaoDivida(List<string> ScannedList, string CodigoCliente, string FolderConfissaoDivida)
        {
            CreateFilePDF(ScannedList, FolderConfissaoDivida, CodigoCliente);
        }

        private BitmapSource ConvertBitmapToBitmapSource(Bitmap bitmap)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
                memoryStream.Position = 0;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze(); // To make the BitmapImage cross-thread accessible

                return bitmapImage;
            }
        }
    }
}
