namespace BlockScanner.Wpf
{
    public class ScannerViewModel
    {
        private Scanner scanner;

        public ScannerViewModel() {}

        public void Initialise(Scanner scanner)
        {
            this.scanner = scanner;
        }
    }
}
