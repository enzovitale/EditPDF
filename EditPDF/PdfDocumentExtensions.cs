using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

using Helpers;

using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Xobject;


namespace EditPDF
{
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
            foreach (var page in pdfDoc.GetPages().AppendOrdinal())
            {
                var xObjects = page.item.GetPdfObject().GetAsDictionary(PdfName.Resources).GetAsDictionary(PdfName.XObject);

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

        #region DumpText
        public static IEnumerable<string> DumpText(this PdfDocument pdfDoc)
        {
            var strategy = new SimpleTextExtractionStrategy();

            foreach (var iPage in pdfDoc.GetPages())
                yield return PdfTextExtractor.GetTextFromPage(iPage, strategy);
        }
        public static IEnumerable<string> DumpText(
            string sourceFilePath
        )
        {
            var pdfDoc = GetReadOnlyPdfDocument(sourceFilePath);
            var result = pdfDoc.DumpText().ToArray();
            pdfDoc.Close();
            return result;
        }
        public static IEnumerable<string> DumpText(
            string sourceFolderPath,
            string sourceFileName
        )
        {
            return DumpText(Path.Combine(sourceFolderPath, sourceFileName));
        }
        #endregion

        #region ExtractPages
        public static void ExtractPages(
            string sourceAndDestinationFolderPath,
            string sourceFileName,
            string destinationFileName,
            params int[] pageNumbers
        ) => ExtractPages(
            Path.Combine(sourceAndDestinationFolderPath, sourceFileName),
            Path.Combine(sourceAndDestinationFolderPath, destinationFileName),
            pageNumbers
        );
        public static void ExtractPages(
            string sourceAndDestinationFolderPath,
            string sourceFileName,
            string destinationFileName,
            IEnumerable<int> pageNumbers
        ) => ExtractPages(
            Path.Combine(sourceAndDestinationFolderPath, sourceFileName),
            Path.Combine(sourceAndDestinationFolderPath, destinationFileName),
            pageNumbers.AsEnumerable()
        );


        public static void ExtractPages(
            string sourceFileFullPath,
            string destinationFileFullPath,
            params int[] pageNumbers
        ) => ExtractPages(sourceFileFullPath, destinationFileFullPath, pageNumbers.AsEnumerable());

        public static void ExtractPages(
            string sourceFileFullPath,
            string destinationFileFullPath,
            IEnumerable<int> pageNumbers
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
            params int[] pageNumbers
        ) => RotatePages(sourceAndDestinationFolderPath, sourceFileName, destinationFileName, angle, pageNumbers.AsEnumerable());
        public static void RotatePages(
            string sourceAndDestinationFolderPath,
            string sourceFileName,
            string destinationFileName,
            int angle,
            IEnumerable<int> pageNumbers
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
            params int[] pageNumbers
        ) => RotatePages(sourceFileFullPath, destinationFileFullPath, angle, pageNumbers.AsEnumerable());
        public static void RotatePages(
            string sourceFileFullPath,
            string destinationFileFullPath,
            int angle,
            IEnumerable<int> pageNumbers
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
}
