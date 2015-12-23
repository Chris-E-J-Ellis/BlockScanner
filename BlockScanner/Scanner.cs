using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace BlockScanner
{
    public class Scanner
    {
        public static Bitmap LoadBMP(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            var image = new Bitmap(path);
            return image;
        }

        public static Tuple<byte[], BitmapData> GetBytes (Bitmap playField)
        {
            Rectangle rect = new Rectangle(0, 0, playField.Width, playField.Height);

            BitmapData data = playField.LockBits(rect, ImageLockMode.ReadWrite, playField.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = data.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(data.Stride) * data.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            return new Tuple<byte[], BitmapData> (rgbValues, data);
        }
    }
}
