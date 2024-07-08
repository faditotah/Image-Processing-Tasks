using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Diagnostics;

namespace CompiledTasks
{ 
    public class BitmapFilter
    { 
        public static int Mean(Bitmap b)
        {
            if (b.PixelFormat == PixelFormat.Format24bppRgb)
            {
                b = Convert24bit(b);
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
                newImage = Convert24bit(b);
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
                /*
                for (int y = 0; y < heightB; ++y)
                {
                    for (int x = 0; x < widthB; ++x)
                    {
                        pN[x + y * strideN] = pB[x + y * strideB];
                    }
                    //pN += nOffsetN;
                    //pB += nOffsetB;
                }
                for (int y = 0; y < heightC; ++y)
                {
                    for (int x = 0; x < widthC; ++x)
                    {
                        pN[(widthB+ x) + y * strideN] = pC[x + y * strideC];
                    }
                    //pN += nOffsetN;
                    //pC += nOffsetC;
                }
                */
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
                /*
                for (int y = 0; y < b.Height; ++y)
                {
                    for (int x = 0; x < b.Width; ++x)
                    {
                        pN[x + y * bmDataN.Stride] = pB[x + y * bmDataB.Stride];
                    }

                }
                for (int y = 0; y < c.Height; ++y)
                {
                    for (int x = 0; x < c.Width; ++x)
                    {
                        pN[(x + (b.Height + y) * bmDataN.Stride)] = pC[x + y * bmDataC.Stride];
                    }
                }
                */
            }
            b8.UnlockBits(bmDataB);
            c8.UnlockBits(bmDataC);
            newImage.UnlockBits(bmDataN);
            
            stopwatch.Stop();
            Console.WriteLine($"Filter execution time: {stopwatch.ElapsedMilliseconds} ms");
            return newImage;
        }
        public static Bitmap Convert24bit(Bitmap b)
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
        public static Bitmap AddPaddingErosion(Bitmap b)
        {
            int width = b.Width;
            int height = b.Height;

            Bitmap paddedIm = new Bitmap(width + 2, height + 2, PixelFormat.Format8bppIndexed);

            ColorPalette palette = paddedIm.Palette;
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            paddedIm.Palette = palette;

            BitmapData bmpData0 = b.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly,
                PixelFormat.Format8bppIndexed);
            BitmapData bmpData1 = paddedIm.LockBits(new Rectangle(0, 0, paddedIm.Width, paddedIm.Height), ImageLockMode.WriteOnly,
                PixelFormat.Format8bppIndexed);

            IntPtr scan0 = bmpData0.Scan0;
            IntPtr scan1 = bmpData1.Scan0;
            int stride0 = bmpData0.Stride;
            int stride1 = bmpData1.Stride;

            unsafe
            {
                byte* p0 = (byte*)(void*)scan0;
                byte* p1 = (byte*)(void*)scan1;

                for (int y = 0; y < paddedIm.Height; y++)
                {
                    for (int x = 0; x < paddedIm.Width; x++)
                    {
                        if (x == 0 || x == paddedIm.Width - 1)
                        {
                            p1[(x) + (y) * stride1] = 255;
                        }
                        else if (y == 0 || y == paddedIm.Height - 1)
                        {
                            p1[(x) + (y) * stride1] = 255;
                        }
                        else
                        {
                            p1[(x) + (y) * stride1] = p0[(x-1) + (y-1) * stride0];
                        }
                    }
                }
            }

            b.UnlockBits(bmpData0);
            paddedIm.UnlockBits(bmpData1);

            return paddedIm;
        }
        public static Bitmap AddPaddingDilation(Bitmap b)
        {
            int width = b.Width;
            int height = b.Height;

            Bitmap paddedIm = new Bitmap(width + 2, height + 2, PixelFormat.Format8bppIndexed);

            ColorPalette palette = paddedIm.Palette;
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            paddedIm.Palette = palette;

            BitmapData bmpData0 = b.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly,
                PixelFormat.Format8bppIndexed);
            BitmapData bmpData1 = paddedIm.LockBits(new Rectangle(0, 0, paddedIm.Width, paddedIm.Height), ImageLockMode.WriteOnly,
                PixelFormat.Format8bppIndexed);

            IntPtr scan0 = bmpData0.Scan0;
            IntPtr scan1 = bmpData1.Scan0;
            int stride0 = bmpData0.Stride;
            int stride1 = bmpData1.Stride;

            unsafe
            {
                byte* p0 = (byte*)(void*)scan0;
                byte* p1 = (byte*)(void*)scan1;

                for (int y = 0; y < paddedIm.Height; y++)
                {
                    for (int x = 0; x < paddedIm.Width; x++)
                    {
                        if (x == 0 || x == paddedIm.Width - 1)
                        {
                            p1[(x) + (y) * stride1] = 0;
                        }
                        else if (y == 0 || y == paddedIm.Height - 1)
                        {
                            p1[(x) + (y) * stride1] = 0;
                        }
                        else
                        {
                            p1[(x) + (y) * stride1] = p0[(x-1) + (y-1) * stride0];
                        }
                    }
                }
            }

            b.UnlockBits(bmpData0);
            paddedIm.UnlockBits(bmpData1);

            return paddedIm;
        }
        public static Bitmap Dilate(Bitmap b)
        {
            Bitmap newImage;
            if (b.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                newImage = (Bitmap)b.Clone();
            }

            else if (b.PixelFormat == PixelFormat.Format1bppIndexed)
            {
                newImage = Convert1bit(b);
            }
            else if (b.PixelFormat == PixelFormat.Format24bppRgb)
            {
                newImage = Convert24bit(b);
            }
            else
            {
                throw new ArgumentException(" Image type is not supported. Must be 1, 8, or 24bit");
            }

            Bitmap paddedIm = AddPaddingDilation(newImage);
            BitmapData bmData = paddedIm.LockBits(new Rectangle(0, 0, paddedIm.Width, paddedIm.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format8bppIndexed);
            BitmapData newbmData = newImage.LockBits(new Rectangle(0, 0, newImage.Width, newImage.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            System.IntPtr Scan0 = bmData.Scan0;
            System.IntPtr Scan1 = newbmData.Scan0;
            
            int stride0 = bmData.Stride;
            int stride1 = newbmData.Stride;

            int nOffset0 = stride0 - paddedIm.Width;
            int nOffset1 = stride1 - newImage.Width;

            int height = newImage.Height;
            int width = newImage.Width;
            
            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                byte* pNew = (byte*)(void*)Scan1;
                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        int k1 = x + y * stride0;
                        int k2 = x + stride0*(y + 1);
                        int k3 = x + stride0 * (y + 2);
                        byte[] group =
                        {
                            p[k1], p[k1+1], p[k1 + 2], p[k2], p[k2 + 2], p[k3], p[k3 + 1], p[k3 +2]
                        };
                        pNew[0] = group.Max();
                        
                        ++pNew;
                    }
                    
                    pNew += nOffset1;
                }
            }
            paddedIm.UnlockBits(bmData);
            newImage.UnlockBits(newbmData);
            return newImage;
        }
        public static Bitmap Erosion(Bitmap b)
        {    
            Bitmap newImage;
            if (b.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                newImage = (Bitmap)b.Clone();
            }

            else if (b.PixelFormat == PixelFormat.Format1bppIndexed)
            {
                newImage = Convert1bit(b);
            }
            else if (b.PixelFormat == PixelFormat.Format24bppRgb)
            {
                newImage = Convert24bit(b);
            }
            else
            {
                throw new ArgumentException(" Image type is not supported. Must be 1, 8, or 24bit");
            }

            Bitmap paddedIm = AddPaddingErosion(newImage);
            BitmapData bmData = paddedIm.LockBits(new Rectangle(0, 0, paddedIm.Width, paddedIm.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format8bppIndexed);
            BitmapData newbmData = newImage.LockBits(new Rectangle(0, 0, newImage.Width, newImage.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            System.IntPtr Scan0 = bmData.Scan0;
            System.IntPtr Scan1 = newbmData.Scan0;
            
            int stride0 = bmData.Stride;
            int stride1 = newbmData.Stride;

            int nOffset0 = stride0 - paddedIm.Width;
            int nOffset1 = stride1 - newImage.Width;

            int height = newImage.Height;
            int width = newImage.Width;
            
            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                byte* pNew = (byte*)(void*)Scan1;
                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        int k1 = x + y * stride0;
                        int k2 = x + stride0*(y + 1);
                        int k3 = x + stride0 * (y + 2);
                        byte[] group =
                        {
                            p[k1], p[k1+1], p[k1 + 2], p[k2], p[k2+1], p[k2 + 2], p[k3], p[k3 + 1], p[k3 +2]
                        };
                        pNew[0] = group.Min();
                        ++pNew;
                    }
                    
                    pNew += nOffset1;
                }
            }
            paddedIm.UnlockBits(bmData);
            newImage.UnlockBits(newbmData);
            return newImage;
        }
        private static int[] Offset(Bitmap b)
        {
            BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            int stride = bData.Stride;
            IntPtr scan = bData.Scan0;
            
            int height = b.Height;
            int width = b.Width;
            
            int[] vals = new int[4]; // Changed to size 4 for top, bottom, left, right offsets
            
            unsafe
            { 
                byte* p = (byte*)(void*)scan;
                // Find top boundary
                bool breakTop = false;
                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        if (p[x + y * stride] != 255) // Corrected index calculation
                        {
                            vals[0] = y;
                            breakTop = true;
                            break;
                        }
                    }

                    if (breakTop)
                    {
                        break;
                    }
                }
    
                // Find bottom boundary
                bool breakBottom = false;
                for (int y = height - 1; y >= 0; --y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        if (p[x + y * stride] != 255) // Corrected index calculation
                        {
                            vals[1] = height - 1 - y;
                            breakBottom = true;
                            break;
                        }
                    }

                    if (breakBottom)
                    {
                        break;
                    }
                }
                // Find left boundary
                bool breakLeft = false;
                for (int x = 0; x < width; ++x)
                {
                    for (int y = 0; y < height; ++y)
                    {
                        if (p[x + y * stride] != 255) // Corrected index calculation
                        {
                            vals[2] = x;
                            breakLeft = true;
                            break;
                        }
                    }
                    if (breakLeft)
                    {
                        break;
                    }
                }
    
                // Find right boundary
                bool breakRight = false;
                for (int x = width - 1; x >= 0; --x)
                {
                    for (int y = 0; y < height; ++y)
                    {
                        if (p[x + y * stride] != 255) // Corrected index calculation
                        {
                            vals[3] = width - 1 - x;
                            breakRight = true;
                            break;
                        }
                    } 
                    if (breakRight)
                    {
                        break; 
                    }
                }
            }
            b.UnlockBits(bData);
            return vals;
        }
        public static Bitmap noWhite(Bitmap b)
        {
            // Start measuring time
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();            
            Bitmap input;
            if (b.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                input = b;
            }
            else if (b.PixelFormat == PixelFormat.Format1bppIndexed)
            {
                input = Convert1bit(b);
            }
            else if (b.PixelFormat == PixelFormat.Format24bppRgb)
            {
                input = Convert24bit(b);
            }
            else
            {
                throw new ArgumentException("Image type is not supported. Must be 1, 8, or 24bit");
            }

            int[] vals = Offset(Threshold(input, 200));

            int outputWidth = input.Width - (vals[2] + vals[3]); 
            int outputHeight = input.Height - (vals[0] + vals[1]); 

            Bitmap output = new Bitmap(outputWidth, outputHeight, PixelFormat.Format8bppIndexed);
            ColorPalette palette = output.Palette;
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            output.Palette = palette;

            BitmapData inputData = input.LockBits(new Rectangle(0, 0, input.Width, input.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            BitmapData outputData = output.LockBits(new Rectangle(0, 0, output.Width, output.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            int strideIn = inputData.Stride;
            int strideOut = outputData.Stride;

            IntPtr inScan = inputData.Scan0;
            IntPtr outScan = outputData.Scan0;

            unsafe
            {
                byte* pIn = (byte*)(void*)inScan;
                byte* pOut = (byte*)(void*)outScan;

                for (int y = 0; y < outputHeight; ++y)
                {
                    for (int x = 0; x < outputWidth; ++x)
                    {
                        pOut[x + y * strideOut] = pIn[(x + vals[2]) + (y + vals[0]) * strideIn];
                    }
                }
            }

            input.UnlockBits(inputData);
            output.UnlockBits(outputData);
            stopwatch.Stop();
            Console.WriteLine($"Filter execution time: {stopwatch.ElapsedMilliseconds} ms");
            return output;
        }
/*
        public static Bitmap Resize(Bitmap b, int factor)
        {
            
        }
       */ 
    }

}
