using System.IO;
using System.Linq;

using iText.Kernel.Pdf;
using iText.Kernel.Utils;


namespace EditPDF
{
    public static class PdfMergerExtensions
    {
        public static void MergeDocument(this PdfMerger merger, PdfDocument document)
        {
            merger.Merge(document, 1, document.GetNumberOfPages());
        }
        public static void MergeDocument(this PdfMerger merger, string documentPath)
        {
            var documentToBeMerged = new PdfDocument(new PdfReader(documentPath));
            merger.MergeDocument(documentToBeMerged);
            documentToBeMerged.Close();
        }
        public static void MergeDocuments(this PdfMerger merger, params string[] documentPaths)
        {
            foreach (var iDocumentPath in documentPaths)
                merger.MergeDocument(iDocumentPath);
        }
        public static void MergeDocuments(string destinationPath, params string[] documentPaths)
        {
            var pdf = new PdfDocument(new PdfWriter(destinationPath));
            var merger = new PdfMerger(pdf);

            merger.MergeDocuments(documentPaths);

            pdf.Close();
        }
        public static void MergeDocuments(string destinationPath, string sourcePath, params string[] documentFileNames)
        {
            MergeDocuments(destinationPath, documentFileNames.Select(fn => Path.Combine(sourcePath, fn)).ToArray());
        }
        public static void MergeDocumentsInFolder(string sourceAndDestinationPath, string destinationFileName, params string[] documentFileNames)
        {
            MergeDocuments(Path.Combine(sourceAndDestinationPath, destinationFileName), sourceAndDestinationPath, documentFileNames);
        }
    }
}
