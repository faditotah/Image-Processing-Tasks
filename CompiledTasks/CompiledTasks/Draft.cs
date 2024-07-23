/*
public static Bitmap Dilation(Bitmap b)
{
    Bitmap newImage;

    // Ensure the input bitmap is an 8-bit indexed grayscale image
    if (b.PixelFormat == PixelFormat.Format8bppIndexed)
    {
        newImage = (Bitmap)b.Clone();
    }
    else if (b.PixelFormat == PixelFormat.Format1bppIndexed || b.PixelFormat == PixelFormat.Format24bppRgb)
    {
        newImage = ConvertTo8Bit(b); // Convert other formats to 8-bit grayscale
    }
    else
    {
        throw new ArgumentException("Image type is not supported. Must be 1, 8, or 24-bit.");
    }

    Bitmap paddedIm = AddZeroPadding(newImage); // Add padding around 8-bit image for dilation

    BitmapData bmData = paddedIm.LockBits(new Rectangle(0, 0, paddedIm.Width, paddedIm.Height),
                                          ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
    BitmapData newbmData = newImage.LockBits(new Rectangle(0, 0, newImage.Width, newImage.Height),
                                             ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

    unsafe
    {
        byte* p = (byte*)bmData.Scan0.ToPointer(); // Pointer to padded image for reading
        byte* pNew = (byte*)newbmData.Scan0.ToPointer(); // Pointer to output image being manipulated

        int stride0 = bmData.Stride; // Stride of padded image
        int stride1 = newbmData.Stride; // Stride of output image

        int nOffset1 = stride1 - newImage.Width; // Calculate offset for output image

        int height = newImage.Height;
        int width = newImage.Width;

        int currentLabel = 1; // Start labeling from 1

        // Arrays to track labels and visited pixels
        int[,] labels = new int[width, height];
        bool[,] visited = new bool[width, height];

        // Initialize arrays
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                labels[x, y] = -1; // -1 means not labeled
                visited[x, y] = false;
            }
        }

        // Iterate over each pixel
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                int byteIndex = x + y * stride0; // Index of current pixel

                if (p[byteIndex] == 255 && !visited[x, y])
                {
                    // Found an unvisited foreground pixel
                    Queue<Point> queue = new Queue<Point>();
                    queue.Enqueue(new Point(x, y));

                    // BFS to label connected component
                    while (queue.Count > 0)
                    {
                        Point pixel = queue.Dequeue();
                        int px = pixel.X;
                        int py = pixel.Y;

                        // Check 8-connectivity
                        for (int ny = py - 1; ny <= py + 1; ny++)
                        {
                            for (int nx = px - 1; nx <= px + 1; nx++)
                            {
                                if (nx >= 0 && nx < width && ny >= 0 && ny < height &&
                                    p[nx + ny * stride0] == 255 && !visited[nx, ny])
                                {
                                    visited[nx, ny] = true;
                                    labels[nx, ny] = currentLabel;
                                    queue.Enqueue(new Point(nx, ny));
                                }
                            }
                        }
                    }

                    currentLabel++; // Move to the next label for the next connected component
                }

                // Assign label to output image
                pNew[byteIndex] = (byte)(labels[x, y]); // Assuming labels are within byte range (0-255)
            }
        }
    }

    paddedIm.UnlockBits(bmData);
    newImage.UnlockBits(newbmData);

    return newImage;
}

        public static Bitmap Dilation(Bitmap b)
   {
       Bitmap newImage;

       // Ensure the input bitmap is an 8-bit indexed grayscale image
       if (b.PixelFormat == PixelFormat.Format8bppIndexed)
       {
           newImage = (Bitmap)b.Clone();
       }
       else if (b.PixelFormat == PixelFormat.Format1bppIndexed || b.PixelFormat == PixelFormat.Format24bppRgb)
       {
           newImage = ConvertTo8Bit(b); // Convert other formats to 8-bit grayscale
       }
       else
       {
           throw new ArgumentException("Image type is not supported. Must be 1, 8, or 24-bit.");
       }

       Bitmap paddedIm = AddZeroPadding(newImage); // Add padding around 8-bit image for dilation

       BitmapData bmData = paddedIm.LockBits(new Rectangle(0, 0, paddedIm.Width, paddedIm.Height),
                                             ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
       BitmapData newbmData = newImage.LockBits(new Rectangle(0, 0, newImage.Width, newImage.Height),
                                                ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

       unsafe
       {
           byte* p = (byte*)bmData.Scan0.ToPointer(); // Pointer to padded image for reading
           byte* pNew = (byte*)newbmData.Scan0.ToPointer(); // Pointer to output image being manipulated

           int stride0 = bmData.Stride; // Stride of padded image
           int stride1 = newbmData.Stride; // Stride of output image

           int nOffset1 = stride1 - newImage.Width; // Calculate offset for output image

           int height = newImage.Height;
           int width = newImage.Width;

           byte[] labelMap = new byte[width * height]; // Array to store labels
           byte currentLabel = 10; // Initial label value

           // Initialize label map to zeros
           for (int i = 0; i < labelMap.Length; i++)
           {
               labelMap[i] = 0;
           }

           // Function to find the root of a label
           Func<byte, byte> findRoot = (label) =>
           {
               byte root = label;
               while (root != labelMap[root])
               {
                   root = labelMap[root];
               }
               return root;
           };

           // Iterate over each pixel in the image
           for (int y = 0; y < height; y++)
           {
               for (int x = 0; x < width; x++)
               {
                   int byteIndex = x + y * stride0;

                   if (p[byteIndex] == 255) // Check if current pixel is foreground
                   {
                       byte[] neighbors = new byte[4];
                       neighbors[0] = labelMap[byteIndex - stride0]; // Top neighbor
                       neighbors[1] = labelMap[byteIndex - 1]; // Left neighbor

                       byte minNeighbor = byte.MaxValue;
                       foreach (byte neighbor in neighbors)
                       {
                           if (neighbor > 0 && neighbor < minNeighbor)
                           {
                               minNeighbor = neighbor;
                           }
                       }

                       if (minNeighbor == byte.MaxValue)
                       {
                           minNeighbor = currentLabel;
                           currentLabel += 5; // Increment label for next component
                       }

                       labelMap[byteIndex] = minNeighbor;
                       pNew[byteIndex] = minNeighbor;
                   }
               }
               pNew += nOffset1; // Move to the next line in the output image
           }
       }

       paddedIm.UnlockBits(bmData);
       newImage.UnlockBits(newbmData);

       return newImage;
   }


*/
        /*
                 public static Bitmap Dilation1(Bitmap b) // Dilation of bitmap using kernel size as input
           {
               Bitmap newImage; // Intialize new bitmap for output
               Console.WriteLine(b.PixelFormat);
               if (b.PixelFormat == PixelFormat.Format8bppIndexed) // Ensure new bitmap is an 8bit replica of input image
               {
                   //newImage = DetectEdge((Bitmap)b.Clone(), 150);
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
               
               //newImage = RemoveWhiteBoundary(newImage);
               newImage = DetectEdge(newImage, 200); // Difference Edge detection
               newImage = Morphology(newImage, 3, 3, true); // 3x3 dilation
               newImage = Resize(newImage, 50, true); // Resizing image to 50% of original size
               newImage = Threshold(newImage, Mean(newImage)); // Binarizing image using mean threshold
               
               Bitmap paddedIm = AddZeroPadding(newImage); // Add padding around 8bit image for reading

               
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
               byte[] group = new byte [4]; // Create a byte matrix with size of kernel
               List<byte> labels = new List<byte>();
               unsafe
               {
                   byte* p = (byte*)(void*)Scan0; // Pointer to padded image for reading
                   byte* pNew = (byte*)(void*)Scan1; // Pointer to image being manipulated
                   byte label = 10;
                   for (int y = 0; y < height; ++y) // Iterate over pixels of output image
                   {
                       for (int x = 0; x < width; ++x)
                       {
                           int byteLine1 = x + y * stride0;
                           int byteLine2 = byteLine1 + stride0;
                           int byteLine3 = byteLine2 + stride0;

                           group[0] = p[byteLine1];
                           group[1] = p[byteLine1 + 1];
                           group[2] = p[byteLine1 + 2];
                           group[3] = p[byteLine2];

                           int sum = group.Sum(i => (int)i);
                           if (p[byteLine2 + 1] == 255)
                           {
                               for (int n = 0; n < 4; ++n)
                               {
                                   if (sum % 255 == 0)
                                   {
                                       pNew[0] = label;
                                       p[byteLine2 + 1] = label;
                                       labels.Add(label);
                                       label += 5;
                                       break;
                                   }
                                   if (group[n] != 0 && group[n] != 255)
                                   {

                                       pNew[0] = group[n]; // Assign shared label
                                       p[byteLine2 + 1] = group[n]; // assign shared label

                                       //break;
                                   }
                               }
                           }
                           ++pNew; // Increment to the next byte
                       }
                       pNew += nOffset1; // Skip offset
                   }
               }

               paddedIm.UnlockBits(bmData); // Unlock bitmaps data and return dilated image
               newImage.UnlockBits(newbmData);
               //Console.WriteLine(labels.Count);
               return newImage;
           }
        public static Bitmap Divide(Bitmap b, List<byte> labels)
        {
            int size = labels.Count;
            Bitmap[] images = new Bitmap[size];
            
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width,
                    b.Height), // Lock in input bitmap (Bitmap would have already been converted to 8bit)
                ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            int stride = bmData.Stride; // Assign stride to integer variable
            IntPtr scan = bmData.Scan0; // Retrieve address of first pixel data

            int height = b.Height; // Assign height and width to integer variables
            int width = b.Width;

            unsafe
            {
                byte* p = (byte*)(void*)scan;
                for (int i = 0; i < size)
                {
                    for (int y = 0; y < height; ++y) 
                    {
                        for (int x = 0; x < width; ++x)
                        {
                            if p[x + y*stride] = labels[i]
                            {
                                
                            }
                        }
                    }
                }


            }
            return images;
        }
        */
        /*
         *         public static Bitmap VerticalCon(Bitmap b, Bitmap c) // Veritical Concatenation
           {
               Bitmap b8; // Initialize input bitmaps for reading
               Bitmap c8;
               if (b.PixelFormat == PixelFormat.Format8bppIndexed)
               {
                   b8 = b;
               } // Check pixel formats of both input bitmaps, and ensure final 8bit format
               else if (b.PixelFormat == PixelFormat.Format1bppIndexed || b.PixelFormat == PixelFormat.Format24bppRgb)
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
               else if (c.PixelFormat == PixelFormat.Format1bppIndexed || c.PixelFormat == PixelFormat.Format24bppRgb)
               {
                   c8 = ConvertTo8Bit(b); // Make name specific
               }
               else
               {
                   throw new ArgumentException("Images must be in 24bit or 8bit format.");
               }

               Bitmap newImage = new Bitmap(
                   Math.Max(b8.Width,
                       c8.Width), // Create new larger bitmap with combined height and max width             
                   b8.Height + c8.Height, PixelFormat.Format8bppIndexed);

               ColorPalette palette = newImage.Palette; // Set color palette of new image to gray scale
               for (int i = 0; i < 256; i++)
               {
                   palette.Entries[i] = Color.FromArgb(i, i, i);
               }

               newImage.Palette = palette;

               BitmapData bmDataB = b8.LockBits(new Rectangle(0, 0, b8.Width, b8.Height), // Lock bitmaps data into memory
                   ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
               BitmapData bmDataC = c8.LockBits(new Rectangle(0, 0, c8.Width, c8.Height),
                   ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
               BitmapData bmDataN = newImage.LockBits(new Rectangle(0, 0, newImage.Width,
                   newImage.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

               System.IntPtr ScanB = bmDataB.Scan0; // Retrieve addresses of first pixel data
               System.IntPtr ScanC = bmDataC.Scan0;
               System.IntPtr ScanN = bmDataN.Scan0;

               int widthB = b8.Width;
               int heightB = b8.Height;
               int strideB = bmDataB.Stride; // Assign bitmap parameters to integer variables
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

                   for (int y = 0; y < heightN; ++y) // Iterate over large output bitmap
                   {
                       for (int x = 0; x < widthN; ++x)
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
                   * /
               }

               b8.UnlockBits(bmDataB);
               c8.UnlockBits(bmDataC);
               newImage.UnlockBits(bmDataN);

               return newImage;
           }

         */


//// Method for milestone
///
///
///         public static Bitmap Dilation0(Bitmap b) // Dilation of bitmap using kernel size as input
/*
        {
            Bitmap newImage; // Intialize new bitmap for output
            Console.WriteLine(b.PixelFormat);
            if (b.PixelFormat == PixelFormat.Format8bppIndexed) // Ensure new bitmap is an 8bit replica of input image
            {
                //newImage = DetectEdge((Bitmap)b.Clone(), 150);
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
            
            //newImage = RemoveWhiteBoundary(newImage);
            newImage = DetectEdge(newImage, 200); // Difference Edge detection
            newImage = Morphology(newImage, 3, 3, true); // 3x3 dilation
            newImage = Resize(newImage, 50 ); // Resizing image to 50% of original size
            newImage = Threshold(newImage, Mean(newImage)); // Binarizing image using mean threshold
            
            Bitmap paddedIm = AddPadding(newImage, 0); // Add padding around 8bit image for reading

            
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
            byte[] group = new byte [4]; // Create a byte matrix with size of kernel
            List<byte> labels = new List<byte>();
            unsafe
            {
                byte* p = (byte*)(void*)Scan0; // Pointer to padded image for reading
                byte* pNew = (byte*)(void*)Scan1; // Pointer to image being manipulated
                byte label = 10;
                for (int y = 0; y < height; ++y) // Iterate over pixels of output image
                {
                    for (int x = 0; x < width; ++x)
                    {
                        int byteLine1 = x + y * stride0;
                        int byteLine2 = byteLine1 + stride0;
                        
                        if (pNew[x+y*stride1] == 0)
                        {
                            labels.Add(p[byteLine2 + 1]);
                            for (int i = 0; i < labels.Count; ++i)
                            {
                                if (labels[i] == 0)
                                {
                                    pNew[x+y*stride1] = label;
                                    p[byteLine2 + 1] = label; 
                                    
                                    labels.Add(p[byteLine1]);
                                    labels.Add(p[byteLine1+1]);
                                    labels.Add(p[byteLine1+2]);
                                    labels.Add(p[byteLine2]);
                                }
                                                        
                            }
                        }

                        label += 5;
                    }
                }
            }
            paddedIm.UnlockBits(bmData); // Unlock bitmaps data and return dilated image
            newImage.UnlockBits(newbmData);
            //Console.WriteLine(labels.Count);
            return newImage;
        }
*/
/*
 *         public static Bitmap DetectEdge(Bitmap b, int thresh)
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
                   group[0] = (byte)Math.Abs(pIn[byteNum0] -
                                             pIn[
                                                 byteNum2 +
                                                 2]); // Copy the first i by j pixels into the byte matrix
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

 */
/*
byte* p = (byte*)(void*)scan;
for (int n = 0; n < size; ++n)
{
    // Create mini bitmaps in array
    images[n] = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
    BitmapData miniData = images[n].LockBits(new Rectangle(0, 0, images[n].Width, // Lock bitmap data into memory
        images[n].Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

    IntPtr miniScan = miniData.Scan0;

    byte* pMini = (byte*)(void*)miniScan;
    for (int y = 0; y < height; ++y)
    {
        int byteLine = y * stride;
        for (int x = 0; x < width; ++x)
        {

            int byteNum = byteLine + x;
            if (p[byteNum] == distinctLabels[n]) pMini[byteNum] = p[byteNum];
            else pMini[byteNum] = 255;
        }
    }
    images[n].UnlockBits(miniData);
    images[n] = RemoveWhiteBoundary(images[n]);
    images[n] = Threshold(images[n], 200);
}
labeledImage.UnlockBits(bmData);


        private static List<Bitmap> LabelConnectedComponents1(Bitmap image)
   {
       Bitmap labeledImage = (Bitmap)image.Clone(); // Create a clone to store labeled image
       int width = labeledImage.Width;
       int height = labeledImage.Height;

       BitmapData imageData = labeledImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
       int stride = imageData.Stride;
       IntPtr scan = imageData.Scan0;

       // Create an array to keep track of labels
       byte[,] labels = new byte[width, height];
       byte currentLabel = 10;
       List<Bitmap> pics = new List<Bitmap>();

       unsafe
       {
           byte* p = (byte*)(void*)scan;

           // Iterate through each pixel in the image
           for (int y = 0; y < height; y++)
           {
               for (int x = 0; x < width; x++)
               {
                   int byteIndex = x + y * stride;

                   // Check if the pixel is a foreground pixel (black)
                   if (p[byteIndex] == 0)
                   {

                       // Perform connected component labeling using a flood fill algorithm (DFS/BFS)
                       Queue<Point> queue = new Queue<Point>();
                       queue.Enqueue(new Point(x, y));

                       while (queue.Count > 0)
                       {
                           Point point = queue.Dequeue();
                           int px = point.X;
                           int py = point.Y;

                           if (px >= 0 && px < width && py >= 0 && py < height && labels[px, py] == 0 && p[px + py * stride] == 0)
                           {
                               // Assign label to the current pixel
                               p[px + py * stride] = currentLabel;
                               labels[px, py] = currentLabel;

                               // Enqueue neighboring pixels
                               queue.Enqueue(new Point(px - 1, py));
                               queue.Enqueue(new Point(px + 1, py));
                               queue.Enqueue(new Point(px, py - 1));
                               queue.Enqueue(new Point(px, py + 1));
                           }
                       }
                       Bitmap temp = new Bitmap(RemoveWhiteBoundary1(labeledImage, currentLabel));
                       temp = ConvertTo8Bit(temp);
                       pics.Add(temp);
                       currentLabel += 5; // Adjust as needed
                   }
               }
           }
       }
       labeledImage.UnlockBits(imageData);
       return (pics);
   } // Returns cropped labeledImage
   
   private static List<Bitmap> LabelConnectedComponents2(Bitmap image)
   {
       Bitmap labeledImage = (Bitmap)image.Clone(); // Create a clone to store labeled image
       int width = labeledImage.Width;
       int height = labeledImage.Height;

       BitmapData imageData = labeledImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
       int stride = imageData.Stride;
       IntPtr scan = imageData.Scan0;

       // Create an array to keep track of labels
       byte[,] labels = new byte[width, height];
       byte currentLabel = 10;
       List<Bitmap> pics = new List<Bitmap>();

       unsafe
       {
           byte* p = (byte*)(void*)scan;

           // Iterate through each pixel in the image
           for (int y = 0; y < height; y++)
           {
               for (int x = 0; x < width; x++)
               {
                   int byteIndex = x + y * stride;

                   // Check if the pixel is a foreground pixel (black)
                   if (p[byteIndex] == 0)
                   {

                       Bitmap temp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
                       BitmapData tempData = temp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

                       int tempStride = tempData.Stride;
                       IntPtr tempScan = tempData.Scan0;
                       byte* tempP = (byte*)(void*)tempScan;

                       // Perform connected component labeling using a flood fill algorithm (DFS/BFS)
                       Queue<Point> queue = new Queue<Point>();
                       queue.Enqueue(new Point(x, y));

                       while (queue.Count > 0)
                       {
                           Point point = queue.Dequeue();
                           int px = point.X;
                           int py = point.Y;

                           if (px >= 0 && px < width && py >= 0 && py < height && labels[px, py] == 0 && p[px + py * stride] == 0)
                           {
                               // Assign label to the current pixel
                               p[px + py * stride] = currentLabel;
                               labels[px, py] = currentLabel;
                               tempP[px + py * tempStride] = currentLabel;

                               // Enqueue neighboring pixels
                               queue.Enqueue(new Point(px - 1, py));
                               queue.Enqueue(new Point(px + 1, py));
                               queue.Enqueue(new Point(px, py - 1));
                               queue.Enqueue(new Point(px, py + 1));
                           }
                       }
                       temp.UnlockBits(tempData);
                       pics.Add(temp);
                       currentLabel += 5; // Adjust as needed
                   }
               }
           }
       }
       labeledImage.UnlockBits(imageData);
       return (pics);
   } // Creates new bitmap for each shape
   
   private static (Bitmap, List<Byte>) LabelConnectedComponents3(Bitmap image)
   {
       Bitmap labeledImage = (Bitmap)image.Clone(); // Create a clone to store labeled image
       int width = labeledImage.Width;
       int height = labeledImage.Height;

       BitmapData imageData = labeledImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
       int stride = imageData.Stride;
       IntPtr scan0 = imageData.Scan0;

       // Create an array to keep track of labels
       byte[,] labels = new byte[width, height];
       byte currentLabel = 10;
       List<Byte> names = new List<Byte>();
       

       unsafe
       {
           byte* p = (byte*)(void*)scan0;

           // Iterate through each pixel in the image
           for (int y = 0; y < height; y++)
           {
               for (int x = 0; x < width; x++)
               {
                   int byteIndex = x + y * stride;

                   // Check if the pixel is a foreground pixel (black)
                   if (p[byteIndex] == 0)
                   {
                       List<Point> coords = new List<Point>();
                       // Perform connected component labeling using a flood fill algorithm (DFS/BFS)
                       Queue<Point> queue = new Queue<Point>();
                       queue.Enqueue(new Point(x, y));

                       while (queue.Count > 0)
                       {
                           Point point = queue.Dequeue();
                           int px = point.X;
                           int py = point.Y;

                           if (px >= 0 && px < width && py >= 0 && py < height && labels[px, py] == 0 && p[px + py * stride] == 0)
                           {
                               // Assign label to the current pixel
                               p[px + py * stride] = currentLabel;
                               labels[px, py] = currentLabel;
                               
                               // Enqueue neighboring pixels
                               queue.Enqueue(new Point(px - 1, py));
                               queue.Enqueue(new Point(px + 1, py));
                               queue.Enqueue(new Point(px, py - 1));
                               queue.Enqueue(new Point(px, py + 1));
                           }
                           if (currentLabel != 255 && !names.Contains(currentLabel))
                           {
                               names.Add(currentLabel);
                           }
                       }
                       // Increment label for the next component
                       currentLabel += 5; // Adjust as needed
                   }
               }
           }
       }

       labeledImage.UnlockBits(imageData);
       return (labeledImage, names);
   } // Returns labeledImage and label names

   private static Bitmap CCL(Bitmap b)
   {
       if (b.PixelFormat != PixelFormat.Format8bppIndexed) ConvertTo8Bit(b);
       Bitmap labeledImage = AddPadding(b, 255);
       int width = labeledImage.Width;
       int height = labeledImage.Height;

       BitmapData imageData = labeledImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
       int stride = imageData.Stride;
       IntPtr scan = imageData.Scan0;

       byte[] group = new byte[4];
       byte label = 1;

       unsafe
       {
           byte* p = (byte*)(void*)scan; 
           for (int y = 0; y < height; ++y)
           {
               for (int x = 0; x < width; ++x)
               {
                   int index = x + y * stride;
                   if (p[index] == 0)
                   {
                       p[index] = label;
                       group[0] = p[(x - 1) + (y - 1) * stride];
                       group[1] = p[(x) + (y - 1) * stride];
                       group[2] = p[(x+1) + (y - 1) * stride];
                       group[3] = p[(x-1) + (y) * stride];

                       for (int i = 0; i < group.Length; ++i)
                       {
                           if (group[i] == 0)
                           {
                               group[i] = label;
                           }
                       }
                   }
                   ++label;
               }

           }
       }
       labeledImage.UnlockBits(imageData);
       return labeledImage;
   }
   public static Bitmap CCL1(Bitmap b)
   {
       if (b.PixelFormat != PixelFormat.Format8bppIndexed) ConvertTo8Bit(b);
       Bitmap labeledImage = AddPadding(b, 255);
       //Bitmap labeledImage = (Bitmap)b.Clone();
       int width = labeledImage.Width;
       int height = labeledImage.Height;

       int[,] matLabels = new int[height, width];
       
       // Set matrix to zeros
       for (int y = 0; y < height; ++y)
       {
           for (int x = 0; x < width; ++x)
           {
               matLabels[x, y] = 0;
           }
       }

       byte[] neighbors = new byte[4];
       
       BitmapData imageData = labeledImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
       int stride = imageData.Stride;
       IntPtr scan = imageData.Scan0;

       // Use a dictionary to keep track of equivalences between labels
       Dictionary<byte, byte> equivalence = new Dictionary<byte, byte>();
       byte currentLabel = 1; // Start labeling from 1

       unsafe
       {
           byte* p = (byte*)(void*)scan;

           for (int y = 0; y < height; ++y)
           {
               for (int x = 0; x < width; ++x)
               {
                   int index = x + y * stride;
                   
                   if (p[index] == 0)
                   {
                       neighbors[0] = p[index - stride - 1];
                       neighbors[1] = p[index - stride];
                       neighbors[2] = p[index - stride + 1];
                       neighbors[3] = p[index - 1];

                       // Find smallest label among neighbors (excluding 0)
                       byte smallestLabel = 0;

                       // All neighbors are background
                       if (neighbors[0] == 255 && neighbors[1] == 255 && neighbors[2] == 255 &&
                           neighbors[3] == 255)
                       {
                           p[index] = currentLabel;
                           matLabels[x, y] = currentLabel;
                       }
                       // One neighbor is labeled
                       foreach (byte neighbor in neighbors)
                       {
                           if (neighbor != 0 && neighbor != 255 && neighbor < currentLabel)
                           {
                               p[index] = neighbor;
                               break;
                           }
                       }

                       {
                           // Assign the smallest neighbor label
                           p[index] = smallestLabel;

                           // Update equivalence table
                           foreach (byte neighbor in neighbors)
                           {
                               if (neighbor != 0 && neighbor != smallestLabel)
                               {
                                   byte root = neighbor;
                                   while (equivalence.ContainsKey(root) && equivalence[root] != root)
                                   {
                                       root = equivalence[root];
                                   }
                                   equivalence[root] = smallestLabel;
                               }
                           }
                       }
                   }
               }
           }

           // Second pass to apply equivalence
           for (int y = 0; y < height; ++y)
           {
               for (int x = 0; x < width; ++x)
               {
                   int index = x + y * stride;
                   byte originalLabel = p[index];
                   if (originalLabel != 0)
                   {
                       byte root = originalLabel;
                       while (equivalence.ContainsKey(root) && equivalence[root] != root)
                       {
                           root = equivalence[root];
                       }
                       p[index] = equivalence[root];
                   }
               }
           }
       }

       labeledImage.UnlockBits(imageData);
       return labeledImage;
   }
   public static Bitmap CCL2(Bitmap b)
   {
       if (b.PixelFormat != PixelFormat.Format8bppIndexed)
           ConvertTo8Bit(b);
       
       Bitmap labeledImage = AddPadding(b, 255); // Ensure proper padding
       int width = labeledImage.Width;
       int height = labeledImage.Height;

       // Matrix to store labels
       int[,] matLabels = new int[height, width];
       
       // Use a dictionary to keep track of equivalences between labels
       Dictionary<byte, byte> equivalence = new Dictionary<byte, byte>();
       byte currentLabel = 1; // Start labeling from 1

       BitmapData imageData = labeledImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
       int stride = imageData.Stride;
       IntPtr scan = imageData.Scan0;

       unsafe
       {
           byte* p = (byte*)(void*)scan;

           // First pass: labeling
           for (int y = 0; y < height; ++y)
           {
               for (int x = 0; x < width; ++x)
               {
                   int index = x + y * stride;
                   
                   if (p[index] == 0)
                   {
                       List<byte> neighborLabels = new List<byte>();
                       
                       // Collect neighboring labels
                       if (x > 0 && y > 0 && p[index - stride - 1] != 255) neighborLabels.Add(p[index - stride - 1]); // Top-left
                       if (y > 0 && p[index - stride] != 255) neighborLabels.Add(p[index - stride]); // Top
                       if (x < width - 1 && y > 0 && p[index - stride + 1] != 255) neighborLabels.Add(p[index - stride + 1]); // Top-right
                       if (x > 0 && p[index - 1] != 255) neighborLabels.Add(p[index - 1]); // Left

                       // Find smallest label among neighbors (excluding 0)
                       byte smallestLabel = 0;
                       foreach (var neighbor in neighborLabels)
                       {
                           if (neighbor != 0 && (smallestLabel == 0 || neighbor < smallestLabel))
                           {
                               smallestLabel = neighbor;
                           }
                       }

                       if (smallestLabel == 0)
                       {
                           // Assign a new label
                           p[index] = currentLabel;
                           matLabels[y, x] = currentLabel; // Note: matLabels uses [y, x] indexing
                           equivalence[currentLabel] = currentLabel; // Add to equivalence dictionary
                           currentLabel++;
                       }
                       else
                       {
                           // Assign the smallest neighbor label
                           p[index] = smallestLabel;

                           // Update equivalence table
                           foreach (byte neighbor in neighborLabels)
                           {
                               if (neighbor != 0 && neighbor != smallestLabel)
                               {
                                   byte root = neighbor;
                                   while (equivalence.ContainsKey(root) && equivalence[root] != root)
                                   {
                                       root = equivalence[root];
                                   }
                                   equivalence[root] = smallestLabel;
                               }
                           }
                       }
                   }
               }
           }

           // Second pass: applying equivalence
           for (int y = 0; y < height; ++y)
           {
               for (int x = 0; x < width; ++x)
               {
                   int index = x + y * stride;
                   byte originalLabel = p[index];
                   if (originalLabel != 0 && p[index] != 255)
                   {
                       byte root = originalLabel;
                       while (equivalence.ContainsKey(root) && equivalence[root] != root)
                       {
                           root = equivalence[root];
                       }
                       p[index] = equivalence[root];
                   }
               }
           }
       }

       labeledImage.UnlockBits(imageData);
       return labeledImage;
   }
        public static List<Bitmap> FloodFill1(Bitmap image)
   {
       Bitmap labeledImage = (Bitmap)image.Clone(); // Create a clone to store labeled image
       int width = labeledImage.Width;
       int height = labeledImage.Height;

       BitmapData imageData = labeledImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
       int stride = imageData.Stride;
       IntPtr scan0 = imageData.Scan0;

       // Create an array to keep track of labels
       byte[,] labels = new byte[width, height];
       byte currentLabel = 10;
       List<Bitmap> images = new List<Bitmap>();

       unsafe
       {
           byte* p = (byte*)(void*)scan0;

           // Iterate through each pixel in the image
           for (int y = 0; y < height; y++)
           {
               for (int x = 0; x < width; x++)
               {
                   int byteIndex = x + y * stride;

                   // Check if the pixel is a foreground pixel (black)
                   if (p[byteIndex] == 0)
                   {
                       List<int> xVals = new List<int>();
                       List<int> yVals = new List<int>();
                       // Perform connected component labeling using a flood fill algorithm (DFS/BFS)
                       Queue<Point> queue = new Queue<Point>();
                       queue.Enqueue(new Point(x, y));

                       while (queue.Count > 0)
                       {
                           Point point = queue.Dequeue();
                           int px = point.X;
                           int py = point.Y;

                           if (px >= 0 && px < width && py >= 0 && py < height && labels[px, py] == 0 && p[px + py * stride] == 0)
                           {
                               // Assign label to the current pixel
                               p[px + py * stride] = currentLabel;
                               labels[px, py] = currentLabel;
                               
                               xVals.Add(px);
                               yVals.Add(py);
                               
                               // Enqueue neighboring pixels
                               queue.Enqueue(new Point(px - 1, py));
                               queue.Enqueue(new Point(px + 1, py));
                               queue.Enqueue(new Point(px, py - 1));
                               queue.Enqueue(new Point(px, py + 1));
                           }
                       }


                       int maxX = xVals.Max();
                       int minX = xVals.Min();
                       int maxY = yVals.Max();
                       int minY = yVals.Min();

                       // Ensure valid rectangle dimensions
                       int cropWidth = maxX - minX + 1;
                       int cropHeight = maxY - minY + 1;
                       if (cropWidth > 0 && cropHeight > 0)
                       {
                           Rectangle cropRect = new Rectangle(minX, minY, cropWidth, cropHeight);
                           Bitmap subImage = (Bitmap)labeledImage.Clone(cropRect, PixelFormat.Format8bppIndexed);
                           images.Add(subImage);
                       }
                       // Increment label for the next component
                       currentLabel += 5; // Adjust as needed
                   }
               }
           }
       }

       labeledImage.UnlockBits(imageData);
       return (images);
   } 
   
   public static (Bitmap, List<byte>) FloodFill2(Bitmap image)
   {
       int width = image.Width;
       int height = image.Height;

       BitmapData imageData = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
       int stride = imageData.Stride;
       IntPtr scan0 = imageData.Scan0;

       byte currentLabel = 1;
       List<byte> names = new List<byte>();
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
                   if (p[byteIndex] == 0 && !checks[x, y])
                   {
                       // Perform connected component labeling using a stack-based approach (DFS)
                       Stack<Point> stack = new Stack<Point>();
                       stack.Push(new Point(x, y));

                       // Variables to keep track of the bounding box of the current component
                       int minX = x, maxX = x, minY = y, maxY = y;

                       while (stack.Count > 0)
                       {
                           Point point = stack.Pop();
                           int px = point.X;
                           int py = point.Y;

                           if (px >= 0 && px < width && py >= 0 && py < height && p[px + py * stride] == 0 && !checks[px, py])
                           {
                               // Assign label to the current pixel
                               p[px + py * stride] = currentLabel;
                               checks[px, py] = true;

                               // Update bounding box
                               if (px < minX) minX = px;
                               if (px > maxX) maxX = px;
                               if (py < minY) minY = py;
                               if (py > maxY) maxY = py;

                               // Push neighboring pixels onto the stack if they are valid and not checked
                               if (px - 1 >= 0 && !checks[px - 1, py]) stack.Push(new Point(px - 1, py));
                               if (px + 1 < width && !checks[px + 1, py]) stack.Push(new Point(px + 1, py));
                               if (py - 1 >= 0 && !checks[px, py - 1]) stack.Push(new Point(px, py - 1));
                               if (py + 1 < height && !checks[px, py + 1]) stack.Push(new Point(px, py + 1));
                           }
                       }

                       // Add current label to names if it hasn't been added yet
                       if (!names.Contains(currentLabel))
                       {
                           names.Add(currentLabel);
                       }

                       // Increment label for the next component
                       currentLabel++;

                       // Early exit if we have already used up all possible labels (255)
                       if (currentLabel == 256)
                       {
                           image.UnlockBits(imageData);
                           return (image, names);
                       }
                   }
               }
           }
       }

       image.UnlockBits(imageData);

       return (image, names);
   }
   public static List<Bitmap> FloodFill3(Bitmap image)
   {
       int width = image.Width;
       int height = image.Height;

       BitmapData imageData = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
       int stride = imageData.Stride;
       IntPtr scan0 = imageData.Scan0;

       byte currentLabel = 1;
       List<byte> labels = new List<byte>();
       
       bool[,] checks = new bool[width, height];

       List<Bitmap> subImages = new List<Bitmap>();

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
                       
                       List<int> xCoordinates = new List<int>();
                       List<int> yCoordinates = new List<int>();
                       
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
                               xCoordinates.Add(px);
                               yCoordinates.Add(py);
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


                       int minX = xCoordinates.Min();
                       int maxX = xCoordinates.Max();
                       int minY = yCoordinates.Min();
                       int maxY = yCoordinates.Max();

                       xCoordinates = xCoordinates.Select(n => n - minX).ToList();
                       yCoordinates = yCoordinates.Select(n => n - minY).ToList();

                       int nWidth = maxX - minX;
                       int nHeight = maxY - minY;

                       Bitmap temp = new Bitmap(DrawBitmap(nWidth, nHeight, xCoordinates, yCoordinates));
                       subImages.Add(temp);
                       
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
       //Bitmap[] cropped = ExtractShapes(image, labels);

       return (subImages);
   } 
           private static Bitmap DrawBitmap(int nWidth, int nHeight, List<int> xVals, List<int> yVals)
   {
       Bitmap output = new Bitmap(nWidth, nHeight, PixelFormat.Format8bppIndexed);
       BitmapData outputData = output.LockBits(new Rectangle(0, 0, output.Width, output.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

       IntPtr oScan = outputData.Scan0;

       int oStride = outputData.Stride;
       unsafe
       {
           byte* pOut = (byte*)(void*)oScan;
           for (int y = 0; y < nHeight; ++y)
           {
               for (int x = 0; x < nWidth; ++x)
               {
                   int byteIndex = x + y * oStride;
                   if (xVals.Contains(x) && yVals.Contains(y))
                   {
                       pOut[byteIndex] = 0;
                   }
                   else pOut[byteIndex] = 255;
               }
           }
       }
       output.UnlockBits(outputData);
       return output;
   }

*/
