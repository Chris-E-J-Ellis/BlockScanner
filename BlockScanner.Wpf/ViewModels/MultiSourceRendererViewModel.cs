namespace BlockScanner.Wpf.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Caliburn.Micro;
    using Detectors;
    using Rendering;
    using Factories;
    using Views;

    public class MultiSourceRendererViewModel : PropertyChangedBase, IRendererViewModel, IDisposable 
    {
        private readonly IEnumerable<IDetector> detectors = new List<IDetector>();
        private readonly IMultiSourceRenderer renderer;
        private readonly List<ScannerViewModel> ScannerViewModelCache = new List<ScannerViewModel>();

        private IScannerSlot selectedScannerSlot;
        private IDetector selectedDetector;

        public MultiSourceRendererViewModel(IMultiSourceRenderer renderer, IEnumerable<IDetector> detectors)
        {
            this.detectors = detectors;
            this.renderer = renderer;

            Initialise();
        }

        public ScannerViewModel Scanner
        {
            get { return ScannerViewModelCache.SingleOrDefault(s => s.Scanner == SelectedScannerSlot.Scanner); }
        }

        public IMultiSourceRenderer Renderer => this.renderer;

        public IEnumerable<IDetector> Detectors => detectors;

        public IEnumerable<IDetector> ValidDetectors => Detectors.Where(d => d.DetectedPointOutputType == SelectedScannerSlot.ScanType);

        public IDetector SelectedDetector
        {
            get { return this.selectedDetector; }
            set
            {
                this.selectedDetector = value;

                NotifyOfPropertyChange(() => SelectedDetector);
            }
        }

        public IEnumerable<IScannerSlot> ScannerSlots => Renderer.ScannerSlots; 

        public IScannerSlot SelectedScannerSlot
        {
            get { return this.selectedScannerSlot; }
            set
            {
                this.selectedScannerSlot = value;

                this.SelectedDetector = this.selectedScannerSlot?.Scanner?.Detector != null
                    ? this.selectedScannerSlot.Scanner.Detector
                    : ValidDetectors.FirstOrDefault();

                NotifyOfPropertyChange(() => SelectedScannerSlot);
                NotifyOfPropertyChange(() => ValidDetectors);
            }
        }

        public void Initialise()
        {
            SelectedScannerSlot = ScannerSlots.First();
            SelectedDetector = ValidDetectors.FirstOrDefault();
        }

        public void AttachScanner()
        {
            Scanner?.Stop();

            DetachScanner();

            var scanner = ScannerFactory.Create(SelectedDetector);

            ScannerViewModelCache.Add(new ScannerViewModel(scanner));

            Renderer.Initialise();

            SelectedScannerSlot.Assign(scanner);

            // TODO: Remove view dependency.
            var captureZone = new CaptureWindow();
            captureZone.ShowDialog();

            Scanner.SetScanArea(captureZone.SelectionAreaRectangle);

            Scanner.DumpScanArea();

            Scanner.Scan();

            NotifyOfPropertyChange(() => Scanner);
        }

        public void DetachScanner()
        {
            SelectedScannerSlot.Clear();

            ScannerViewModelCache.Remove(ScannerViewModelCache.SingleOrDefault(s => s == SelectedScannerSlot.Scanner));
        }

        public override string ToString()
        {
            return Renderer.GetType().Name;
        }

        //Temporary whilst restructuring.
        public void KillScan()
        {
            ScannerViewModelCache.ForEach(svm => svm.Stop());
        }

        public void Dispose()
        {
        }
    }
}
