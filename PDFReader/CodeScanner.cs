using Spire.Barcode;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;

namespace PDFReader
{
    class CodeScanner
    {     
        public void FileLooper(DirectoryInfo dir)
        {           
            foreach (var file in dir.GetFiles("*.pdf"))
            {
                ReadCode(file.FullName, file.Name);

                Console.WriteLine();
            }
        }

        // scans pdf for both barcode and QR code
        public void ReadCode(string filePath, string fileName)
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
                    QRreader(filePath, fileName);
                }
                else
                {
                    Console.WriteLine(fileName + ":");
                    Console.WriteLine("     Barcode: " + barcodeData[0]);
                }
            }
            else
            {
                throw new FileNotFoundException("File does not exist: " + filePath);
            }
        }

        public void QRreader(string filepath, string fileName)
        {
            // save the pdf in a larger size (2245px x 3179px)
            PdfDocument doc = new PdfDocument();
            doc.LoadFromFile(filepath);
            PdfDocument newPDF = new PdfDocument();
            foreach (PdfPageBase page in doc.Pages)
            {
                PdfPageBase newPage = newPDF.Pages.Add(PdfPageSize.A1, new PdfMargins(0));
                PdfTextLayout loLayout = new PdfTextLayout();
                loLayout.Layout = PdfLayoutType.OnePage;
                page.CreateTemplate().Draw(newPage, new PointF(0, 0), loLayout);
            }

            // temporary pdf file path
            string fileNameOnly = Path.GetFileNameWithoutExtension(filepath);
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
                Console.WriteLine(fileName + ":");
                Console.WriteLine("     Barcode/QR code not compatible");
            }
            else
            {
                Console.WriteLine(fileName + ":");
                Console.WriteLine("     QRcode: " + qrcodeResult);
            }
        }

        public void ReadBarCode(string pdf)
        {
            PdfDocument doc = new PdfDocument();
            doc.LoadFromFile(pdf);
            Bitmap image = (Bitmap)doc.SaveAsImage(0);
            string[] datas = BarcodeScanner.Scan(image);

            Console.WriteLine(datas[0]);
        }

        public void ReadQRCode(string pdf)
        {
            PdfDocument doc = new PdfDocument();
            doc.LoadFromFile(pdf);
            Bitmap image = (Bitmap)doc.SaveAsImage(0);
            string[] datas = BarcodeScanner.Scan(image, BarCodeType.QRCode);

            Console.WriteLine(datas[0]);
        }
    }
}
