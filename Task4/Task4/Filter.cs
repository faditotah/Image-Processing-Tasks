using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Task4
{
    public class BitmapFilter
    {
        public static Bitmap Convert1BitTo8Bit(Bitmap b)                                        // Converts 1bit bitmap to 8bit
        {
            if (b.PixelFormat != PixelFormat.Format1bppIndexed)                                    // Check input bitmap format
            {
                throw new ArgumentException(" Input image must be 1bit");
            }
            int width = b.Width;                                                                // Assign width and height of 1bit input to int variables                                                         
            int height = b.Height;

            Bitmap newImage = new Bitmap(width, height, PixelFormat.Format8bppIndexed);         // Create new 8bit indexed bitmap for drawing

            ColorPalette palette = newImage.Palette;                                            // Set color palette of output image to grayscale
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            newImage.Palette = palette;

            BitmapData bmpData = b.LockBits(new Rectangle(0, 0, width, height),             // Lock input and output bitmap data into memory
                ImageLockMode.ReadOnly, PixelFormat.Format1bppIndexed);
            BitmapData newBmpData = newImage.LockBits(new Rectangle(0, 0, width,
                    height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            int stride1 = bmpData.Stride;                                                       // Assign stride to integer variables
            int stride8 = newBmpData.Stride;
            int nOffset = stride8 - newImage.Width;                                             // Determine 8bit offset 
            System.IntPtr Scan0 = bmpData.Scan0;                                                // Retrieve address for first pixel data of both bitmaps
            System.IntPtr Scan1 = newBmpData.Scan0;


            unsafe
            {
                byte* p = (byte*)(void*)Scan0;                                                  // Cast addresses to byte pointers
                byte* pNew = (byte*)(void*)Scan1;
                for (int y = 0; y < height; y++)                                                // Iterate over the pixels of the bitmap
                {
                    for (int x = 0; x < width; x++)
                    {
                        int index = x / 8;                                                      // Determine which byte a bit is located in at a certain pixel, given 8bits in a byte
                        int bitOffset = 7 - (x % 8);                                            // Determine the position of the bit within the byte. x%8 gives the position of the bit in the byte(0-7)
                                                                                                // Subtracting the mode from 7 gives the bit offset within the byte (bit order 0-7)

                        byte bitValue = (byte)((p[index] >> bitOffset) & 0x01);                 // We use the index to shift the bits in the byte by the offset, so that the bit at pixel x reaches the 
                                                                                                // rightmost position in the byte (LSB). 
                                                                                                // Using AND to mask every bit except LSB, returns 1 if bit is 1, and 0 if bit is 0. Cast to byte
                        pNew[0] = (byte)(bitValue * 255);                                       // Multiply bit by 255, and assign to corresponding 8bit pixel
                        ++pNew;                                                                 // Increment to next 8bti pixel                                            
                    }
                    p += stride1;                                                               // Move to next scanline at end of pixel row          
                    pNew += nOffset;                                                            // Skip offset at end of pixel row
                }
            }
            b.UnlockBits(bmpData);                                                              // Unlock data and return 8bit image
            newImage.UnlockBits(newBmpData);
            return newImage;
        }
    }
}
