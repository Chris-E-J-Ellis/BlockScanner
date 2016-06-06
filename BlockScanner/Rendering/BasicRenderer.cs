namespace BlockScanner.Rendering
{
    using System;
    using System.Linq;

    public class BasicRenderer : BaseRenderer<bool[][]>
    {
        public static readonly Action<bool[][]> RenderFunc =
            frameData =>
                {
                    var output = string.Join(Environment.NewLine,
                        frameData.Select(row => row.Select(block => block ? "#" : "_"))
                        .Select(block => string.Join(string.Empty, block)));

                    Console.WriteLine(output);
                };

        public override void Render(bool[][] data)
        {
            RenderFunc(data);
        }
    }
}
