using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Task1
{
    public class BitmapFilter
    {
        public static bool BWMean(Bitmap b)
        {
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride; 
            System.IntPtr Scan0 = bmData.Scan0; 

            unsafe 
            {
                byte* p = (byte*)(void*)Scan0; 
                int nOffset = stride - b.Width*3; 

                float totalIntensity = 0; // initalize intensity
                int numPixels = b.Width * b.Height; // number of pixels

                for (int y = 0; y < b.Height; ++y)
                {
                    for (int x = 0; x < b.Width; ++x)
                    {
                        totalIntensity += p[0]; // Assuming pixel intensities are equal across all channels (grayscale)
                        p += 3; 
                    }
                    p += nOffset;
                }
                float meanIntensity = totalIntensity / numPixels;

                // Binarization
                p = (byte*)(void*)Scan0;
                for (int y = 0; y < b.Height; ++y)
                {
                    for (int x = 0; x < b.Width; ++x)
                    {
                        byte intensity = p[0];
                        byte binaryValue = (intensity >= meanIntensity) ? (byte)255 : (byte)0; // apply threshold depending on condition
                        p[0] = p[1] = p[2] = binaryValue;
                        p += 3;
                    }
                    p += nOffset;
                }
            }
            b.UnlockBits(bmData);
            return true;
        }

        public static bool BWStatic(Bitmap b, int nVal)
        {

            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;
            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                int nOffset = stride - b.Width*3;
                for (int y = 0; y < b.Height; ++y)
                {
                    for (int x = 0; x < b.Width; ++x)
                    {
                        byte intensity = p[0]; // assuming value is equal across color channels

                        byte binaryValue = (intensity >= nVal) ? (byte)255 : (byte)0; // apply threshold depending on condition
                        p[0] = p[1] = p[2] = binaryValue;
                        p += 3;

                    }
                    p += nOffset;
                }
            }
            b.UnlockBits(bmData);
            return true;
        }
    }
}
