using System;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace BlockScanner.Rendering
{
    public class MonochromeBitmapRender : IRenderer<bool>
    {
        public static readonly Action<bool[][]> RenderFunc =
            frameData =>
            {
                int height = frameData.Length;
                int width = frameData.Max(x => x.Length);

                var output = new Bitmap(width, height);

                for (int i = 0; i < height; i++)
                    for (int j = 0; j < width; j++)
                        output.SetPixel(j, i, frameData[i][j] ? Color.White : Color.Black);


                output.Save("Images/output.bmp");

                var doubleSize = new Bitmap(output.Width * 2, output.Height * 2);

                using (Graphics g = Graphics.FromImage(doubleSize))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g.DrawImage(output, 0, 0, width * 2, height * 2);
                }

                doubleSize.Save("Images/outputDouble.bmp");

                // Temporary choke method, shouldn't use this to actually render output (bitmap writing is slow).
                Thread.Sleep(10000);
            };

        public void Render(bool[][] data)
        {
            RenderFunc(data);
        }
    }
}
