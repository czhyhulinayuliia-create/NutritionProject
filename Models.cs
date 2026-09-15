using System;
using System.Collections.Generic;

namespace FinalNutritionProject.Models
{
    public enum Gender { Male, Female }

    public class UserAccount
    {
        public string Username { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public UserProfile Profile { get; set; } = new UserProfile();
    }

    public class UserProfile
    {
        public double WeightKg { get; set; } = 70;
        public double HeightCm { get; set; } = 175;
        public int Age { get; set; } = 25;
        public Gender UserGender { get; set; } = Gender.Male;
        public int ActivityLevelIndex { get; set; } = 1;
        public int GoalIndex { get; set; } = 1;
        public string AllergyKeywords { get; set; } = "";
    }

    public class DishItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = "";
        public string Category { get; set; } = "";
        public int Calories { get; set; }
        public double Protein { get; set; }
        public double Fat { get; set; }
        public double Carbs { get; set; }
        public int ServingSizeGrams { get; set; } = 100;
        public string Tags { get; set; } = "";
    }
}