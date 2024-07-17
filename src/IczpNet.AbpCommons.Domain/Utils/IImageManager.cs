using System.Collections.Generic;
using System.Threading.Tasks;

namespace IczpNet.AbpCommons.Utils;

public interface IImageManager
{
    Task<string> CreateDirectory(string path);

    Task<byte[]> GetEmptyPngBytes(int width, int height);

    /// <summary>
    /// 设置圆角
    /// </summary>
    /// <param name="imageBytes"></param>
    /// <param name="cornerRadius">圆角的半径</param>
    /// <returns></returns>
    Task<byte[]> SetImageRoundCorner(byte[] imageBytes, int cornerRadius);

    /// <summary>
    /// 九宫格图片 by Iczp.net
    /// </summary>
    /// <param name="imageBytes">图片</param>
    /// <param name="size">大小（宽高）</param>
    /// <param name="cornerRadius">圆角的半径</param>
    /// <param name="gap">间距</param>
    /// <returns></returns>
    Task<byte[]> MakeMergeThumbnails(List<byte[]> imageBytes, int size, int gap = 10, int cornerRadius = 10);

    /// <summary>
    /// 九宫格图片 by Iczp.net
    /// </summary>
    /// <param name="filePaths">图片物理路径</param>
    /// <param name="savePath">保存路径</param>
    /// <param name="widthAndHeight">宽高（正方形图片）</param>
    /// <param name="margin">间距</param>
    Task MakeMergeThumbnails(List<string> filePaths, string savePath, int widthAndHeight, int margin = 10);

    /// <summary>
    /// 图片处理为圆角
    /// </summary>
    /// <param name="image"></param>
    /// <returns></returns>
    Task<System.Drawing.Image> DrawTransparentRoundCornerImage(System.Drawing.Image image);
}
