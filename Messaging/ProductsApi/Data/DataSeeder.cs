using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductsApi.Data.Entities;
using System.Text.Json;

namespace ProductsApi.Data
{
    public static class DataSeeder
    {
        public static void SeedData(ProductContext _context)
        {
            if (!_context.Products.Any())
            {
                _context.Products.AddRange(LoadProducts());
                _context.SaveChanges();
            }
            if (!_context.Categories.Any())
            {
                _context.Categories.AddRange(LoadCategories());
                _context.SaveChanges();
            }
        }

        private static List<Product> LoadProducts()
        {
            var jsonPath = @"C:\Coding projects\Microservices\Messaging\ProductsApi\data.json";
            using (StreamReader file = File.OpenText(jsonPath))
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

        private static List<Category> LoadCategories()
        {
            var jsonPath = @"C:\Coding projects\Microservices\Messaging\ProductsApi\category.json";
            using (StreamReader file = File.OpenText(jsonPath))
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
