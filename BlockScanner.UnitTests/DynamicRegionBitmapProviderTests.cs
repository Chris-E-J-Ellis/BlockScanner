namespace BlockScanner.UnitTests
{
    using Helpers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Threading.Tasks;    
    
    // Nothing particularly great in here, just sanity checking.
    [TestClass]
    public class DynamicRegionBitmapProviderTests
    {
        [TestMethod]
        public void RegisterRegionOfInterest_SingleRegion()
        {
            // Arrange.
            var rectOne = new Rectangle(0, 0, 100, 100);
            var provider = new DynamicRegionBitmapProvider();

            // Act.
            provider.RegisterRegionOfInterest(rectOne);

            // Assert.
            Assert.AreEqual(rectOne, provider.CaptureArea);
        }

        [TestMethod]
        public void RegisterRegionOfInterest_MultipleNonOverlappingRegions()
        {
            // Arrange.
            var rectOne = new Rectangle(0, 0, 100, 100);
            var rectTwo = new Rectangle(200, 200, 100, 100);
            var expectedCaptureArea = new Rectangle(0, 0, 300, 300);
            var provider = new DynamicRegionBitmapProvider();

            // Act.
            provider.RegisterRegionOfInterest(rectOne);
            provider.RegisterRegionOfInterest(rectTwo);

            // Assert.
            Assert.AreEqual(expectedCaptureArea, provider.CaptureArea);
        }

        [TestMethod]
        public void RegisterRegionOfInterest_MultipleOverlappingRegions()
        {
            // Arrange.
            var rectOne = new Rectangle(10, 10, 200, 200);
            var rectTwo = new Rectangle(100, 100, 200, 200);
            var rectThree = new Rectangle(190, 190, 300, 300);
            var expectedCaptureArea = new Rectangle(10, 10, 480, 480);
            var provider = new DynamicRegionBitmapProvider();

            // Act.
            provider.RegisterRegionOfInterest(rectOne);
            provider.RegisterRegionOfInterest(rectTwo);
            provider.RegisterRegionOfInterest(rectThree);

            // Assert.
            Assert.AreEqual(expectedCaptureArea, provider.CaptureArea);
        }

        [TestMethod]
        public void CaptureScreenRegion_MultipleOverlappingRegions()
        {
            // Arrange.
            var rectOne = new Rectangle(10, 10, 200, 200);
            var rectTwo = new Rectangle(100, 100, 200, 200);
            var rectThree = new Rectangle(190, 190, 300, 300);
            var expectedCaptureArea = new Rectangle(10, 10, 480, 480);
            var provider = new DynamicRegionBitmapProvider();

            provider.RegisterRegionOfInterest(rectOne);
            provider.RegisterRegionOfInterest(rectTwo);
            provider.RegisterRegionOfInterest(rectThree);

            // Act.
            var captureRegion = provider.CaptureScreenRegion(rectTwo);
            var entireCaptureRegion = provider.MainCapturedRegion;

            // Assert.
            // Check the size, although doesn't explicity check the contents.
            Assert.AreEqual(rectTwo.Width, captureRegion.Width);
            Assert.AreEqual(rectTwo.Height, captureRegion.Height);
            Assert.AreEqual(entireCaptureRegion.Width, expectedCaptureArea.Width);
            Assert.AreEqual(entireCaptureRegion.Height, expectedCaptureArea.Width);
            // Check other registered regions are populated.
            Assert.AreEqual(provider.CapturedRegions.Count, 2);
            Assert.AreEqual(provider.CapturedRegions[rectOne].Width, rectOne.Width);
            Assert.AreEqual(provider.CapturedRegions[rectOne].Height, rectOne.Height);
            Assert.AreEqual(provider.CapturedRegions[rectThree].Width, rectThree.Width);
            Assert.AreEqual(provider.CapturedRegions[rectThree].Height, rectThree.Height);
        }

        // Avert your eyes.
        [TestMethod]
        public void BenchMark_MultipleOverlappingRegionsManyThreads()
        {
            // Arrange.
            var rectOne = new Rectangle(10, 10, 200, 200);
            var rectTwo = new Rectangle(100, 100, 200, 200);
            var rectThree = new Rectangle(190, 190, 300, 300);
            var rectFour = new Rectangle(390, 390, 300, 300);

            //var provider = new SimpleBitmapProvider();
            var provider = new DynamicRegionBitmapProvider();

            provider.RegisterRegionOfInterest(rectOne);
            provider.RegisterRegionOfInterest(rectTwo);
            provider.RegisterRegionOfInterest(rectThree);
            provider.RegisterRegionOfInterest(rectFour);

            int iterations = 200;
            var captureOne = new Task<string>(() => TimerLoop(() => provider.CaptureScreenRegion(rectOne), iterations, "One"));
            var captureTwo = new Task<string>(() => TimerLoop(() => provider.CaptureScreenRegion(rectTwo), iterations, "Two"));
            var captureThree = new Task<string>(() => TimerLoop(() => provider.CaptureScreenRegion(rectThree), iterations, "Three"));
            var captureFour = new Task<string>(() => TimerLoop(() => provider.CaptureScreenRegion(rectFour), iterations, "Four"));
            
            // Act.
            captureOne.Start();
            captureTwo.Start();
            captureThree.Start();
            captureFour.Start();

            captureOne.Wait();
            captureTwo.Wait();
            captureThree.Wait();
            captureFour.Wait();

            Trace.WriteLine(captureOne.Result);
            Trace.WriteLine(captureTwo.Result);
            Trace.WriteLine(captureThree.Result);
            Trace.WriteLine(captureFour.Result);
            // ~16ms on my machine with Aero theme. ~8ms with basic theme.
        }

        private string TimerLoop(Action action, int iterations, string name = "")
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < iterations; i++)
            {
                action();
            }

            stopwatch.Stop();

            return (name + " took " + stopwatch.Elapsed.TotalMilliseconds / iterations);
        }
    }
}
