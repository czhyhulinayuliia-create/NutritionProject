using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.Generic;
using System.Linq;

namespace FinalNutritionProject
{
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

    public partial class MainWindow : Window
    {
        private List<ProductUI> _database = new List<ProductUI>
        {
            new ProductUI("🥣 Вівсянка з бананом", 310, "глютен"),
            new ProductUI("🥗 Салат з куркою", 290, ""),
            new ProductUI("🍳 Яєчня з томатами", 260, "яйця"),
            new ProductUI("🐟 Філе лосося", 450, "риба"),
            new ProductUI("🥛 Йогурт грецький", 120, "лактоза"),
            new ProductUI("🥩 Стейк яловичий", 520, ""),
            new ProductUI("🥦 Броколі на пару", 140, ""),
            new ProductUI("🥧 Сирна запіканка", 340, "лактоза, яйця"),
            new ProductUI("🥜 Мигдаль (жменя)", 240, "горіхи"),
            new ProductUI("🥪 Сендвіч з тунцем", 380, "риба, глютен"),
            new ProductUI("🥔 Запечена картопля", 280, ""),
            new ProductUI("🍛 Рис з овочами", 310, ""),
            new ProductUI("🍤 Креветки гриль", 220, "риба"),
            new ProductUI("🥣 Суп сочевичний", 250, ""),
            new ProductUI("🍖 Курячі ніжки", 410, ""),
            new ProductUI("🥗 Салат з тофу", 210, ""),
            new ProductUI("🍏 Зелене яблуко", 90, ""),
            new ProductUI("🥞 Млинці з творогом", 390, "глютен, лактоза, яйця"),
            new ProductUI("🧀 Шматочок сиру", 110, "лактоза"),
            new ProductUI("🥤 Протеїновий шейк", 190, "лактоза")
        };

        public MainWindow()
        {
            InitializeComponent();
        }

        public void OnCalculateClick(object sender, RoutedEventArgs e)
        {
            double.TryParse(WeightInput.Text, out double w);
            double.TryParse(HeightInput.Text, out double h);
            int.TryParse(AgeInput.Text, out int a);
            string allergies = AllergyInput.Text?.ToLower() ?? "";

            double bmr = (GenderInput.SelectedIndex == 0) 
                ? (10 * w + 6.25 * h - 5 * a + 5) 
                : (10 * w + 6.25 * h - 5 * a - 161);

            if (bmr > 500)
                BmrResult.Text = $"🔥 ВАША НОРМА: {bmr:F0} ККАЛ";
            else
                BmrResult.Text = "⚠️ ПЕРЕВІРТЕ ДАНІ";

            var restrictions = allergies.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();

            ResultList.ItemsSource = _database.Where(p => 
                !restrictions.Any(r => p.Restriction.Contains(r))
            ).ToList();
        }
    }
}