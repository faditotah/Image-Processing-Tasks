using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Diagnostics;

namespace Task1
{
    public class BitmapFilter
    {
        private static Bitmap Convert24BitTo8Bit(Bitmap b)
        {
            Bitmap bit8 = new Bitmap(b.Width, b.Height, PixelFormat.Format8bppIndexed);          // Create 8bit Bitmap for new Image

            ColorPalette palette = bit8.Palette;                                                 // Set color palette of new image to gray scale
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
                        pNew[0] = (byte)(0.299 * p[2] + 0.587 * p[1] + 0.114 * p[0]);          // Set the value of each pixel (byte) of the 8bit image to the
                                                                                               // corresponding grayscale value from the 24bit image
                        p += 3;                                                                // Increment to the next 24bit pixel
                        ++pNew;                                                                // Iterate to the next 8bit pixel
                    }
                    p += nOffset24;                                                            // Forgo offset at end of bitmap width 
                    pNew += nOffset8;
                }
            }
            b.UnlockBits(bmpData);                                                             // Unlock bitmap data from memory
            bit8.UnlockBits(newBmpData);
            return bit8;                                                                       // Return 8bit image
        } 
        public static int Mean(Bitmap b)                                                       // Method calculates mean intensity of 8bit grayscale images
        {
            if (b.PixelFormat == PixelFormat.Format24bppRgb)                                   // If input is in 24bit, use previous method to convert to 8bit
            {
                b = Convert24BitTo8Bit(b);
            }
            else if (b.PixelFormat == PixelFormat.Format8bppIndexed)                           // Ensures 8bit format is acceptable
            {
            }
            else
            {                                                                                 // Only 24bit or 8bit inputs are acceptable
                throw new ArgumentException("Image must be 24bit or 8bit grayscale");
            }
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),        // Locks bitmap data into memory
                ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            IntPtr scan0 = bmData.Scan0;                                                      // Retrieves address of first pixel data
            int nOffset = bmData.Stride - b.Width;                                            // Assigns offset to integer vairable
            int height = b.Height;                                                            // Assign bitmap width and height to variables
            int width = b.Width;
            int sum = 0;                                                                      // Initialize sum to zero
            int numPix = width * height;                                                      // Number of pixels in bitmap
            unsafe
            {
                byte* p = (byte*)(void*)scan0;                                                // Cast address to byte pointer
                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)                                           // Iterate over pixels in each row and add intensity value to running sum
                    {
                        sum += p[0];
                        ++p;
                    }                                      
                    p += nOffset;                   
                }
            }
            b.UnlockBits(bmData);
            int meanInt = sum / numPix;                                                       // Calculate intensity using summed intensity values and number of pixels
            return meanInt;                                                                   // Return mean value
        }
        public static Bitmap Threshold(Bitmap b, int nVal)                                    // Binarization method using threshold nVal
        {
            Bitmap newImage;                                                                  // Initialize output bitmap
            if (b.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                newImage = (Bitmap)b.Clone();                                                 // If input is 8bit, then output is a manipulated version of input
            }
            else if (b.PixelFormat == PixelFormat.Format24bppRgb)
            {
                newImage = Convert24BitTo8Bit(b);                                             // If input is 24bit, then output bitmap is a manipulated version of converted input
            }
            else
            {
                throw new ArgumentException("Image must be 24bit or 8bit grayscale");
            }
            BitmapData bmData = newImage.LockBits(new Rectangle(0, 0, newImage.Width,     // Lock bitmap data into memory
                newImage.Height), ImageLockMode.ReadWrite, newImage.PixelFormat);
                
            IntPtr Scan2 = bmData.Scan0;                                                      // Obtain address of first pixel data
            int nOffset = bmData.Stride - newImage.Width;                                     // Assign offset to integer variable
            int height = newImage.Height; int width = newImage.Width;                         // Assign bitmap width and height to integers
            unsafe
            {
                byte* p = (byte*)(void*)Scan2;                                                // Cast address to byte pointer
                for (int y = 0; y < height; ++y)                                              // Iterate over pixels in bitmap
                {
                    for (int x = 0; x < width; ++x)
                    {
                        p[0] = (p[0] >= nVal) ? (byte)255 : (byte)0;                          // Check value of each byte (pixel), then assign it to either black or white
                        ++p;                                                                  // Increment to next byte
                    }
                    p += nOffset;                                                             // Skip offset at end of bitmap row
                }
            }
            newImage.UnlockBits(bmData);                                                      // Unlock bitmap data from memory
            return newImage;                                                                  // Return binarized imag
        }
        
        
    }
}