using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace FinalNutritionProject
{
    public class DataAccess
    {
        private readonly string _productsFile = "products.json";
        private readonly string _logFile = "log.txt";

        public void LogAction(string action)
        {
            try
            {
                File.AppendAllText(_logFile, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {action}\n");
            }
            catch { }
        }

        public List<Product> LoadProducts()
        {
            if (!File.Exists(_productsFile))
            {
                var defaultDb = GetDefaultProducts();
                SaveProducts(defaultDb);
                return defaultDb;
            }
            try
            {
                string json = File.ReadAllText(_productsFile);
                return JsonSerializer.Deserialize<List<Product>>(json) ?? GetDefaultProducts();
            }
            catch
            {
                return GetDefaultProducts();
            }
        }

        public void SaveProducts(List<Product> products)
        {
            try
            {
                string json = JsonSerializer.Serialize(products, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_productsFile, json);
                LogAction("Database updated.");
            }
            catch (Exception ex)
            {
                LogAction($"Error saving database: {ex.Message}");
            }
        }

        public void ExportReport(string content)
        {
            try
            {
                File.WriteAllText("report.txt", content);
                LogAction("Report exported.");
            }
            catch (Exception ex)
            {
                LogAction($"Error exporting report: {ex.Message}");
            }
        }

        private List<Product> GetDefaultProducts()
        {
            return new List<Product>
            {
                new Product { Name = "🥣 Вівсянка з бананом", Calories = 310, Category = "Сніданок", DietRestrictions = new List<string>{"глютен"} },
                new Product { Name = "🥗 Салат з куркою", Calories = 290, Category = "Обід" },
                new Product { Name = "🍳 Яєчня з томатами", Calories = 260, Category = "Сніданок", DietRestrictions = new List<string>{"яйця"} },
                new Product { Name = "🐟 Філе лосося", Calories = 450, Category = "Вечеря", DietRestrictions = new List<string>{"риба"} },
                new Product { Name = "🥛 Йогурт грецький", Calories = 120, Category = "Перекус", DietRestrictions = new List<string>{"лактоза"} },
                new Product { Name = "🥩 Стейк яловичий", Calories = 520, Category = "Обід" },
                new Product { Name = "🥦 Броколі на пару", Calories = 140, Category = "Вечеря" },
                new Product { Name = "🥧 Сирна запіканка", Calories = 340, Category = "Сніданок", DietRestrictions = new List<string>{"лактоза", "яйця"} },
                new Product { Name = "🥜 Мигдаль (жменя)", Calories = 240, Category = "Перекус", DietRestrictions = new List<string>{"горіхи"} },
                new Product { Name = "🥪 Сендвіч з тунцем", Calories = 380, Category = "Обід", DietRestrictions = new List<string>{"риба", "глютен"} }
            };
        }
    }
}