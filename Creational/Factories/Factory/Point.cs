/// <summary>
/// Simple Factory
/// </summary>

public partial class Point
{
    private double x, y;

    public Point(double x, double y)
    {
        this.x = x; this.y = y;
    }
    Point(float r, float theta)
    {
        x = r * Math.Cos(theta);
        y = r * Math.Sin(theta);
    }

    //  public Point(double a,
    //double b, // names do not communicate intent
    //CoordinateSystem cs = CoordinateSystem.Cartesian)
    //  {
    //      switch (cs)
    //      {
    //          case CoordinateSystem.Polar:
    //              x = a * Math.Cos(b);
    //              y = a * Math.Sin(b);
    //              break;
    //          default:
    //              x = a;
    //              y = b;
    //              break;
    //      }
    //  }

    public static Point NewCartesianPoint(double x, double y)
    {
        return new Point(x, y);
    }
    public static Point NewPolarPoint(double rho, double theta)
    {
        return new Point(rho * Math.Cos(theta), rho * Math.Sin(theta));
    }

    public override string ToString()
    {
        return $"x = {x}, y = {y}";
    }
}


public partial class Point
{
    public static class Factory
    {
        public static Point NewCartesianPoint(double x, double y)
        {
            return new Point(x, y);
        }
    }
}

class PointFactory
{
    public static Point NewCartesianPoint(float x, float y)
    {
        return new Point(x, y);
    }
}

public enum CoordinateSystem
{
    Cartesian,
    Polar
}

public class FactoryResult
{
    public void Result01()
    {
        var point = Point.NewPolarPoint(5, Math.PI / 4);
        Console.WriteLine(point);
    }
}