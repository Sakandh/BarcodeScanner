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
            CodeScanner codeScanner = new CodeScanner();
            string fileDirectory = @"C:\BarcodeTest\Loop";

            DirectoryInfo dir = new DirectoryInfo(fileDirectory);
            int totalFiles = dir.GetFiles().Length;            

            //Console.ReadLine();

            FileStream ostrm;
            StreamWriter writer;
            TextWriter oldOut = Console.Out;
            try
            {
                ostrm = new FileStream(fileDirectory + @"\ScannedResults\Results.txt", FileMode.OpenOrCreate, FileAccess.Write);
                writer = new StreamWriter(ostrm);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot open Redirect.txt for writing");
                Console.WriteLine(e.Message);
                return;
            }
            Console.SetOut(writer);

            codeScanner.FileLooper(dir);

            Console.WriteLine("-------------------------------");
            Console.WriteLine("       Total files: " + totalFiles);
            Console.WriteLine("-------------------------------");

            Console.SetOut(oldOut);
            writer.Close();
            ostrm.Close();
        }        
    }
}
