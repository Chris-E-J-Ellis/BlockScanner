namespace BlockScanner.Wpf.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Drawing;
    using Caliburn.Micro;
    using Detectors;
    using Rendering;
    using Factories;
    using Views;

    public class MultiSourceRendererViewModel : PropertyChangedBase, IRendererViewModel, IDisposable 
    {
        private readonly List<IDetector> detectors = new List<IDetector>();
        private readonly IMultiSourceRenderer renderer;
        private readonly List<ScannerViewModel> ScannerViewModelCache = new List<ScannerViewModel>();

        private IScannerSlot selectedScannerSlot;
        private IDetector selectedDetector;

        public MultiSourceRendererViewModel(IMultiSourceRenderer renderer, IEnumerable<IDetector> detectors)
        {
            this.detectors.AddRange(detectors);
            this.renderer = renderer;

            Initialise();
        }

        public IMultiSourceRenderer Renderer => this.renderer;

        public IEnumerable<IDetector> ValidDetectors => detectors.Where(d => d.DetectedPointOutputType == SelectedScannerSlot.ScanType);

        public ScannerViewModel SelectedScanner => ScannerViewModelCache.SingleOrDefault(s => s.Scanner == SelectedScannerSlot.Scanner);

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

                // Ensure a valid detector is selected (if one exists).
                this.SelectedDetector = ValidDetectors.FirstOrDefault();

                NotifyOfPropertyChange(() => SelectedScannerSlot);
                NotifyOfPropertyChange(() => ValidDetectors);
            }
        }


        public void Initialise()
        {
            SelectedScannerSlot = ScannerSlots.First();
        }

        public void AttachScanner()
        {
            // Stop any existing scanner.
            SelectedScanner?.Stop();
            DetachScanner();

            Renderer.Initialise();

            // Assign a new Scanner.
            var scannerVm = CreateScannerViewModel(selectedDetector);
            SelectedScannerSlot.Assign(scannerVm.Scanner);

            scannerVm.SetScanArea(GetSelectionArea());
            scannerVm.DumpScanArea();
            scannerVm.Scan();

            NotifyOfPropertyChange(() => SelectedScanner);
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

        public void HaltScanners()
        {
            ScannerViewModelCache.ForEach(svm => svm.Stop());
        }

        public void Dispose()
        {
            ScannerViewModelCache.ForEach(svm => svm.Dispose());
        }

        private ScannerViewModel CreateScannerViewModel(IDetector detector)
        {
            var scanner = ScannerFactory.Create(detector);

            var scannerVm = new ScannerViewModel(scanner);

            ScannerViewModelCache.Add(scannerVm);

            return scannerVm;
        }

        private Rectangle GetSelectionArea()
        {
            var captureZone = new CaptureWindow();

            captureZone.ShowDialog();

            return captureZone.SelectionAreaRectangle;
        }
    }
}
