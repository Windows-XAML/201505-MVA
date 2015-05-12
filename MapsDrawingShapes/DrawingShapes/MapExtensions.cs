using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml;

namespace MappingUtilities
{
  /// <summary>
  /// Helper methods to easily set and get view area in a more or less consistent way
  /// </summary>
  public static class MapExtensions
  {
    public static GeoboundingBox GetViewArea(this MapControl map)
    {
      Geopoint p1, p2;
      map.GetLocationFromOffset(new Point(0, 0), out p1);
      map.GetLocationFromOffset(new Point(map.ActualWidth, map.ActualHeight), out p2);
      return new GeoboundingBox(p1.Position, p2.Position);
    }

    public static void SetViewArea(this MapControl map, Geopoint p1, Geopoint p2)
    {
      var b = GeoboundingBox.TryCompute(new[] { p1.Position, p2.Position });

      map.TrySetViewBoundsAsync(b, new Thickness(1.0), MapAnimationKind.Bow);
    }
  }
}
