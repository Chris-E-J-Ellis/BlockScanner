namespace BlockScanner.Helpers
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    public static class BitmapHelper
    {
        public static BitmapData ExtractBitmapData(Bitmap bitmap, Rectangle lockRect)
        {
            BitmapData data = bitmap.LockBits(lockRect, ImageLockMode.ReadWrite, bitmap.PixelFormat);

            bitmap.UnlockBits(data);

            return data;
        }

        public static byte[] ExtractBitmapBytes(Bitmap bitmap, Rectangle lockRect)
        {
            BitmapData data;

            return ExtractBitmapBytes(bitmap, lockRect, out data);
        }

        public static byte[] ExtractBitmapBytes(Bitmap bitmap, Rectangle lockRect, out BitmapData data)
        {
            data = bitmap.LockBits(lockRect, ImageLockMode.ReadWrite, bitmap.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = data.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(data.Stride) * data.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            // Unlock the bits.
            bitmap.UnlockBits(data);

            return rgbValues;
        }
    }
}
