﻿using iText.Kernel.Pdf;
using iText.Kernel.Utils;

using static EditPDF.PdfDocumentExtensions;


namespace EditPDF;

public static class PdfMergerExtensions
{
    public static void MergeDocument(
        this PdfMerger merger,
        PdfDocument document
    )
    {
        merger.Merge(document, 1, document.GetNumberOfPages());
    }
    public static void MergeDocument(
        this PdfMerger merger,
        string documentPath
    )
    {
        var documentToBeMerged = new PdfDocument(new PdfReader(documentPath));
        merger.MergeDocument(documentToBeMerged);
        documentToBeMerged.Close();
    }
    public static void MergeDocuments(
        this PdfMerger merger,
        params IEnumerable<string> documentPaths
    )
    {
        foreach (var iDocumentPath in documentPaths)
            merger.MergeDocument(iDocumentPath);
    }

    #region High-level APIs for merging multiple entire documents into one
    public static void MergeDocuments(
        string destinationPath,
        params IEnumerable<string> documentPaths
    )
    {
        var pdf = GetWriteOnlyPdfDocument(destinationPath);
        var merger = new PdfMerger(pdf);

        merger.MergeDocuments(documentPaths);

        pdf.Close();
    }
    public static void MergeDocuments(
        string destinationPath,
        string sourcePath,
        params IEnumerable<string> documentFileNames
    )
    {
        MergeDocuments(destinationPath, documentFileNames.Select(fn => Path.Combine(sourcePath, fn)).ToArray());
    }
    public static void MergeDocumentsInFolder(
        string sourceAndDestinationPath,
        string destinationFileName,
        params IEnumerable<string> documentFileNames
    )
    {
        MergeDocuments(Path.Combine(sourceAndDestinationPath, destinationFileName), sourceAndDestinationPath, documentFileNames);
    }
    #endregion

    public static void MergeDocuments(
        this PdfMerger merger, 
        params IEnumerable<(string documentPath, IEnumerable<int> pages)> pathAndPagesPairs
    )
    {
        foreach ((var iDocumentPath, var iPages) in pathAndPagesPairs)
        {
            var documentToBeMerged = GetReadOnlyPdfDocument(iDocumentPath);

            merger.Merge(documentToBeMerged, [.. iPages]);

            documentToBeMerged.Close();
        }
    }

    public static void MergeDocuments(
        this PdfMerger merger,
        params IEnumerable<(string documentFullPath, int page)> pathAndPagePairs
    )
    {
        // Open all documents
        var documentsToBeMerged = pathAndPagePairs.Select(p => p.documentFullPath).Distinct().ToDictionary(p => p, GetReadOnlyPdfDocument);

        // Merge pages one by one
        foreach (var (iDocumentFullPath, iPage) in pathAndPagePairs)
            merger.Merge(documentsToBeMerged[iDocumentFullPath], [iPage]);

        // Close all documents
        foreach (var d in documentsToBeMerged.Values)
            d.Close();
    }

    #region High-level APIs for merging parts of multiple documents into one
    public static void MergeDocuments(
        string destinationFullPath, 
        params IEnumerable<(string sourceFullPath, int page)> sourceFullPathAndPagePairs
    )
    {
        var pdf = GetWriteOnlyPdfDocument(destinationFullPath);
        var merger = new PdfMerger(pdf);

        merger.MergeDocuments(sourceFullPathAndPagePairs);

        pdf.Close();
    }
    public static void MergeDocumentsInFolder(
        string sourceAndDestinationPath, 
        string destinationFileName,
        params IEnumerable<(string sourceFileName, int page)> sourceFileNameAndPagePairs
    )
    {
        MergeDocuments(
            Path.Combine(sourceAndDestinationPath, destinationFileName), 
            sourceFileNameAndPagePairs.Select(p => (Path.Combine(sourceAndDestinationPath, p.sourceFileName), p.page)).ToArray()
        );
    }
    #endregion
}
