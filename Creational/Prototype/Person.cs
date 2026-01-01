using Centralized;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Xml.Serialization;

namespace Centralized
{
    public class Address
    {
        public readonly string StreetName;
        public int HouseNumber;
        public Address(string streetName, int houseNumber)
        {
            StreetName = streetName;
            HouseNumber = houseNumber;
        }

        public Address(Address other)
        {
            StreetName = other.StreetName;
            HouseNumber = other.HouseNumber;
        }
    }
}

namespace ShallowCopy
{
    public class Person
    {
        public string Name;
        public readonly Address Address;
        public Person(string name, Address address)
        {
            Name = name;
            Address = address;
        }

        public override string ToString()
        {
            return $"{Name} lives at {Address.StreetName} {Address.HouseNumber}";
        }
    }


    /// <summary>
    /// Shallow Copy
    /// </summary>
    /// 

    public static class Prototype01Result
    {
        static void Result01()
        {
            var john = new Person
                ("John Smith", new Address("London Road", 123));

            Console.WriteLine(john);

            var jane = john;
            jane.Name = "Jane Smith";
            jane.Address.HouseNumber = 321;

            Console.WriteLine(jane);

        }
    }
}

namespace ICloneable
{
    public class Person : System.ICloneable
    {
        public string Name;
        public readonly Address Address;

        public object Clone()
        {
            return (Person) MemberwiseClone();
        }

        public Person(string name, Address address)
        {
            Name = name;
            Address = address;
        }

        public override string ToString()
        {
            return $"{Name} lives at {Address.StreetName} {Address.HouseNumber}";
        }
    }

    public static class Prototype02Result
    {
        public static void Result()
        {
            var john = new Person("John Smith", new Address("London Road", 123));

            Console.WriteLine(john);

            var jane = (Person)john.Clone();
            jane.Name = "Jane Smith";
            jane.Address.HouseNumber = 321;

            Console.WriteLine(jane);
        }
    }
}

namespace CopyConstruction
{
    public class Person
    {
        public string Name;
        public readonly Address Address;

        public Person(string name, Address address)
        {
            Name = name;
            Address = address;
        }

        public Person(Person other)
        {
            Name = other.Name;
            Address = new Address(other.Address);
        }

        public override string ToString()
        {
            return $"{Name} lives at {Address.StreetName} {Address.HouseNumber}";
        }
    }

    public static class Prototype03Result
    {
        static void Result()
        {
            var john = new Person(
                 "John Smith", new Address("London Road", 123));

            var jane = new Person(john);
            jane.Name = "Jane Smith";
            jane.Address.HouseNumber = 321;

            Console.WriteLine(john);
            Console.WriteLine(jane);
        }
    }

    public static class Prototype04Result
    {
        static void Result()
        {
            List<int> items = new() { 1, 2, 3 };
            List<int> replica = new(items); // copy constructor
        }
    }
}

namespace DeepCopying
{

    public static class Extension
    {
        public static T DeepCopy<T>(this IDeepCopyable<T> item)
      where T : new()
        {
            return item.DeepCopy();
        }
    }

    public interface IDeepCopyable<T> where T : new()
    {
        void CopyTo(T target);

        public T DeepCopy()
        {
            T t = new T();
            CopyTo(t);
            return t;
        }
    }

    public class Address : IDeepCopyable<Address>
    {
        public string StreetName { get; set; }
        public int HouseNumber { get; set; }

        public Address()
        {
        }

        public Address(string streetName, int houseNumber)
        {
            StreetName = streetName;
            HouseNumber = houseNumber;
        }

        public Address(Address other)
        {
            StreetName = other.StreetName;
            HouseNumber = other.HouseNumber;
        }

        public void CopyTo(Address target)
        {
            target.StreetName = StreetName;
            target.HouseNumber = HouseNumber;
        }

        //public Address DeepCopy()
        //{
        //    return new Address(this);
        //}

        public override string ToString()
        {
            return $"{nameof(StreetName)}: {StreetName}, {nameof(HouseNumber)}: {HouseNumber}";
        }
    }

    public class Person : IDeepCopyable<Person>
    {
        public string[] Names { get; set; }
        public Address Address { get; set; }

        public Person()
        {
        }

        public Person(string[] names, Address address)
        {
            Names = (string[])names.Clone();
            Address = address.DeepCopy();
        }

        // Copy constructor
        public Person(Person other)
        {
            Names = (string[])other.Names.Clone();
            Address = other.Address.DeepCopy();
        }

        //public virtual Person DeepCopy()
        //{
        //    return new Person(this);
        //}

        public void CopyTo(Person target)
        {
            target.Names = (string[])Names.Clone();
            target.Address = Address.DeepCopy();
        }

        public override string ToString()
        {
            return $"{nameof(Names)}: {string.Join(",", Names)}, {nameof(Address)}: {Address}";
        }
    }

    public class Employee : Person, IDeepCopyable<Employee>
    {
        public int Salary { get; set; }

        public void CopyTo(Employee target)
        {
            base.CopyTo(target);
            target.Salary = Salary;
        }

        //public override Person DeepCopy()
        //{
        //    return new Employee(this);
        //}
        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(Salary)}: {Salary}";
        }
    }

    public class DeepCopyResult
    {
        public void Result()
        {
            //var john = new Employee(
            //    new[] { "John", "Smith" },
            //    new Address("London Road", 123),
            //    10000);

            //Person copy = john.DeepCopy();
            //Console.WriteLine(copy is Employee); // false
        }

        public void Result02()
        {
            var john = new Employee();
            john.Names = new[] { "John", "Doe" };
            john.Address = new Address { HouseNumber = 123, StreetName = "London Road" };
            john.Salary = 321000;
            var copy = john.DeepCopy<Employee>();

            copy.Names[1] = "Smith";
            copy.Address.HouseNumber++;
            copy.Salary = 123000;

            Console.WriteLine(john);
            Console.WriteLine(copy);


            copy.Address = john.Address.DeepCopy();
            Console.WriteLine(copy);
        }

        public void Result03()
        {

            // Dictionary Deep Copy.

            var people = new Dictionary<string, Address>
            {
                ["John"] = new("London Road", 38),
                ["Jane"] = new("Jane Street", 72)
            };

            var peopleCopies = people.ToDictionary(
                x => x.Key,
                x => x.Value.DeepCopy());
        }

        public void Result04()
        {
            // Array or HashSet using LINQ deep copy
            var addresses = new HashSet<Address>
            {
                new("London Road", 38),
                new("Jane Street",72)
            };

            var replicas = new HashSet<Address>(
                addresses.Select(a => a.DeepCopy()));
        }

    }
}

namespace ArrayDeepCopy
{
    public interface IDeepCopyable<T> where T : new()
    {
        void CopyTo(T target);

        public T DeepCopy()
        {
            T t = new T();
            CopyTo(t);
            return t;
        }
    }

    public static class Extension
    {
        public static T DeepCopy<T>(this IDeepCopyable<T> item)
      where T : new()
        {
            return item.DeepCopy();
        }
    }

    public class Address : IDeepCopyable<Address>
    {
        public string StreetName { get; set; }
        public int HouseNumber { get; set; }

        public Address() { }
      
        public Address(string streetName, int houseNumber)
        {
            StreetName = streetName;
            HouseNumber = houseNumber;
        }

        public Address(Address other)
        {
            StreetName = other.StreetName;
            HouseNumber = other.HouseNumber;
        }

        public void CopyTo(Address target)
        {
            target.StreetName = StreetName;
            target.HouseNumber = HouseNumber;
        }

        public override string ToString()
        {
            return $"{nameof(StreetName)}: {StreetName}, {nameof(HouseNumber)}: {HouseNumber}";
        }
    }

    public class Person
    {
        public string[] Names;
        public Address[] Addresses;

        public Person(string[] names, Address[] addresses)
        {
            Names = (string[])names.Clone();
            Addresses = addresses;
        }

        public Person() { }

        public Person DeepCopy()
        {
            var copy = new Person();
            copy.Names = (string[])Names.Clone();
            copy.Addresses = Array.ConvertAll(Addresses, a => a.DeepCopy());

            return copy;
        }
    }

    public class ArrayDeepCopy
    {
        public void Result()
        {
            var p1 = new Person(
                             new[] { "Felipe", "Amanda" },
                             new[]
                             {
                                new Address { HouseNumber = 123, StreetName = "London Road" },
                                new Address { HouseNumber = 327, StreetName = "Cafezeiro" }
                             });

            var p2 = p1.DeepCopy();

            PrintNames(p1, p2);
            PrintAddresses(p1, p2);

            p2.Addresses = new[]
            {
                new Address { HouseNumber = 223, StreetName = "London Road 2" },
                new Address { HouseNumber = 427, StreetName = "Cafezeiro 2" }
            };

            PrintNames(p1, p2);
            PrintAddresses(p1, p2);

            static void PrintNames(Person p1, Person p2)
            {
                for (var i = 0; i < p1.Names.Length; i++)
                {
                    Console.WriteLine(p1.Names[i]);
                    Console.WriteLine(p2.Names[i]);
                }
            }

            static void PrintAddresses(Person p1, Person p2)
            {
                for (var i = 0; i < p1.Addresses.Length; i++)
                {
                    Console.WriteLine(p1.Addresses[i]);
                    Console.WriteLine(p2.Addresses[i]);
                }
            }
        }
    }

}

namespace MemberwiseDeepCopy
{
    public interface IDeepCopyable<T>
    {
        T DeepCopy();
    }

    // Abstract base class for trivial deep copy using MemberwiseClone
    public abstract class TriviallyCopyable<T> : IDeepCopyable<T>
        where T : class
    {
        public T DeepCopy()
        {
            return (T)MemberwiseClone();
        }
    }

    // Example: Person class using TriviallyCopyable
    public class Person : TriviallyCopyable<Person>
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public Person() { }

        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public override string ToString()
        {
            return $"{Name}, Age: {Age}";
        }
    }

    // Example: record type PersonRecord
    public record PersonRecord(string Name, int Age)
    {
        public PersonRecord DeepCopy()
        {
            return this with { };
            // or: return new PersonRecord(this);
        }
    }

    // Example usage and result
    public static class MemberwiseCloneDemo
    {
        public static void Result()
        {
            // Using Person class
            var john = new Person("John", 30);
            var johnCopy = john.DeepCopy();
            johnCopy.Name = "Jane";
            johnCopy.Age = 25;
            Console.WriteLine(john);     // John, Age: 30
            Console.WriteLine(johnCopy); // Jane, Age: 25

            // Using PersonRecord
            var recordJohn = new PersonRecord("John", 30);
            var recordCopy = recordJohn.DeepCopy();
            recordCopy = recordCopy with { Name = "Jane", Age = 25 };
            Console.WriteLine(recordJohn); // PersonRecord { Name = John, Age = 30 }
            Console.WriteLine(recordCopy); // PersonRecord { Name = Jane, Age = 25 }
        }
    }
}

namespace Serialization
{
    public static class MappingExtensions
    {
        public static T? DeepCopy<T>(this T self)
        {
            // Use the fully qualified object.ReferenceEquals to avoid context errors
            if (object.ReferenceEquals(self, null))
                return default;

            // Serialize the object to JSON and back to create a new instance
            // This is the .NET 8 standard replacement for BinaryFormatter
            var json = JsonSerializer.Serialize(self);
            return JsonSerializer.Deserialize<T>(json);
        }

        public static T DeepCopyXml<T>(this T self)
        {
            using var ms = new MemoryStream();
            XmlSerializer s = new XmlSerializer(typeof(T));
            s.Serialize(ms, self);
            ms.Position = 0;
            return (T) s.Deserialize(ms);
        }
    }


    public class Foo
    {
        public uint Stuff { get; set; }
        public Bar Whatever { get; set; }

        public override string ToString()
        {
            return $"{nameof(Stuff)}: {Stuff}, {nameof(Whatever)}: {Whatever}";
        }
    }

    public class Bar
    {
        public string Baz { get; set; }

        public override string ToString()
        {
            return $"{nameof(Baz)}: {Baz}";
        }

    }

    public class SerializationResult
    {
        public void Result()
        {
            var foo = new Foo { Stuff = 42, Whatever = new Bar { Baz = "abc" } };
            var foo2 = foo.DeepCopy();
            var foo3 = foo;

            Console.WriteLine(foo);

            foo2.Whatever.Baz = "xyz";

            Console.WriteLine(foo);
            Console.WriteLine(foo2);

            Console.WriteLine(Object.ReferenceEquals(foo, foo2)); // False.
            Console.WriteLine(Object.ReferenceEquals(foo, foo3)); // True.
        }

        public void Result02()
        {

            Foo foo = new() { Stuff = 42, Whatever = new Bar { Baz = "abc" } };

            //Foo foo2 = foo.DeepCopy(); // crashes without [Serializable]
            Foo foo2 = foo.DeepCopyXml();

            foo2.Whatever.Baz = "xyz";
            Console.WriteLine(foo);
            Console.WriteLine(foo2);
        }
    }

}

namespace PrototypeFactory
{
    using DeepCopying;

    public class EmployeeFactory
    {
        private static Person main =
          new Person(new[] { "" }, new Address("East Dr", 123));
        private static Person aux =
          new Person(new[] {""} , new Address("East Dr Texas", 123));
        public static Person NewMainOfficeEmployee(string[] name, int houseNumber) =>
          NewEmployee(main, name, houseNumber);
        public static Person NewAuxOfficeEmployee(string[] name, int houseNumber) =>
          NewEmployee(aux, name, houseNumber);

        private static Person NewEmployee(Person proto, string[] name, int houseNumber)
        {
            var copy = proto.DeepCopy();
            copy.Names = name;
            copy.Address.HouseNumber = houseNumber;
            return copy;
        }
    }

    public class EmployeeFactoryResult
    {
        public void Result() {
            var john = EmployeeFactory.NewMainOfficeEmployee(new[] { "John doe" }, 100);
            var jane = EmployeeFactory.NewAuxOfficeEmployee(new[] { "Jane Smith" }, 123);

            Console.WriteLine(john);
            Console.WriteLine(jane);
        }
    }
}