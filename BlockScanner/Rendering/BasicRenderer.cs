using System;
using System.Linq;

namespace BlockScanner.Rendering
{
    public class BasicRenderer : IRenderer<bool>
    {
        public static readonly Action<bool[][]> RenderFunc =
            frameData =>
                {
                    var output = string.Join(Environment.NewLine,
                        frameData.Select(row => row.Select(block => block ? "#" : "_"))
                        .Select(block => string.Join(string.Empty, block)));

                    Console.WriteLine(output);
                };

        public void Render(bool[][] data)
        {
            RenderFunc(data);
        }
    }
}
