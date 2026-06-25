using System;
using System.Collections.Generic;
using System.Linq;

namespace FinalNutritionProject
{
    public class NutritionCalculator
    {
        public double CalculateBMR(UserProfile profile)
        {
            if (profile.Weight <= 0 || profile.Height <= 0 || profile.Age <= 0) return 0;

            if (profile.Gender == "Чоловік")
                return 10 * profile.Weight + 6.25 * profile.Height - 5 * profile.Age + 5;
            else
                return 10 * profile.Weight + 6.25 * profile.Height - 5 * profile.Age - 161;
        }

        public List<Product> FilterProducts(List<Product> database, HashSet<string> allergies)
        {
            var resultStack = new Stack<Product>();
            foreach (var prod in database)
            {
                bool hasAllergy = false;
                foreach (var restriction in prod.DietRestrictions)
                {
                    if (allergies.Contains(restriction))
                    {
                        hasAllergy = true;
                        break;
                    }
                }
                if (!hasAllergy)
                {
                    resultStack.Push(prod);
                }
            }
            return resultStack.ToList();
        }

        public List<Product> SearchWithFuzzy(List<Product> products, string query, int maxDistance = 2)
        {
            if (string.IsNullOrWhiteSpace(query)) return products;
            
            var result = new List<Product>();
            string lowerQuery = query.ToLower().Trim();

            foreach (var p in products)
            {
                string lowerName = p.Name.ToLower();
                if (lowerName.Contains(lowerQuery))
                {
                    result.Add(p);
                    continue;
                }

                string[] words = lowerName.Split(' ');
                foreach (var word in words)
                {
                    string cleanWord = new string(word.Where(char.IsLetter).ToArray());
                    if (LevenshteinDistance(cleanWord, lowerQuery) <= maxDistance)
                    {
                        result.Add(p);
                        break;
                    }
                }
            }
            return result;
        }

        private int LevenshteinDistance(string s, string t)
        {
            if (string.IsNullOrEmpty(s)) return t?.Length ?? 0;
            if (string.IsNullOrEmpty(t)) return s.Length;

            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 0; j <= m; d[0, j] = j++) ;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }

        public MealPlan GeneratePlan(List<Product> allowedProducts, double targetCalories)
        {
            var plan = new MealPlan { TargetCalories = targetCalories };
            if (targetCalories <= 500 || !allowedProducts.Any()) return plan;

            var breakfast = new Meal { MealType = "Сніданок" };
            var lunch = new Meal { MealType = "Обід" };
            var dinner = new Meal { MealType = "Вечеря" };

            var bProd = allowedProducts.FirstOrDefault(p => p.Category == "Сніданок") ?? allowedProducts.First();
            var lProd = allowedProducts.FirstOrDefault(p => p.Category == "Обід") ?? allowedProducts.First();
            var dProd = allowedProducts.FirstOrDefault(p => p.Category == "Вечеря") ?? allowedProducts.First();

            breakfast.Products.Add(bProd);
            lunch.Products.Add(lProd);
            dinner.Products.Add(dProd);

            plan.Meals.AddRange(new[] { breakfast, lunch, dinner });
            return plan;
        }
    }
}