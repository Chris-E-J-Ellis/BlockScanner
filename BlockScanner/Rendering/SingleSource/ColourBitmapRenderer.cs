﻿namespace BlockScanner.Rendering.SingleSource
{
    using System.Drawing;
    using System.Linq;
    using System.Threading;

    // Just a rough example.
    public class ColourBitmapRenderer : BaseSingleSourceRenderer<Color[][]>
    {
        public override void Render(Color[][] frameData)
        {
            int height = frameData.Length;
            int width = frameData.Max(x => x.Length);

            Bitmap output = new Bitmap(width, height);

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    output.SetPixel(j, i, frameData[i][j]);

            output.Save("Images/output.bmp");

            // Temporary choke method, shouldn't use this to actually render output (bitmap writing is slow).
            Thread.Sleep(1000);
        }
    }
}
