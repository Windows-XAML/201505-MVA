using System;
using System.Collections.Generic;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.DragDrop;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;


namespace DragAndDropSample
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        async private void UpdateOutput(DragEventArgs e)
        {
            ClearOutput();

            OutputCursorPosition.Text = e.GetPosition(MainGrid).X + "," + e.GetPosition(MainGrid).Y;

            string keys = "";
            if (e.Modifiers.HasFlag(DragDropModifiers.Alt))
                keys += "Alt ";
            if (e.Modifiers.HasFlag(DragDropModifiers.Control))
                keys += "Control ";
            if (e.Modifiers.HasFlag(DragDropModifiers.LeftButton))
                keys += "LeftButton ";
            if (e.Modifiers.HasFlag(DragDropModifiers.MiddleButton))
                keys += "MiddleButton ";
            if (e.Modifiers.HasFlag(DragDropModifiers.None))
                keys += "None ";
            if (e.Modifiers.HasFlag(DragDropModifiers.RightButton))
                keys += "RightButton ";
            if (e.Modifiers.HasFlag(DragDropModifiers.Shift))
                keys += "Shift ";
            OutputModifiers.Text = keys;

            string acceptedOperations = "";
            if (e.AcceptedOperation.HasFlag(DataPackageOperation.Copy))
                acceptedOperations += "Copy ";
            if (e.AcceptedOperation.HasFlag(DataPackageOperation.Move))
                acceptedOperations += "Move ";
            if (e.AcceptedOperation.HasFlag(DataPackageOperation.Link))
                acceptedOperations += "Link ";
            if (e.AcceptedOperation.HasFlag(DataPackageOperation.None))
                acceptedOperations += "None ";
            OutputAcceptedOperations.Text = acceptedOperations;

            string requestedOperations = "";
            if (e.DataView.RequestedOperation.HasFlag(DataPackageOperation.Copy))
                requestedOperations += "Copy ";
            if (e.DataView.RequestedOperation.HasFlag(DataPackageOperation.Move))
                requestedOperations += "Move ";
            if (e.DataView.RequestedOperation.HasFlag(DataPackageOperation.Link))
                requestedOperations += "Link ";
            if (e.DataView.RequestedOperation.HasFlag(DataPackageOperation.None))
                requestedOperations += "None ";
            OutputRequestedOperations.Text = requestedOperations;

            if (e.DataView.Contains(StandardDataFormats.ApplicationLink))
                OutputDPAppLink.Text = (await e.DataView.GetApplicationLinkAsync()).ToString();
            if (e.DataView.Contains(StandardDataFormats.WebLink))
                OutputDPWebLink.Text = (await e.DataView.GetWebLinkAsync()).ToString();
            if (e.DataView.Contains(StandardDataFormats.Text))
                OutputDPText.Text = await e.DataView.GetTextAsync();
            if (e.DataView.Contains(StandardDataFormats.Rtf))
                OutputDPRtf.Text = await e.DataView.GetRtfAsync();
            if (e.DataView.Contains(StandardDataFormats.Html))
                OutputDPHTML.Text = await e.DataView.GetHtmlFormatAsync();

            if (e.DataView.Contains(StandardDataFormats.Bitmap))
            {
                BitmapImage image = new BitmapImage();
                var streamRef = await e.DataView.GetBitmapAsync();
                var stream = await streamRef.OpenReadAsync();
                image.SetSource(stream);
                OutputDPBitmap.Source = image;
            }

            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var files = await e.DataView.GetStorageItemsAsync();
                foreach (var file in files)
                {
                    TextBlock t = new TextBlock();
                    t.Text = file.Name;
                    OutputDPFiles.Children.Add(t);
                }
            }
        }

        private void ClearOutput()
        {
            OutputCursorPosition.Text = "";
            OutputModifiers.Text = "";
            OutputAcceptedOperations.Text = "";
            OutputRequestedOperations.Text = "";
            OutputDPAppLink.Text = "";
            OutputDPWebLink.Text = "";
            OutputDPText.Text = "";
            OutputDPRtf.Text = "";
            OutputDPHTML.Text = "";
            OutputDPBitmap.Source = null;
            OutputDPFiles.Children.Clear();
        }

        private void Move_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move;
            UpdateOutput(e);
        }

        private void Copy_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
            UpdateOutput(e);
        }

        private void Link_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Link;
            UpdateOutput(e);
        }

        private void None_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.None;
            UpdateOutput(e);
        }

        private void RemoveDecoration_DragOver(object sender, DragEventArgs e)
        {
            e.DragUIOverride.IsCaptionVisible = false;
            e.DragUIOverride.IsGlyphVisible = false;
            e.DragUIOverride.IsContentVisible = false;
            UpdateOutput(e);
        }

        private void ApplyDecoration_DragOver(object sender, DragEventArgs e)
        {
            e.DragUIOverride.IsCaptionVisible = true;
            e.DragUIOverride.IsGlyphVisible = true;
            e.DragUIOverride.IsContentVisible = true;
            UpdateOutput(e);
        }

        private void Custom_DragOver(object sender, DragEventArgs e)
        {
            e.DragUIOverride.SetContentFromBitmapImage(new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/jerry.png")));
            e.DragUIOverride.IsGlyphVisible = true;
            e.DragUIOverride.IsContentVisible = true;
            UpdateOutput(e);
        }

        private void ApplyCustomText_DragOver(object sender, DragEventArgs e)
        {
            e.DragUIOverride.IsCaptionVisible = true;
            e.DragUIOverride.IsGlyphVisible = true;
            e.DragUIOverride.IsContentVisible = true;
            e.DragUIOverride.Caption = CustomText.Text;
            UpdateOutput(e);
        }

        private void Clear_DragLeave(object sender, DragEventArgs e)
        {
            ClearOutput();
        }

        async private void DragImage_DragStarting(UIElement sender, DragStartingEventArgs args)
        {
            // get the file
            StorageFolder installFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFolder subFolder = await installFolder.GetFolderAsync("Assets");
            var file = await subFolder.GetFileAsync("MSLogoImage.png");

            // turn it into a stream and set it as a bitmap in the datapackage
            var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            args.Data.SetBitmap(RandomAccessStreamReference.CreateFromStream(stream));

            // also set it as a file in the datapackage
            var files = new List<StorageFile>();
            files.Add(file);
            args.Data.SetStorageItems(files);

            args.Data.RequestedOperation = SetRequestedOperation();
        }

        private void DragText_DragStarting(UIElement sender, DragStartingEventArgs args)
        {
            args.Data.SetText(DragText.Text);
            args.Data.SetRtf(DragText.Text);
            args.Data.RequestedOperation = SetRequestedOperation();
        }

        private DataPackageOperation SetRequestedOperation()
        {
            if (MoveRequestedOperation.IsChecked.Value && CopyRequestedOperation.IsChecked.Value && LinkRequestedOperation.IsChecked.Value)
                return DataPackageOperation.Move | DataPackageOperation.Copy | DataPackageOperation.Link;
            else if (MoveRequestedOperation.IsChecked.Value && CopyRequestedOperation.IsChecked.Value)
                return DataPackageOperation.Move | DataPackageOperation.Copy;
            else if (MoveRequestedOperation.IsChecked.Value && LinkRequestedOperation.IsChecked.Value)
                return DataPackageOperation.Move | DataPackageOperation.Link;
            else if (CopyRequestedOperation.IsChecked.Value && LinkRequestedOperation.IsChecked.Value)
                return DataPackageOperation.Copy | DataPackageOperation.Link;
            else if (CopyRequestedOperation.IsChecked.Value)
                return DataPackageOperation.Copy;
            else if (MoveRequestedOperation.IsChecked.Value)
                return DataPackageOperation.Move;
            else if (LinkRequestedOperation.IsChecked.Value)
                return DataPackageOperation.Link;
            else
                return DataPackageOperation.None;
        }

        private void DragTriggerDetection_Checked(object sender, RoutedEventArgs e)
        {
            if (DragTriggerDetection.IsChecked.Value) // let xaml do the drag trigger detection
            {
                DragImage.PointerPressed -= DragImage_PointerPressed;
                DragImage.PointerReleased -= DragImage_PointerReleased;
                DragImage.PointerMoved -= DragImage_PointerMoved;
                DragImage.CanDrag = true;

                DragText.PointerPressed -= DragText_PointerPressed;
                DragText.PointerReleased -= DragText_PointerReleased;
                DragText.PointerMoved -= DragText_PointerMoved;
                DragText.CanDrag = true;
            }
            else // use manual drag trigger and call StartDragAsync
            {
                DragImage.PointerPressed += DragImage_PointerPressed;
                DragImage.PointerReleased += DragImage_PointerReleased;
                DragImage.PointerMoved += DragImage_PointerMoved;
                DragImage.CanDrag = false;

                DragText.PointerPressed += DragText_PointerPressed;
                DragText.PointerReleased += DragText_PointerReleased;
                DragText.PointerMoved += DragText_PointerMoved;
                DragText.CanDrag = false;
            }
        }

        #region DetectDragStartForDragImage
        uint _dragPointerId;
        Point _lastPressedPosition;

        private void DragImage_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (_dragPointerId == 0)
            {
                _dragPointerId = e.Pointer.PointerId;
                _lastPressedPosition = e.GetCurrentPoint(this).Position;
            }
        }

        private void DragImage_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _dragPointerId = 0;
        }

        private async void DragImage_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_dragPointerId == e.Pointer.PointerId)
            {
                var pt = e.GetCurrentPoint(this).Position;
                if (Math.Abs(pt.X - _lastPressedPosition.X) >= 5 || Math.Abs(pt.Y - _lastPressedPosition.Y) >= 5)
                {
                    _dragPointerId = 0;
                    await DragImage.StartDragAsync(e.GetCurrentPoint(DragImage));
                }
            }
        }
        #endregion

        #region DetectDragStartForDragText
        private void DragText_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (_dragPointerId == 0)
            {
                _dragPointerId = e.Pointer.PointerId;
                _lastPressedPosition = e.GetCurrentPoint(this).Position;
            }
        }

        private void DragText_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _dragPointerId = 0;
        }

        private async void DragText_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_dragPointerId == e.Pointer.PointerId)
            {
                var pt = e.GetCurrentPoint(this).Position;
                if (Math.Abs(pt.X - _lastPressedPosition.X) >= 5 || Math.Abs(pt.Y - _lastPressedPosition.Y) >= 5)
                {
                    _dragPointerId = 0;
                    await DragText.StartDragAsync(e.GetCurrentPoint(DragText));
                }
            }
        }
        #endregion

    }
}
