using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Task1
{
    public class BitmapFilter
    {
        public static Bitmap BWMean(Bitmap b)
        {      
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
            // Address
            System.IntPtr Scan2 = bmData.Scan0;
            
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
            }
            newImage.UnlockBits(bmData);

            return newImage;
        }
        public static Bitmap BWStatic(Bitmap b, int nVal)
        {            
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
            int size = newImage.Height * Math.Abs(bmData.Stride);
            unsafe
            {
                byte* p = (byte*)(void*)Scan2;
                for (int y = 0; y < size; ++y)
                {
                    p[0] = (p[0] >= nVal) ? (byte)255 : (byte)0; // apply threshold depending on condition
                    ++p;
                }
            }
            newImage.UnlockBits(bmData);
            return newImage;
        }   
    }
}
