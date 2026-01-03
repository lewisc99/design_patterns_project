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