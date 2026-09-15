using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using FinalNutritionProject.Models;

namespace FinalNutritionProject.Services
{
    public static class Logger
    {
        private static readonly string LogFile = "log.txt";
        public static void Log(string message)
        {
            try
            {
                File.AppendAllText(LogFile, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}");
            }
            catch { }
        }
    }

    public class DataRepository
    {
        private const string UsersFile = "users.json";
        private const string DishesFile = "dishes.json";

        public List<UserAccount> LoadUsers()
        {
            if (!File.Exists(UsersFile)) return new List<UserAccount>();
            try
            {
                var json = File.ReadAllText(UsersFile);
                return JsonSerializer.Deserialize<List<UserAccount>>(json) ?? new List<UserAccount>();
            }
            catch (Exception ex)
            {
                Logger.Log($"Error loading users: {ex.Message}");
                return new List<UserAccount>();
            }
        }

        public void SaveUsers(List<UserAccount> users)
        {
            try
            {
                var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(UsersFile, json);
            }
            catch (Exception ex)
            {
                Logger.Log($"Error saving users: {ex.Message}");
            }
        }

        public List<DishItem> LoadDishes()
        {
            if (!File.Exists(DishesFile)) return GetSeedDishes();
            try
            {
                var json = File.ReadAllText(DishesFile);
                var items = JsonSerializer.Deserialize<List<DishItem>>(json);
                return (items != null && items.Count > 0) ? items : GetSeedDishes();
            }
            catch (Exception ex)
            {
                Logger.Log($"Error loading dishes: {ex.Message}");
                return GetSeedDishes();
            }
        }

        public void SaveDishes(List<DishItem> dishes)
        {
            try
            {
                var json = JsonSerializer.Serialize(dishes, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(DishesFile, json);
            }
            catch (Exception ex)
            {
                Logger.Log($"Error saving dishes: {ex.Message}");
            }
        }

        private List<DishItem> GetSeedDishes()
        {
            return new List<DishItem>
            {
                new DishItem { Title = "Вівсянка з ягодами та горіхами", Category = "Ранок", Calories = 420, Protein = 12, Fat = 10, Carbs = 65, ServingSizeGrams = 300, Tags = "вівсянка, горіхи, каша" },
                new DishItem { Title = "Омлет із 3 яєць з томатами", Category = "Ранок", Calories = 380, Protein = 22, Fat = 28, Carbs = 6, ServingSizeGrams = 250, Tags = "яйця, овочі, омлет" },
                new DishItem { Title = "Сирники з медом та сметаною", Category = "Ранок", Calories = 510, Protein = 25, Fat = 18, Carbs = 58, ServingSizeGrams = 220, Tags = "сир, лактоза, десерт" },
                new DishItem { Title = "Авокадо-тост із яйцем пашот", Category = "Ранок", Calories = 360, Protein = 14, Fat = 22, Carbs = 28, ServingSizeGrams = 200, Tags = "хліб, авокадо, яйця" },
                new DishItem { Title = "Гранола з грецьким йогуртом", Category = "Ранок", Calories = 440, Protein = 18, Fat = 14, Carbs = 60, ServingSizeGrams = 250, Tags = "йогурт, гранола, ягоди" },
                new DishItem { Title = "Млинці з кисломолочним сиром", Category = "Ранок", Calories = 480, Protein = 20, Fat = 16, Carbs = 62, ServingSizeGrams = 240, Tags = "млинці, сир, лактоза" },

                new DishItem { Title = "Борщ український з яловичиною", Category = "Обід", Calories = 450, Protein = 25, Fat = 18, Carbs = 42, ServingSizeGrams = 400, Tags = "суп, яловичина, овочі" },
                new DishItem { Title = "Гречка з телячою котлетою", Category = "Обід", Calories = 580, Protein = 38, Fat = 16, Carbs = 68, ServingSizeGrams = 350, Tags = "гречка, м'ясо, телятина" },
                new DishItem { Title = "Стейк із лосося з диким рисом", Category = "Обід", Calories = 640, Protein = 42, Fat = 26, Carbs = 54, ServingSizeGrams = 320, Tags = "риба, рис, лосось" },
                new DishItem { Title = "Куряча грудка з булгуром та овочами", Category = "Обід", Calories = 520, Protein = 46, Fat = 10, Carbs = 60, ServingSizeGrams = 380, Tags = "курка, булгур, овочі" },
                new DishItem { Title = "Паста Болоньєзе з твердих сортів", Category = "Обід", Calories = 610, Protein = 30, Fat = 20, Carbs = 75, ServingSizeGrams = 350, Tags = "паста, фарш, томати" },
                new DishItem { Title = "Крем-суп із грибів із сухариками", Category = "Обід", Calories = 390, Protein = 10, Fat = 22, Carbs = 38, ServingSizeGrams = 350, Tags = "суп, гриби, вершки" },

                new DishItem { Title = "Протеїновий коктейль з бананом", Category = "Перекус", Calories = 280, Protein = 26, Fat = 4, Carbs = 35, ServingSizeGrams = 350, Tags = "протеїн, банан, молоко" },
                new DishItem { Title = "Жменя мигдалю та свіже яблуко", Category = "Перекус", Calories = 240, Protein = 6, Fat = 15, Carbs = 22, ServingSizeGrams = 150, Tags = "горіхи, фрукти, мигдаль" },
                new DishItem { Title = "Запечене яблуко з корицею та горіхами", Category = "Перекус", Calories = 190, Protein = 3, Fat = 8, Carbs = 28, ServingSizeGrams = 200, Tags = "десерт, фрукти, горіхи" },
                new DishItem { Title = "Хумус із овочевими паличками", Category = "Перекус", Calories = 260, Protein = 9, Fat = 14, Carbs = 25, ServingSizeGrams = 220, Tags = "нут, овочі, морква" },
                new DishItem { Title = "Рисові хлібці з арахісовою пастою", Category = "Перекус", Calories = 310, Protein = 10, Fat = 18, Carbs = 28, ServingSizeGrams = 100, Tags = "хлібці, арахіс" },

                new DishItem { Title = "Запечена тріска з броколі", Category = "Вечір", Calories = 340, Protein = 36, Fat = 6, Carbs = 18, ServingSizeGrams = 300, Tags = "риба, броколі, тріска" },
                new DishItem { Title = "Куряче філе на грилі зі спаржею", Category = "Вечір", Calories = 410, Protein = 45, Fat = 9, Carbs = 12, ServingSizeGrams = 280, Tags = "курка, овочі, спаржа" },
                new DishItem { Title = "Салат з тунцем та яйцем", Category = "Вечір", Calories = 380, Protein = 34, Fat = 18, Carbs = 10, ServingSizeGrams = 300, Tags = "салат, тунець, яйця" },
                new DishItem { Title = "Запечена індичка з кабачками", Category = "Вечір", Calories = 360, Protein = 40, Fat = 8, Carbs = 14, ServingSizeGrams = 310, Tags = "індичка, кабачки, овочі" },
                new DishItem { Title = "Стейк із тофу з овочами на пару", Category = "Вечір", Calories = 310, Protein = 22, Fat = 14, Carbs = 16, ServingSizeGrams = 290, Tags = "тофу, вегетаріанське, овочі" }
            };
        }
    }

    public static class NutritionCalculator
    {
        public static (int targetCalories, int protein, int fat, int carbs) CalculateTargets(UserProfile profile)
        {
            double genderBonus = profile.UserGender == Gender.Male ? 5 : -161;
            double bmr = 10 * profile.WeightKg + 6.25 * profile.HeightCm - 5 * profile.Age + genderBonus;

            double actMult = profile.ActivityLevelIndex switch
            {
                0 => 1.2,
                1 => 1.375,
                2 => 1.55,
                _ => 1.375
            };

            double tdee = bmr * actMult;

            double targetCal = profile.GoalIndex switch
            {
                0 => tdee - 500,
                1 => tdee,
                2 => tdee + 400,
                _ => tdee
            };

            int finalCal = (int)Math.Round(targetCal);
            int p = (int)Math.Round((finalCal * 0.30) / 4);
            int f = (int)Math.Round((finalCal * 0.30) / 9);
            int c = (int)Math.Round((finalCal * 0.40) / 4);

            return (finalCal, p, f, c);
        }
    }

    public static class StringDistance
    {
        public static int LevenshteinDistance(string s, string t)
        {
            if (string.IsNullOrEmpty(s)) return string.IsNullOrEmpty(t) ? 0 : t.Length;
            if (string.IsNullOrEmpty(t)) return s.Length;

            int[] v0 = new int[t.Length + 1];
            int[] v1 = new int[t.Length + 1];

            for (int i = 0; i < v0.Length; i++) v0[i] = i;

            for (int i = 0; i < s.Length; i++)
            {
                v1[0] = i + 1;
                for (int j = 0; j < t.Length; j++)
                {
                    int cost = (s[i] == t[j]) ? 0 : 1;
                    v1[j + 1] = Math.Min(Math.Min(v1[j] + 1, v0[j + 1] + 1), v0[j] + cost);
                }
                for (int j = 0; j < v0.Length; j++) v0[j] = v1[j];
            }
            return v1[t.Length];
        }
    }
}