using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductsApi.Data.Entities;
using System.Text.Json;

namespace ProductsApi.Data
{
    public static class DataSeeder
    {
        public static void SeedData(ProductContext _context, string dataPath, string categoryDataPath)
        {
            if (!_context.Products.Any())
            {
                _context.Products.AddRange(LoadProducts(dataPath));
                _context.SaveChanges();
            }
            if (!_context.Categories.Any())
            {
                _context.Categories.AddRange(LoadCategories(categoryDataPath));
                _context.SaveChanges();
            }
        }

        private static List<Product> LoadProducts(string dataPath)
        {
            using (StreamReader file = File.OpenText(dataPath))
            {
                List<Product> products = new List<Product>();
                string json = file.ReadToEnd();
                try
                {
                    products = JsonConvert.DeserializeObject<List<Product>>(json);

                }
                catch (JsonSerializationException ex)
                {
                    Console.WriteLine($"Deserialization error: {ex.Message}");
                    throw;
                }
                return products;
            }
        }

        private static List<Category> LoadCategories(string categoryDataPath)
        {
            using (StreamReader file = File.OpenText(categoryDataPath))
            {
                List<Category> categories = new List<Category>();
                string json = file.ReadToEnd();
                try
                {
                    categories = JsonConvert.DeserializeObject<List<Category>>(json);

                }
                catch (JsonSerializationException ex)
                {
                    Console.WriteLine($"Deserialization error: {ex.Message}");
                    throw;
                }
                return categories;
            }
        }
    }
}
