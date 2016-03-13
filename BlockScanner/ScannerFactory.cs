using BlockScanner.Detectors;
using BlockScanner.Rendering;
using System;

namespace BlockScanner
{
    public static class ScannerFactory
    {
        public static IScanner CreateBasic()
        {
            return new Scanner<bool>(new BasicDetector(), new BasicRenderer());
        }

        public static IScanner Create(IDetector detector, IRenderer renderer)
        {
            Type scannerType = typeof(Scanner<>);

            // Create a scanner using the type defined by the detector.
            var constructedType = scannerType.MakeGenericType(detector.DetectedPointOutputType);

            var scanner = Activator.CreateInstance(constructedType, new object[] { detector, renderer });

            return scanner as IScanner;
        }

        public static IScanner<T> Create<T>(IDetector<T> detector, IRenderer<T> renderer)
        {
            return new Scanner<T>(detector, renderer);
        }
    }
}
