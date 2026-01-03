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