using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

namespace Task5
{
    public class BitmapFilter
    {
        private static Bitmap Convert24BitTo8Bit(Bitmap b)                                      // Converts 24bit bitmap to 8bit
        {
            Bitmap bit8 = new Bitmap(b.Width, b.Height, PixelFormat.Format8bppIndexed);         // Create 8bit Bitmap for new Image

            ColorPalette palette = bit8.Palette;                                                // Set color palette of new image to gray scale
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            bit8.Palette = palette;

            BitmapData bmpData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),         // Lock input bitmap data into memory
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
        private static Bitmap Convert1BitTo8Bit(Bitmap b)                                        // Converts 1bit bitmap to 8bit
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
        private static Bitmap AddZeroPadding(Bitmap b)                                          // Add zero padding around 8bit bitmap
        {
            int width = b.Width;                                                                // Assign input width and height to variables
            int height = b.Height;

            Bitmap paddedIm = new Bitmap(width + 2, height + 2,                      // Create new bitmap with padding layer
                PixelFormat.Format8bppIndexed);

            ColorPalette palette = paddedIm.Palette;                                            // Set color palette of new bitmap to gray scale 
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            paddedIm.Palette = palette;

            BitmapData bmpData0 = b.LockBits(new Rectangle(0, 0, width, height),            // Lock bitmaps data into memory
                ImageLockMode.ReadOnly,
                PixelFormat.Format8bppIndexed);
            BitmapData bmpData1 = paddedIm.LockBits(new Rectangle(0, 0, paddedIm.Width, 
                    paddedIm.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            IntPtr scan0 = bmpData0.Scan0;                                                      // Retrieve first pixel data address
            IntPtr scan1 = bmpData1.Scan0;
            int stride0 = bmpData0.Stride;                                                      // Assign stride to integer variable
            int stride1 = bmpData1.Stride;

            unsafe
            {
                byte* p0 = (byte*)(void*)scan0;                                                 // Cast address to byte pointer
                byte* p1 = (byte*)(void*)scan1;

                for (int y = 0; y < paddedIm.Height; y++)                                       // Iterate over padded image
                {
                    for (int x = 0; x < paddedIm.Width; x++)
                    {   
                        if (x == 0 || x == paddedIm.Width - 1)                                  // Set pixels in first and last column to black
                        {
                            p1[(x) + (y) * stride1] = 0;
                        }
                        else if (y == 0 || y == paddedIm.Height - 1)                            // Set pixels in first and last row to black
                        {
                            p1[(x) + (y) * stride1] = 0;
                        }
                        else
                        {
                            p1[(x) + (y) * stride1] = p0[(x-1) + (y-1) * stride0];              // Fill in input bitmap in center of padded image
                        }
                    }
                }
            }

            b.UnlockBits(bmpData0);                                                             // Unlock bitmap data and return padded image
            paddedIm.UnlockBits(bmpData1);
            return paddedIm;
        }
        private static Bitmap Add255Padding(Bitmap b)                                           // Add white padding around 8bit bitmap
        {
            int width = b.Width;                                                                // Assign input width and height to variables
            int height = b.Height;

            Bitmap paddedIm = new Bitmap(width + 2, height + 2,                      // Create new bitmap with padding layer
                PixelFormat.Format8bppIndexed);

            ColorPalette palette = paddedIm.Palette;                                            // Set color palette of new bitmap to gray scale 
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            paddedIm.Palette = palette;

            BitmapData bmpData0 = b.LockBits(new Rectangle(0, 0, width, height),            // Lock bitmaps data into memory
                ImageLockMode.ReadOnly,
                PixelFormat.Format8bppIndexed);
            BitmapData bmpData1 = paddedIm.LockBits(new Rectangle(0, 0, paddedIm.Width, 
                    paddedIm.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            IntPtr scan0 = bmpData0.Scan0;                                                      // Retrieve first pixel data address
            IntPtr scan1 = bmpData1.Scan0;
            int stride0 = bmpData0.Stride;                                                      // Assign stride to integer variable
            int stride1 = bmpData1.Stride;

            unsafe
            {
                byte* p0 = (byte*)(void*)scan0;                                                 // Cast address to byte pointer
                byte* p1 = (byte*)(void*)scan1;

                for (int y = 0; y < paddedIm.Height; y++)                                       // Iterate over padded image
                {
                    for (int x = 0; x < paddedIm.Width; x++)
                    {   
                        if (x == 0 || x == paddedIm.Width - 1)                                  // Set pixels in first and last column to white
                        {
                            p1[(x) + (y) * stride1] = 255;
                        }
                        else if (y == 0 || y == paddedIm.Height - 1)                            // Set pixels in first and last row to white
                        {
                            p1[(x) + (y) * stride1] = 255;
                        }
                        else
                        {
                            p1[(x) + (y) * stride1] = p0[(x-1) + (y-1) * stride0];              // Fill in input bitmap in center of padded image
                        }
                    }
                }
            }

            b.UnlockBits(bmpData0);                                                             // Unlock bitmap data and return padded image
            paddedIm.UnlockBits(bmpData1);
            return paddedIm;
        } 
        public static Bitmap Morphology(Bitmap b, int row, int col, bool condition)              // Dilation of bitmap using kernel size as input
        {
            if ((row % 2 == 0) || (col % 2 == 0))                                                // Kernel must have odd rows and columns to ensure presence of center kernel
            {
                throw new ArgumentException("Structuring Element is not valid. " +
                                            "Number of rows and columns must be odd");
            }
            if (row > b.Width/3)                                                                // Ensure kernel size is within acceptable boundaries. Boundary was arbitrarily set. 
            {
                throw new ArgumentException("Row size is too large");
            }
            if (col > b.Height/3)
            {
                throw new ArgumentException(" Column size is too large");
            }
            
            Bitmap newImage;                                                                    // Intialize new bitmap for output
            if (b.PixelFormat == PixelFormat.Format8bppIndexed)                                 // Ensure new bitmap is an 8bit replica of input image
            {
                newImage = (Bitmap)b.Clone();
            }
            else if (b.PixelFormat == PixelFormat.Format1bppIndexed)
            {
                newImage = Convert1BitTo8Bit(b); // Make name specific
            }
            else if (b.PixelFormat == PixelFormat.Format24bppRgb)
            {
                newImage = Convert24BitTo8Bit(b);
            }
            else
            {
                throw new ArgumentException(" Image type is not supported. Must be 1, 8, or 24bit");
            }

            Bitmap paddedIm;
            if (condition)
                paddedIm = AddZeroPadding(newImage);                                             // Add padding around 8bit image for reading
            else
                paddedIm = Add255Padding(newImage);                                              // Add padding around 8bit image for reading
            
            BitmapData bmData = paddedIm.LockBits(new Rectangle(0, 0, paddedIm.Width,        // Lock bitmap data into memory
                    paddedIm.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            BitmapData newbmData = newImage.LockBits(new Rectangle(0, 0, newImage.Width, 
                    newImage.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            System.IntPtr Scan0 = bmData.Scan0;                                                  // Retrieve first pixel data address
            System.IntPtr Scan1 = newbmData.Scan0;
            
            int stride0 = bmData.Stride;                                                         // Assign bitmap strides to 
            int stride1 = newbmData.Stride;

            int nOffset1 = stride1 - newImage.Width;                                             // Calculate output image offset

            int height = newImage.Height;                                                        // Assign output image parameters to integer variables
            int width = newImage.Width;
            byte[] group = new byte [row*col];                                                   // Create a byte matrix with size of kernel
            
            unsafe
            {
                byte* p = (byte*)(void*)Scan0;                                                   // Pointer to padded image for reading
                byte* pNew = (byte*)(void*)Scan1;                                                // Pointer to image being manipulated
                for (int y = 0; y < height; ++y)                                                 // Iterate over pixels of output image
                {
                    for (int x = 0; x < width; ++x)
                    {
                        int byteNum = x + y * stride0;
                        int num = 0;
                        for (int i = 0; i < row; ++i)                                            // Iterate over entries in byte matrix
                        {
                            for (int j = 0; j < col; ++j)
                            {
                                group[num] = p[byteNum + i + j * stride0];                       // Copy the first i by j pixels into the byte matrix
                                ++num;
                            }
                        }
                        if (condition) 
                            pNew[0] = group.Max();                                              // Assign the max byte to the corresponding byte in the output image
                        else
                            pNew[0] = group.Min(); 
                        ++pNew;                                                                 // Increment to the next byte
                    }
                    pNew += nOffset1;                                                           // Skip offset
                }
            }
            paddedIm.UnlockBits(bmData);                                                        // Unlock bitmaps data and return dilated image
            newImage.UnlockBits(newbmData);
            return newImage;
        }
        
    }
}



 
