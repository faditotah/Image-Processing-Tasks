using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Diagnostics;

namespace Task1
{
    public class BitmapFilter
    {
        /* ################################################# MEAN OPTION 1: Expects 8bit, returns 8bit #################################################################################################################
        public static Bitmap BWMean(Bitmap b)
        {
            if (b.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                throw new ArgumentException("Image must be 8bit indexed");
            }
            Bitmap newImage = (Bitmap)b.Clone();

            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            BitmapData bmData1 = newImage.LockBits(new Rectangle(0, 0, newImage.Width, newImage.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            int nOffset = bmData.Stride - b.Width;
            int nOffset1 = bmData1.Stride - newImage.Width;
            
            byte[] grayVals = new byte [Math.Abs(bmData.Stride) * b.Height];
            System.Runtime.InteropServices.Marshal.Copy(bmData.Scan0, grayVals, 0,Math.Abs(bmData.Stride) * b.Height );
            int meanInt = (grayVals.Sum(x => x)/ (Math.Abs(bmData.Stride) * b.Height));
            
            unsafe 
            {
                byte* p = (byte*)(void*)bmData.Scan0; 
                byte* pNew = (byte*)(void*)bmData1.Scan0; 
                
                for (int y = 0; y < b.Height; ++y)
                {
                    for (int x = 0; x < b.Width; ++x)
                    {
                        pNew[0] = (p[0] >= meanInt) ? (byte)255 : (byte)0;
                        ++p;
                        ++pNew;
                    }
                    p += nOffset;
                    pNew += nOffset1;
                }
            }
            b.UnlockBits(bmData);
            newImage.UnlockBits(bmData1);
            return newImage;
        }
        */
        //######################################################################## MEAN OPTION 2: Takes any 24bit or 8bit, returns 8bit ##########################################################################
        public static Bitmap BWMean(Bitmap b)
        {
            // Start measuring time
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            Bitmap newImage; 
            if (b.PixelFormat == PixelFormat.Format24bppRgb)
            {
                Bitmap clone = (Bitmap)b.Clone();
                newImage = new Bitmap(clone.Width, clone.Height, PixelFormat.Format8bppIndexed);

                ColorPalette palette = newImage.Palette;
                for (int i = 0; i < 256; i++)
                {
                    palette.Entries[i] = Color.FromArgb(i, i, i);
                }
                newImage.Palette = palette;

                BitmapData bmpData = clone.LockBits(new Rectangle(0, 0, clone.Width, clone.Height),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format24bppRgb);
                BitmapData newBmpData = newImage.LockBits(new Rectangle(0, 0, clone.Width, clone.Height),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format8bppIndexed);

                IntPtr Scan0 = bmpData.Scan0;
                IntPtr Scan1 = newBmpData.Scan0;
                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;
                    byte* pNew = (byte*)(void*)Scan1;

                    for (int y = 0; y < clone.Height; y++)
                    {
                        for (int x = 0; x < clone.Width; x++)
                        {
                            pNew[0] = (byte)(0.299 * p[2] + 0.587 * p[1] + 0.114 * p[0]);
                            p += 3;
                            ++pNew;
                        }
                        p += bmpData.Stride - clone.Width * 3;
                        pNew += newBmpData.Stride - newImage.Width;
                    }
                }

                clone.UnlockBits(bmpData);
                newImage.UnlockBits(newBmpData);
            }
            else if (b.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                newImage = (Bitmap)b.Clone();
            }
            else
            {
                throw new ArgumentException("Image is not compatible. Must be 24 or 8 bit");
            }
            
            BitmapData bmData = newImage.LockBits(new Rectangle(0, 0, newImage.Width, newImage.Height), ImageLockMode.ReadWrite, newImage.PixelFormat);
            // Address and offset
            System.IntPtr Scan2 = bmData.Scan0;
            int nOffset = bmData.Stride - newImage.Width;
            
            // Mean Threshold Calculation
            int size = Math.Abs(bmData.Stride) * newImage.Height; 
            byte[] grayVals = new byte [size];
            System.Runtime.InteropServices.Marshal.Copy(Scan2, grayVals, 0,size );
            int meanInt = (grayVals.Sum(x => x)/ (size));
            
            // Bit Manipulation
            unsafe 
            {
                byte* p = (byte*)(void*)Scan2; 
                for (int y = 0; y < size; ++y)
                {
                    p[0] = (p[0] >= meanInt) ? (byte)255 : (byte)0;
                    ++p;
                }
                //p += nOffset; 
            }
            newImage.UnlockBits(bmData);
            stopwatch.Stop();
            Console.WriteLine($"Filter execution time: {stopwatch.ElapsedMilliseconds} ms");

            return newImage;
        }
        // ########################################################################### OPTION 3: Input 24bit or bit, returns B&W w same format###################################################################
        // Limitation: no consideration whether input image is grayscale, treats every image the same 
        /*
        public static Bitmap BWMean(Bitmap b)
        {
            Bitmap newImage = (Bitmap)b.Clone();
            
            BitmapData bmData = newImage.LockBits(new Rectangle(0, 0, newImage.Width, newImage.Height), ImageLockMode.ReadWrite, newImage.PixelFormat);

            System.IntPtr Scan2 = bmData.Scan0;
            int nOffset;
            int width;
            
            if (newImage.PixelFormat == PixelFormat.Format24bppRgb)
            {
                nOffset = bmData.Stride - newImage.Width * 3;
                width = newImage.Width * 3;
                ColorPalette palette = newImage.Palette;
                for (int i = 0; i < 256; i++)
                {
                    palette.Entries[i] = Color.FromArgb(i, i, i);
                }
                newImage.Palette = palette;
            }
            else if (newImage.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                nOffset = bmData.Stride - newImage.Width;
                width = newImage.Width;
            }
            else
            {
                throw new ArgumentException("Image must be 24bit or 8bit grayscale");
            }
            byte[] grayVals = new byte [Math.Abs(bmData.Stride) * newImage.Height];
            System.Runtime.InteropServices.Marshal.Copy(Scan2, grayVals, 0,Math.Abs(bmData.Stride) * newImage.Height );
            //int numPixels = b.Width * b.Height; // number of pixels
            int meanInt = (grayVals.Sum(x => x)/ (Math.Abs(bmData.Stride) * newImage.Height));
            
            unsafe 
            {
                byte* p = (byte*)(void*)Scan2; 
                for (int y = 0; y < newImage.Height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        p[0] = (p[0] >= meanInt) ? (byte)255 : (byte)0;
                        ++p;
                    }
                    p += nOffset;
                }
            }
            newImage.UnlockBits(bmData);
            
            return newImage;
        }
        */
    //###############################################################################################################################################################################################################
    
    // BW
    public static Bitmap BWStatic(Bitmap b, int nVal)
    {
        // Start measuring time
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        
        Bitmap newImage = null; 
        if (b.PixelFormat == PixelFormat.Format24bppRgb)
        {
            Bitmap clone = (Bitmap)b.Clone();
            newImage = new Bitmap(clone.Width, clone.Height, PixelFormat.Format8bppIndexed);

            ColorPalette palette = newImage.Palette;
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }

            newImage.Palette = palette;

            BitmapData bmpData = clone.LockBits(new Rectangle(0, 0, clone.Width, clone.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);
            BitmapData newBmpData = newImage.LockBits(new Rectangle(0, 0, clone.Width, clone.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format8bppIndexed);

            int stride = bmpData.Stride;
            System.IntPtr Scan0 = bmpData.Scan0;
            System.IntPtr Scan1 = newBmpData.Scan0;
            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                byte* pNew = (byte*)(void*)Scan1;

                for (int y = 0; y < clone.Height; y++)
                {
                    for (int x = 0; x < clone.Width; x++)
                    {
                        pNew[0] = (byte)(0.299 * p[2] + 0.587 * p[1] + 0.114 * p[0]);
                        p += 3;
                        ++pNew;
                    }
                    p += stride - clone.Width * 3;
                    pNew += newBmpData.Stride - newImage.Width;
                }
            }

            clone.UnlockBits(bmpData);
            newImage.UnlockBits(newBmpData);
        }
        else if (b.PixelFormat == PixelFormat.Format8bppIndexed)
        {
            newImage = (Bitmap)b.Clone();
        }
        else
        {
            throw new ArgumentException("Image must be 24bit or 8bit grayscale");
        }
        BitmapData bmData = newImage.LockBits(new Rectangle(0, 0, newImage.Width, newImage.Height), ImageLockMode.ReadWrite, newImage.PixelFormat);
            
        System.IntPtr Scan2 = bmData.Scan0;
        int nOffset = bmData.Stride - newImage.Width;
        int size = newImage.Height * Math.Abs(bmData.Stride);
        unsafe
        {
            byte* p = (byte*)(void*)Scan2;
            for (int y = 0; y < size; ++y)
            {
                p[0] = (p[0] >= nVal) ? (byte)255 : (byte)0; // apply threshold depending on condition
                ++p;
            }
            //p += nOffset;
        }
        newImage.UnlockBits(bmData);
        stopwatch.Stop();
        Console.WriteLine($"Filter execution time: {stopwatch.ElapsedMilliseconds} ms");

        return newImage;
    }
        /*
        public static Bitmap BWStatic(Bitmap b, int nVal)
        {
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            Bitmap newImage = new Bitmap(b.Width, b.Height, PixelFormat.Format8bppIndexed);
            BitmapData bmData1 = newImage.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;
            int stride1 = bmData1.Stride;
            System.IntPtr Scan1 = bmData1.Scan0;
            
            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                byte* pNew = (byte*)(void*)Scan1;

                int nOffset = stride - b.Width;
                int nOffset1 = stride1 - newImage.Width;

                for (int y = 0; y < b.Height; ++y)
                {
                    for (int x = 0; x < b.Width; ++x)
                    {
                        pNew[0] = (p[0] >= nVal) ? (byte)255 : (byte)0; // apply threshold depending on condition
                        ++p;
                        ++pNew;
                    }
                    p += nOffset;
                    pNew += nOffset1;
                }
            }
            b.UnlockBits(bmData);
            newImage.UnlockBits(bmData1);
            return newImage;
        }
        */
    }
}
