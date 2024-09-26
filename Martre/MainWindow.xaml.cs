using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Martre;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Loaded += (o, e) =>
        {
            try
            {
                var image = new Image<Rgba32>((int)ActualWidth, (int)ActualHeight);

                image.Mutate(x => x.DrawPolygon(Pens.Solid(SixLabors.ImageSharp.Color.Black), [new PointF(0f, 0f), new PointF(5f, 0f), new PointF(0f, 5f), new PointF(0f, 0f)]));

                var bmp = new WriteableBitmap(image.Width, image.Height, image.Metadata.HorizontalResolution, image.Metadata.VerticalResolution, PixelFormats.Bgra32, null);

                bmp.Lock();
                try
                {

                    using Image<Rgba32> _image = image;
                    _image.ProcessPixelRows(accessor =>
                    {
                        var backBuffer = bmp.BackBuffer;

                        for (var y = 0; y < image.Height; y++)
                        {
                            Span<Rgba32> pixelRow = accessor.GetRowSpan(y);

                            for (var x = 0; x < image.Width; x++)
                            {
                                var backBufferPos = backBuffer + (y * image.Width + x) * 4;
                                var rgba = pixelRow[x];
                                var color = rgba.A << 24 | rgba.R << 16 | rgba.G << 8 | rgba.B;

                                System.Runtime.InteropServices.Marshal.WriteInt32(backBufferPos, color);
                            }
                        }
                    });

                    bmp.AddDirtyRect(new Int32Rect(0, 0, image.Width, image.Height));
                }
                finally
                {
                    bmp.Unlock();
                }

                DrawingCanvas.Source = bmp;
            }
            catch (Exception ex)
            {
                ;
            }
        };
    }
}