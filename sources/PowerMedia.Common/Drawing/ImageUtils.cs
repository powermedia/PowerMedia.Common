using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace PowerMedia.Common.Drawing
{
    public static class ImageUtils
    {
        /// <summary>
        /// Creates a byte[] thumbnail image 
        /// </summary>
        /// <param name="orginalImageData">byte representation of image to resize</param>
        /// <param name="normalizationEdge">pixel length of a longest edge of the thumbnail</param>
        /// <returns></returns>
        public static Byte[] GetThumbnailData(Byte[] orginalImageData, float normalizationEdge, ImageFormat format )
        {
            byte[] data = orginalImageData;
            MemoryStream ms = new MemoryStream(data);

            Image image = Image.FromStream(ms);
            ms.Close();

            float scale;
            if (image.Width > image.Height)
            {
                scale = normalizationEdge / image.Width;
            }
            else
            {
                scale = normalizationEdge / image.Height;
            }

            float thumbnailWidth = image.Width * scale;
            float thumbnailHeight = image.Height * scale;

            Image thumbnail = new Bitmap((int)thumbnailWidth,(int)thumbnailHeight);
            Graphics.FromImage(thumbnail).DrawImage(image, 0, 0, (int)thumbnailWidth, (int)thumbnailHeight);

            MemoryStream thumbnailMemoryStream = new MemoryStream();
            byte[] thumbnailData;

            thumbnail.Save(thumbnailMemoryStream, format);
            thumbnailData = thumbnailMemoryStream.ToArray();
            thumbnailMemoryStream.Close();

            return thumbnailData;
        }
    }
}
