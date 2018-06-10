using System.Windows;
using Caliburn.Micro;

namespace BlockScanner.Wpf.ViewModels
{
    public class ResizableRegionViewModel : PropertyChangedBase 
    {
        public Rect Region { get; set; }

        public int ResizeHandleWidth => 10;

        public int ResizeHandleHeight => 10;

        public int TranslateHandleX => ResizeHandleWidth;

        public int TranslateHandleY => ResizeHandleHeight;

        public ResizableRegionViewModel()
        {
            Region = new Rect(200, 200, 100, 100);
        }
    }
}
