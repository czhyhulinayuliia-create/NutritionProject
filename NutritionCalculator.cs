#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinalNutritionProject
{
    public class NutritionCalculator
    {
        public double CalculateBMR(UserProfile p)
        {
            if (p == null) return 2000;
            double baseBmr = (10 * p.Weight) + (6.25 * p.Height) - (5 * p.Age);
            baseBmr = p.Gender == "Чоловік" ? baseBmr + 5 : baseBmr - 161;

            double[] activityMultipliers = { 1.2, 1.55, 1.9 };
            double bmr = baseBmr * activityMultipliers[p.ActivityLevel];

            if (p.Goal == 0) bmr -= 400;
            else if (p.Goal == 2) bmr += 400;

            return bmr;
        }

        public (double P, double F, double C) CalculateMacros(double calories, int goal)
        {
            double pPct = 0.3, fPct = 0.3, cPct = 0.4;
            if (goal == 0) { pPct = 0.4; fPct = 0.25; cPct = 0.35; }
            else if (goal == 2) { pPct = 0.35; fPct = 0.25; cPct = 0.4; }

            return ((calories * pPct) / 4, (calories * fPct) / 9, (calories * cPct) / 4);
        }

        public List<Product> FilterProducts(List<Product> prods, List<string> allergies)
        {
            if (allergies == null || allergies.Count == 0) return prods;
            return prods.Where(p => !p.DietRestrictions.Any(r => allergies.Contains(r.ToLower()))).ToList();
        }

        public DailyMealPlan GeneratePlan(List<Product> prods, double target)
        {
            var plan = new DailyMealPlan();
            string[] cats = { "Сніданок", "Обід", "Вечеря", "Перекус" };
            foreach (var c in cats) {
                var m = new Meal { MealType = c };
                var matched = prods.Where(x => x.Category == c).ToList();
                if (matched.Count > 0) m.Products.Add(matched[new Random().Next(matched.Count)]);
                plan.Meals.Add(m);
            }
            return plan;
        }

        public int LevenshteinDistance(string s, string t)
        {
            if (string.IsNullOrEmpty(s)) return t?.Length ?? 0;
            if (string.IsNullOrEmpty(t)) return s.Length;
            int[,] d = new int[s.Length + 1, t.Length + 1];
            for (int i = 0; i <= s.Length; d[i, 0] = i++) { }
            for (int j = 0; j <= t.Length; j++) { d[0, j] = j; }
            for (int i = 1; i <= s.Length; i++) {
                for (int j = 1; j <= t.Length; j++) {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }
            return d[s.Length, t.Length];
        }
    }
}