using System.Drawing;

using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;

using Helpers;
using static Helpers.ImageExtensions;


namespace EditPDF;

public static class PdfDocumentExtensions
{
    public static PdfDocument GetReadOnlyPdfDocument(
        string sourceFileFullPath
    )
        => new(new PdfReader(sourceFileFullPath));

    public static PdfDocument GetReadOnlyPdfDocument(
        string sourceFolderPath,
        string sourceFileName
    )
        => GetReadOnlyPdfDocument(Path.Combine(sourceFolderPath, sourceFileName));

    public static PdfDocument GetReadWritePdfDocument(
        string sourceFileFullPath,
        string destinationFileFullPath
    )
        => new(
            new PdfReader(sourceFileFullPath),
            new PdfWriter(destinationFileFullPath)
        );
    public static PdfDocument GetReadWritePdfDocument(
        string sourceAndDestinationFolderPath,
        string sourceFileName,
        string destinationFileName
    )
        => GetReadWritePdfDocument(
            Path.Combine(sourceAndDestinationFolderPath, sourceFileName),
            Path.Combine(sourceAndDestinationFolderPath, destinationFileName)
        );

    public static PdfDocument GetWriteOnlyPdfDocument(
        string destinationFileFullPath
    )
        => new(new PdfWriter(destinationFileFullPath));
    public static PdfDocument GetWriteOnlyPdfDocument(
        string destinationFolderPath,
        string destinationFileName
    )
        => GetWriteOnlyPdfDocument(Path.Combine(destinationFolderPath, destinationFileName));

    public static Document GetWriteOnlyDocument(
        string destinationFileFullPath
    )
        => new(GetWriteOnlyPdfDocument(destinationFileFullPath));
    public static Document GetWriteOnlyDocument(
        string destinationFolderPath,
        string destinationFileName
    )
        => GetWriteOnlyDocument(Path.Combine(destinationFolderPath, destinationFileName));


    public static IEnumerable<PdfPage> GetPages(this PdfDocument document)
    {
        for (int iPage = 1; iPage <= document.GetNumberOfPages(); iPage++)
            yield return document.GetPage(iPage);
    }


    #region ResizeImages
    public static void ResizeImages(
        this PdfDocument pdfDoc,
        double resizeFactor
    )
    {
        // Iterate over all pages to get all images.
        foreach (var (item, position) in pdfDoc.GetPages().AppendOrdinal())
        {
            var xObjects = item.GetPdfObject().GetAsDictionary(PdfName.Resources).GetAsDictionary(PdfName.XObject);

            // Get images
            foreach (var iKey in xObjects.KeySet().ToList())
            {
                // Get the original image
                var image = new PdfImageXObject(xObjects.GetAsStream(iKey));

                // Generate the resized image
                var resizedImage = image.ToGeneralImage().Scale(resizeFactor);
                if (false) resizedImage.RotateFlip(RotateFlipType.Rotate180FlipNone);

                // Replace the original image with the resized image
                xObjects.PutImage(iKey, resizedImage);
            }
        }
    }
    public static void ResizeImages(
        string sourceAndDestinationFolderPath,
        string sourceFileName,
        string destinationFileName,
        double resizeFactor
    )
    {
        var pdfDoc = GetReadWritePdfDocument(sourceAndDestinationFolderPath, sourceFileName, destinationFileName);
        pdfDoc.ResizeImages(resizeFactor);
        pdfDoc.Close();
    }
    public static void ResizeImages(
        string sourceFileFullPath,
        double resizeFactor
    )
    {
        var sourceAndDestinationFolderPath = new FileInfo(sourceFileFullPath).Directory.FullName;
        var sourceFileName = new FileInfo(sourceFileFullPath).Name;
        var pdfDoc = GetReadWritePdfDocument(sourceAndDestinationFolderPath, sourceFileName, sourceFileName.Replace(".pdf", ".resized.pdf"));
        pdfDoc.ResizeImages(resizeFactor);
        pdfDoc.Close();
    }
    #endregion


    #region DumpImages
    public static void DumpImages(
        this PdfDocument pdfDoc,
        string outputFolderPath
    )
    {
        // Iterate over all pages to get all images.
        foreach (var page in pdfDoc.GetPages().AppendOrdinal())
        {
            PdfDictionary xObjects = page.item.GetPdfObject().GetAsDictionary(PdfName.Resources).GetAsDictionary(PdfName.XObject);

            // Get images
            foreach (var iKey in xObjects.KeySet().ToList())
            {
                // Get image
                var image = new PdfImageXObject(xObjects.GetAsStream(iKey));

                image.GetImageBytes().WriteToFile(Path.Combine(outputFolderPath, $"p{page.position + 1}_{iKey.GetValue()}.{image.IdentifyImageFileExtension()}"));
            }
        }
    }
    public static void DumpImages(
        string sourceAndDestinationFolderPath,
        string sourceFileName
    )
    {
        var pdfDoc = GetReadOnlyPdfDocument(sourceAndDestinationFolderPath, sourceFileName);
        pdfDoc.DumpImages(sourceAndDestinationFolderPath);
        pdfDoc.Close();
    }
    #endregion


    #region CreateDocumentFromImage
    public static void CreateDocumentsFromImages(
        string sourceAndDestinationFolderPath,
        string searchPattern
    )
    {
        foreach (var iSourceFilePath in Directory.EnumerateFiles(sourceAndDestinationFolderPath, searchPattern))
            CreateDocumentFromImage(sourceAndDestinationFolderPath, Path.GetFileName(iSourceFilePath));
    }
    public static void CreateDocumentFromImage(
        string sourceAndDestinationFolderPath,
        string sourceFileName
    )
    {
        var sourceImageFileFullPath = Path.Combine(sourceAndDestinationFolderPath, sourceFileName);
        var destinationFileFullPath = sourceImageFileFullPath.Replace(Path.GetExtension(sourceImageFileFullPath), ".pdf");

        CreateDocumentFromImage(sourceImageFileFullPath, destinationFileFullPath, RotationAngle.None);
    }
    public static void CreateDocumentFromImage(
        string sourceImageFileFullPath,
        string destinationFileFullPath,
        RotationAngle Angle
    )
    {
        CreateDocumentFromImage(sourceImageFileFullPath, destinationFileFullPath, Angle.ToRadians());
    }
    public static void CreateDocumentFromImage(
        string sourceImageFileFullPath,
        string destinationFileFullPath,
        double? radAngle = null
    )
    {
        var image = PdfImageExtensions.GetImage(sourceImageFileFullPath);
        if (radAngle.HasValue) image.SetRotationAngle(-radAngle.Value);

        CreateDocumentFromImage(image, destinationFileFullPath);
    }
    public static void CreateDocumentFromImage(
        iText.Layout.Element.Image image,
        string destinationFileFullPath
    )
    {
        // Creating a Document
        var document = GetWriteOnlyDocument(destinationFileFullPath);

        // Adding image to the document
        document.Add(image);

        // Closing the document
        document.Close();
    }
    #endregion


    #region DumpText
    public static IEnumerable<string> DumpText(
        string sourceFolderPath,
        string sourceFileName
    )
        => DumpText(Path.Combine(sourceFolderPath, sourceFileName));

    public static IEnumerable<string> DumpText(
        string sourceFilePath
    )
    {
        var pdfDoc = GetReadOnlyPdfDocument(sourceFilePath);
        var result = pdfDoc.DumpText().ToArray();
        pdfDoc.Close();
        return result;
    }

    public static IEnumerable<string> DumpText(this PdfDocument pdfDoc)
    {
        var strategy = new SimpleTextExtractionStrategy();

        foreach (var iPage in pdfDoc.GetPages())
            yield return PdfTextExtractor.GetTextFromPage(iPage, strategy);
    }
    #endregion


    #region ExtractPages
    public static void ExtractPages(
        string sourceAndDestinationFolderPath,
        string sourceFileName,
        params IEnumerable<(string destinationFileName, IEnumerable<int> pageNumbers)> destinationPairs
    )
    {
        foreach (var (destinationFileName, pageNumbers) in destinationPairs)
            ExtractPages(
                sourceAndDestinationFolderPath,
                sourceFileName,
                destinationFileName,
                pageNumbers
            );
    }

    public static void ExtractPages(
        string sourceAndDestinationFolderPath,
        string sourceFileName,
        string destinationFileName,
        params IEnumerable<int> pageNumbers
    )
        => ExtractPages(
            Path.Combine(sourceAndDestinationFolderPath, sourceFileName),
            Path.Combine(sourceAndDestinationFolderPath, destinationFileName ?? sourceFileName.Replace(".pdf", ".extracted.pdf")),
            pageNumbers
        );

    public static void ExtractPages(
        string sourceFileFullPath,
        string destinationFileFullPath,
        params IEnumerable<int> pageNumbers
    )
    {
        var sourceDoc = GetReadOnlyPdfDocument(sourceFileFullPath);
        var destinationDoc = GetWriteOnlyPdfDocument(destinationFileFullPath);

        sourceDoc.CopyPagesTo(pageNumbers.ToList(), destinationDoc);

        destinationDoc.Close();
        sourceDoc.Close();
    }
    #endregion


    #region RotatePages
    public static void RotatePages(
        string sourceAndDestinationFolderPath,
        string sourceFileName,
        string destinationFileName,
        int angle,
        params IEnumerable<int> pageNumbers
    )
        => RotatePages(
            Path.Combine(sourceAndDestinationFolderPath, sourceFileName),
            Path.Combine(sourceAndDestinationFolderPath, destinationFileName),
            angle,
            pageNumbers
        );

    public static void RotatePages(
        string sourceFileFullPath,
        string destinationFileFullPath,
        int angle,
        params IEnumerable<int> pageNumbers
    )
    {
        var pdfDoc = GetReadWritePdfDocument(sourceFileFullPath, destinationFileFullPath);

        foreach (int p in pageNumbers)
        {
            var page = pdfDoc.GetPage(p);

            int rotate = page.GetRotation();
            page.SetRotation(rotate == 0 ? angle : (rotate + angle) % 360);
        }

        pdfDoc.Close();
    }
    #endregion
}
