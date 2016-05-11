namespace BlockScanner.Wpf.ViewModels
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Drawing;
    using Detectors;
    using Factories;
    using Rendering;
    using Helpers;
    using Views;

    public class ShellViewModel : Caliburn.Micro.PropertyChangedBase, IShell
    {
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public readonly List<IDetector> detectors = new List<IDetector>();
        public readonly List<IRenderer> renderers = new List<IRenderer>();

        public IDetector selectedDetector;

        public ShellViewModel()
        {
            Initialise();
        }

        public ScannerViewModel Scanner { get; private set; }

        public List<IDetector> Detectors { get; private set; } = new List<IDetector>();
        public List<IRenderer> Renderers { get; private set; } = new List<IRenderer>();

        public IDetector SelectedDetector
        {
            get { return this.selectedDetector; }
            set
            {
                this.selectedDetector = value;
                NotifyOfPropertyChange(() => SelectedDetector);
            }
        }

        public IRenderer SelectedRenderer { get; set; }

        public void Initialise()
        {
            InteropHelpers.AllocConsole();

            LoadDetectors();
            LoadRenderers();

            Scanner = new ScannerViewModel(ScannerFactory.CreateBasic());
        }

        public void BeginSelection()
        {
            // TODO: Remove view dependency.
            var captureZone = new CaptureWindow();
            captureZone.ShowDialog();

            // Should use a cancellation token.
            cancellationTokenSource.Cancel();

            cancellationTokenSource = new CancellationTokenSource();

            Task scanTask = new Task(() =>
            {
                Scanner.SetScanArea(captureZone.SelectionAreaRectangle);

                Scanner.Scan(cancellationTokenSource.Token);
            }, cancellationTokenSource.Token);

            scanTask.Start();
        }

        public void DumpScanArea()
        {
            // TODO: Move path to config.
            Scanner.Scanner.DumpScanArea("Images/cap.bmp");
        }

        public void RunTestArea()
        {
            cancellationTokenSource.Cancel();

            cancellationTokenSource = new CancellationTokenSource();

            Task scanTask = new Task(() =>
            {
                Scanner.SetScanArea(new Rectangle(0, 0, 400, 400));

                Scanner.Scan(cancellationTokenSource.Token);
            }, cancellationTokenSource.Token);

            scanTask.Start();
        }

        public void CreateScanner()
        {
            Scanner.Dispose();

            var scanner = ScannerFactory.Create(SelectedDetector, SelectedRenderer);
            Scanner = new ScannerViewModel(scanner);

            NotifyOfPropertyChange(() => Scanner);
        }

        private void LoadDetectors()
        {
            var detectors = DetectorFactory.Instance.LoadConcreteObjects();

            Detectors.AddRange(detectors);
        }

        private void LoadRenderers()
        {
            var renderers = RendererFactory.Instance.LoadConcreteObjects();

            Renderers.AddRange(renderers);
        }
    }
}