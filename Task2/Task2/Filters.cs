using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Task2
{
    public class BitmapFilter
    {

        public static Bitmap HorizontalCon(Bitmap b)
        {
            int newWidth = b.Width * 2;
            int newHeight = b.Height;
            Bitmap newImage = new Bitmap(newWidth, newHeight);

            using (Graphics g = Graphics.FromImage(newImage))
            {
                // Draw the original image on the left side of the new bitmap
                g.DrawImage(b, 0, 0);

                // Draw the original image again on the right side of the new bitmap
                g.DrawImage(b, b.Width, 0);
            }

            return newImage;
        }

        public static Bitmap VerticalCon(Bitmap b)
        {
            int newWidth = b.Width;
            int newHeight = b.Height*2;
            Bitmap newImage = new Bitmap(newWidth, newHeight);

            using (Graphics g = Graphics.FromImage(newImage))
            {
                // Draw the original image on the left side of the new bitmap
                g.DrawImage(b, 0, 0);

                // Draw the original image again on the right side of the new bitmap
                g.DrawImage(b, 0, b.Height);
            }

            return newImage;
        }

        public static Bitmap HorizontalCon1(Bitmap b, Bitmap c)
        {
            int newWidth = b.Width + c.Width;
            int newHeight = Math.Max(b.Height, c.Height);
            Bitmap newImage = new Bitmap(newWidth, newHeight);

            using (Graphics g = Graphics.FromImage(newImage))
            {
                // Draw the first image on the left side of the new bitmap
                g.DrawImage(b, 0, 0);

                // Draw a new image on the right side of the new bitmap
                g.DrawImage(c, b.Width, 0);
            }

            return newImage;
        }
        public static Bitmap VerticalCon1(Bitmap b, Bitmap c)
        {
            int newWidth = Math.Max(b.Width, c.Width);
            int newHeight = b.Height + c.Height;
            Bitmap newImage = new Bitmap(newWidth, newHeight);

            using (Graphics g = Graphics.FromImage(newImage))
            {
                // Draw the original image on the left side of the new bitmap
                g.DrawImage(b, 0, 0);

                // Draw a new image on the right side of the new bitmap
                g.DrawImage(c, 0, b.Height);
            }

            return newImage;
        }
    }

}

 