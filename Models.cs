#nullable disable
using System.Collections.Generic;

namespace FinalNutritionProject
{
    public class Product
    {
        public string Name { get; set; }
        public double Calories { get; set; }
        public string Category { get; set; }
        public List<string> DietRestrictions { get; set; } = new List<string>();
    }

    public class UserProfile
    {
        public double Weight { get; set; }
        public double Height { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public int ActivityLevel { get; set; }
        public int Goal { get; set; }
        public List<string> Allergies { get; set; } = new List<string>();
    }

    public class Meal
    {
        public string MealType { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
    }

    public class DailyMealPlan
    {
        public List<Meal> Meals { get; set; } = new List<Meal>();
        public double TotalCalories { get; set; }
    }

    public class ProductUI
    {
        public string Name { get; set; }
        public double Calories { get; set; }
        public string Category { get; set; }
        public string Restriction { get; set; }

        public ProductUI(string name, double calories, string category, string restriction)
        {
            Name = name;
            Calories = calories;
            Category = category;
            Restriction = restriction;
        }
    }
}