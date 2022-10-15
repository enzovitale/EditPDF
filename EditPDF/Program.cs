using System;
using System.Collections.Generic;
using System.IO;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

using static EditPDF.PdfDocumentExtensions;

using static Helpers.ImageExtensions;


namespace EditPDF;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello World!");

        ResizeImagesInDocument();
        if (false) DumpImagesFromDocument();
        if (false) CreateNewDocument();
        if (false) DumpTextFromDocument();
        if (false) ExtractPagesFromDocument();
        if (false) RotatePagesFromDocument();
        if (false) MergeDocuments();
        if (false) MergeEvenAndOddPages();
        if (false) CreateDocumentFromImage();
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


    public static void ResizeImagesInDocument()
    {
        var FolderPath = @"D:\personal\";

        ResizeImages(
            Path.Combine(FolderPath, "myFile.pdf"),
            1.0
        );
    }


    public static void DumpImagesFromDocument()
    {
        DumpImages(
            @"D:\personal\",
            "myFile.pdf"
        );
    }


    public static void DumpTextFromDocument()
    {
        string filePath = @"D:\personal\";

        var result = DumpText(filePath);

        //result.WriteToFile();
    }


    public static void ExtractPagesFromDocument()
    {
        var SourceAndDestinationFolderPath = @"D:\personal\";
        string sourceFilePath = @"myFile.pdf";
        string destinationFilePath = @"myFile.pdf";

        ExtractPages(SourceAndDestinationFolderPath, sourceFilePath, destinationFilePath, 12);
    }


    public static void RotatePagesFromDocument()
    {
        var SourceAndDestinationFolderPath = @"D:\personal\";
        string sourceFilePath = @"myFile.pdf";
        string destinationFilePath = sourceFilePath.Replace(".pdf", " - rotated.pdf");

        RotatePages(SourceAndDestinationFolderPath, sourceFilePath, destinationFilePath, RotationAngle.HundredEightyDegrees.ToDegrees(), 1);
    }


    public static void MergeDocuments()
    {
        var SourceAndDestinationFolderPath = @"D:\personal\";

        PdfMergerExtensions.MergeDocumentsInFolder(
            SourceAndDestinationFolderPath,

            "myFile.pdf",

            "myFile.pdf",
            "myFile.pdf"
        );
    }
    public static void MergeEvenAndOddPages()
    {
        const string SourceAndDestinationFolderPath = @"D:\personal\";
        const string SourceFileName1 = @"odd pages.pdf";
        const string SourceFileName2 = @"even pages.pdf";
        const int NumberOfPages = 10;

        var SourceFileNameAndPagePairs = new List<(string, int)>();
        for (int i = 1; i <= NumberOfPages; i++)
        {
            SourceFileNameAndPagePairs.Add((SourceFileName1, i));
            SourceFileNameAndPagePairs.Add((SourceFileName2, i));
        }

        PdfMergerExtensions.MergeDocumentsInFolder(
            SourceAndDestinationFolderPath,

            "dest.pdf",

            SourceFileNameAndPagePairs.ToArray()
        );
    }


    public static void CreateDocumentFromImage()
    {
        var SourceAndDestinationFolderPath = @"D:\personal\";

        var imageFile = Path.Combine(SourceAndDestinationFolderPath, "myFile.jpg");
        var dest = imageFile.Replace(Path.GetExtension(imageFile), ".pdf");

        PdfDocumentExtensions.CreateDocumentFromImage(imageFile, dest, RotationAngle.NinetyDegreesCounterClockwise);
    }
}
