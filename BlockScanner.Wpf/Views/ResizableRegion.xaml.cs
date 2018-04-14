using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace BlockScanner.Wpf.Views
{
    /// <summary>
    /// Interaction logic for ResizableRegion.xaml
    /// </summary>
    public partial class ResizableRegion : UserControl, INotifyPropertyChanged
    {
        public ResizableRegion()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int ResizeHandleWidth => 10;

        public int ResizeHandleHeight => 10;

        public int TranslateHandleX => ResizeHandleWidth;

        public int TranslateHandleY => ResizeHandleHeight;

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //Move the Thumb to the mouse position during the drag operation
            var thumb = sender as Thumb;

            var newThumbX = Canvas.GetLeft(thumb) + e.HorizontalChange;
            var newThumbY = Canvas.GetTop(thumb) + e.VerticalChange;

            Canvas.SetLeft(thumb, newThumbX);
            Canvas.SetTop(thumb, newThumbY);

            var newRegionX = Canvas.GetLeft(Region);
            var newRegionY = Canvas.GetTop(Region);
            var newRegionWidth = Region.Width;
            var newRegionHeight = Region.Height;

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
                case nameof(Region):
                    newRegionX = newThumbX;
                    newRegionY = newThumbY;
                    break;
                default:
                    break;
            }

            UpdateRegion(newRegionX, newRegionY, newRegionWidth, newRegionHeight);
            UpdateHandlePositions();
        }

        private void UpdateRegion(double x, double y, double width, double height)
        {
            Canvas.SetLeft(Region, x);
            Canvas.SetTop(Region, y);

            Region.Width = width > 0 ? width : Region.Width;
            Region.Height = height > 0 ? height : Region.Height;
        }

        private void UpdateHandlePositions()
        {
            var regionX = Canvas.GetLeft(Region);
            var regionY = Canvas.GetTop(Region);
            var regionWidth = Region.Width;
            var regionHeight = Region.Height;
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
    }
}
