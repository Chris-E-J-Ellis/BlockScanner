namespace BlockScanner.Wpf.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using Detectors;
    using Factories;
    using Helpers;
    using Caliburn.Micro;
    using Rendering;
    using System;
    public class ShellViewModel : PropertyChangedBase, IShell
    {
        private readonly List<IDetector> detectors = new List<IDetector>();
        private readonly List<IRendererViewModel> rendererSetups = new List<IRendererViewModel>();

        private IRendererViewModel selectedRendererSetup;

        private int consoleVisible = InteropHelper.SW_SHOW;

        public ShellViewModel()
        {
            Initialise();
        }

        public IRendererViewModel SelectedRendererSetup
        {
            get { return this.selectedRendererSetup; }
            set
            {
                this.selectedRendererSetup?.HaltScanners();
                this.selectedRendererSetup = value;

                NotifyOfPropertyChange(() => SelectedRendererSetup);
            }
        }

        public IEnumerable<IRendererViewModel> RendererSetups => rendererSetups;

        public void Initialise()
        {
            InteropHelper.AllocConsole();
            consoleVisible = InteropHelper.SW_SHOW;

            LoadDetectors();
            LoadRendererSetups();

            SelectedRendererSetup = rendererSetups.FirstOrDefault();
        }

        public void ToggleConsole()
        {
            var handle = InteropHelper.GetConsoleWindow();

            consoleVisible = consoleVisible == InteropHelper.SW_HIDE
                ? InteropHelper.SW_SHOW
                : InteropHelper.SW_HIDE;

            InteropHelper.ShowWindow(handle, consoleVisible);
        }

        private void LoadDetectors()
        {
            try
            {
                var loadedDetectors = DetectorFactory.Instance.LoadConcreteObjects();

                detectors.AddRange(loadedDetectors);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An exception occurred whilst loading detectors: {ex}");
            }
        }

        private void LoadRendererSetups()
        {
            try
            {
                var loadedRenderers = RendererFactory.Instance.LoadConcreteObjects();

                rendererSetups.AddRange(loadedRenderers.OfType<ISingleSourceRenderer>().Select(r => new SingleSourceRendererViewModel(r, detectors)));
                rendererSetups.AddRange(loadedRenderers.OfType<IMultiSourceRenderer>().Select(r => new MultiSourceRendererViewModel(r, detectors)));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An exception occurred whilst loading renderers: {ex}");
            }
        }
    }
}