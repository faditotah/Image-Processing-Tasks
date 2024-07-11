using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Task7
{
    public class BitmapFilter
    {
        public static Bitmap Convert24BitTo8Bit(Bitmap b)                                                               // Converts 24bit bitmap to 8bit
        {
            Bitmap bit8 = new Bitmap(b.Width, b.Height, PixelFormat.Format8bppIndexed);                                 // Create 8bit Bitmap for new Image

            ColorPalette palette = bit8.Palette;                                                                        // Set color palette of new image to gray scale
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }

            bit8.Palette = palette;

            BitmapData bmpData = b.LockBits(
                new Rectangle(0, 0, b.Width, b.Height),                                                             // Lock input bitmap data into memory
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);
            BitmapData newBmpData = bit8.LockBits(
                new Rectangle(0, 0, b.Width, b.Height),                                                             // Lock output bitmap data into memory
                ImageLockMode.WriteOnly,
                PixelFormat.Format8bppIndexed);

            IntPtr Scan0 = bmpData.Scan0;                                                                               // Retrieve address for first pixel data
            IntPtr Scan1 = newBmpData.Scan0;
            int height = b.Height;
            int width = b.Width;                                                                                        // Assign bMap parameters to integer variables
            int nOffset8 = newBmpData.Stride - bit8.Width;                                                              // Calculate offset for 8bit output
            int nOffset24 = bmpData.Stride - b.Width * 3;                                                               // Calculate offset for 24bit input
            unsafe
            {
                byte* p = (byte*)(void*)Scan0;                                                                          // Cast scan0 to byte pointers
                byte* pNew = (byte*)(void*)Scan1;

                for (int y = 0; y < height; y++)                                                                        // Iterate over the pixels of the input image
                {
                    for (int x = 0; x < width; x++)
                    {
                        pNew[0] = (byte)(0.299 * p[2] + 0.587 * p[1] + 0.114 * p[0]);                                   // Set the value of each pixel (byte) of the 8bit image to the
                                                                                                                        // corresponding grayscale value from the 24bit image
                        p += 3;                                                                                         // Increment to the next 24bit pixel
                        ++pNew;                                                                                         // Iterate to the next 8bit pixel
                    }
                    p += nOffset24;                                                                                     // Skip offset at end of bitmap width 
                    pNew += nOffset8;
                }
            }

            b.UnlockBits(bmpData);                                                                                      // Unlock bitmap data from memory
            bit8.UnlockBits(newBmpData);
            return bit8;                                                                                                // Return 8bit image
        }
        public static Bitmap Convert1BitTo8Bit(Bitmap b)                                                                // Converts 1bit bitmap to 8bit
        {
            int width = b.Width;                                                                                        // Assign width and height of 1bit input to int variables                                                         
            int height = b.Height;

            Bitmap newImage =
                new Bitmap(width, height, PixelFormat.Format8bppIndexed);                                               // Create new 8bit indexed bitmap for drawing

            ColorPalette palette = newImage.Palette;                                                                    // Set color palette of output image to grayscale
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }

            newImage.Palette = palette;

            BitmapData bmpData = b.LockBits(new Rectangle(0, 0, width, height),                                     // Lock input and output bitmap data into memory
                ImageLockMode.ReadOnly, PixelFormat.Format1bppIndexed);
            BitmapData newBmpData = newImage.LockBits(new Rectangle(0, 0, width,
                height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            int stride1 = bmpData.Stride;                                                                               // Assign stride to integer variables
            int stride8 = newBmpData.Stride;
            int nOffset = stride8 - newImage.Width;                                                                     // Determine 8bit offset 
            System.IntPtr Scan0 = bmpData.Scan0;                                                                        // Retrieve address for first pixel data of both bitmaps
            System.IntPtr Scan1 = newBmpData.Scan0;


            unsafe
            {
                byte* p = (byte*)(void*)Scan0;                                                                          // Cast addresses to byte pointers
                byte* pNew = (byte*)(void*)Scan1;
                for (int y = 0; y < height; y++)                                                                        // Iterate over the pixels of the bitmap
                {
                    for (int x = 0; x < width; x++)
                    {
                        int index = x / 8;                                                                              // Determine which byte a bit is located in at a certain pixel, given 8bits in a byte
                        int bitOffset =
                            7 - (x % 8);                                                                                // Determine the position of the bit within the byte. x%8 gives the position of the bit in the byte(0-7)
                                                                                                                        // Subtracting the mode from 7 gives the bit offset within the byte (bit order 0-7)

                        byte bitValue =
                            (byte)((p[index] >> bitOffset) & 0x01);                                                     // We use the index to shift the bits in the byte by the offset, so that the bit at pixel x reaches the 
                                                                                                                        // rightmost position in the byte (LSB). 
                                                                                                                        // Using AND to mask every bit except LSB, returns 1 if bit is 1, and 0 if bit is 0. Cast to byte
                        pNew[0] = (byte)(bitValue * 255);                                                               // Multiply bit by 255, and assign to corresponding 8bit pixel
                        ++pNew;                                                                                         // Increment to next 8bti pixel                                            
                    }
                    p += stride1;                                                                                       // Move to next scanline at end of pixel row          
                    pNew += nOffset;                                                                                    // Skip offset at end of pixel row
                }
            }
            b.UnlockBits(bmpData);                                                                                      // Unlock data and return 8bit image
            newImage.UnlockBits(newBmpData);
            return newImage;
        }
        public static Bitmap Resize(Bitmap b, int nVal, bool condition)                                                 // Rescale images to best fit using Bilinear Interpolation
        {
            Bitmap input;                                                                                               // Initialize input bitmap for reading                            
            if (b.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                input = b;
            }                                                    // Ensure Format of input is 8bit                        
            else if (b.PixelFormat == PixelFormat.Format1bppIndexed)
            {
                input = Convert1BitTo8Bit(b); 
            }
            else if (b.PixelFormat == PixelFormat.Format24bppRgb)
            {
                input = Convert24BitTo8Bit(b);
            }
            else
            {
                throw new ArgumentException("Image type is not supported. Must be 1, 8, or 24bit");
            }

            int width = input.Width;                                                                                    // Assign input dimensions to integer variables
            int height = input.Height;

            int nWidth, nHeight;                                                                                        // Initialize output dimensions
            if (condition)                                                                                              // Bool statement depending on whether width or height is being rescaled
            {
                nWidth = (int)((nVal / 100.0) * width);                                                                 // Width is manipulated based on %nVal
                nHeight = nWidth / (width / height);                                                                    // Height is adjusted accordingly to maintain aspect ratio
            }
            else
            {
                nHeight = (int)((nVal / 100.0) * height);                                                               // Height is manipulated based on %nVal
                nWidth = nHeight * (width / height);                                                                    // Width is adjusted accordingly to maintain aspect ratio
            }

            double nXFactor = (double)width / nWidth;                                                                   // Calculate ratio of input to output dimensions
            double nYFactor = (double)height / nHeight;
            Bitmap output =
                new Bitmap(nWidth, nHeight,
                    PixelFormat.Format8bppIndexed);                                                                     // Initialize output bitmap with adjusted dimensions
            ColorPalette palette = output.Palette;                                                                      // Set color palette to grayscale    
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            output.Palette = palette;

            BitmapData inData = input.LockBits(new Rectangle(0, 0, input.Width,                                     // Lock input and output bitmap data into memory
                input.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            BitmapData outData = output.LockBits(new Rectangle(0, 0, output.Width,
                output.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            IntPtr scanIn = inData.Scan0;                                                                               // Retrieve first pixel data address
            IntPtr scanOut = outData.Scan0;

            int strideIn = inData.Stride;                                                                               // Assign strides to integer variables
            int strideOut = outData.Stride;

            double left, up, right, down;                                                                               // Initialize distances to 4 coordinates (weights)
            int ceil_x, ceil_y, floor_x, floor_y;                                                                       // Initialize integer variables for coordinates
            byte p1, p2, p3, p4;                                                                                        // Initialize variables to hold weighted contributions

            unsafe
            {
                byte* pIn = (byte*)(void*)scanIn;                                                                       // Cast addresses to byte pointers
                byte* pOut = (byte*)(void*)scanOut;

                for (int y = 0; y < nHeight; ++y)                                                                       // Iterate over output image
                {
                    int byteLine = y * strideOut;                                                                       // Initialize number of pixel rows to skip when assigning values to output
                    for (int x = 0; x < nWidth; ++x)
                    {
                        // Coordinates of 4 values
                        floor_x = (int)Math.Floor(x * nXFactor);                                                        // Determine floor x coordinate (left coordinates)
                        floor_y = (int)Math.Floor(y * nYFactor);                                                        // Determine floor y coordinate (top coordinates)

                        ceil_x = floor_x + 1;                                                                           // Determine ceiling coordinates and set cutoff
                        if (ceil_x >= width) ceil_x = floor_x;
                        ceil_y = floor_y + 1;
                        if (ceil_y >= height) ceil_y = floor_y;
                        
                        left = x * nXFactor - floor_x;                                                                  // Determine distance of x/y coordinate to floor coordinate
                        up = y * nYFactor - floor_y;
                        right = 1.0 - left;                                                                             // Determine distance of x/y coordinates to ceiling coordinates
                        down = 1.0 - up;
                        
                        // Retrieving 4 values & Multiplying by weights
                        p1 = (byte)(pIn[ceil_x + ceil_y * strideIn] * left * up);                                       // Determine contribution of each coordinate by multiplying its intensity with the diagonally opposite area
                        p2 = (byte)(pIn[ceil_x + floor_y * strideIn] * left * down);
                        p3 = (byte)(pIn[floor_x + ceil_y * strideIn] * right * up);
                        p4 = (byte)(pIn[floor_x + floor_y * strideIn] * right * down);
                        pOut[x + byteLine] = (byte)(p1 + p2 + p3 + p4);                                                 // Sum up contributions of each coordinate
                    }
                }
            }

            input.UnlockBits(inData);
            output.UnlockBits(outData);
            return output;
        }
    }
}