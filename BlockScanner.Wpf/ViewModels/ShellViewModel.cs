namespace BlockScanner.Wpf.ViewModels
{
    using System.Collections.Generic;
    using Detectors;
    using Factories;
    using Rendering;
    using Helpers;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Drawing;
    using Views;

    public class ShellViewModel : Caliburn.Micro.PropertyChangedBase, IShell
    {
        private IScanner scanner;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public ShellViewModel()
        {
            Initialise();
        }

        public List<IDetector> Detectors { get; private set; } = new List<IDetector>();
        public List<IRenderer> Renderers { get; private set; } = new List<IRenderer>();

        public IDetector SelectedDetector { get; set; }
        public IRenderer SelectedRenderer { get; set; }

        public string XCoord { get; set; }
        public string YCoord { get; set; }
        public string ScanWidth { get; set; }
        public string ScanHeight { get; set; }

        public void Initialise()
        {
            InteropHelpers.AllocConsole();

            DetectorFactory.Instance.LoadTypes();
            RendererFactory.Instance.LoadTypes();

            LoadDetectors();
            LoadRenderers();

            scanner = ScannerFactory.CreateBasic();
        }

        public void BeginSelection()
        {
            var captureZone = new CaptureWindow(null);
            captureZone.ShowDialog();

            XCoord = captureZone.SelectionX.ToString();
            YCoord = captureZone.SelectionY.ToString();
            ScanWidth = captureZone.SelectionWidth.ToString();
            ScanHeight = captureZone.SelectionHeight.ToString();

            // Should use a cancellation token.
            cancellationTokenSource.Cancel();

            cancellationTokenSource = new CancellationTokenSource();

            Task scanTask = new Task(() =>
            {
                scanner.Initialise(
                    new Rectangle((int)captureZone.SelectionX, (int)captureZone.SelectionY, (int)captureZone.SelectionWidth, (int)captureZone.SelectionHeight));

                scanner.Scan(cancellationTokenSource.Token);
            }, cancellationTokenSource.Token);

            scanTask.Start();
        }

        public void DumpScanArea()
        {
            scanner.DumpScanArea("Images/cap.bmp");
        }

        public void RunTestArea()
        {
            cancellationTokenSource.Cancel();

            cancellationTokenSource = new CancellationTokenSource();

            Task scanTask = new Task(() =>
            {
                scanner.Initialise(new Rectangle(0, 0, 400, 400));
                scanner.Scan(cancellationTokenSource.Token);
            }, cancellationTokenSource.Token);

            scanTask.Start();
        }

        public void CreateScanner()
        {
            this.scanner = ScannerFactory.Create(SelectedDetector, SelectedRenderer);
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