namespace SingletonByConvention
{
    public class Database
    {

        //Please do not create more than on instance
        public Database() { }
    }

    public static class Globals
    {
        public static Database Database = new Database();
    }

    public class SingletonByConvention
    {
        public void Result()
        {
            Database database = Globals.Database;
        }
    }
}

namespace ClassicImplementation
{
    public class Database
    {
        private static int instanceCount = 0;

        public Database()
        {
            if (++instanceCount > 1)
                throw new InvalidOperationException("Cannot make >1 database!");
        }
    }

    public class ClassicImplementationResult
    {
        public void Result()
        {
            Database database = new Database();


            // throws an exception
            Database database1 = new Database();
        }
    }
}

namespace HidingConstructor
{
    public class Database
    {
        private Database() { }
        public static Database Instance { get; } = new();
    }

    public class HidingConstructorResult
    {
        public void Result()
        {
            Database database = Database.Instance;

            // show an error
            //Database database1 = new Database();
        }
    }
}

namespace LazyLoading
{
    /// <summary>
    /// The goal of this code is to create a "Database" object that is created only once. No matter how many times you ask for the database in your application, you will always get the exact same object. This is often done to manage shared resources like database connections.
    /// </summary>
    /// 
    public class MyDatabase
    {
        private MyDatabase()
        {
            Console.WriteLine("Initializing database");
        }

        private static Lazy<MyDatabase> instance = new(() => new MyDatabase());
        public static MyDatabase Instance => instance.Value;
    }

    public class LazyLoadingResult
    {
        public void Result()
        {
            // The database is NOT created yet.

            var db1 = MyDatabase.Instance; // Prints: "Initializing database"
            var db2 = MyDatabase.Instance; // Does NOT print anything. Returns the same object.

            bool areSame = (db1 == db2);   // True. They are the same instance.
        }
    }
}

namespace LazyReusableBaseClass
{

    /// <summary>
    /// public class MyDatabase : Singleton<MyDatabase>: This line says, "I am MyDatabase, and I want to use the Singleton blueprint. Use Me (MyDatabase) as the placeholder T."

    /// By inheriting, MyDatabase should automatically get the Instance property from the parent.
    /// </summary>
    public class MyDatabase : Singleton<MyDatabase>
    {
        private MyDatabase()
        {
            Console.WriteLine("Initializing database");
        }

        //private static Lazy<MyDatabase> instance = new(() => new MyDatabase());
        //public static MyDatabase Instance => instance.Value;
    }


    public abstract class Singleton<T> where T : Singleton<T>
    {
        private static readonly Lazy<T> lazy =
            new Lazy<T>(() => Activator.CreateInstance(typeof(T), true) as T);

        public static T Instance => lazy.Value;
    }

    public class LazyLoadingResult
    {
        public void Result()
        {
            // The database is NOT created yet.

            var db1 = MyDatabase.Instance; // Prints: "Initializing database"
            var db2 = MyDatabase.Instance; // Does NOT print anything. Returns the same object.

            bool areSame = (db1 == db2);   // True. They are the same instance.
        }
    }
}

namespace TroubleWithSngleton
{
    public interface IDatabase
    {
        int GetPopulation(string name);
    }

    public class SingletonDatabase : IDatabase
    {
        private Dictionary<string, int> capitals;

        private SingletonDatabase()
        {
            Console.WriteLine("Initializing database");
            var lines = File.ReadAllLines(
                Path.Combine(
                    Path.GetDirectoryName(typeof(IDatabase).Assembly.Location) ?? string.Empty,
                    "Creational/Singleton/capitals.txt")
                );

            capitals = new Dictionary<string, int>();
            for (int i = 0; i < lines.Length; i += 2)
            {
                var city = lines[i].Trim();
                var population = int.Parse(lines[i + 1]);
                capitals[city] = population;
            }
        }

        public int GetPopulation(string name)
        {
            return capitals[name]; 
        }

        private static Lazy<SingletonDatabase> instance =
            new Lazy<SingletonDatabase>(() =>
            {
                return new SingletonDatabase();
            });

        public static IDatabase Instance => instance.Value;
    }


    /// <summary>
    /// The trouble is that SingletonRecordFinder is now firmly dependent on SingletonDatabase. This presents an issue for testing – if we want to check that SingletonRecordFinder works correctly, we need to use data from the actual database, that is:
    /// </summary>
    public class SingletonRecordFinder
    {
        public int TotalPopulation(IEnumerable<string> names)
        {
            int result = 0;
            foreach (var name in names)
                result += SingletonDatabase.Instance.GetPopulation(name);
            return result;
        }
    }

    public class ConfigurableRecordFinder
    {
        private IDatabase database;

        public ConfigurableRecordFinder(IDatabase database)
        {
            this.database = database;
        }

        public int GetTotalPopulation(IEnumerable<string> names)
        {
            int result = 0;
            foreach (var name in names)
                result += database.GetPopulation(name);
            return result;
        }
    }

    public class DummyDatabase : IDatabase
    {
        public int GetPopulation(string name)
        {
            return new Dictionary<string, int>
            {
                ["alpha"] = 1,
                ["beta"] = 2,
                ["gamma"] = 3
            }[name];
        }
    }
    public class SingletonRecordFinderResult
    {
        /// <summary>
        /// This is a terrible unit test. It tries to read a live database (something that you typically don’t want to do too often), but it’s also very fragile, because it depends on the concrete values in the database. What if the population of Seoul changes (as a result of North Korea opening its borders, perhaps)? Then the test will break. But of course, many people run tests on continuous integration systems that are isolated from live databases, so that fact makes the approach even more dubious.
        /// 
        /// This test is also bad for ideological reasons. Remember, we want a unit test where the unit we’re testing is the SingletonRecordFinder. However, the test we wrote is not a unit test but an integration test because the record finder uses SingletonDatabase, so in effect we’re testing both systems at the same time. Nothing wrong with that if an integration test is what you wanted, but we would really prefer to test the record finder in isolation.
        /// </summary>

        public void Result()
        {
            // testing on a live database
            var rf = new SingletonRecordFinder();
            var names = new[] { "Seoul", "Mexico City" };
            int tp = rf.TotalPopulation(names);

            int expected = 17500000 + 17400000;
            if (tp == expected)
            {
                Console.WriteLine($"Test passed: {tp} == {expected}");
            }
            else
            {
                Console.WriteLine($"Test failed: {tp} != {expected}");
            }
        }

        public void Result02()
        {
            var db = new DummyDatabase();
            var rf = new ConfigurableRecordFinder(db);

            int expected = 4;

            var population = rf.GetTotalPopulation(new[] { "alpha", "gamma" });

            if (population == expected)
            {
                Console.WriteLine($"Test passed: {population} == {expected}");
            }
            else
            {
                Console.WriteLine($"Test failed: {population} != {expected}");
            }

        }
    }
}