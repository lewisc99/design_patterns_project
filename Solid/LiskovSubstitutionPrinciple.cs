namespace DesignPatterns.Solid.LSP.BadExample
{
    // The Base Class

    /// <summary>
    /// 
    /// The Scenario
    //    You have a system that handles geometric shapes.Mathematically, a Square is a Rectangle. So, in your code, you decide to make the Square class inherit from the Rectangle class.

    //However, a Square has a specific rule: its width and height must always be equal.Enforcing this rule inside a subclass breaks the behavior that other parts of your code expect from a standard Rectangle.
    /// </summary>
    public class Rectangle
    {
        // Virtual allows the child class to override logic
        public virtual int Width { get; set; }
        public virtual int Height { get; set; }

        public Rectangle() { }
        public Rectangle(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public override string ToString()
        {
            return $"{nameof(Width)}: {Width}, {nameof(Height)}: {Height}";
        }
    }

    // The Child Class (Violates LSP)
    public class Square : Rectangle
    {
        public Square(int side)
        {
            Width = Height = side;
        }

        // We enforce that Width and Height must always match.
        // This 'side effect' violates the behavior expected of a Rectangle.
        public override int Width
        {
            set { base.Width = base.Height = value; }
        }

        public override int Height
        {
            set { base.Width = base.Height = value; }
        }
    }

    //    The Violation: The Square class links the two properties together.When UseIt sets the Height to 10, the Square secretly sets the Width to 10 as well.

    //The Failure: The program calculates an area of 100 instead of 50. The subclass (Square) was not substitutable for the base class (Rectangle) because it altered the expected behavior.
    public class LiskovSubstitutionPrinciple
    {
        // This function assumes 'r' acts like a standard Rectangle.
        // It assumes changing Height does NOT affect Width.
        public static void UseIt(Rectangle r)
        {
            int width = r.Width;
            r.Height = 10;

            Console.WriteLine($"[Logic Check] Setup: Width={width}, set Height=10.");
            Console.WriteLine($"[Expected] Area should be {width} * 10 = {width * 10}");
            Console.WriteLine($"[Actual]   Area is {r.Width} * {r.Height} = {r.Width * r.Height}");
        }

        public static void LiskovSubstitutionPrincipleResult(string[] args)
        {
            Console.WriteLine("--- Testing Rectangle (Base) ---");
            Rectangle rc = new Rectangle(2, 3);
            UseIt(rc);

            Console.WriteLine("\n--- Testing Square (Child) ---");
            Rectangle sq = new Square(5);
            UseIt(sq);
        }
    }
}


namespace DesignPatterns.Solid.LSP.BadExample
{
    namespace SolidPrinciples.LSP_Fixed
    {
        // 1. The Common Abstraction
        // We define what is common to both: they are shapes, and they have an Area.
        // We do NOT include mutable Width/Height here because the rules for setting them differ.
        public interface IShape
        {
            int Area { get; }
        }

        // 2. Rectangle (Implements IShape)
        public class Rectangle : IShape
        {
            public int Width { get; set; }
            public int Height { get; set; }

            public Rectangle(int width, int height)
            {
                Width = width;
                Height = height;
            }

            public int Area => Width * Height;
        }

        // 3. Square (Implements IShape, NOT Rectangle)
        // Square is now its own standalone entity. It has its own rules.
        public class Square : IShape
        {
            public int Side { get; set; }

            public Square(int side)
            {
                Side = side;
            }

            // We calculate Area based on Side
            public int Area => Side * Side;
        }

        public class Program
        {
            // 4. This method requires a Rectangle specifically.
            // It relies on the behavior that Width and Height are independent.
            public static void IncreaseHeight(Rectangle r)
            {
                r.Height = 10;
                // Because 'r' is guaranteed to be a Rectangle, changing Height
                // will NEVER affect Width. The logic is safe.
            }

            public static void Main(string[] args)
            {
                // Scenario A: Working with a Rectangle
                Rectangle rc = new Rectangle(2, 3);
                IncreaseHeight(rc);
                Console.WriteLine($"Rectangle Area: {rc.Area}"); // 2 * 10 = 20. Correct.

                // Scenario B: Working with a Square
                Square sq = new Square(5);

                // UNCOMMENTING THE LINE BELOW WOULD CAUSE A COMPILE ERROR:
                // IncreaseHeight(sq); 

                // ^ This is the fix! The compiler prevents you from passing a Square 
                // into a method that expects Rectangle behavior. You are saved from the bug.

                Console.WriteLine($"Square Area: {sq.Area}"); // 25. Correct.
            }
        }
    }
}