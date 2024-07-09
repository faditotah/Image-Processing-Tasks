using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

namespace Task3
{
    public class BitmapFilter
    {
        public static Bitmap Convert24BitTo8Bit(Bitmap b)                                            // Converts 24bit bitmap to 8bit
        {
            if (b.PixelFormat != PixelFormat.Format24bppRgb)                                   // Check input before proceeding
            {
                throw new ArgumentException("Filter can only convert 24bit images");
            }
            
            Bitmap bit8 = new Bitmap(b.Width, b.Height, PixelFormat.Format8bppIndexed);         // Create 8bit Bitmap for new Image

            ColorPalette palette = bit8.Palette;                                                // Set color palette of new image to gray scale
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            bit8.Palette = palette;

            BitmapData bmpData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),          // Lock input bitmap data into memory
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);
            BitmapData newBmpData = bit8.LockBits(new Rectangle(0, 0, b.Width, b.Height),   // Lock output bitmap data into memory
                ImageLockMode.WriteOnly,
                PixelFormat.Format8bppIndexed);

            IntPtr Scan0 = bmpData.Scan0;                                                       // Retrieve address for first pixel data
            IntPtr Scan1 = newBmpData.Scan0;
            int height = b.Height; int width = b.Width;                                         // Assign bMap parameters to integer variables
            int nOffset8 = newBmpData.Stride - bit8.Width;                                      // Calculate offset for 8bit output
            int nOffset24 = bmpData.Stride - b.Width * 3;                                       // Calculate offset for 24bit input
            unsafe
            {
                byte* p = (byte*)(void*)Scan0;                                                  // Cast scan0 to byte pointers
                byte* pNew = (byte*)(void*)Scan1;
                
                for (int y = 0; y < height; y++)                                                // Iterate over the pixels of the input image
                {
                    for (int x = 0; x < width; x++)
                    {
                        pNew[0] = (byte)(0.299 * p[2] + 0.587 * p[1] + 0.114 * p[0]);           // Set the value of each pixel (byte) of the 8bit image to the
                                                                                                // corresponding grayscale value from the 24bit image
                        p += 3;                                                                 // Increment to the next 24bit pixel
                        ++pNew;                                                                 // Increment to the next 8bit pixel
                    }
                    p += nOffset24;                                                             // Forgo offset at end of bitmap width 
                    pNew += nOffset8;
                }
            }
            b.UnlockBits(bmpData);                                                              // Unlock bitmap data from memory
            bit8.UnlockBits(newBmpData);
            return bit8;                                                                        // Return 8bit image
        } 
    }
            
}