using Centralized;

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
}


namespace DeepCopying
{
    public interface IDeepCopyable<out T>
    {
        T DeepCopy();
    }

    public class Address : IDeepCopyable<Address>
    {
        public string StreetName { get; }
        public int HouseNumber { get; }

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

        public Address DeepCopy()
        {
            return new Address(this);
        }
    }

    public class Person : IDeepCopyable<Person>
    {
        public string[] Names { get; }
        public Address Address { get; }

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

        public virtual Person DeepCopy()
        {
            return new Person(this);
        }
    }

    public class Employee : Person
    {
        public int Salary { get; }

        public Employee(string[] names, Address address, int salary)
            : base(names, address)
        {
            Salary = salary;
        }
    }

    public class DeepCopyResult
    {
        public void Result()
        {
            var john = new Employee(
                new[] { "John", "Smith" },
                new Address("London Road", 123),
                10000);

            Person copy = john.DeepCopy();
            Console.WriteLine(copy is Employee); // false
        }
    }
}