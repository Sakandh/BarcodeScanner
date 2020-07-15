using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spire.Barcode;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using ZXing;

namespace PDFReader
{
    class Program
    {
        static void Main(string[] args)
        {
            var pdfFile = @"C:\BarcodeTest\BCMerged.pdf";
            var pngFile = @"C:\BarcodeTest\TLGQRtest2-1.png";

            //ReadBarCode(pdfFile);
            //ReadQRCode(pdfFile);
            ReadCode(pdfFile);
            //QRreader(pdfFile);
            Console.ReadLine();

        }

        public static void ReadBarCode(string pdf)
        {
            PdfDocument doc = new PdfDocument();
            doc.LoadFromFile(pdf);
            Bitmap image = (Bitmap)doc.SaveAsImage(0);
            string[] datas = BarcodeScanner.Scan(image);

            Console.WriteLine(datas[0]);
        }

        public static void ReadQRCode(string pdf)
        {
            PdfDocument doc = new PdfDocument();
            doc.LoadFromFile(pdf);
            Bitmap image = (Bitmap)doc.SaveAsImage(0);
            string[] datas = BarcodeScanner.Scan(image, BarCodeType.QRCode);

            Console.WriteLine(datas[0]);
        }

        public static void QRreader(string pdf)
        {
            // save the pdf in a larger size (2245px x 3179px)
            PdfDocument doc = new PdfDocument();
            doc.LoadFromFile(pdf);
            PdfDocument newPDF = new PdfDocument();
            foreach (PdfPageBase page in doc.Pages)
            {
                PdfPageBase newPage = newPDF.Pages.Add(PdfPageSize.A1, new PdfMargins(0));
                PdfTextLayout loLayout = new PdfTextLayout();
                loLayout.Layout = PdfLayoutType.OnePage;
                page.CreateTemplate().Draw(newPage, new PointF(0, 0), loLayout);
            }

            // temporary pdf file path
            string fileNameOnly = Path.GetFileNameWithoutExtension(pdf);
            string extension = ".pdf";

            string tempPDF = Path.GetTempPath() + fileNameOnly + extension;
            newPDF.SaveToFile(tempPDF);
            doc.LoadFromFile(tempPDF);

            // converting pdf to image
            Bitmap image = (Bitmap)doc.SaveAsImage(0);

            // reading qr code from the new larger pdf
            var qrcodeReader = new BarcodeReader();
            var qrcodeResult = qrcodeReader.Decode(image);

            if (qrcodeResult == null)
            {
                Console.WriteLine("Barcode/QR code not compatible");
            }
            else
            {
                Console.WriteLine("Decode barcode text: " + qrcodeResult);
                Console.WriteLine("QR Code Successful");
            }
        }

        public static void ReadCode(string filePath)
        {
            if (filePath == null)
                Console.WriteLine("No file");

            PdfDocument pdfDocument = new PdfDocument();

            if (File.Exists(filePath))
            {
                // opens the pdf and returns the barcode value
                pdfDocument.LoadFromFile(filePath);
                Bitmap image = (Bitmap)pdfDocument.SaveAsImage(0);
                string[] barcodeData = BarcodeScanner.Scan(image, BarCodeType.Code39Extended);

                // if there's no barcode then it returns the no barcode message
                if (barcodeData.Length < 1)
                {
                    QRreader(filePath);
                }
                else
                {
                    Console.WriteLine(barcodeData[0]);
                }
            }
            else
            {
                throw new FileNotFoundException("File does not exist: " + filePath);
            }
        }
    }
}
