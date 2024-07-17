using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using System.IO;
using SixLabors.ImageSharp.Drawing;

namespace IczpNet.AbpCommons.Utils;

/// <summary>
/// 图片处理类- by Iczp.net
/// </summary>
public static class ImageHelper
{

    /// <summary>
    /// 创建文件夹
    /// </summary>
    /// <param name="path">虚拟路径，如：C:\\uploads</param>
    /// <returns></returns>
    public static string CreateDirectory(string path)
    {
        var dir = System.IO.Path.GetDirectoryName(path);
        //var dir = HostingEnvironment.MapPath(path);
        var paths = dir.Split('\\');
        var pathList = new List<string>()
        {
            paths[0]
        };
        for (var i = 1; i < paths.Length; i++)
        {
            pathList.Add(paths[i]);
            var dirPath = string.Join("\\", pathList);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }
        return dir;
    }

    public static byte[] GetEmptyPngBytes(int width, int height)
    {
        // 创建一个空白图像
        using Image<Rgba32> image = new Image<Rgba32>(width, height);

        // 将图像保存到内存流中
        using MemoryStream memoryStream = new MemoryStream();
        image.Save(memoryStream, new PngEncoder());

        // 返回字节数组
        return memoryStream.ToArray();
    }

    /// <summary>
    /// 九宫格图片 by Iczp.net
    /// </summary>
    /// <param name="imageBytes">图片</param>
    /// <param name="size">大小（宽高）</param>
    /// <param name="cornerRadius">圆角的半径</param>
    /// <param name="gap">间距</param>
    /// <returns></returns>
    public static byte[] MakeMergeThumbnails(List<byte[]> imageBytes, int size, int gap = 10, int cornerRadius = 10)
    {
        using Image<Rgba32> bitmap = new Image<Rgba32>(size, size);
        int num = (size - gap) / 3;
        int num2 = 0;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (num2 >= imageBytes.Count)
                {
                    break;
                }

                byte[] buffer = imageBytes[num2];
                num2++;
                Rectangle destRect = new Rectangle(num * j + gap, num * i + gap, num - gap, num - gap);

                using MemoryStream stream = new MemoryStream(buffer);
                using Image<Rgba32> image = Image.Load<Rgba32>(stream);

                int x = 0;
                int y = 0;
                int num3 = ((image.Width >= image.Height) ? image.Height : image.Width);
                if (image.Width >= image.Height)
                {
                    x = (image.Width - image.Height) / 2;
                }
                else
                {
                    y = (image.Height - image.Width) / 2;
                }

                Rectangle srcRect = new Rectangle(x, y, num3, num3);

                // 裁剪并调整图像大小
                using Image<Rgba32> croppedImage = image.Clone(ctx => ctx.Crop(srcRect).Resize(destRect.Width, destRect.Height));

                if (cornerRadius > 0)
                {
                    IPathCollection paths = BuildCorners(destRect, cornerRadius);
                    bitmap.Mutate(ctx => ctx.Fill(Color.Transparent, paths));
                    bitmap.Mutate(ctx => ctx.DrawImage(croppedImage, destRect.Location, 1));
                }
                else
                {
                    bitmap.Mutate(ctx => ctx.DrawImage(croppedImage, destRect.Location, 1));
                }
            }
        }

        using MemoryStream memoryStream = new MemoryStream();
        bitmap.Save(memoryStream, new PngEncoder());
        return memoryStream.ToArray();
    }

    private static IPathCollection BuildCorners(Rectangle rect, float cornerRadius)
    {
        var rects = new RectangularPolygon[]
        {
        new RectangularPolygon(rect.X, rect.Y, cornerRadius, cornerRadius),
        new RectangularPolygon(rect.Right - cornerRadius, rect.Y, cornerRadius, cornerRadius),
        new RectangularPolygon(rect.Right - cornerRadius, rect.Bottom - cornerRadius, cornerRadius, cornerRadius),
        new RectangularPolygon(rect.X, rect.Bottom - cornerRadius, cornerRadius, cornerRadius)
        };

        var corners = new IPath[]
        {
        rects[0].Clip(new EllipsePolygon(rect.X + cornerRadius, rect.Y + cornerRadius, cornerRadius)),
        rects[1].Clip(new EllipsePolygon(rect.Right - cornerRadius, rect.Y + cornerRadius, cornerRadius)),
        rects[2].Clip(new EllipsePolygon(rect.Right - cornerRadius, rect.Bottom - cornerRadius, cornerRadius)),
        rects[3].Clip(new EllipsePolygon(rect.X + cornerRadius, rect.Bottom - cornerRadius, cornerRadius))
        };

        return new PathCollection(corners);
    }

    public static byte[] SetImageRoundCorner(byte[] imageBytes, int cornerRadius)
    {
        using MemoryStream inputMemoryStream = new MemoryStream(imageBytes);
        using Image<Rgba32> image = Image.Load<Rgba32>(inputMemoryStream);

        // 创建一个与输入图像大小相同的空白图像
        using Image<Rgba32> roundedImage = new Image<Rgba32>(image.Width, image.Height);

        // 创建一个路径，用于定义圆角区域
        IPathCollection roundedCornersPath = BuildCorners(new Rectangle(0, 0, image.Width, image.Height), cornerRadius);

        // 使用填充圆角路径的方式，将输入图像绘制到空白图像上
        roundedImage.Mutate(ctx => ctx.Fill(Color.Transparent));
        roundedImage.Mutate(ctx => ctx.Fill(Color.White, roundedCornersPath));
        roundedImage.Mutate(ctx => ctx.DrawImage(image, new Point(0, 0), new GraphicsOptions { BlendPercentage = 1 }));

        // 将处理后的图像保存到内存流中
        using MemoryStream outputMemoryStream = new MemoryStream();
        roundedImage.Save(outputMemoryStream, new PngEncoder());

        // 返回字节数组
        return outputMemoryStream.ToArray();
    }


    #region 其它

    /// <summary>
    /// 判断文件类型是否为WEB格式图片
    /// (注：JPG,GIF,BMP,PNG)
    /// </summary>
    /// <param name="contentType">HttpPostedFile.ContentType</param>
    /// <returns></returns>
    public static bool IsImageContentType(string contentType)
    {
        var types = new List<string>() { "image/pjpeg", "image/jpeg", "image/gif", "image/bmp", "image/png" };
        return types.Contains(contentType);
        //if (contentType == "image/pjpeg" || contentType == "image/jpeg" || contentType == "image/gif" || contentType == "image/bmp" || contentType == "image/png")
        //{
        //    return true;
        //}
        //else
        //{
        //    return false;
        //}
    }

    #endregion

}//end class

