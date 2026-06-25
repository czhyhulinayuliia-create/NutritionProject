using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinalNutritionProject
{
    public partial class MainWindow : Window
    {
        private readonly DataAccess _dbLayer = new DataAccess();
        private readonly NutritionCalculator _logicLayer = new NutritionCalculator();
        private List<Product> _productsTable;

        public MainWindow()
        {
            InitializeComponent();
            _productsTable = _dbLayer.LoadProducts();
            _dbLayer.LogAction("Application started.");
        }

        public void OnCalculateClick(object sender, RoutedEventArgs e)
        {
            double.TryParse(WeightInput.Text, out double w);
            double.TryParse(HeightInput.Text, out double h);
            int.TryParse(AgeInput.Text, out int a);
            string allergiesText = AllergyInput.Text ?? "";

            if (w <= 0 || h <= 0 || a <= 0)
            {
                BmrResult.Text = "⚠️ ПЕРЕВІРТЕ ДАНІ";
                _dbLayer.LogAction("Calculation error: invalid input.");
                return;
            }

            var profile = new UserProfile
            {
                Weight = w,
                Height = h,
                Age = a,
                Gender = (GenderInput.SelectedIndex == 0) ? "Чоловік" : "Жінка"
            };

            var restrictions = allergiesText.Split(',')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s));

            foreach (var res in restrictions)
            {
                profile.Allergies.Add(res);
            }

            double bmr = _logicLayer.CalculateBMR(profile);
            BmrResult.Text = $"🔥 ВАША НОРМА: {bmr:F0} ККАЛ";

            var filtered = _logicLayer.FilterProducts(_productsTable, profile.Allergies);
            var mealPlan = _logicLayer.GeneratePlan(filtered, bmr);

            var displayList = new List<ProductUI>();
            foreach (var meal in mealPlan.Meals)
            {
                foreach (var p in meal.Products)
                {
                    displayList.Add(new ProductUI($"{meal.MealType}: {p.Name}", p.Calories, string.Join(", ", p.DietRestrictions)));
                }
            }

            ResultList.ItemsSource = displayList;

            StringBuilder report = new StringBuilder();
            report.AppendLine("=== ЗВІТ ПРОГРАМИ NUTRITION PRO ===");
            report.AppendLine($"Дата та час: {DateTime.Now}");
            report.AppendLine($"Параметри: Вага {w} кг, Зріст {h} см, Вік {a}, Стать {profile.Gender}");
            report.AppendLine($"Обмеження/Алергії: {allergiesText}");
            report.AppendLine($"Розрахована добова норма: {bmr:F0} ккал");
            report.AppendLine("\nЗгенероване меню:");
            foreach (var item in displayList)
            {
                report.AppendLine($"- {item.Name} — {item.Calories} ккал");
            }
            
            _dbLayer.ExportReport(report.ToString());
            _dbLayer.LogAction($"Calculation successful for BMR = {bmr:F0}");
        }
    }
}