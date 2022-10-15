
using Helpers;

using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;


namespace EditPDF;

public static class PdfImageExtensions
{
    public static System.Drawing.Image ToGeneralImage(this PdfImageXObject image) => System.Drawing.Image.FromStream(new MemoryStream(image.GetImageBytes()));
    public static iText.Layout.Element.Image ToPdfImage(this System.Drawing.Image image) => new(ImageDataFactory.Create(image.ToByteArray()));
    public static void PutImage(this PdfDictionary objects, PdfName key, System.Drawing.Image image) => objects.Put(key, image.ToPdfImage().GetXObject().GetPdfObject());

    public static iText.Layout.Element.Image GetImage(string imageFile) => new(ImageDataFactory.Create(imageFile));
}
