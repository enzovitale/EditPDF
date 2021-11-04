using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;


namespace Helpers
{
    public static class ImageExtensions
    {
        public static byte[] ToByteArray(this System.Drawing.Image imageIn)
        {
            using MemoryStream ms = new();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }


        public static Bitmap Scale(this Image originalImage, double scalingFactor) 
            => originalImage.Resize(
                Convert.ToInt32(originalImage.Width * scalingFactor), 
                Convert.ToInt32(originalImage.Height * scalingFactor)
            );

        public static Bitmap Resize(this Image originalImage, int newWidth, int newHeight)
        {
            var resizedImage = new Bitmap(newWidth, newHeight);
            var resizedGraph = Graphics.FromImage(resizedImage);
            resizedGraph.CompositingQuality = CompositingQuality.HighQuality;
            resizedGraph.SmoothingMode = SmoothingMode.HighQuality;
            resizedGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

            resizedGraph.DrawImage(originalImage, new Rectangle(0, 0, newWidth, newHeight));

            return resizedImage;
        }
    }
}
