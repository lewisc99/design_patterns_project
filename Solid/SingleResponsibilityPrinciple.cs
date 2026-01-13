namespace SingleResponsibility
{
    public class JournalWrongClass
    {
        /// <summary>
        ///  This violates SRP because the class has two reasons to change. If you want to change how entries are formatted OR if you want to change how files are saved (e.g., to a database instead of a file), you have to modify the same class.
        /// </summary>

        private readonly List<string> entries = new();
        private static int count = 0;

        // Responsibility 1: Managing Entries
        public void AddEntry(string text)
        {
            entries.Add($"{++count}: {text}");
        }

        // Responsibility 2: Persistence (Violates SRP)
        public void Save(string fileName)
        {

            using (StreamWriter sw = new StreamWriter(fileName))
            {
                File.WriteAllText(fileName, string.Join(Environment.NewLine, entries));
            }
        }
    }

    public class Journal
    {
        private readonly List<string> entries = new();
        private static int count = 0;

        public void AddEntry(string text)
        {
            entries.Add($"{++count}: {text}");
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, entries);
        }
    }

    public class PersistenceManager
    {
        public void SaveToFile(Journal journal, string fileName, bool overwrite = false)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                if (overwrite || !File.Exists(fileName))
                {
                    File.WriteAllText(fileName, journal.ToString());
                }
            }
        }
    }

    public class SolidResult
    {
        public void Result()
        {
            Journal journal = new Journal();

            journal.AddEntry("Ola sou lewis");
            journal.AddEntry("Como está você");

            PersistenceManager pm = new();

            pm.SaveToFile(journal, "lewis.txt");
        }
    }
}