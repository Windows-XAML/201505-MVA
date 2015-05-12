using System;
using System.Linq;
using System.Text;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Navigation;
using MappingUtilities;
using Windows.UI.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DrawingShapes
{
  /// <summary>
  /// LocalJoost Drawing shapes demo
  /// </summary>
  public sealed partial class MainPage : Page
  {
    public MainPage()
    {
      this.InitializeComponent();

      this.NavigationCacheMode = NavigationCacheMode.Required;
    }

    /// <summary>
    /// Invoked when this page is about to be displayed in a Frame.
    /// </summary>
    /// <param name="e">Event data that describes how this page was reached.
    /// This parameter is typically used to configure the page.</param>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      //Geopoint and construction
      MyMap.Center = new Geopoint(new BasicGeoposition { Latitude = 52.181427, Longitude = 5.399780 });
      MyMap.ZoomLevel = 16;
      base.OnNavigatedTo(e);
    }

    private void DrawLines(object sender, RoutedEventArgs e)
    {
        if (!DeleteShapesFromLevel(4))
        {
            var strokeColor = Colors.Green;

            //Drawing lines, notice the use of Geopath. Consists out of BasicGeopositions
            foreach (var dataObject in PointList.GetLines())
            {
                ///////////////////////////////////////////////////
                // Creating the a MapPolyLine 
                //   (stroke, path (list of BasicGeopositions)
                var shape = new MapPolyline
                {
                    StrokeThickness = 9,
                    StrokeColor = strokeColor,
                    StrokeDashed = false,
                    ZIndex = 4,
                    Path = new Geopath(dataObject.Points.Select(p => p.Position))
                };
                shape.AddData(dataObject);

                MyMap.MapElements.Add(shape);
            }
        }
    }

    private void DrawShapes(object sender, RoutedEventArgs e)
    {
        if (!DeleteShapesFromLevel(3))
        {
            var strokeColor = Colors.DarkBlue;
            var fillColor = Colors.Blue;

            ///////////////////////////////////////////////////
            // Creating the a MapPolygons 
            //   (stroke, fill, path (list of BasicGeopositions)
            foreach (var dataObject in PointList.GetAreas())
            {
                var shape = new MapPolygon
                {
                    StrokeThickness = 3,
                    StrokeColor = strokeColor,
                    FillColor = fillColor,
                    StrokeDashed = false,
                    ZIndex = 3,
                    Path = new Geopath(dataObject.Points.Select(p => p.Position))
                };
                shape.AddData(dataObject);
                MyMap.MapElements.Add(shape);
            }
        }
    }

    private void DrawPoints(object sender, RoutedEventArgs e)
    {
      // How to draw a new MapIcon with a label, anchorpoint and custom  icon.
      // Icon comes from shared project assets
      var anchorPoint = new Point(0.5, 0.5);
      var image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/wplogo.png"));

      // Helper extension method
      var area = MyMap.GetViewArea();

      // GeoboundingBox countains two BasicGeopositions....
      // PointList is just a helper class that gives 'some data'
      var points = PointList.GetRandomPoints(new Geopoint(area.NorthwestCorner), new Geopoint(area.SoutheastCorner), 50);
      foreach (var dataObject in points)
      {
        ///////////////////////////////////////////////////
        // Creating the MapIcon 
        //   (text, image, location, anchor point)
        var shape = new MapIcon
        {
          Title = dataObject.Name,
          Location = dataObject.Points.First(),
          NormalizedAnchorPoint = anchorPoint,
          Image = image,
          ZIndex = 5
        };
        shape.AddData(dataObject);

        MyMap.MapElements.Add(shape);
      }
    }

    private void DeleteAll(object sender, RoutedEventArgs e)
    {
      MyMap.MapElements.Clear();
    }

    private bool DeleteShapesFromLevel(int zIndex)
    {
      // Delete shapes by z-index 
      var shapesOnLevel = MyMap.MapElements.Where(p => p.ZIndex == zIndex);
      if (shapesOnLevel.Any())
      {
        foreach (var shape in shapesOnLevel.ToList())
        {
          MyMap.MapElements.Remove(shape);
        }
        return true;
      }
      return false;
    }

    //MapTapped - not Map.Tapped!!
    private async void MyMap_MapTapped(MapControl sender, MapInputEventArgs args)
    {
      var resultText = new StringBuilder();
      resultText.AppendLine(string.Format("Position={0},{1}", args.Position.X, args.Position.Y));
      resultText.AppendLine(string.Format("Location={0:F9},{1:F9}", args.Location.Position.Latitude, args.Location.Position.Longitude));

      foreach( var mapObject in sender.FindMapElementsAtOffset(args.Position))
      {
        resultText.AppendLine("Found: " + mapObject.ReadData<PointList>().Name);
      }
      var dialog = new MessageDialog(resultText.ToString());
      await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await dialog.ShowAsync());
    }

    private void ZoomOut(object sender, RoutedEventArgs e)
    {
      var newZoom = MyMap.ZoomLevel - 1;
      if (newZoom < 1) newZoom = 1;
      MyMap.ZoomLevel = newZoom;
    }

    private void ZoomIn(object sender, RoutedEventArgs e)
    {
      var newZoom = MyMap.ZoomLevel + 1;
      if (newZoom > MyMap.MaxZoomLevel) newZoom = MyMap.MaxZoomLevel;
      MyMap.ZoomLevel = newZoom;
    }
  }
}
