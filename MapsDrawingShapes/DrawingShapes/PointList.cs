using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;


namespace DrawingShapes
{
  public class PointList
  {
    public PointList()
    {
      Points = new List<Geopoint>();
    }
    public string Name { get; set; }

    public List<Geopoint> Points { get; set; }

    public static IEnumerable<PointList> GetLines()
    {
      var result = new List<PointList>
      {
        new PointList
        {
          Name = "Line1",
          Points = new List<Geopoint>
          {
            new Geopoint(new BasicGeoposition{Latitude = 52.1823604591191, Longitude = 5.3976580593735}),
            new Geopoint(new BasicGeoposition{Latitude = 52.182687940076 , Longitude = 5.39744247682393}),
            new Geopoint(new BasicGeoposition{Latitude = 52.1835449058563, Longitude = 5.40016567334533}),
            new Geopoint(new BasicGeoposition{Latitude = 52.1837400365621, Longitude = 5.40009761229157})
          }
        },
        new PointList
        {
          Name = "Line2",
          Points = new List<Geopoint>
          {
            new Geopoint(new BasicGeoposition{Latitude = 52.181295119226  , Longitude = 5.39748212322593}),
            new Geopoint(new BasicGeoposition{Latitude = 52.1793784294277, Longitude = 5.39909915998578})
          }
        }
      };

      return result;
    }

    public static IEnumerable<PointList> GetAreas()
    {
      var result = new List<PointList>
      {
        new PointList
        {
          Name = "Area1",
          Points = new List<Geopoint>
          {
            new Geopoint(new BasicGeoposition{Latitude = 52.1807858347893, Longitude = 5.39981396868825}),
            new Geopoint(new BasicGeoposition{Latitude = 52.1802563499659, Longitude = 5.40086925029755}),
            new Geopoint(new BasicGeoposition{Latitude = 52.1797477360815, Longitude = 5.40002955123782}),
            new Geopoint(new BasicGeoposition{Latitude = 52.180378222838 , Longitude = 5.39925254881382})
          }
        },
        new PointList
        {
          Name = "Area2",
          Points = new List<Geopoint>
          {
           new Geopoint(new BasicGeoposition{Latitude = 52.1818170603365, Longitude = 5.39659146219492}),
           new Geopoint(new BasicGeoposition{Latitude = 52.1818030625582, Longitude = 5.39704534225166}),
           new Geopoint(new BasicGeoposition{Latitude = 52.1812735777348, Longitude = 5.39751053787768}),
           new Geopoint(new BasicGeoposition{Latitude = 52.1816498413682, Longitude = 5.39849775843322}),
           new Geopoint(new BasicGeoposition{Latitude = 52.1813920140266, Longitude = 5.39914450608194}),
           new Geopoint(new BasicGeoposition{Latitude = 52.1806813124567, Longitude = 5.39800984784961}),
           new Geopoint(new BasicGeoposition{Latitude = 52.1812735777348, Longitude = 5.39748790673912}),
           new Geopoint(new BasicGeoposition{Latitude = 52.1810854878277, Longitude = 5.39656874723732}),
           new Geopoint(new BasicGeoposition{Latitude = 52.18176827766  , Longitude = 5.39652340114117})
          }
        }
      };
      return result;
    }

    public static IEnumerable<PointList> GetRandomPoints(Geopoint point1, Geopoint point2, int nrOfPoints)
    {
      var result = new List<PointList>();
      var p1 = new BasicGeoposition
      {
        Latitude = Math.Min(point1.Position.Latitude, point2.Position.Latitude),
        Longitude = Math.Min(point1.Position.Longitude, point2.Position.Longitude)
      };
      var p2 = new BasicGeoposition
      {
        Latitude = Math.Max(point1.Position.Latitude, point2.Position.Latitude),
        Longitude = Math.Max(point1.Position.Longitude, point2.Position.Longitude)
      };

      var dLat = p2.Latitude - p1.Latitude;
      var dLon = p2.Longitude - p1.Longitude;

      var r = new Random(DateTime.Now.Millisecond);
      for (var i = 0; i < nrOfPoints; i++)
      {
        var item = new PointList{ Name="Point " + i};
        item.Points.Add( new Geopoint(
          new BasicGeoposition{ Latitude = p1.Latitude + (r.NextDouble() * dLat),
            Longitude = p1.Longitude + (r.NextDouble() * dLon)}));
        result.Add(item);
      }
      return result;
    }
  }
}
