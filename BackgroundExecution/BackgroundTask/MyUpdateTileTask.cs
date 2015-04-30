using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Template10
{
    public sealed class MyUpdateTileTask : XamlRenderingBackgroundTask
    {
        protected override async void OnRun(IBackgroundTaskInstance taskInstance)
        {
            var cost = BackgroundWorkCost.CurrentBackgroundWorkCost;
            if (cost == BackgroundWorkCostValue.High)
            {
                Debug.WriteLine("Background task aborted (cost is high)");
            }
            else
            {
                // handle canceled
                taskInstance.Canceled += (s, e) =>
                {
                    Debug.WriteLine("Background task canceled");
                };

                // fetch values
                taskInstance.Task.Trigger as ApplicationTrigger

                // perform task
                var deferral = taskInstance.GetDeferral();
                try
                {
                    await UpdateTileAsync();
                    Debug.WriteLine("Background task complete");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Background task error: " + ex.Message);
                }
                finally
                {
                    deferral.Complete();
                }

            }
        }

        private async Task UpdateTileAsync()
        {
            // create UI
            var grid = new Grid
            {
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch,
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch,
            };
            var star = new FontIcon
            {
                Glyph = "",
                FontSize = 50
            };
            grid.Children.Add(star);
            var number = new TextBlock
            {
                Text = 12.ToString(),
                FontSize = 25,
                Foreground = new SolidColorBrush(Colors.White)
            };
            grid.Children.Add(number);

            // create bitmap
            var bitmap = new RenderTargetBitmap();
            await bitmap.RenderAsync(grid, 150, 150);

            // create destination file
            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync("tile.png", CreationCollisionOption.ReplaceExisting);

            // write bitmap to file
            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                var pixels = await bitmap.GetPixelsAsync();
                // using System.Runtime.InteropServices.WindowsRuntime;
                var bytes = pixels.ToArray();
                var encoder = await BitmapEncoder
                    .CreateAsync(BitmapEncoder.PngEncoderId, stream);
                var format = BitmapPixelFormat.Bgra8;
                var alpha = BitmapAlphaMode.Ignore;
                var width = (uint)bitmap.PixelWidth;
                var height = (uint)bitmap.PixelHeight;
                var dpi = DisplayInformation.GetForCurrentView().LogicalDpi;
                encoder.SetPixelData(format, alpha, width, height, dpi, dpi, bytes);
                await encoder.FlushAsync();
                stream.Seek(0);
            }
            await FileIO.WriteBytesAsync(file, null);

            // replace existing tile
            var type = TileTemplateType.TileSquare150x150Image;
            var xml = TileUpdateManager.GetTemplateContent(type);

            // no text on tile
            var elements = xml.GetElementsByTagName("binding");
            (elements[0] as XmlElement).SetAttribute("branding", "none");

            // image on tile
            elements = xml.GetElementsByTagName("image");
            (elements[0] as XmlElement).SetAttribute("src", file.Path);

            // update tile
            var notification = new TileNotification(xml);
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.Clear();
            updater.Update(notification);

            // prepare notification toast
            var toastTemplate = ToastTemplateType.ToastText01;
            xml = ToastNotificationManager.GetTemplateContent(toastTemplate);
            var text = xml.CreateTextNode(string.Format("Tile image updated to {0}", number));
            elements = xml.GetElementsByTagName("text");
            elements[0].AppendChild(text);

            // send toast
            var toast = new ToastNotification(xml);
            var notifier = ToastNotificationManager.CreateToastNotifier();
            notifier.Show(toast);

            Debug.WriteLine("Background task toast shown: " + number.ToString());
        }
    }
}
