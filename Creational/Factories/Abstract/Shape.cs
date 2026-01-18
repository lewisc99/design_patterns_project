
/// <summary>
/// Abstract Factory
/// </summary>
/// 
namespace Creational.Abstract
{
    public interface IShape
    {
        void Draw();
    }

    public class Square : IShape
    {
        public Square() { }
        public void Draw() => Console.WriteLine("Basic Square");
    }
    public class Rectangle : IShape
    {
        public Rectangle() { }

        public void Draw() => Console.WriteLine("Basic rectangle");
    }

    public class RoundedSquare : IShape
    {
        public void Draw() => Console.WriteLine("Rounded square");
    }
    public class RoundedRectangle : IShape
    {
        public void Draw() => Console.WriteLine("Rounded rectangle");
    }

    public enum Shape
    {
        Square,
        Rectangle
    }


    public abstract class ShapeFactory
    {
        public abstract IShape Create(Shape shape);
    }

    public class BasicShapeFactory : ShapeFactory
    {
        public override IShape Create(Shape shape)
        {
            switch (shape)
            {
                case Shape.Square:
                    return new Square();
                case Shape.Rectangle:
                    return new Rectangle();
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(shape), shape, null);
            }
        }
    }

    public class RoundedShapeFactory : ShapeFactory
    {
        public override IShape Create(Shape shape)
        {
            switch (shape)
            {
                case Shape.Square:
                    return new RoundedSquare();
                case Shape.Rectangle:
                    return new RoundedRectangle();
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(shape), shape, null);
            }
        }
    }


    public static class Factory
    {
        public static ShapeFactory GetFactory(bool rounded)
        {
            if (rounded)
                return new RoundedShapeFactory();
            else
                return new BasicShapeFactory();
        }
    }

    public class AbtractFactoryResult
    {
        public async void Result01()
        {
            // basic factory from BasicShapeFactory
            var basic = Factory.GetFactory(false);

            var basicRectangle = basic.Create(Shape.Rectangle);
            basicRectangle.Draw();

            // Rounded Shape Factory from RoundedShapeFactory
            var rounded = Factory.GetFactory(true).Create(Shape.Square);

            rounded.Draw();
        }
    }
}