namespace ProductsApi.Data
{
    public static class DbContextExtenstion
    {

        public static void EnsureSeeded(this ProductContext context, string dataPath, string categoryDataPath)
        {
            DataSeeder.SeedData(context, dataPath, categoryDataPath);
        }

    }
}
