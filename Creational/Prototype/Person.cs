using Centralized;
using System.Runtime.CompilerServices;

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