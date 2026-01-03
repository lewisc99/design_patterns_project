using SingletonByConvention;
using System;

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