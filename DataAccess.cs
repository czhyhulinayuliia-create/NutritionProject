#nullable disable
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace FinalNutritionProject
{
    public class DataAccess
    {
        private const string JsonPath = "products.json";
        private const string LogPath = "log.txt";

        public List<Product> LoadProducts()
        {
            try {
                if (!File.Exists(JsonPath)) {
                    var list = GenerateBigMenu();
                    SaveProducts(list);
                    return list;
                }
                return JsonSerializer.Deserialize<List<Product>>(File.ReadAllText(JsonPath)) ?? GenerateBigMenu();
            } catch { return GenerateBigMenu(); }
        }

        public void SaveProducts(List<Product> list)
        {
            try {
                File.WriteAllText(JsonPath, JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true }));
            } catch { }
        }

        public void LogAction(string m) { try { File.AppendAllText(LogPath, $"[{DateTime.Now}] {m}\n"); } catch { } }
        public void ExportReport(string c) { try { File.WriteAllText("report.txt", c); } catch { } }

        private List<Product> GenerateBigMenu()
        {
            return new List<Product> {
                new Product { Name = "🥣 Вівсяна каша з лохиною та медом", Calories = 290, Category = "Сніданок" },
                new Product { Name = "🍳 Скрембл з трьох яєць та шпинатом", Calories = 340, Category = "Сніданок" },
                new Product { Name = "🥞 Протеїнові млинці", Calories = 410, Category = "Сніданок", DietRestrictions = new List<string>{"лактоза"} },
                new Product { Name = "🥪 Кранч-тост з лососем та гуакамоле", Calories = 380, Category = "Сніданок" },
                new Product { Name = "🧇 Вафлі з сиропом агави", Calories = 360, Category = "Сніданок" },
                new Product { Name = "🥩 Філе-міньйон з печеною картоплею", Calories = 620, Category = "Обід" },
                new Product { Name = "🍲 Борщ з яловичиною та зеленню", Calories = 410, Category = "Обід" },
                new Product { Name = "🐟 Стейк лосося на грилі з диким рисом", Calories = 510, Category = "Обід" },
                new Product { Name = "🍜 Суп-локшина курячий", Calories = 280, Category = "Обід", DietRestrictions = new List<string>{"глютен"} },
                new Product { Name = "🍛 Крем-суп з гарбуза та насіння", Calories = 310, Category = "Обід" },
                new Product { Name = "🥗 Теплий салат з індичкою", Calories = 320, Category = "Вечеря" },
                new Product { Name = "🥦 Котлети з тріски та броколі", Calories = 240, Category = "Вечеря" },
                new Product { Name = "🐟 Хек запечений у фользі з томатами", Calories = 270, Category = "Вечеря" },
                new Product { Name = "🍚 Різотто з морепродуктами", Calories = 440, Category = "Вечеря" },
                new Product { Name = "🥛 Натуральний йогурт з чіа", Calories = 140, Category = "Перекус", DietRestrictions = new List<string>{"лактоза"} },
                new Product { Name = "🥜 Мигдаль та кеш'ю", Calories = 240, Category = "Перекус", DietRestrictions = new List<string>{"горіхи"} },
                new Product { Name = "🍎 Зелене яблуко", Calories = 65, Category = "Перекус" },
                new Product { Name = "🍌 Стиглий банан", Calories = 95, Category = "Перекус" }
            };
        }
    }
}