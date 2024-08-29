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
using ImageMagick;

namespace CamScan.Class
{
    class ConverToPdf
    {
        private List<string> ImagesHEIC {  get; set; }
        public List<string> ImagesJPEG { get; set; }

        ConvertImageMagick convertImageMagick = new ConvertImageMagick();
        private PdfDocument PDFDocument {  get;  set; }

        public ConverToPdf()
        {
            PDFDocument = new PdfDocument();
            ImagesJPEG = new List<string>();
        }

        public void AddListHEIC(List<string> Scannedlist)
        {
            foreach (var image in Scannedlist)
            {
                var imageInJPEG = convertImageMagick.ConvertBitmapToJpeg(image);
                ImagesJPEG.Add(imageInJPEG);
            }
        }

        public void ConvertListHEICinDocumetPdf()
        {
            foreach (var image in ImagesHEIC)
            {
                using (MagickImage magickImage = new MagickImage(image))
                {
                    PdfPage pdfPage = PDFDocument.AddPage();
                    pdfPage.Size = PdfSharp.PageSize.A4;

                    using(MemoryStream ms = new MemoryStream())
                    {
                        magickImage.Write(ms);
                        ms.Position = 0;

                        XImage xImage = XImage.FromStream(ms);

                        double imageRatio = (double)xImage.PixelWidth / xImage.PixelHeight;
                        double pageWidth = pdfPage.Width;
                        double pageHeight = pdfPage.Height;
                        double pageRatio = pageWidth / pageHeight;

                        double drawWidth, drawHeight;

                        if (imageRatio > pageRatio)
                        {
                            drawWidth = pageWidth;
                            drawHeight = drawWidth / imageRatio;
                        }
                        else
                        {
                            drawHeight = pageHeight;
                            drawWidth = drawHeight * imageRatio;
                        }

                        // Centraliza a imagem na página
                        double xOffset = (pageWidth - drawWidth) / 2;
                        double yOffset = (pageHeight - drawHeight) / 2;

                        // Desenha a imagem na página PDF, centralizada
                        using (XGraphics gfx = XGraphics.FromPdfPage(pdfPage))
                        {
                            gfx.DrawImage(xImage, xOffset, yOffset, drawWidth, drawHeight);
                        }
                    }
                }
            }
        }

        public void ConvertListJPEGinDocumetPdf()
        {
            foreach (var image in ImagesJPEG)
            {
                using (MagickImage magickImage = new MagickImage(image))
                {
                    PdfPage pdfPage = PDFDocument.AddPage();
                    pdfPage.Size = PdfSharp.PageSize.A4;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        magickImage.Write(ms);
                        ms.Position = 0;

                        XImage xImage = XImage.FromStream(ms);

                        double imageRatio = (double)xImage.PixelWidth / xImage.PixelHeight;
                        double pageWidth = pdfPage.Width;
                        double pageHeight = pdfPage.Height;
                        double pageRatio = pageWidth / pageHeight;

                        double drawWidth, drawHeight;

                        if (imageRatio > pageRatio)
                        {
                            drawWidth = pageWidth;
                            drawHeight = drawWidth / imageRatio;
                        }
                        else
                        {
                            drawHeight = pageHeight;
                            drawWidth = drawHeight * imageRatio;
                        }

                        // Centraliza a imagem na página
                        double xOffset = (pageWidth - drawWidth) / 2;
                        double yOffset = (pageHeight - drawHeight) / 2;

                        // Desenha a imagem na página PDF, centralizada
                        using (XGraphics gfx = XGraphics.FromPdfPage(pdfPage))
                        {
                            gfx.DrawImage(xImage, xOffset, yOffset, drawWidth, drawHeight);
                        }
                    }
                }
            }
        }

        private void ConvertListBitmapImageinDocumetPdf(List<BitmapImage> ScannedImages)
        {
            
            foreach (var image in ScannedImages)
            {
                PdfPage pdfPage = PDFDocument.AddPage();
                pdfPage.Size = PdfSharp.PageSize.A4;

                using (MemoryStream ms = new MemoryStream())
                {
                    BitmapEncoder encoder = new BmpBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(image));
                    encoder.Save(ms);
                    ms.Position = 0;

                    XImage xImage = XImage.FromStream(ms);

                    double imageRatio = (double)xImage.PixelWidth / xImage.PixelHeight;
                    double pageWidth = pdfPage.Width;
                    double pageHeight = pdfPage.Height;
                    double pageRatio = pageWidth / pageHeight;

                    double drawWidth, drawHeight;

                    if (imageRatio > pageRatio)
                    {
                        drawWidth = pageWidth;
                        drawHeight = drawWidth / imageRatio;
                    }
                    else
                    {
                        drawHeight = pageHeight;
                        drawWidth = drawHeight * imageRatio;
                    }

                    // Centraliza a imagem na página
                    double xOffset = (pageWidth - drawWidth) / 2;
                    double yOffset = (pageHeight - drawHeight) / 2;

                    // Desenha a imagem na página PDF, centralizada
                    using (XGraphics gfx = XGraphics.FromPdfPage(pdfPage))
                    {
                        gfx.DrawImage(xImage, xOffset, yOffset, drawWidth, drawHeight);
                    }
                }
            }
        }
        public void SaveDocumentinPdf(string LocalSave)
        {
            PDFDocument.Save(LocalSave);
        }
    }
}
