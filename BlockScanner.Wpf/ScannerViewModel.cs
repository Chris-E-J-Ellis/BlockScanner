namespace BlockScanner.Wpf
{
    public class ScannerViewModel
    {
        private IScanner scanner;

        public ScannerViewModel() {}

        public void Initialise(IScanner scanner)
        {
            this.scanner = scanner;
        }
    }
}
