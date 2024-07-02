using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Diagnostics;

namespace Task1
{
    public class BitmapFilter
    {
        private static Bitmap Convert24(Bitmap b)
        {
            Bitmap bit8 = new Bitmap(b.Width, b.Height, PixelFormat.Format8bppIndexed);

            ColorPalette palette = bit8.Palette;
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            bit8.Palette = palette;

            BitmapData bmpData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);
            BitmapData newBmpData = bit8.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format8bppIndexed);

            IntPtr Scan0 = bmpData.Scan0;
            IntPtr Scan1 = newBmpData.Scan0;
            int height = b.Height; int width = b.Width;
            int nOffset8 = newBmpData.Stride - bit8.Width;
            int nOffset24 = bmpData.Stride - b.Width * 3;
            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                byte* pNew = (byte*)(void*)Scan1;
                
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        pNew[0] = (byte)(0.299 * p[2] + 0.587 * p[1] + 0.114 * p[0]);
                        p += 3;
                        ++pNew;
                    }
                    p += nOffset24;
                    pNew += nOffset8;
                }
            }
            b.UnlockBits(bmpData);
            bit8.UnlockBits(newBmpData);
            return bit8;
        } 
        public static int Mean(Bitmap b)
        {
            if (b.PixelFormat == PixelFormat.Format24bppRgb)
            {
                b = Convert24(b);
            }
            else if (b.PixelFormat == PixelFormat.Format8bppIndexed)
            {
            }
            else
            {
                throw new ArgumentException("Image must be 24bit or 8bit grayscale");
            }
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            IntPtr scan0 = bmData.Scan0;
            int nOffset = bmData.Stride - b.Width;
            int height = b.Height; 
            int width = b.Width;
            int sum = 0;
            int numPix = width * height;
            unsafe
            {
                byte* p = (byte*)(void*)scan0;
                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        sum += p[0];
                        ++p;
                    }
                    p += nOffset;
                }
            }
            b.UnlockBits(bmData);
            int meanInt = sum / numPix;
            return meanInt;
        }
        public static Bitmap Threshold(Bitmap b, int nVal)
        {
            // Start measuring time
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            Bitmap newImage; 
            if (b.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                newImage = (Bitmap)b.Clone();
            }
            else if (b.PixelFormat == PixelFormat.Format24bppRgb)
            {
                newImage = Convert24(b);
            }
            else
            {
                throw new ArgumentException("Image must be 24bit or 8bit grayscale");
            }
            BitmapData bmData = newImage.LockBits(new Rectangle(0, 0, newImage.Width, newImage.Height), ImageLockMode.ReadWrite, newImage.PixelFormat);
                
            IntPtr Scan2 = bmData.Scan0;
            int nOffset = bmData.Stride - newImage.Width;
            int height = newImage.Height; int width = newImage.Width;
            unsafe
            {
                byte* p = (byte*)(void*)Scan2;
                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        p[0] = (p[0] >= nVal) ? (byte)255 : (byte)0;
                        ++p;
                    }
                    p += nOffset;
                }
            }
            newImage.UnlockBits(bmData);
            stopwatch.Stop();
            Console.WriteLine($"Filter execution time: {stopwatch.ElapsedMilliseconds} ms");
            return newImage;
        }
        
    }
}