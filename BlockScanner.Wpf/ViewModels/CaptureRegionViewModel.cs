using System.Windows;

namespace BlockScanner.Wpf.ViewModels
{
    public class CaptureRegionViewModel
    {
        public Rect Region { get; private set; }

        public CaptureRegionViewModel()
        {
            Region = new Rect(100, 100, 200, 200);
        }
    }
}
