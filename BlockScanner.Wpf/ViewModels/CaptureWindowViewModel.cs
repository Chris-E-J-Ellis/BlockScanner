using Caliburn.Micro;
using System.Collections.Generic;
using System.Windows;

namespace BlockScanner.Wpf.ViewModels
{
    internal class CaptureWindowViewModel : Screen
    {
        public double SelectionX { get; private set; }
        public double SelectionY { get; private set; }
        public double SelectionWidth { get; private set; }
        public double SelectionHeight { get; private set; }

        public Rect SelectionArea { get; private set; }
        public ResizableRegionViewModel CaptureRegion { get; private set; } = new ResizableRegionViewModel();

        public ICollection<ResizableRegionViewModel> CaptureRegions => new List<ResizableRegionViewModel>() { CaptureRegion };
    }
}
