using System.Drawing;
using Caliburn.Micro;

namespace BlockScanner.Wpf.ViewModels
{
    public class ResizableRegionViewModel : PropertyChangedBase
    {
        public Rectangle Region => new Rectangle(X, Y, Width, Height);

        public int X { get; set; } = 8;

        public int Y { get; set; } = 8;

        public int Width { get; set; } = 100;

        public int Height { get; set; } = 100;

        public int ResizeHandleWidth => 10;

        public int ResizeHandleHeight => 10;

        public int TranslateHandleX => ResizeHandleWidth;

        public int TranslateHandleY => ResizeHandleHeight;
    }
}
