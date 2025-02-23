using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

using static Helpers.ImageExtensions.RotationAngle;


namespace Helpers;

public static class ImageExtensions
{
    public static byte[] ToByteArray(this Image imageIn)
    {
        using MemoryStream ms = new();
        imageIn.Save(ms, ImageFormat.Jpeg);
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


    public enum RotationAngle
    {
        None,
        NinetyDegreesClockwise,
        NinetyDegreesCounterClockwise,
        HundredEightyDegrees
    }

    public static int ToDegrees(this RotationAngle angle) 
        => angle switch
        {
            None => 0,
            NinetyDegreesClockwise => 90,
            NinetyDegreesCounterClockwise => 270,
            HundredEightyDegrees => 180,

            _ => throw new ArgumentException()
        };

    public static double ToRadians(this RotationAngle angle)
        => angle switch
        {
            None => 0,
            NinetyDegreesClockwise => Math.PI / 2,
            NinetyDegreesCounterClockwise => - Math.PI / 2,
            HundredEightyDegrees => Math.PI,

            _ => throw new ArgumentException()
        };
}
