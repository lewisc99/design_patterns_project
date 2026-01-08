using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

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

namespace PerThreadSingletonClass
{
    ///
    ///We’ve talked about thread safety in relation to the construction of the singleton, but what about thread safety with respect to a singleton’s own operations? It might be the case that, instead of one singleton shared between all threads in an application, you need one singleton to exist per thread.
    ///

    public sealed class PerThreadSingleton
    {
        private static ThreadLocal<PerThreadSingleton> threadInstance
             = new(() => new PerThreadSingleton());
        public int Id;

        private PerThreadSingleton()
        {
            Id = Thread.CurrentThread.ManagedThreadId;
        }

        public static PerThreadSingleton Instance => threadInstance.Value;
    }

    public class PerThreadSingletonResult
    {
        public void Result()
        {
            var t1 = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("t1: " + PerThreadSingleton.Instance.Id);
            });

            var t2 = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("t2: " + PerThreadSingleton.Instance.Id);
                Console.WriteLine("t2 again: " + PerThreadSingleton.Instance.Id);
            });

            Task.WaitAll(t1, t2);
        }
    }
}


namespace AmbientContext
{
    public sealed class BuildingContext : IDisposable
    {
        private BuildingContext() { }
        public int Height { get; private set; }

        private static readonly Stack<BuildingContext> stack = new();
        static BuildingContext() { stack.Push(new BuildingContext()); }
        private BuildingContext(BuildingContext other)
        {
            Height = other.Height;
        }

        public static BuildingContext Current => stack.Peek();

        public static IDisposable WithHeight(int height)
        {
            var copy = new BuildingContext(Current);
            copy.Height = height;

            stack.Push(copy);
            return copy;
        }

        public void Dispose()
        {
            if (stack.Count > 1) stack.Pop();
        }
    }

    public class Wall
    {
        public Point Start;
        public Point End;
        public int Height;
        public Wall(Point start, Point end, int? height = null)
        {
            Start = start;
            End = end;
            Height = height ?? BuildingContext.Current.Height;
        }
    }
    public class Building
    {
        public List<Wall> Walls = new();
    }


    public class BuildingContextResult
    {
        public void Result()
        {

            var buildings = new Building();

            using (BuildingContext.WithHeight(2000))
            {
                buildings.Walls.Add(new Wall(
                    new Point(0, 1000), new Point(1000, 1000)));
                using (BuildingContext.WithHeight(1000))
                {
                    buildings.Walls.Add(new Wall(
                        new Point(1000, 2000), new Point(2000, 3000)));

                    buildings.Walls.Add(new Wall(
                        new Point(0, 1000), new Point(1000, 1000)));
                }

                foreach (var wall in buildings.Walls)
                {
                    Console.WriteLine(wall.Height);
                }
            }
        }
    }
}

namespace SingletonsAndInversionOfControl
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    //Instead of writing static code that is hard to test, you rely on the container(like.NET's built-in Dependency Injection) to guarantee that only one instance is ever created.


    //Real-World Scenario: A Shared Configuration Cache
    //Imagine an application that needs to load heavy configuration settings (like feature flags or database strings) from a remote server.You only want to load this once and share it everywhere.

    public interface IConfigurationCache
    {
        string GetValue(string Key);
        void SetValue(string Key, string Value);
    }

    public class ConfigurationCache : IConfigurationCache
    {
        private readonly Dictionary<string, string> _cache = new();

        public ConfigurationCache()
        {
            // Simulate loading data efficiently (e.g., from a file or DB)
            Console.WriteLine("Initializing Cache (This should happen only once!)");

            _cache["AppName"] = "MyRealWorldApp";
            _cache["Version"] = "1.0.0";
        }

        public string GetValue(string key) => _cache.ContainsKey(key) ? _cache[key] : null;

        public void SetValue(String key, string value) => _cache[key] = value;
    }

    public class ReportingService
    {
        private readonly IConfigurationCache _cache;

        public ReportingService(IConfigurationCache cache)
        {
            _cache = cache;
        }

        public void PrintReport()
        {
            Console.WriteLine($"Report for: {_cache.GetValue("AppName")}");
        }
    }

    public class AdminService
    {
        private readonly IConfigurationCache _cache;

        public AdminService(IConfigurationCache cache)
        {
            _cache = cache;
        }

        public void UpdateName()
        {
            // This change will be visible to ReportingService too!
            _cache.SetValue("AppName", "New Enterprise App");
        }
    }

    public class SingletonsAndInversionOfControl
    {
        public void Result()
        {
            // --- 1. SETUP ---
            // The value is just "args" directly in CreateApplicationBuilder just to mimic here because is a class outside Program.cs
            var args = new string[] { };

            var builder = Host.CreateApplicationBuilder(args);

            // Register the Singleton (The star of the show)
            // The container will create this ONCE and keep it alive forever.
            builder.Services.AddSingleton<IConfigurationCache, ConfigurationCache>();

            // Register the consumers
            // We usually register these as 'Transient' (created every time they are asked for),
            // but because they depend on the Singleton Cache, they will all share the same data.
            builder.Services.AddTransient<ReportingService>();
            builder.Services.AddTransient<AdminService>();

            using IHost host = builder.Build();

            // --- 2. EXECUTION ---
            // We create a scope to simulate the application running and resolving services
            using (var scope = host.Services.CreateScope())
            {
                var provider = scope.ServiceProvider;

                // Resolve the services. The container looks at the constructor, sees it needs 
                // IConfigurationCache, and injects the SINGLETON instance we registered.
                var admin = provider.GetRequiredService<AdminService>();
                var reporter = provider.GetRequiredService<ReportingService>();

                Console.WriteLine("--- Initial State ---");
                reporter.PrintReport(); // Should show default "MyRealWorldApp"

                Console.WriteLine("\n--- Admin Updates the Cache ---");
                admin.UpdateName(); // Changes value in the Singleton

                Console.WriteLine("\n--- Reporting Service sees the change? ---");
                reporter.PrintReport(); // Should show "New Enterprise App"
            }

            // To run this, ensure the console window stays open
            Console.ReadLine();
        }

        public class MockConfigurationCache : IConfigurationCache
        {
            private Dictionary<string, string> _fakeStore = new();

            public string GetValue(string key) => _fakeStore.ContainsKey(key) ? _fakeStore[key] : "DEFAULT_TEST_VAL";
            public void SetValue(string key, string value) => _fakeStore[key] = value;
        }

        public void Result02()
        {
            Console.WriteLine("--- RUNNING MANUAL TEST ---");

            // 1. ARRANGE
            // Create the fake dependency manually
            var fakeCache = new MockConfigurationCache();
            // Pre-load it with test data (controlled environment)
            fakeCache.SetValue("AppName", "TEST_ENV_01");

            // 2. ACT
            // We manually inject the fake cache. 
            // The ReportingService doesn't know (or care) it's a fake!
            var serviceToTest = new ReportingService(fakeCache);

            // 3. ASSERT (Visual Verification)
            Console.WriteLine("Expected: Report for: TEST_ENV_01");
            Console.Write("Actual:   ");
            serviceToTest.PrintReport();

            Console.WriteLine("---------------------------");
        }
    }
}