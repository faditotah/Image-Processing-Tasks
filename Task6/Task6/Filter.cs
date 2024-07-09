using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Task6
{
    public class BitmapFilter
    {
        private static int[] Offset(Bitmap b)                                                   // Determine size of white boundaries
        {
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),          // Lock in input bitmap (Bitmap would have already been converted to 8bit)
                ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            int stride = bmData.Stride;                                                         // Assign stride to integer variable
            IntPtr scan = bmData.Scan0;                                                         // Retrieve address of first pixel data

            int height = b.Height;                                                              // Assign height and width to integer variables
            int width = b.Width;

            int[] vals = new int[4];                                                            // Initialize integer array to store top, bottom, left, and right offset values, respectively

            unsafe
            {
                byte* p = (byte*)(void*)scan;                                                   // Cast starting address to byte pointer
                // Find top boundary
                bool breakTop = false;                                                          // boolean check used to break for-loop
                for (int y = 0; y < height; ++y)                                                // Iterate over bitmap, row by row
                {
                    for (int x = 0; x < width; ++x)
                    {
                        if (p[x + y * stride] != 255)                                           // Once a non-white pixel is reached
                        {
                            vals[0] = y;                                                        // Store the row number at the first position in vals
                            breakTop = true;                                                    // change bool status of breakTop to break outer for loop
                            break;                                                              // Break inner for-loop
                        }
                    }

                    if (breakTop)                                                               // BreakTop is true when a non-white pixel is reached
                    {
                        break;                                                                  // Hence, outer for-loop is broken
                    }
                }

                // Find bottom boundary
                bool breakBottom = false;
                for (int y = height - 1; y >= 0; --y)                                           // Iterate over bitmap beginning from bottom row and going up
                {
                    for (int x = 0; x < width; ++x)
                    {
                        if (p[x + y * stride] != 255)                                           // Once a non-white pixel is reached
                        {
                            vals[1] = height - 1 - y;                                           // Store bottom row value at second position in vals
                            breakBottom = true;                                                 // Break for-loops
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
                for (int x = 0; x < width; ++x)                                                 // Iterate bitmap, beginning from left, column by column
                {
                    for (int y = 0; y < height; ++y)
                    {
                        if (p[x + y * stride] != 255)
                        {
                            vals[2] = x;                                                        // Once non-white pixel is reached, store column number at third position in vals
                            breakLeft = true;                                                   // Break for-loops
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
                for (int x = width - 1; x >= 0; --x)                                            // Iterate bitmap, beginning from right, column by column
                {
                    for (int y = 0; y < height; ++y)
                    {
                        if (p[x + y * stride] != 255)
                        {
                            vals[3] = width - 1 - x;                                            // Once non-white pixel is reached, store column number in last position of vals
                            breakRight = true;                                                  // Break for-loops
                            break;
                        }
                    }
                    if (breakRight)
                    {
                        break;
                    }
                }
            }
            b.UnlockBits(bmData);
            return vals;
        }
        public static Bitmap noWhite(Bitmap b)                                                  // Remove white boundaries from bitmap
        {
            Bitmap input;                                                                       // Initialize input bitmap
            if (b.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                input = b;
            }                            // Ensure input is an 8bit version of bitmap b
            else if (b.PixelFormat == PixelFormat.Format1bppIndexed)
            {
                input = Convert1BitTo8Bit(b);
            }
            else if (b.PixelFormat == PixelFormat.Format24bppRgb)
            {
                input = Convert24bitTo8Bit(b);
            }
            else
            {
                throw new ArgumentException("Image type is not supported. Must be 1, 8, or 24bit");
            }

            int[] vals = Offset(Threshold(input, 200));                                   // Retrieve the boundary offset values using the binarized version of input

            int outputWidth = input.Width - (vals[2] + vals[3]);                                // Determine size of output image by subtracting boundary offsets from input image parameters
            int outputHeight = input.Height - (vals[0] + vals[1]);

            Bitmap output = new Bitmap(outputWidth, outputHeight,                               // Create final bitmap with subtracted offsets 
                PixelFormat.Format8bppIndexed);
            ColorPalette palette = output.Palette;                                              // Set color palette to grayscale
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            output.Palette = palette;

            BitmapData inputData = input.LockBits(new Rectangle(0, 0, input.Width,          // Lock into memory input and output bitmaps
                input.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            BitmapData outputData = output.LockBits(new Rectangle(0, 0, output.Width,
                output.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            int strideIn = inputData.Stride;                                                    // Assign stride to integer variables
            int strideOut = outputData.Stride;

            IntPtr inScan = inputData.Scan0;                                                    // Retrieve starting pixel address
            IntPtr outScan = outputData.Scan0;

            unsafe
            {
                byte* pIn = (byte*)(void*)inScan;                                               // Cast pixel address to byte pointer
                byte* pOut = (byte*)(void*)outScan;

                for (int y = 0; y < outputHeight; ++y)                                          // Iterate over pixels of output bitmap
                {
                    for (int x = 0; x < outputWidth; ++x)
                    {
                        pOut[x + y * strideOut] = pIn[(x + vals[2]) +
                                                      (y + vals[0]) * strideIn];                // Copy pixel values from input image, starting at boundary offsets
                    }
                }
            }

            input.UnlockBits(inputData);                                                        // Unlock bitmaps data from memory
            output.UnlockBits(outputData);
            return output;                                                                      // Return output image
        }
        public static Bitmap Threshold(Bitmap b, int nVal)                                     // Binarization method using threshold nVal
        {
            Bitmap newImage;                                                                  // Initialize output bitmap
            if (b.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                newImage = (Bitmap)b.Clone();                                                 // If input is 8bit, then output is a manipulated version of input
            }
            else if (b.PixelFormat == PixelFormat.Format24bppRgb)
            {
                newImage = Convert24bitTo8Bit(b);                                                   // If input is 24bit, then output bitmap is a manipulated version of converted input
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
        private static Bitmap Convert24bitTo8Bit(Bitmap b)                                      // Converts 24bit bitmap to 8bit
        {
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
                        ++pNew;                                                                 // Iterate to the next 8bit pixel
                    }
                    p += nOffset24;                                                             // Forgo offset at end of bitmap width 
                    pNew += nOffset8;
                }
            }
            b.UnlockBits(bmpData);                                                              // Unlock bitmap data from memory
            bit8.UnlockBits(newBmpData);
            return bit8;                                                                        // Return 8bit image
        }
        private static Bitmap Convert1BitTo8Bit(Bitmap b)                                       // Converts 1bit bitmap to 8bit
        {
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
