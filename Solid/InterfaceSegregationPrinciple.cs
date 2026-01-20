using System.Reflection.Metadata;

// Interface Segregation Principle (ISP)

namespace Solid.ISP.BadDesign
{
    // BAD DESIGN: One interface to rule them all
    public interface IMachine
    {
        void Print(Document d);
        void Scan(Document d);
        void Fax(Document d);
    }

    // The user is forced to implement methods they don't need
    public class OldFashionedPrinter : IMachine
    {
        public void Print(Document d)
        {
            // Actual printing logic 
            Console.WriteLine($"Printing {d.Name}");
        }

        public void Scan(Document d)
        {
            // VIOLATION: We have to fake it or throw an error.
            throw new NotImplementedException();
        }

        public void Fax(Document d)
        {
            throw new NotImplementedException();
        }
    }
}

namespace Solid.ISP.GoodDesign
{
    /// <summary>
    /// The Interface Segregation Principle (ISP) is a SOLID principle in software design stating that clients shouldn't be forced to depend on methods they don't use, meaning large, general interfaces should be split into smaller, role-specific ones, preventing "fat interfaces" and promoting cleaner, more decoupled code where classes only implement what's necessary for their specific function
    /// 
    //"No client should be forced to depend on methods it   does not use".
    //Instead of one big interface (e.g., IWorker with work(), eat(), sleep()), create smaller, focused interfaces(e.g., IWorkable, IEatable, ISleepable).

    /// </summary>
    public class Document { public string Name; }

    // 1. Segregated Interfaces (Small & Specific)
    public interface IPrinter
    {
        void Print(Document d);
    }

    public interface IScanner
    {
        void Scan(Document d);
    }

    // 2. The Old Printer (Only implements what it needs)
    public class OldFashionedPrinter : IPrinter
    {
        public void Print(Document d)
        {
            Console.WriteLine($"[Old Printer] Printing {d.Name}...");
        }
    }

    // 3. The Modern Machine (Implements multiple interfaces)
    // We can also create a combined interface if needed
    public interface IMultiFunctionDevice : IPrinter, IScanner { }

    public class Photocopier : IMultiFunctionDevice
    {
        public void Print(Document d)
        {
            Console.WriteLine($"[Photocopier] Printing {d.Name}...");
        }

        public void Scan(Document d)
        {
            Console.WriteLine($"[Photocopier] Scanning {d.Name}...");
        }
    }

    public class ISP
    {
        public static void Result01()
        {
            var doc = new Document { Name = "MyReport.pdf" };

            // Scenario A: Using a simple printer
            // It ONLY has the Print method. You physically cannot call Scan() on it.
            IPrinter simple = new OldFashionedPrinter();
            simple.Print(doc);

            // Scenario B: Using a complex machine
            IMultiFunctionDevice advanced = new Photocopier();
            advanced.Print(doc);
            advanced.Scan(doc);
        }
    }
}