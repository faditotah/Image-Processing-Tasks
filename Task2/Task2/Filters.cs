using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace Task2
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

        public static Bitmap Convert1bit(Bitmap b)
        {
            int width = b.Width;
            int height = b.Height;
            
            Bitmap newImage = new Bitmap(width, height, PixelFormat.Format8bppIndexed);

            ColorPalette palette = newImage.Palette;
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            newImage.Palette = palette;

            BitmapData bmpData = b.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly,
                PixelFormat.Format1bppIndexed);
            BitmapData newBmpData = newImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly,
                PixelFormat.Format8bppIndexed);
            int stride1 = bmpData.Stride;
            int stride8 = newBmpData.Stride;
            int nOffset = stride8 - newImage.Width;
            System.IntPtr Scan0 = bmpData.Scan0;
            System.IntPtr Scan1 = newBmpData.Scan0;
            
            
            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                byte* pNew = (byte*)(void*)Scan1;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int index = x / 8;
                        int bitOffset = 7 - (x % 8); 

                        byte bitValue = (byte)((p[index] >> bitOffset) & 0x01);

                        byte grayscaleValue = (byte)(bitValue * 255);

                        pNew[0] = grayscaleValue;
                        ++pNew;
                    }
                    p += stride1; 
                    pNew += nOffset;
                }
            }
            b.UnlockBits(bmpData);
            newImage.UnlockBits(newBmpData);
            return newImage;
        }
        
        public static Bitmap HorizontalCon(Bitmap b, Bitmap c)
        {
            // Start measuring time
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Bitmap b8;
            Bitmap c8;
            
            if (b.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                b8 = b;
            }
            else if (b.PixelFormat == PixelFormat.Format1bppIndexed)
            {
                b8 = Convert1bit(b);
            }
            else if (b.PixelFormat == PixelFormat.Format24bppRgb)
            {
                b8 = Convert24bit(b);
            }
            else
            {
                throw new ArgumentException("Images must be in 24bit or 8bit format.");
            }
            if (c.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                c8 = c;
            }
            else if (c.PixelFormat == PixelFormat.Format1bppIndexed)
            {
                c8 = Convert1bit(c);
            }
            else if (c.PixelFormat == PixelFormat.Format24bppRgb)
            {
                c8 = Convert24bit(c);
            }
            else
            {
                throw new ArgumentException("Images must be in 24bit or 8bit format.");
            }
            
            Bitmap newImage = new Bitmap(b8.Width + c8.Width, Math.Max(b8.Height,c8.Height), PixelFormat.Format8bppIndexed);
            
            ColorPalette palette = newImage.Palette;
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            newImage.Palette = palette;
            
            BitmapData bmDataB = b8.LockBits(new Rectangle(0, 0, b8.Width, b8.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format8bppIndexed);
            BitmapData bmDataC = c8.LockBits(new Rectangle(0, 0, c8.Width, c8.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format8bppIndexed);
            BitmapData bmDataN = newImage.LockBits(new Rectangle(0, 0, newImage.Width, newImage.Height), ImageLockMode.WriteOnly,
                PixelFormat.Format8bppIndexed);

            System.IntPtr ScanB = bmDataB.Scan0;
            System.IntPtr ScanC = bmDataC.Scan0;
            System.IntPtr ScanN = bmDataN.Scan0;

            int nOffsetB = bmDataB.Stride - b.Width;
            int nOffsetC = bmDataC.Stride - c.Width;
            int nOffsetN = bmDataN.Stride - newImage.Width;
            int widthB = b8.Width; int heightB = b8.Height; int strideB = bmDataB.Stride;
            int widthC = c8.Width; int heightC = c8.Height; int strideC = bmDataC.Stride;
            int widthN = newImage.Width; int heightN = newImage.Height; int strideN = bmDataN.Stride;

            unsafe
            {
                byte* pB = (byte*)(void*)ScanB;
                byte* pC = (byte*)(void*)ScanC;
                byte* pN = (byte*)(void*)ScanN;
                
                for (int y = 0; y < heightN; ++y)
                {
                    for (int x = 0; x < widthN; ++x)
                    {
                        if (x < widthB & y < heightB) 
                        {
                            pN[x + y * strideN] = pB[x + y * strideB];
                        }
                        else if (x >= widthB & y < heightC)
                        {
                            pN[x + y * strideN] = pC[(x - widthB) + y * strideC];
                        }
                        else
                        {
                            pN[x + y * strideN] = 255;
                        }
                    }
                }
            }
            b8.UnlockBits(bmDataB);
            c8.UnlockBits(bmDataC);
            newImage.UnlockBits(bmDataN);

            stopwatch.Stop();
            Console.WriteLine($"Filter execution time: {stopwatch.ElapsedMilliseconds} ms");

            return newImage;
        }
        
        public static Bitmap VerticalCon(Bitmap b, Bitmap c)
        {
            // Start measuring time
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Bitmap b8;
            Bitmap c8;
            
            if (b.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                b8 = b;
            }
            else if (b.PixelFormat == PixelFormat.Format1bppIndexed)
            {
                b8 = Convert1bit(b);
            }
            else if (b.PixelFormat == PixelFormat.Format24bppRgb)
            {
                b8 = Convert24bit(b);
            }
            else
            {
                throw new ArgumentException("Images must be in 24bit or 8bit format.");
            }
            if (c.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                c8 = c;
            }
            else if (c.PixelFormat == PixelFormat.Format1bppIndexed)
            {
                c8 = Convert1bit(c);
            }
            else if (c.PixelFormat == PixelFormat.Format24bppRgb)
            {
                c8 = Convert24bit(c);
            }
            else
            {
                throw new ArgumentException("Images must be in 24bit or 8bit format.");
            }

            Bitmap newImage = new Bitmap(Math.Max(b8.Width, c8.Width), b8.Height + c8.Height, PixelFormat.Format8bppIndexed);
            
            ColorPalette palette = newImage.Palette;
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            newImage.Palette = palette;
            
            BitmapData bmDataB = b8.LockBits(new Rectangle(0, 0, b8.Width, b8.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format8bppIndexed);
            BitmapData bmDataC = c8.LockBits(new Rectangle(0, 0, c8.Width, c8.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format8bppIndexed);
            BitmapData bmDataN = newImage.LockBits(new Rectangle(0, 0, newImage.Width, newImage.Height), ImageLockMode.WriteOnly,
                PixelFormat.Format8bppIndexed);

            System.IntPtr ScanB = bmDataB.Scan0;
            System.IntPtr ScanC = bmDataC.Scan0;
            System.IntPtr ScanN = bmDataN.Scan0;
            
            int nOffsetB = bmDataB.Stride - b8.Width;
            int nOffsetC = bmDataC.Stride - c8.Width;
            int nOffsetN = bmDataN.Stride - newImage.Width;
            int widthB = b8.Width; int heightB = b8.Height; int strideB = bmDataB.Stride;
            int widthC = c8.Width; int heightC = c8.Height; int strideC = bmDataC.Stride;
            int widthN = newImage.Width; int heightN = newImage.Height; int strideN = bmDataN.Stride;

            unsafe
            {
                byte* pB = (byte*)(void*)ScanB;
                byte* pC = (byte*)(void*)ScanC;
                byte* pN = (byte*)(void*)ScanN;
                
                for (int y = 0; y < heightN; ++y)
                {
                    for (int x = 0; x < widthN; ++x)
                    {
                        if (x < widthB & y < heightB)
                        {
                            pN[x + y * strideN] = pB[x + y * strideB];
                        }
                        else if (x < widthC & y >= heightB)
                        {
                            pN[(x + y * strideN)] = pC[x + (y-heightB) * strideC];
                        }
                        else
                        {
                            pN[(x + y * strideN)] = 255;
                        }
                    }
                }
            }
            b8.UnlockBits(bmDataB);
            c8.UnlockBits(bmDataC);
            newImage.UnlockBits(bmDataN);
            
            stopwatch.Stop();
            Console.WriteLine($"Filter execution time: {stopwatch.ElapsedMilliseconds} ms");
            return newImage;
        }

    }
}

 
