using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace BlockScanner.Wpf.Views
{
    /// <summary>
    /// Interaction logic for ResizableRegionView.xaml
    /// </summary>
    public partial class ResizableRegionView : UserControl
    {
        public ResizableRegionView()
        {
            InitializeComponent();
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //Move the Thumb to the mouse position during the drag operation
            var thumb = sender as Thumb;

            var newThumbX = Canvas.GetLeft(thumb) + e.HorizontalChange;
            var newThumbY = Canvas.GetTop(thumb) + e.VerticalChange;

            Canvas.SetLeft(thumb, newThumbX);
            Canvas.SetTop(thumb, newThumbY);

            var newRegionX = Canvas.GetLeft(MainRegion);
            var newRegionY = Canvas.GetTop(MainRegion);
            var newRegionWidth = MainRegion.Width;
            var newRegionHeight = MainRegion.Height;

            // Resize rectangle.
            switch (thumb.Name)
            {
                case nameof(ThumbTL):
                    newRegionX = newThumbX;
                    newRegionY = newThumbY;
                    newRegionWidth = newRegionWidth - e.HorizontalChange;
                    newRegionHeight = newRegionHeight - e.VerticalChange;
                    break;
                case nameof(ThumbTR):
                    newRegionY = newThumbY;
                    newRegionWidth = newRegionWidth + e.HorizontalChange;
                    newRegionHeight = newRegionHeight - e.VerticalChange;
                    break;
                case nameof(ThumbBL):
                    newRegionX = newThumbX;
                    newRegionWidth = newRegionWidth - e.HorizontalChange;
                    newRegionHeight = newRegionHeight + e.VerticalChange;
                    break;
                case nameof(ThumbBR):
                    newRegionWidth = newRegionWidth + e.HorizontalChange;
                    newRegionHeight = newRegionHeight + e.VerticalChange;
                    break;
                case nameof(MainRegion):
                    break;
                default:
                    break;
            }

            UpdateRegion(newRegionX, newRegionY, newRegionWidth, newRegionHeight);
            UpdateHandlePositions();
        }

        private void UpdateRegion(double x, double y, double width, double height)
        {
            Canvas.SetLeft(MainRegion, x);
            Canvas.SetTop(MainRegion, y);

            MainRegion.Width = width > 0 ? width : MainRegion.Width;
            MainRegion.Height = height > 0 ? height : MainRegion.Height;
        }

        private void UpdateHandlePositions()
        {
            var regionX = Canvas.GetLeft(MainRegion);
            var regionY = Canvas.GetTop(MainRegion);
            var regionWidth = MainRegion.Width;
            var regionHeight = MainRegion.Height;
            var translationX = 10;
            var translationY = 10;

            Canvas.SetLeft(ThumbTL, regionX);
            Canvas.SetLeft(ThumbBL, regionX);
            Canvas.SetLeft(ThumbTR, regionX + regionWidth - translationX);
            Canvas.SetLeft(ThumbBR, regionX + regionWidth - translationX);
            Canvas.SetTop(ThumbTL, regionY);
            Canvas.SetTop(ThumbTR, regionY);
            Canvas.SetTop(ThumbBL, regionY + regionHeight - translationY);
            Canvas.SetTop(ThumbBR, regionY + regionHeight - translationY);
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            AddScreenCorrectionTransforms();

            UpdateHandlePositions();
        }

        private void AddScreenCorrectionTransforms()
        {
            // A slight hack to deal with Windows and variable window borders. 
            // I'm not sure this will work well, or at all with DPI scaling.
            var screenDelta =  MainRegion.PointToScreen(new Point(0,0));
            var screenCorrectionTransform = new TranslateTransform(-screenDelta.X, -screenDelta.Y);

            MainRegion.RenderTransform = new TransformGroup() { Children = new TransformCollection() { screenCorrectionTransform } };
            AddTransform(ThumbBL, screenCorrectionTransform);
            AddTransform(ThumbBR, screenCorrectionTransform);
            AddTransform(ThumbTL, screenCorrectionTransform);
            AddTransform(ThumbTR, screenCorrectionTransform);
        }

        private void AddTransform(Thumb item, TranslateTransform transform)
        {
            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(item.RenderTransform);
            transformGroup.Children.Add(transform);
            item.RenderTransform = transformGroup;
        }
    }
}
