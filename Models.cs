using System;
using System.Collections.Generic;
using System.Linq;

namespace FinalNutritionProject
{
    public class UserProfile
    {
        public double Weight { get; set; }
        public double Height { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public HashSet<string> Allergies { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    public class Product
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public double Calories { get; set; }
        public double Proteins { get; set; }
        public double Fats { get; set; }
        public double Carbohydrates { get; set; }
        public string Category { get; set; }
        public List<string> DietRestrictions { get; set; } = new List<string>();
    }

    public class Meal
    {
        public string MealType { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
        public double TotalCalories => Products.Sum(p => p.Calories);
    }

    public class MealPlan
    {
        public DateTime Date { get; set; } = DateTime.Now;
        public List<Meal> Meals { get; set; } = new List<Meal>();
        public double TargetCalories { get; set; }
        public double TotalPlanCalories => Meals.Sum(m => m.TotalCalories);
    }

    public class ProductUI
    {
        public string Name { get; set; }
        public double Calories { get; set; }
        public string Restriction { get; set; }

        public ProductUI(string name, double calories, string restriction)
        {
            Name = name;
            Calories = calories;
            Restriction = restriction;
        }
    }
}