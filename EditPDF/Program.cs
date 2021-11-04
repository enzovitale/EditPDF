using System;

using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

using static EditPDF.PdfDocumentExtensions;


namespace EditPDF
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            if (false) MergeDocuments();
            if (false) ResizeImagesInDocument();
            if (false) DumpImagesInDocument();
            if (false) CreateNewDocument();
            if (false) DumpTextInDocument();
            if (false) ExtractPagesFromDocument();
            RotatePagesFromDocument();
        }

        public static void CreateNewDocument()
        {
            // Create a PdfFont
            var font_COURIER_BOLD = PdfFontFactory.CreateFont(StandardFonts.COURIER_BOLD);


            var dest = @"C:\New Folder\mySecondPDF.pdf";
            var writer = new PdfWriter(dest);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            // Add a Paragraph
            document.Add(new Paragraph("iText is:").SetFont(font_COURIER_BOLD));

            List list = new List()
                .SetSymbolIndent(12)
                .SetListSymbol("\u2022")
                .SetFont(font_COURIER_BOLD)

                .Add(new ListItem("Never gonna give you up"))
                .Add(new ListItem("Never gonna let you down"))
                .Add(new ListItem("Never gonna run around and desert you"))
                .Add(new ListItem("Never gonna make you cry"))
                .Add(new ListItem("Never gonna say goodbye"))
                .Add(new ListItem("Never gonna tell a lie and hurt you"));

            // Add the list
            document.Add(list);


            // ------------------------------------------------------------------------------------

            var p = new Paragraph("An old picture: ")
                .Add(new Image(ImageDataFactory.Create(@"C:\New folder\resources\IMG-20191121-WA0001.jpg")));

            document.Add(p);


            // Finally close the document
            document.Close();
        }


        public static void MergeDocuments()
        {
            var SourceAndDestinationFolderPath = @"D:\personal\";

            PdfMergerExtensions.MergeDocumentsInFolder(
                SourceAndDestinationFolderPath,

                "dest.pdf",

                "source003.pdf",
                "source004.pdf",
                "source005.pdf"
            );
        }


        public static void ResizeImagesInDocument()
        {
            var FolderPath = @"D:\personal\";

            ResizeImages(
                FolderPath + "source.pdf",
                0.8
            );
        }


        public static void DumpImagesInDocument()
        {
            DumpImages(
                @"D:\personal\",
                "source.pdf"
            );
        }


        public static void DumpTextInDocument()
        {
            string filePath = @"D:\personal\source.pdf";

            var result = DumpText(filePath);
        }


        public static void ExtractPagesFromDocument()
        {
            var SourceAndDestinationFolderPath = @"D:\personal\";
            string sourceFilePath = @"source.pdf";
            string destinationFilePath = @"dest.pdf";

            ExtractPages(SourceAndDestinationFolderPath, sourceFilePath, destinationFilePath, 2);
        }


        public static void RotatePagesFromDocument()
        {
            var SourceAndDestinationFolderPath = @"D:\personal\";
            string sourceFilePath = @"source.pdf";
            string destinationFilePath = @"source - rotated.pdf";

            RotatePages(SourceAndDestinationFolderPath, sourceFilePath, destinationFilePath, 270, 1, 2);
        }
    }
}
