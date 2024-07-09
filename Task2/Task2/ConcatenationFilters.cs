using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace Task2
{
    public class BitmapFilter
    {
        public static Bitmap HorizontalCon(Bitmap b, Bitmap c)                                  // Horizontal concatenation 
        {
            Bitmap b8;                                                                          // Initialize input bitmaps
            Bitmap c8;
            
            if (b.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                b8 = b;
            }                            // Check pixel formats of both input bitmaps, and ensure final 8bit format
            else if (b.PixelFormat == PixelFormat.Format1bppIndexed)
            {
                b8 = Convert1BitTo8Bit(b);
            }
            else if (b.PixelFormat == PixelFormat.Format24bppRgb)
            {
                b8 = Convert24BitTo8Bit(b);
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
                c8 = Convert1BitTo8Bit(c);
            }
            else if (c.PixelFormat == PixelFormat.Format24bppRgb)
            {
                c8 = Convert24BitTo8Bit(c);
            }
            else
            {
                throw new ArgumentException("Images must be in 24bit or 8bit format.");
            }
            
            Bitmap newImage = new Bitmap(b8.Width + c8.Width,                              // Initialize new bitmap with combined with and maximal height
                Math.Max(b8.Height,c8.Height), PixelFormat.Format8bppIndexed);
            
            ColorPalette palette = newImage.Palette;                                            // Set color palette of new image to grayscale
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            newImage.Palette = palette;
            
            BitmapData bmDataB = b8.LockBits(new Rectangle(0, 0, b8.Width, b8.Height),      // Lock into memory input and output bitmaps
                ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            BitmapData bmDataC = c8.LockBits(new Rectangle(0, 0, c8.Width, c8.Height), 
                ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            BitmapData bmDataN = newImage.LockBits(new Rectangle(0, 0, newImage.Width, 
                    newImage.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            System.IntPtr ScanB = bmDataB.Scan0;                                                // Retrieve address for first pixel data of input images (B, C) and output (N)
            System.IntPtr ScanC = bmDataC.Scan0;
            System.IntPtr ScanN = bmDataN.Scan0;
            
            int widthB = b8.Width; int heightB = b8.Height; int strideB = bmDataB.Stride;       // Assign all bitmap parameters (width, height, stride) to integer variables
            int widthC = c8.Width; int heightC = c8.Height; int strideC = bmDataC.Stride;
            int widthN = newImage.Width; int heightN = newImage.Height; int strideN = bmDataN.Stride;

            unsafe
            {
                byte* pB = (byte*)(void*)ScanB;                                                 // Cast addresses to byte pointers
                byte* pC = (byte*)(void*)ScanC;
                byte* pN = (byte*)(void*)ScanN;
                
                for (int y = 0; y < heightN; ++y)                                               // Iterate over output image pixels (Has combined parameters)
                {
                    for (int x = 0; x < widthN; ++x)
                    {
                        if (x < widthB & y < heightB)                                           // In an area the size of bitmap b's width and height, draw image b, using x to indicate 
                                                                                                // the byte number in a scanline, and 'y*stride' to indicate the scanline number
                        {
                            pN[x + y * strideN] = pB[x + y * strideB];
                        }
                        else if (x >= widthB & y < heightC)                                     // Repeat process for area the size of bitmap c
                        {
                            pN[x + y * strideN] = pC[(x - widthB) + y * strideC];               // subtract widthB from x to begin iterating from first byte of c. 
                                                                                                // No need to subtract from pN, since we are drawing c next to b
                        }
                        else
                        {
                            pN[x + y * strideN] = 255;                                          // Fill in remaining bytes with a white pixel
                        }
                    }
                }
                // Alternate method for concatenating images using two nested for loops
                // In first nested for-loop, iterate over bitmap b and copy each pixel into one side of bitmap n
                // In second nested for-loop, iterate over bitmap c and copy each pixel into the left of b 
                /*
                for (int y = 0; y < heightB; ++y)                                       
                {
                    for (int x = 0; x < widthB; ++x)
                    {
                        pN[x + y * strideN] = pB[x + y * strideB];
                    }
                }
                for (int y = 0; y < heightC; ++y)
                {
                    for (int x = 0; x < widthC; ++x)
                    {
                        pN[(widthB+ x) + y * strideN] = pC[x + y * strideC];
                    }
                }
                */
            }
            b8.UnlockBits(bmDataB);                                                             // Unlock bitmap data from memory and return concatenated image
            c8.UnlockBits(bmDataC);
            newImage.UnlockBits(bmDataN);
            return newImage;
        }
        public static Bitmap VerticalCon(Bitmap b, Bitmap c)                                    // Veritical Concatenation
        {
            Bitmap b8;                                                                          // Initialize input bitmaps for reading
            Bitmap c8;
            if (b.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                b8 = b;
            }                            // Check pixel formats of both input bitmaps, and ensure final 8bit format
            else if (b.PixelFormat == PixelFormat.Format1bppIndexed)
            {
                b8 = Convert1BitTo8Bit(b);
            }
            else if (b.PixelFormat == PixelFormat.Format24bppRgb)
            {
                b8 = Convert24BitTo8Bit(b);
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
                c8 = Convert1BitTo8Bit(c);
            }
            else if (c.PixelFormat == PixelFormat.Format24bppRgb)
            {
                c8 = Convert24BitTo8Bit(c);
            }
            else
            {
                throw new ArgumentException("Images must be in 24bit or 8bit format.");
            }

            Bitmap newImage = new Bitmap(Math.Max(b8.Width, c8.Width),                     // Create new larger bitmap with combined height and max width             
                b8.Height + c8.Height, PixelFormat.Format8bppIndexed);
            
            ColorPalette palette = newImage.Palette;                                            // Set color palette of new image to gray scale
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            newImage.Palette = palette;
            
            BitmapData bmDataB = b8.LockBits(new Rectangle(0, 0, b8.Width, b8.Height),      // Lock bitmaps data into memory
                ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            BitmapData bmDataC = c8.LockBits(new Rectangle(0, 0, c8.Width, c8.Height), 
                ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            BitmapData bmDataN = newImage.LockBits(new Rectangle(0, 0, newImage.Width, 
                newImage.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            System.IntPtr ScanB = bmDataB.Scan0;                                                // Retrieve addresses of first pixel data
            System.IntPtr ScanC = bmDataC.Scan0;
            System.IntPtr ScanN = bmDataN.Scan0;
            
            int widthB = b8.Width; int heightB = b8.Height; int strideB = bmDataB.Stride;       // Assign bitmap parameters to integer variables
            int widthC = c8.Width; int heightC = c8.Height; int strideC = bmDataC.Stride;
            int widthN = newImage.Width; int heightN = newImage.Height; int strideN = bmDataN.Stride;

            unsafe
            {
                byte* pB = (byte*)(void*)ScanB;                                                 // Cast addresses to byte pointers
                byte* pC = (byte*)(void*)ScanC;
                byte* pN = (byte*)(void*)ScanN;
                
                for (int y = 0; y < heightN; ++y)                                               // Iterate over large output bitmap
                {
                    for (int x = 0; x < widthN; ++x)
                    {
                        if (x < widthB & y < heightB)
                        {
                            pN[x + y * strideN] = pB[x + y * strideB];                          // In area the size of bitmap b, draw b
                        }
                        else if (x < widthC & y >= heightB)
                        {
                            pN[(x + y * strideN)] = pC[x + (y-heightB) * strideC];              // Draw c next to b
                        }
                        else
                        {
                            pN[(x + y * strideN)] = 255;                                        // Fill in white everywhere else
                        }
                    }
                }
                //// Alternate method for concatenation using two nested for-loops
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
            
            return newImage;
        }
        private static Bitmap Convert1BitTo8Bit(Bitmap b)                                              // Converts 1bit bitmap to 8bit
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
        private static Bitmap Convert24BitTo8Bit(Bitmap b)                                            // Converts 24bit bitmap to 8bit
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

    }
}

 