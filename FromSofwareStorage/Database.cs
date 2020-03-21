namespace FromSoftwareStorage
{
    public static class Database
    {
        public static IDatabaseProvider DatabaseProvider { get; }

        static Database()
        {
            DatabaseProvider = new DatabaseEngineProvider();
        }
    }
}
