using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Emit;

namespace CompiledTasks
{
    public class BitmapFilter
    {
        // Task 1: Image Binarization using Static and Mean Thresholds //
        public static int Mean(Bitmap b) // Method calculates mean intensity of 8bit grayscale images
        {
            if (b.PixelFormat == PixelFormat.Format24bppRgb) // If input is in 24bit, use previous method to convert to 8bit
            {
                b = Convert24BitTo8Bit(b);
            }
            else if (b.PixelFormat == PixelFormat.Format8bppIndexed) // Ensures 8bit format is acceptable
            {
            }
            else
            {
                // Only 24bit or 8bit inputs are acceptable
                throw new ArgumentException("Image must be 24bit or 8bit grayscale");
            }

            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), // Locks bitmap data into memory
                ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            IntPtr scan0 = bmData.Scan0; // Retrieves address of first pixel data
            int nOffset = bmData.Stride - b.Width; // Assigns offset to integer vairable
            int height = b.Height; // Assign bitmap width and height to variables
            int width = b.Width;
            int sum = 0; // Initialize sum to zero
            int numPix = width * height; // Number of pixels in bitmap
            unsafe
            {
                byte* p = (byte*)(void*)scan0; // Cast address to byte pointer
                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0;
                         x < width;
                         ++x) // Iterate over pixels in each row and add intensity value to running sum
                    {
                        sum += p[0];
                        ++p;
                    }

                    p += nOffset;
                }
            }

            b.UnlockBits(bmData);
            int meanInt = sum / numPix; // Calculate intensity using summed intensity values and number of pixels
            return meanInt; // Return mean value
        }
        public static Bitmap Threshold(Bitmap b, int nVal) // Binarization method using threshold nVal
        {
            Bitmap newImage; // Initialize output bitmap
            if (b.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                newImage = (Bitmap)b.Clone(); // If input is 8bit, then output is a manipulated version of input
            }
            else if (b.PixelFormat == PixelFormat.Format24bppRgb)
            {
                newImage = Convert24BitTo8Bit(
                    b); // If input is 24bit, then output bitmap is a manipulated version of converted input
            }
            else
            {
                throw new ArgumentException("Image must be 24bit or 8bit grayscale");
            }

            BitmapData bmData = newImage.LockBits(new Rectangle(0, 0, newImage.Width, // Lock bitmap data into memory
                newImage.Height), ImageLockMode.ReadWrite, newImage.PixelFormat);

            IntPtr Scan2 = bmData.Scan0; // Obtain address of first pixel data
            int nOffset = bmData.Stride - newImage.Width; // Assign offset to integer variable
            int height = newImage.Height;
            int width = newImage.Width; // Assign bitmap width and height to integers
            unsafe
            {
                byte* p = (byte*)(void*)Scan2; // Cast address to byte pointer
                for (int y = 0; y < height; ++y) // Iterate over pixels in bitmap
                {
                    for (int x = 0; x < width; ++x)
                    {
                        p[0] = (p[0] >= nVal)
                            ? (byte)255
                            : (byte)0; // Check value of each byte (pixel), then assign it to either black or white
                        ++p; // Increment to next byte
                    }

                    p += nOffset; // Skip offset at end of bitmap row
                }
            }

            newImage.UnlockBits(bmData); // Unlock bitmap data from memory
            return newImage; // Return binarized imag
        }

        // Task 2: Horizontal and Vertical Concatenation
        public static Bitmap HorizontalCon(Bitmap b, Bitmap c, bool condition) // Horizontal concatenation 
        {
            Bitmap b8; // Initialize input bitmaps
            Bitmap c8;

            if (b.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                b8 = b;
            } // Check pixel formats of both input bitmaps, and ensure final 8bit format
            else if (b.PixelFormat == PixelFormat.Format1bppIndexed || b.PixelFormat == PixelFormat.Format24bppRgb || b.PixelFormat == PixelFormat.Format32bppRgb )
            {
                b8 = ConvertTo8Bit(b); // Make name specific
            }
            else
            {
                throw new ArgumentException("Images must be in 24bit or 8bit format.");
            }

            if (c.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                c8 = c;
            }
            else if (c.PixelFormat == PixelFormat.Format1bppIndexed || c.PixelFormat == PixelFormat.Format24bppRgb|| c.PixelFormat == PixelFormat.Format32bppRgb)
            {
                c8 = ConvertTo8Bit(c); // Make name specific
            }
            else
            {
                throw new ArgumentException("Images must be in 24bit or 8bit format.");
            }

            Bitmap newImage;
            
            if (condition)
            {
                newImage = new Bitmap(b8.Width + c8.Width, // Initialize new bitmap with combined with and maximal height
                                Math.Max(b8.Height, c8.Height), PixelFormat.Format8bppIndexed);
            }
            else
            {
                newImage = new Bitmap(Math.Max(b8.Width, c8.Width), // Create new larger bitmap with combined height and max width             
                    b8.Height + c8.Height, PixelFormat.Format8bppIndexed);
            }

            ColorPalette palette = newImage.Palette; // Set color palette of new image to grayscale
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            newImage.Palette = palette;

            BitmapData bmDataB = b8.LockBits(new Rectangle(0, 0, b8.Width, b8.Height), // Lock into memory input and output bitmaps
                ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            BitmapData bmDataC = c8.LockBits(new Rectangle(0, 0, c8.Width, c8.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            BitmapData bmDataN = newImage.LockBits(new Rectangle(0, 0, newImage.Width,
                newImage.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            System.IntPtr ScanB = bmDataB.Scan0; // Retrieve address for first pixel data of input images (B, C) and output (N)
            System.IntPtr ScanC = bmDataC.Scan0;
            System.IntPtr ScanN = bmDataN.Scan0;

            int widthB = b8.Width;
            int heightB = b8.Height;
            int strideB = bmDataB.Stride; // Assign all bitmap parameters (width, height, stride) to integer variables
            int widthC = c8.Width;
            int heightC = c8.Height;
            int strideC = bmDataC.Stride;
            int widthN = newImage.Width;
            int heightN = newImage.Height;
            int strideN = bmDataN.Stride;

            unsafe
            {
                byte* pB = (byte*)(void*)ScanB; // Cast addresses to byte pointers
                byte* pC = (byte*)(void*)ScanC;
                byte* pN = (byte*)(void*)ScanN;
                
                for (int y = 0; y < heightN; ++y) // Iterate over output image pixels (Has combined parameters)
                {
                    for (int x = 0; x < widthN; ++x)
                    {
                        if (condition)
                        {
                            if (x < widthB & y < heightB) // In an area the size of bitmap b's width and height, draw image b, using x to indicate 
                                // the byte number in a scanline, and 'y*stride' to indicate the scanline number
                            {
                                pN[x + y * strideN] = pB[x + y * strideB];
                            }
                            else if (x >= widthB & y < heightC) // Repeat process for area the size of bitmap c
                            {
                                pN[x + y * strideN] = pC[(x - widthB) + y * strideC]; // subtract widthB from x to begin iterating from first byte of c. 
                                // No need to subtract from pN, since we are drawing c next to b
                            }
                            else
                            {
                                pN[x + y * strideN] = 255; // Fill in remaining bytes with a white pixel
                            }
                        }
                        else
                        {
                            if (x < widthB & y < heightB)
                            {
                                pN[x + y * strideN] = pB[x + y * strideB]; // In area the size of bitmap b, draw b
                            }
                            else if (x < widthC & y >= heightB)
                            {
                                pN[(x + y * strideN)] = pC[x + (y - heightB) * strideC]; // Draw c next to b
                            }
                            else
                            {
                                pN[(x + y * strideN)] = 255; // Fill in white everywhere else
                            }
                        }
                        

                    }
                }
            }

            b8.UnlockBits(bmDataB); // Unlock bitmap data from memory and return concatenated image
            c8.UnlockBits(bmDataC);
            newImage.UnlockBits(bmDataN);
            return newImage;
        }
        
        // Task 3: Convert 24bit Bitmap to 8bit //
        public static Bitmap Convert24BitTo8Bit(Bitmap b) // Converts 24bit bitmap to 8bit
        {
            /*
            if (b.PixelFormat != PixelFormat.Format24bppRgb) // Check input before proceeding
            {
                throw new ArgumentException("Filter can only convert 24bit images");
            }
            */

            Bitmap bit8 =
                new Bitmap(b.Width, b.Height, PixelFormat.Format8bppIndexed); // Create 8bit Bitmap for new Image

            ColorPalette palette = bit8.Palette; // Set color palette of new image to gray scale
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }

            bit8.Palette = palette;

            BitmapData bmpData = b.LockBits(
                new Rectangle(0, 0, b.Width, b.Height), // Lock input bitmap data into memory
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);
            BitmapData newBmpData = bit8.LockBits(
                new Rectangle(0, 0, b.Width, b.Height), // Lock output bitmap data into memory
                ImageLockMode.WriteOnly,
                PixelFormat.Format8bppIndexed);

            IntPtr Scan0 = bmpData.Scan0; // Retrieve address for first pixel data
            IntPtr Scan1 = newBmpData.Scan0;
            int height = b.Height;
            int width = b.Width; // Assign bMap parameters to integer variables
            int nOffset8 = newBmpData.Stride - bit8.Width; // Calculate offset for 8bit output
            int nOffset24 = bmpData.Stride - b.Width * 3; // Calculate offset for 24bit input
            unsafe
            {
                byte* p = (byte*)(void*)Scan0; // Cast scan0 to byte pointers
                byte* pNew = (byte*)(void*)Scan1;

                for (int y = 0; y < height; y++) // Iterate over the pixels of the input image
                {
                    for (int x = 0; x < width; x++)
                    {
                        pNew[0] = (byte)(0.299 * p[2] + 0.587 * p[1] +
                                         0.114 * p[0]); // Set the value of each pixel (byte) of the 8bit image to the
                        // corresponding grayscale value from the 24bit image
                        p += 3; // Increment to the next 24bit pixel
                        ++pNew; // Iterate to the next 8bit pixel
                    }

                    p += nOffset24; // Forgo offset at end of bitmap width 
                    pNew += nOffset8;
                }
            }

            b.UnlockBits(bmpData); // Unlock bitmap data from memory
            bit8.UnlockBits(newBmpData);
            return bit8; // Return 8bit image
        }

        // Task 4: Convert 1bit Bitmap to 8bit //
        public static Bitmap Convert1BitTo8Bit(Bitmap b) // Converts 1bit bitmap to 8bit
        {
            /*
            if (b.PixelFormat != PixelFormat.Format1bppIndexed) // Check input bitmap format
            {
                throw new ArgumentException(" Input image must be 1bit");
            }
            */
            int
                width = b.Width; // Assign width and height of 1bit input to int variables                                                         
            int height = b.Height;

            Bitmap newImage =
                new Bitmap(width, height, PixelFormat.Format8bppIndexed); // Create new 8bit indexed bitmap for drawing

            ColorPalette palette = newImage.Palette; // Set color palette of output image to grayscale
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }

            newImage.Palette = palette;

            BitmapData bmpData = b.LockBits(
                new Rectangle(0, 0, width, height), // Lock input and output bitmap data into memory
                ImageLockMode.ReadOnly, PixelFormat.Format1bppIndexed);
            BitmapData newBmpData = newImage.LockBits(new Rectangle(0, 0, width,
                height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            int stride1 = bmpData.Stride; // Assign stride to integer variables
            int stride8 = newBmpData.Stride;
            int nOffset = stride8 - newImage.Width; // Determine 8bit offset 
            System.IntPtr Scan0 = bmpData.Scan0; // Retrieve address for first pixel data of both bitmaps
            System.IntPtr Scan1 = newBmpData.Scan0;


            unsafe
            {
                byte* p = (byte*)(void*)Scan0; // Cast addresses to byte pointers
                byte* pNew = (byte*)(void*)Scan1;
                for (int y = 0; y < height; y++) // Iterate over the pixels of the bitmap
                {
                    for (int x = 0; x < width; x++)
                    {
                        int index = x /
                                    8; // Determine which byte a bit is located in at a certain pixel, given 8bits in a byte
                        int bitOffset =
                            7 - (x % 8); // Determine the position of the bit within the byte. x%8 gives the position of the bit in the byte(0-7)
                        // Subtracting the mode from 7 gives the bit offset within the byte (bit order 0-7)

                        byte bitValue =
                            (byte)((p[index] >> bitOffset) &
                                   0x01); // We use the index to shift the bits in the byte by the offset, so that the bit at pixel x reaches the 
                        // rightmost position in the byte (LSB). 
                        // Using AND to mask every bit except LSB, returns 1 if bit is 1, and 0 if bit is 0. Cast to byte
                        pNew[0] = (byte)(bitValue * 255); // Multiply bit by 255, and assign to corresponding 8bit pixel
                        ++pNew; // Increment to next 8bti pixel                                            
                    }

                    p += stride1; // Move to next scanline at end of pixel row          
                    pNew += nOffset; // Skip offset at end of pixel row
                }
            }

            b.UnlockBits(bmpData); // Unlock data and return 8bit image
            newImage.UnlockBits(newBmpData);
            return newImage;
        }

        // Task 3 and 4 Combined //
        private static Bitmap ConvertTo8Bit(Bitmap input)
        {
            Bitmap output;
            if (input.PixelFormat == PixelFormat.Format24bppRgb || input.PixelFormat == PixelFormat.Format32bppArgb || input.PixelFormat == PixelFormat.Format32bppRgb )
                output = Convert24BitTo8Bit(input);
            else
                output = Convert1BitTo8Bit(input);
            return output;
        }

        // Task 5: Dilation & Erosion //
        private static Bitmap AddPadding(Bitmap b, byte pad) // Add zero padding around 8bit bitmap
        {
            int width = b.Width; // Assign input width and height to variables
            int height = b.Height;

            Bitmap paddedIm = new Bitmap(width + 2, height + 2, // Create new bitmap with padding layer
                PixelFormat.Format8bppIndexed);

            ColorPalette palette = paddedIm.Palette; // Set color palette of new bitmap to gray scale 
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }

            paddedIm.Palette = palette;

            BitmapData bmpData0 = b.LockBits(new Rectangle(0, 0, width, height), // Lock bitmaps data into memory
                ImageLockMode.ReadOnly,
                PixelFormat.Format8bppIndexed);
            BitmapData bmpData1 = paddedIm.LockBits(new Rectangle(0, 0, paddedIm.Width,
                paddedIm.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            IntPtr scan0 = bmpData0.Scan0; // Retrieve first pixel data address
            IntPtr scan1 = bmpData1.Scan0;
            int stride0 = bmpData0.Stride; // Assign stride to integer variable
            int stride1 = bmpData1.Stride;

            unsafe
            {
                byte* p0 = (byte*)(void*)scan0; // Cast address to byte pointer
                byte* p1 = (byte*)(void*)scan1;

                for (int y = 0; y < paddedIm.Height; y++) // Iterate over padded image
                {
                    for (int x = 0; x < paddedIm.Width; x++)
                    {
                        if (x == 0 || x == paddedIm.Width - 1) // Set pixels in first and last column to black
                        {
                            p1[(x) + (y) * stride1] = pad;
                        }
                        else if (y == 0 || y == paddedIm.Height - 1) // Set pixels in first and last row to black
                        {
                            p1[(x) + (y) * stride1] = pad;
                        }
                        else
                        {
                            p1[(x) + (y) * stride1] =
                                p0[(x - 1) + (y - 1) * stride0]; // Fill in input bitmap in center of padded image
                        }
                    }
                }
            }

            b.UnlockBits(bmpData0); // Unlock bitmap data and return padded image
            paddedIm.UnlockBits(bmpData1);
            return paddedIm;
        }
        public static Bitmap Morphology(Bitmap b, int row, int col, bool condition) // Dilation of bitmap using kernel size as input
        {
            if ((row % 2 == 0) ||
                (col % 2 == 0)) // Kernel must have odd rows and columns to ensure presence of center kernel
            {
                throw new ArgumentException("Structuring Element is not valid. " +
                                            "Number of rows and columns must be odd");
            }

            if (row > b.Width / 3) // Ensure kernel size is within acceptable boundaries. Boundary was arbitrarily set. 
            {
                throw new ArgumentException("Row size is too large");
            }

            if (col > b.Height / 3)
            {
                throw new ArgumentException(" Column size is too large");
            }

            Bitmap newImage; // Intialize new bitmap for output
            if (b.PixelFormat == PixelFormat.Format8bppIndexed) // Ensure new bitmap is an 8bit replica of input image
            {
                newImage = (Bitmap)b.Clone();
            }
            else if (b.PixelFormat == PixelFormat.Format1bppIndexed || b.PixelFormat == PixelFormat.Format24bppRgb)
            {
                newImage = ConvertTo8Bit(b); // Make name specific
            }
            else
            {
                throw new ArgumentException(" Image type is not supported. Must be 1, 8, or 24bit");
            }

            Bitmap paddedIm;
            if (condition)
                paddedIm = AddPadding(newImage, 0); // Add padding around 8bit image for reading
            else
                paddedIm = AddPadding(newImage, 255); // Add padding around 8bit image for reading

            BitmapData bmData = paddedIm.LockBits(new Rectangle(0, 0, paddedIm.Width, // Lock bitmap data into memory
                paddedIm.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            BitmapData newbmData = newImage.LockBits(new Rectangle(0, 0, newImage.Width,
                newImage.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            System.IntPtr Scan0 = bmData.Scan0; // Retrieve first pixel data address
            System.IntPtr Scan1 = newbmData.Scan0;

            int stride0 = bmData.Stride; // Assign bitmap strides to 
            int stride1 = newbmData.Stride;

            int nOffset1 = stride1 - newImage.Width; // Calculate output image offset

            int height = newImage.Height; // Assign output image parameters to integer variables
            int width = newImage.Width;
            byte[] group = new byte [row * col]; // Create a byte matrix with size of kernel

            unsafe
            {
                byte* p = (byte*)(void*)Scan0; // Pointer to padded image for reading
                byte* pNew = (byte*)(void*)Scan1; // Pointer to image being manipulated
                for (int y = 0; y < height; ++y) // Iterate over pixels of output image
                {
                    for (int x = 0; x < width; ++x)
                    {
                        int byteNum = x + y * stride0;
                        int num = 0;
                        for (int i = 0; i < row; ++i) // Iterate over entries in byte matrix
                        {
                            for (int j = 0; j < col; ++j)
                            {
                                group[num] = p[byteNum + i + j * stride0]; // Copy the first i by j pixels into the byte matrix
                                ++num;
                            }
                        }

                        if (condition)
                            pNew[0] = group.Max(); // Assign the max byte to the corresponding byte in the output image
                        else
                            pNew[0] = group.Min();
                        ++pNew; // Increment to the next byte
                    }

                    pNew += nOffset1; // Skip offset
                }
            }

            paddedIm.UnlockBits(bmData); // Unlock bitmaps data and return dilated image
            newImage.UnlockBits(newbmData);
            return newImage;
        }

        // Task 6: Removal of White Boundaries //
        private static int[] Offset(Bitmap b, byte nVal) // Determine size of white boundaries
        {
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), // Lock in input bitmap (Bitmap would have already been converted to 8bit)
                ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            int stride = bmData.Stride; // Assign stride to integer variable
            IntPtr scan = bmData.Scan0; // Retrieve address of first pixel data

            int height = b.Height; // Assign height and width to integer variables
            int width = b.Width;

            int[] vals = new int[4]; // Initialize integer array to store top, bottom, left, and right offset values, respectively

            unsafe
            {
                byte* p = (byte*)(void*)scan; // Cast starting address to byte pointer
                // Find top boundary
                bool breakTop = false; // boolean check used to break for-loop
                for (int y = 0; y < height; ++y) // Iterate over bitmap, row by row
                {
                    for (int x = 0; x < width; ++x)
                    {
                        int basePix = x + y*stride;

                        if (p[basePix] == nVal) // Once a non-white pixel is reached
                        {
                            vals[0] = y; // Store the row number at the first position in vals
                            breakTop = true; // change bool status of breakTop to break outer for loop
                            break; // Break inner for-loop
                            
                        }
                    }

                    if (breakTop) // BreakTop is true when a non-white pixel is reached
                    {
                        break; // Hence, outer for-loop is broken
                    }
                }
                // Find bottom boundary
                bool breakBottom = false;
                for (int y = height - 1; y >= 0; --y) // Iterate over bitmap beginning from bottom row and going up
                {
                    for (int x = 0; x < width; ++x)
                    {
                        int basePix = x + y*stride;

                        if (p[basePix] == nVal) // Once a non-white pixel is reached
                        {
                            vals[1] = height - 1 - y; // Store bottom row value at second position in vals
                            breakBottom = true; // Break for-loops
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
                for (int x = 0; x < width; ++x) // Iterate bitmap, beginning from left, column by column
                {
                    for (int y = 0; y < height; ++y)
                    {
                        int basePix = x + y*stride;
                        
                        if (p[basePix] == nVal)
                        {
                            vals[2] = x; // Once non-white pixel is reached, store column number at third position in vals
                            breakLeft = true; // Break for-loops
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
                for (int x = width - 1; x >= 0; --x) // Iterate bitmap, beginning from right, column by column
                {
                    for (int y = 0; y < height; ++y)
                    {
                        int basePix = x + y*stride;

                        if (p[basePix] == nVal)
                        {
                            vals[3] = width - 1 - x; // Once non-white pixel is reached, store column number in last position of vals
                            breakRight = true; // Break for-loops
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
        public static Bitmap RemoveWhiteBoundary(Bitmap b, byte nVal) // Remove white boundaries from bitmap
        {
            Bitmap input; 
            
            // Ensure Format is 8Bit
            if (b.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                input = (Bitmap)b.Clone();
            } 
            else if (b.PixelFormat == PixelFormat.Format1bppIndexed || b.PixelFormat == PixelFormat.Format24bppRgb)
            {
                input = ConvertTo8Bit(b); 
            }
            else
            {
                throw new ArgumentException("Image type is not supported. Must be 1, 8, or 24bit");
            }
            // Retrieve size of white boundaries
            int[] vals = Offset(Threshold(input, 250), nVal); // Retrieve the boundary offset values using the binarized version of input
            
            int outputWidth = input.Width - (vals[2] + vals[3]); // Determine size of output image by subtracting boundary offsets from input image parameters
            int outputHeight = input.Height - (vals[0] + vals[1]);

            Bitmap output = new Bitmap(outputWidth, outputHeight, // Create final bitmap with subtracted offsets 
                PixelFormat.Format8bppIndexed);
            
            ColorPalette palette = output.Palette; // Set color palette to grayscale
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            output.Palette = palette;

            BitmapData inputData = input.LockBits(new Rectangle(0, 0, input.Width, // Lock into memory input and output bitmaps
                input.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            BitmapData outputData = output.LockBits(new Rectangle(0, 0, output.Width,
                output.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            int strideIn = inputData.Stride; // Assign stride to integer variables
            int strideOut = outputData.Stride;

            IntPtr inScan = inputData.Scan0; // Retrieve starting pixel address
            IntPtr outScan = outputData.Scan0;

            unsafe
            {
                byte* pIn = (byte*)(void*)inScan; // Cast pixel address to byte pointer
                byte* pOut = (byte*)(void*)outScan;

                for (int y = 0; y < outputHeight; ++y) // Iterate over pixels of output bitmap
                {
                    int byteLineOut = y * strideOut;
                    int byteLineIn = (y + vals[0]) * strideIn;
                    
                    for (int x = 0; x < outputWidth; ++x)
                    {
                        pOut[x + byteLineOut] = pIn[(x + vals[2]) + byteLineIn]; // Copy pixel values from input image, starting at boundary offsets
                    }
                }
            }

            input.UnlockBits(inputData); // Unlock bitmaps data from memory
            output.UnlockBits(outputData);
            return output; // Return output image
        }
                
        // Task 6: Removal of White Boundaries with Speckles //
        private static int[] Offset0(Bitmap b) // Determine size of white boundaries
        {
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), // Lock in input bitmap (Bitmap would have already been converted to 8bit)
                ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            int stride = bmData.Stride; // Assign stride to integer variable
            IntPtr scan = bmData.Scan0; // Retrieve address of first pixel data

            int height = b.Height; // Assign height and width to integer variables
            int width = b.Width;

            int[] vals = new int[4]; // Initialize integer array to store top, bottom, left, and right offset values, respectively
            unsafe
            {
                byte* p = (byte*)(void*)scan; // Cast starting address to byte pointer
                // Find top boundary
                for (int y = 0; y < height; ++y) // Iterate over bitmap, row by row
                {
                    int whiteCount = 0;
                    int blackCount = 0;
                    for (int x = 0; x < width; ++x)
                    {
                        int basePix = x + y*stride;
                        
                        int pix1 = basePix + 1;
                        int pix2 = basePix + 2;
                        int pix3 = basePix + 3;
                        int pix4 = basePix + 4;
                        int pix5 = basePix + 5;
                        if (p[basePix] == 255) whiteCount += 1;
                        
                        if (p[basePix] == 0) // Once a non-white pixel is reached
                        {
                            if (pix1 > width + y * stride) break;
                            
                            if (p[pix1] != 255 && p[pix2] != 255 && p[pix3] != 255 && p[pix4] != 255 && p[pix5] != 255)
                            {
                                vals[0] = y; // Store the row number at the first position in vals
                                goto foundTop;
                            }
                        }
                    }
                }
                foundTop:
                // Find bottom boundary
                for (int y = height - 1; y >= 0; --y) // Iterate over bitmap beginning from bottom row and going up
                {
                    for (int x = 0; x < width; ++x)
                    {
                        int basePix = x + y*stride;
                        
                        int pix1 = basePix + 1;
                        int pix2 = basePix + 2;
                        int pix3 = basePix + 3;
                        int pix4 = basePix + 4;
                        int pix5 = basePix + 5;

                        if (p[basePix] != 255) // Once a non-white pixel is reached
                        {
                            if (pix1 > width + y * stride) break;
                            if (p[pix1] != 255 && p[pix2] != 255 && p[pix3] != 255 && p[pix4] != 255 && p[pix5] != 255)
                            {
                                vals[1] = height - 1 - y; // Store bottom row value at second position in vals
                                goto foundBottom;
                            }
                        }
                    }
                }
                foundBottom:
                
                // Find left boundary
                for (int x = 0; x < width; ++x) // Iterate bitmap, beginning from left, column by column
                {
                    for (int y = 0; y < height; ++y)
                    {
                        int basePix = x + y*stride;
                        
                        int pix1 = basePix + stride;
                        int pix2 = basePix + 2*stride;
                        int pix3 = basePix + 3*stride;
                        int pix4 = basePix + 4*stride;
                        int pix5 = basePix + 5 * stride;
                        
                        if (p[basePix] == 0)
                        {
                            if (pix1 > x + height * stride) break;
                            if (p[pix1] != 255 && p[pix2] != 255 && p[pix3] != 255 && p[pix4] != 255 && p[pix5] != 255 )
                            {
                                vals[2] = x; // Once non-white pixel is reached, store column number at third position in vals
                                goto foundLeft;
                            }
                        }
                    }
                }
                foundLeft:
                // Find right boundary
                for (int x = width - 1; x >= 0; --x) // Iterate bitmap, beginning from right, column by column
                {
                    for (int y = 0; y < height; ++y)
                    {
                        int basePix = x + y*stride;
                        
                        int pix1 = basePix + stride;
                        int pix2 = basePix + 2*stride;
                        int pix3 = basePix + 3*stride;
                        int pix4 = basePix + 4*stride;
                        int pix5 = basePix + 5 * stride;

                        if (p[basePix] == 0)
                        {
                            if (pix1 > x + height * stride) break;
                            if (p[pix1] != 255 && p[pix2] != 255 && p[pix3] != 255 && p[pix4] != 255 && p[pix5] != 255)
                            {
                                vals[3] = width - 1 - x; // Once non-white pixel is reached, store column number in last position of vals
                                goto foundRight;
                            }
                        }
                    }
                }
                foundRight: ;
            }
            b.UnlockBits(bmData);
            return vals;
        }
        
        // Removing White boundaries not from thresholded image
        public static Bitmap RemoveWhiteBoundary1(Bitmap b, byte nVal) // Remove white boundaries from bitmap
        {
            Bitmap input; 
            
            // Ensure Format is 8Bit
            if (b.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                input = (Bitmap)b.Clone();
            } 
            else if (b.PixelFormat == PixelFormat.Format1bppIndexed || b.PixelFormat == PixelFormat.Format24bppRgb)
            {
                input = ConvertTo8Bit(b); 
            }
            else
            {
                throw new ArgumentException("Image type is not supported. Must be 1, 8, or 24bit");
            }
            // Retrieve size of white boundaries
            int[] vals = Offset(input, nVal); // Retrieve the boundary offset values using the binarized version of input
            
            int outputWidth = input.Width - (vals[2] + vals[3]); // Determine size of output image by subtracting boundary offsets from input image parameters
            int outputHeight = input.Height - (vals[0] + vals[1]);

            Bitmap output = new Bitmap(outputWidth, outputHeight, // Create final bitmap with subtracted offsets 
                PixelFormat.Format8bppIndexed);
            
            ColorPalette palette = output.Palette; // Set color palette to grayscale
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            output.Palette = palette;

            BitmapData inputData = input.LockBits(new Rectangle(0, 0, input.Width, // Lock into memory input and output bitmaps
                input.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            BitmapData outputData = output.LockBits(new Rectangle(0, 0, output.Width,
                output.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            int strideIn = inputData.Stride; // Assign stride to integer variables
            int strideOut = outputData.Stride;

            IntPtr inScan = inputData.Scan0; // Retrieve starting pixel address
            IntPtr outScan = outputData.Scan0;

            unsafe
            {
                byte* pIn = (byte*)(void*)inScan; // Cast pixel address to byte pointer
                byte* pOut = (byte*)(void*)outScan;

                for (int y = 0; y < outputHeight; ++y) // Iterate over pixels of output bitmap
                {
                    int byteLineOut = y * strideOut;
                    int byteLineIn = (y + vals[0]) * strideIn;
                    
                    for (int x = 0; x < outputWidth; ++x)
                    {
                        pOut[x + byteLineOut] = pIn[(x + vals[2]) + byteLineIn]; // Copy pixel values from input image, starting at boundary offsets
                    }
                }
            }

            input.UnlockBits(inputData); // Unlock bitmaps data from memory
            output.UnlockBits(outputData);
            return output; // Return output image
        }

        // Task 7: Resize Bitmap to Best Fit //
        public static Bitmap Resize(Bitmap b, int percent) // Rescale images to best fit using Bilinear Interpolation
        {
            Bitmap input;                            
            if (b.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                input = (Bitmap)b.Clone();;
            } // Ensure Format of input is 8bit                        
            else if (b.PixelFormat == PixelFormat.Format1bppIndexed || b.PixelFormat == PixelFormat.Format24bppRgb)
            {
                input = ConvertTo8Bit(b); 
            }
            else
            {
                throw new ArgumentException("Image type is not supported. Must be 1, 8, or 24bit");
            }

            int width = input.Width; // Assign input dimensions to integer variables
            int height = input.Height;

            int nWidth, nHeight; // Initialize output variables
            nWidth = (int)((percent / 100.0) * width); // Width is manipulated based on %nVal
            nHeight = (int)((percent / 100.0) * height); // Height is adjusted accordingly to maintain aspect ratio
            //nHeight = nWidth / (width / height); // Height is adjusted accordingly to maintain aspect ratio

            double nXFactor = (double)width / nWidth; // Calculate ratio of input to output dimensions
            double nYFactor = (double)height / nHeight;
            Bitmap output = new Bitmap(nWidth, nHeight,
                    PixelFormat.Format8bppIndexed); // Initialize output bitmap with adjusted dimensions
            ColorPalette palette = output.Palette; // Set color palette to grayscale    
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            output.Palette = palette;

            BitmapData inData = input.LockBits(new Rectangle(0, 0,
                input.Width, // Lock input and output bitmap data into memory
                input.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            BitmapData outData = output.LockBits(new Rectangle(0, 0, output.Width,
                output.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            IntPtr scanIn = inData.Scan0; // Retrieve first pixel data address
            IntPtr scanOut = outData.Scan0;

            int strideIn = inData.Stride; // Assign strides to integer variables
            int strideOut = outData.Stride;

            unsafe
            {
                byte* pIn = (byte*)(void*)scanIn; // Cast addresses to byte pointers
                byte* pOut = (byte*)(void*)scanOut;
                
                double left, up, right, down; // Initialize distances to 4 coordinates (weights)
                int ceil_x, ceil_y, floor_x, floor_y; // Initialize integer variables for coordinates
                byte p1, p2, p3, p4; // Initialize variables to hold weighted contributions

                for (int y = 0; y < nHeight; ++y) // Iterate over output image
                {
                    int byteLine = y * strideOut; // Initialize number of pixel rows to skip when assigning values to output
                    for (int x = 0; x < nWidth; ++x)
                    {
                        // Coordinates of 4 values
                        floor_x = (int)Math.Floor(x * nXFactor); // Determine floor x coordinate (left coordinates)
                        floor_y = (int)Math.Floor(y * nYFactor); // Determine floor y coordinate (top coordinates)

                        ceil_x = floor_x + 1; // Determine ceiling coordinates and set cutoff
                        if (ceil_x >= width) ceil_x = floor_x;
                        ceil_y = floor_y + 1;
                        if (ceil_y >= height) ceil_y = floor_y;

                        // Distance to 4 values (Weights)                                       // Determine distance of x/y coordinate to floor coordinate
                        left = x * nXFactor - floor_x;
                        up = y * nYFactor - floor_y;
                        right = 1.0 - left; // Determine distance of x/y coordinates to ceiling coordinates
                        down = 1.0 - up;
                        // Retrieving 4 values & Multiplying by weights
                        p1 = (byte)(pIn[ceil_x + ceil_y * strideIn] * left * up); // Determine contribution of each coordinate by multiplying its intensity with the diagonally opposite area
                        p2 = (byte)(pIn[ceil_x + floor_y * strideIn] * left * down);
                        p3 = (byte)(pIn[floor_x + ceil_y * strideIn] * right * up);
                        p4 = (byte)(pIn[floor_x + floor_y * strideIn] * right * down);

                        pOut[x + byteLine] = (byte)(p1 + p2 + p3 + p4); // Sum up contributions of each coordinate
                    }
                }
            }

            input.UnlockBits(inData);
            output.UnlockBits(outData);
            return output;
        }

        // Milestone: Detection of Objects in Bitmap //
        public static Bitmap DetectEdge(Bitmap b, int thresh)
        {
            Bitmap output; // Intialize new bitmap for output
            if (b.PixelFormat == PixelFormat.Format8bppIndexed) // Ensure new bitmap is an 8bit replica of input image
            {
                output = (Bitmap)b.Clone();
            }
            else if (b.PixelFormat == PixelFormat.Format1bppIndexed || b.PixelFormat == PixelFormat.Format24bppRgb)
            {
                output = ConvertTo8Bit(b); // Make name specific
            }
            else
            {
                throw new ArgumentException(" Image type is not supported. Must be 1, 8, or 24bit");
            }

            Bitmap input;
            //if (condition)
            input = AddPadding(output, 255); // Add padding around 8bit image for reading
            //else
            // paddedIm = Add255Padding(newImage);                                              // Add padding around 8bit image for reading

            BitmapData bmData = input.LockBits(new Rectangle(0, 0, input.Width, // Lock bitmap data into memory
                input.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            BitmapData newbmData = output.LockBits(new Rectangle(0, 0, output.Width,
                output.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            System.IntPtr Scan0 = bmData.Scan0; // Retrieve first pixel data address
            System.IntPtr Scan1 = newbmData.Scan0;

            int stride0 = bmData.Stride; // Assign bitmap strides to 
            int stride1 = newbmData.Stride;

            int nOffset1 = stride1 - output.Width; // Calculate output image offset

            int height = output.Height; // Assign output image parameters to integer variables
            int width = output.Width;
            byte[] group = new byte [4]; // Create a byte matrix with size of kernel

            unsafe
            {
                byte* pIn = (byte*)(void*)Scan0; // Pointer to padded image for reading
                byte* pOut = (byte*)(void*)Scan1; // Pointer to image being manipulated
                for (int y = 0; y < height; ++y) // Iterate over pixels of output image
                {
                    for (int x = 0; x < width; ++x)
                    {
                        int byteNum0 = x + y * stride0;
                        int byteNum1 = byteNum0 + stride0;
                        int byteNum2 = byteNum1 + stride0;
                        group[0] = (byte)Math.Abs(pIn[byteNum0] - pIn[byteNum2 + 2]); // Copy the first i by j pixels into the byte matrix
                        group[1] = (byte)Math.Abs(pIn[byteNum0 + 1] - pIn[byteNum2 + 1]);
                        group[2] = (byte)Math.Abs(pIn[byteNum0 + 2] - pIn[byteNum2]);
                        group[3] = (byte)Math.Abs(pIn[byteNum1] - pIn[byteNum1 + 2]);
                        int pixel = group.Max();
                        if (pixel < thresh) pixel = 0;
                        pOut[0] = (byte)pixel; // Assign the max byte to the corresponding byte in the output image
                        ++pOut; // Increment to the next byte
                    }

                    pOut += nOffset1; // Skip offset
                }
            }

            input.UnlockBits(bmData); // Unlock bitmaps data and return dilated image
            output.UnlockBits(newbmData);
            return output;
        }
        private static Bitmap[] LabelAndExtract(Bitmap image)
        {
            int width = image.Width;
            int height = image.Height;

            BitmapData imageData = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            int stride = imageData.Stride;
            IntPtr scan0 = imageData.Scan0;

            byte currentLabel = 1;
            List<byte> labels = new List<byte>();
            bool[,] checks = new bool[width, height];
            unsafe
            {
                byte* p = (byte*)(void*)scan0;

                // Iterate through each pixel in the image
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int byteIndex = x + y * stride;

                        // Check if the pixel is a foreground pixel (black) and hasn't been checked yet
                        if (p[byteIndex] == 0)
                        {
                            // Perform connected component labeling 
                            Queue<Point> queue = new Queue<Point>();
                            queue.Enqueue(new Point(x, y));

                            //List<Point> componentPoints = new List<Point>();

                            while (queue.Count > 0)
                            {
                                Point point = queue.Dequeue();
                                int px = point.X;
                                int py = point.Y;

                                if (px >= 0 && px < width && py >= 0 && py < height && p[px + py * stride] == 0 && !checks[px, py])
                                {
                                    // Assign label to the current pixel
                                    p[px + py * stride] = currentLabel;
                                    checks[px, py] = true;

                                    // Enqueue neighboring pixels if they are valid and not checked
                                    if (px - 1 >= 0 && !checks[px - 1, py]) queue.Enqueue(new Point(px - 1, py));
                                    if (px + 1 < width && !checks[px + 1, py]) queue.Enqueue(new Point(px + 1, py));
                                    if (py - 1 >= 0 && !checks[px, py - 1]) queue.Enqueue(new Point(px, py - 1));
                                    if (py + 1 < height && !checks[px, py + 1]) queue.Enqueue(new Point(px, py + 1));
                                    if (py + 1 < height && px + 1 < width && !checks[px + 1, py + 1]) queue.Enqueue(new Point(px + 1, py + 1));
                                    if (px - 1 >= 0 && py + 1 < height && !checks[px - 1, py + 1]) queue.Enqueue(new Point(px - 1, py + 1));
                                    if (py - 1 >= 0 && px + 1 < width && !checks[px + 1, py - 1]) queue.Enqueue(new Point(px + 1, py - 1));
                                    if (px - 1 >= 0 && py - 1 >= 0 && !checks[px- 1, py - 1]) queue.Enqueue(new Point(px - 1, py - 1));
                                }
                            }
                    
                            // Add current label to labels if it hasn't been added yet
                            if (!labels.Contains(currentLabel))
                            {
                                labels.Add(currentLabel);
                            }

                            // Increment label for the next component
                            currentLabel++;

                            // Early exit if we have already used up all possible labels (255)
                            if (currentLabel == 256)
                            {
                                throw new ArgumentException("Too many shapes detected");
                            }
                        }
                    }
                }
            }

            image.UnlockBits(imageData);
            Bitmap[] cropped = ExtractShapes(image, labels);

            return (cropped);
        }
        private static Bitmap[] ExtractShapes(Bitmap b, List<byte> labels)
        {
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, // Lock bitmap data into memory
                b.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            int stride = bmData.Stride;
            IntPtr scan = bmData.Scan0;

            int width = b.Width;
            int height = b.Height;
            int size = labels.Count;
            
            // Store images and shape names in arrays
            Bitmap[] images = new Bitmap[size];
            //String[] labels = new string[size];

            unsafe
            {
                byte* p = (byte*)(void*)scan;
                for (int n = 0; n < size; ++n)
                {
                    // Create mini bitmaps in array
                    images[n] = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
                    BitmapData miniData = images[n].LockBits(new Rectangle(0, 0,
                        images[n].Width, // Lock bitmap data into memory
                        images[n].Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

                    IntPtr miniScan = miniData.Scan0;

                    byte* pMini = (byte*)(void*)miniScan;
                    for (int y = 0; y < height; ++y)
                    {
                        int byteLine = y * stride;
                        for (int x = 0; x < width; ++x)
                        {

                            int byteNum = byteLine + x;
                            if (p[byteNum] == labels[n]) pMini[byteNum] = p[byteNum];
                            else pMini[byteNum] = 255;
                        }
                    }

                    images[n].UnlockBits(miniData);
                    images[n] = RemoveWhiteBoundary(images[n], 0);
                    images[n] = Threshold(images[n], 200);
                }

                b.UnlockBits(bmData);
            }

            return images;
        }
        private static String[] NameShapes(Bitmap[] images)
        {
            String[] labels = new string[images.Length];
            int length = images.Length;
            unsafe
            {
                for (int m = 0; m < length; ++m)
                {
                    BitmapData miniData = images[m].LockBits(new Rectangle(0, 0,
                        images[m].Width, // Lock bitmap data into memory
                        images[m].Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

                    int miniStride = miniData.Stride;
                    IntPtr miniScan = miniData.Scan0;

                    int width = images[m].Width;
                    int height = images[m].Height;
                    
                    int topCount = 0;
                    int bottomCount = 0;
                    int leftCount = 0;
                    int rightCount = 0;
                    int blackCount = 0;
                    int pixSize = width * height;

                    byte* pMini = (byte*)(void*)miniScan;
                    
                    for (int y = 0; y < height; ++y)
                    {
                        for (int x = 0; x < width; ++x)
                        {
                            int byteNum = x + y * miniStride;
                            if (pMini[byteNum] == 0) blackCount += 1;
                            if (pMini[byteNum] == 0 && y == 0) topCount+=1;
                            if (pMini[byteNum] == 0 && y == height - 1) bottomCount+=1;
                            if (pMini[byteNum] == 0 && x == 0) leftCount+=1;
                            if (pMini[byteNum] == 0 && x == width - 1) rightCount+=1;
                            if (x == width - 1 && y == height - 1)
                            {
                                if (topCount != bottomCount || leftCount != rightCount && (pMini[0] == 255 || pMini[width] == 255 || pMini[miniStride * height] == 255 || pMini[width + miniStride*height] == 255)) labels[m] = "Triangle";
                                if (topCount == bottomCount && rightCount == leftCount && width == height) labels[m] = "Square";
                                if (topCount == bottomCount && rightCount == leftCount&& width != height) labels[m] = "Rectangle";
                                if (pMini[0] == 255 && pMini[width] == 255 && pMini[miniStride * height] == 255 && pMini[width + miniStride*height] == 255 && topCount > 2 && topCount < width && bottomCount < width) labels[m] = "Circle";
                            }
                        }
                    }
                    images[m].UnlockBits(miniData);
                }
            }
            return labels;
        }
        private static Bitmap[] CreateTextBitmaps(Bitmap[] images, string[] labels)
        {
            Bitmap[] textBitmaps = new Bitmap[labels.Length];

            for (int i = 0; i < labels.Length; i++)
            {
                string text = labels[i];

                // Calculate size of the bitmap based on the text and font
                int width, height;
                using (var tempImage = new Bitmap(1, 1))
                using (var g = Graphics.FromImage(tempImage))
                {
                    var size = g.MeasureString(text, SystemFonts.DefaultFont);
                    width = (int)Math.Ceiling(size.Width);
                    height = (int)Math.Ceiling(size.Height);
                }

                // Create a new bitmap with calculated size
                Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);

                // Draw the text onto the bitmap
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.White); // Clear with background coltextColoror
                    using (Brush brush = new SolidBrush(Color.Black))
                    {
                        g.DrawString(text, SystemFonts.DefaultFont, brush, new PointF(0, 0));
                    }
                }

                // Store the bitmap in the array
                textBitmaps[i] = HorizontalCon(images[i], ConvertTo8Bit(bitmap), true);
            }

            return textBitmaps;
        }
        public static Bitmap Identify(Bitmap b)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine(b.PixelFormat);
            Bitmap newImage;
            // Clone or convert the input bitmap based on its pixel format
            if (b.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                newImage = (Bitmap)b.Clone();
            }
            else if (b.PixelFormat == PixelFormat.Format1bppIndexed || b.PixelFormat == PixelFormat.Format24bppRgb || b.PixelFormat == PixelFormat.Format32bppRgb || b.PixelFormat == PixelFormat.Format32bppArgb)
            {
                newImage = ConvertTo8Bit(b); // Implement ConvertTo8Bit method appropriately
            }
            else throw new ArgumentException("Image type is not supported. Must be 1, 8, or 24bit");

            //newImage = Threshold(Morphology(newImage, 3, 3, false), 200); // 3x3 dilation
            
            newImage = Threshold(newImage, 200); // 3x3 dilation
            
            //var (labeledImage, distinctLabels) = LabelConnectedComponents(newImage);

            //Bitmap[] images = FloodFill(newImage);

            Bitmap[] images = LabelAndExtract(newImage);

            //List<Bitmap> pics = LabelConnectedComponents1(newImage);

            //Bitmap[] images = ExtractShapes(labeledImage, distinctLabels);

            //List<Bitmap> images = LabelConnectedComponents(newImage);

            String[] labels = NameShapes(images);

            //String[] labels = NameShapes(pics);

            for (int i = 0; i < images.Length; ++i) images[i] = AddPadding(images[i], 255);

            images = CreateTextBitmaps(images, labels);
            
            int arrayLength = images.Length;
            
            for (int m = 1; m < arrayLength; ++m)
            {
                images[m] = HorizontalCon(images[m-1], images[m], false);
            }
            
            /*
            for (int i = 0; i < arrayLength; ++i)
            {
                Console.WriteLine(labels[i]);
            }
            */
            stopwatch.Stop();
            TimeSpan elapsed = stopwatch.Elapsed;
            Console.WriteLine($"Elapsed time: {elapsed.TotalMilliseconds} milliseconds");
            return images[arrayLength -1];

        } 
    }
}