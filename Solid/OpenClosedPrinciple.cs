using System;
using System.Drawing;

namespace DesignPatterns.Solid.OpenClosedBadExample
{
    // BAD DESIGN: You have to modify this class every time a new filter is needed
    /// <summary>
    /// 1. The Problem (Violating OCP)
    //Initially, your boss asks you to filter products by Color.You write a ProductFilter class. Later, they ask to filter by Size.You have to go back into the class and add a new method.Then, they ask to filter by both.You have to modify the class again.

    //This violates OCP because the class is not closed for modification.Every time a new requirement comes in, you have to change the existing, tested code.
    /// </summary>
    /// 

    public enum Color { Red, Green, Blue }
    public enum Size { Small, Medium, Large, Huge }
    public record Product(string Name, Color Color, Size Size);

    public class ProductFilter
    {
        public IEnumerable<Product> FilterByColor(IEnumerable<Product> products, Color color)
        {
            foreach (var p in products)
                if (p.Color == color) yield return p;
        }

        // Modification 1: Boss asked for Size filtering
        public IEnumerable<Product> FilterBySize(IEnumerable<Product> products, Size size)
        {
            foreach (var p in products)
                if (p.Size == size) yield return p;
        }

        // Modification 2: Boss asked for Both
        public IEnumerable<Product> FilterBySizeAndColor(IEnumerable<Product> products, Size size, Color color)
        {
            foreach (var p in products)
                if (p.Size == size && p.Color == color) yield return p;
        }
    }
}


namespace DesignPatterns.Solid.OpenClosed
{
    // Defines a rule for filtering (e.g., "Must be Green")
    /// <summary>
    /// To follow OCP, we separate the "filtering mechanism" from the "filtering criteria." We use the Specification Pattern. We define what we want (Specification) and a way to process it (Filter) using interfaces.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// 

    public enum Color { Red, Green, Blue }
    public enum Size { Small, Medium, Large, Huge }
    public record Product(string Name, Color Color, Size Size);

    public interface ISpecification<T>
    {
        bool IsSatisfied(T item);
    }

    // Defines a component that can filter items based on a rule
    public interface IFilter<T>
    {
        IEnumerable<T> Filter(IEnumerable<T> items, ISpecification<T> spec);
    }

    /// <summary>
    /// Step B: Implement the Filter (The Fixed Logic) This class creates the filtering logic once. You never need to touch this class again, even if you add 100 new ways to filter products.
    /// </summary>
    public class BetterFilter : IFilter<Product>
    {
        public IEnumerable<Product> Filter(IEnumerable<Product> items, ISpecification<Product> spec)
        {
            foreach (var i in items)
                if (spec.IsSatisfied(i))
                    yield return i;
        }
    }

    // Requirement 1: Filter by Color
    /// <summary>
    /// Step C: Extend with New Features (The "Open" Part) Now, if the boss asks for "Color" filtering, you just create a small class. If they ask for "Size", you create another. You are extending the system without modifying the core filter.
    /// </summary>
    public class ColorSpecification : ISpecification<Product>
    {
        private Color color;
        public ColorSpecification(Color color) => this.color = color;
        public bool IsSatisfied(Product p) => p.Color == color;
    }

    // Requirement 2: Filter by Size (Added later, no changes to BetterFilter!)
    public class SizeSpecification : ISpecification<Product>
    {
        private Size size;
        public SizeSpecification(Size size) => this.size = size;
        public bool IsSatisfied(Product p) => p.Size == size;
    }

    public class OpenClosedResult
    {
        public void Result()
        {
            // 1. Create some dummy products
            var apple = new Product("Apple", Color.Green, Size.Small);
            var tree = new Product("Tree", Color.Green, Size.Large);
            var house = new Product("House", Color.Blue, Size.Large);

            Product[] products = { apple, tree, house };

            // 2. Initialize the Filter
            var filter = new BetterFilter();

            // 3. Define the specification: We want "Green" items
            var greenSpec = new ColorSpecification(Color.Green);

            // 4. Run the filter and print results
            Console.WriteLine("Green products (new):");
            foreach (var p in filter.Filter(products, greenSpec))
            {
                Console.WriteLine($" - {p.Name} is Green");
            }

            //Green products(new):
            // -Apple is Green
            // - Tree is Green
        }
    }
}