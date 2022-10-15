using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;


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
            RotationAngle.None => 0,
            RotationAngle.NinetyDegreesClockwise => 90,
            RotationAngle.NinetyDegreesCounterClockwise => 270,
            RotationAngle.HundredEightyDegrees => 180,

            _ => throw new ArgumentException()
        };

    public static double ToRadians(this RotationAngle angle)
        => angle switch
        {
            RotationAngle.None => 0,
            RotationAngle.NinetyDegreesClockwise => Math.PI / 2,
            RotationAngle.NinetyDegreesCounterClockwise => - Math.PI / 2,
            RotationAngle.HundredEightyDegrees => Math.PI,

            _ => throw new ArgumentException()
        };
}
