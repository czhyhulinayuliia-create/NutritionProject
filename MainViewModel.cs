using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DietAppWorking
{
    public class FoodPreset
    {
        public string Title { get; set; } = "";
        public double Cal100g { get; set; }
        public double Prot100g { get; set; }
        public double Fat100g { get; set; }
        public double Carb100g { get; set; }

        public override string ToString() => $"{Title} ({Cal100g} ккал / 100г)";
    }

    public class LoggedFoodItem
    {
        public string Name { get; set; } = "";
        public double Grams { get; set; }
        public double Calories { get; set; }
        public double Proteins { get; set; }
        public double Fats { get; set; }
        public double Carbs { get; set; }

        public override string ToString() => $"{Name} ({Grams}g) — {Calories:F0} ккал [Б: {Proteins:F1}g, Ж: {Fats:F1}g, У: {Carbs:F1}g]";
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        private bool _isLoggedIn = false;
        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set 
            { 
                _isLoggedIn = value; 
                OnPropertyChanged(); 
                OnPropertyChanged(nameof(IsLoggedOut));
            }
        }

        public bool IsLoggedOut => !IsLoggedIn;

        private bool _isRegMode = false;
        public bool IsRegMode
        {
            get => _isRegMode;
            set 
            { 
                _isRegMode = value; 
                OnPropertyChanged(); 
                OnPropertyChanged(nameof(AuthBtnText)); 
                OnPropertyChanged(nameof(ToggleAuthModeText)); 
            }
        }

        public string AuthBtnText => IsRegMode ? "Зарегистрироваться" : "Войти";
        public string ToggleAuthModeText => IsRegMode ? "Уже есть аккаунт? Войти" : "Нет аккаунта? Зарегистрироваться";

        public string Username { get; set; } = "Алексей";
        public string Password { get; set; } = "";

        private string _authStatusMessage = "";
        public string AuthStatusMessage
        {
            get => _authStatusMessage;
            set { _authStatusMessage = value; OnPropertyChanged(); }
        }

        private int _currentTab = 0;
        public int CurrentTab
        {
            get => _currentTab;
            set
            {
                _currentTab = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsTab0));
                OnPropertyChanged(nameof(IsTab1));
                OnPropertyChanged(nameof(IsTab2));
                OnPropertyChanged(nameof(IsTab3));
            }
        }

        public bool IsTab0 => CurrentTab == 0;
        public bool IsTab1 => CurrentTab == 1;
        public bool IsTab2 => CurrentTab == 2;
        public bool IsTab3 => CurrentTab == 3;

        private double _weight = 75;
        public double Weight
        {
            get => _weight;
            set { _weight = value; OnPropertyChanged(); }
        }

        private double _targetWeight = 70;
        public double TargetWeight
        {
            get => _targetWeight;
            set { _targetWeight = value; OnPropertyChanged(); }
        }

        private double _height = 178;
        public double Height
        {
            get => _height;
            set { _height = value; OnPropertyChanged(); }
        }

        private int _age = 24;
        public int Age
        {
            get => _age;
            set { _age = value; OnPropertyChanged(); }
        }

        private double _dailyCalorieNorm = 2200;
        public double DailyCalorieNorm
        {
            get => _dailyCalorieNorm;
            set { _dailyCalorieNorm = value; OnPropertyChanged(); UpdateStats(); }
        }

        public ObservableCollection<LoggedFoodItem> LoggedFoods { get; set; } = new();
        public LoggedFoodItem? SelectedLoggedFood { get; set; }

        public List<FoodPreset> PresetFoods { get; set; } = new()
        {
            new FoodPreset { Title = "Куриная грудка вареная", Cal100g = 165, Prot100g = 31, Fat100g = 3.6, Carb100g = 0 },
            new FoodPreset { Title = "Гречка отварная", Cal100g = 110, Prot100g = 4.2, Fat100g = 0.8, Carb100g = 21 },
            new FoodPreset { Title = "Овсянка на воде", Cal100g = 88, Prot100g = 3, Fat100g = 1.7, Carb100g = 15 },
            new FoodPreset { Title = "Яйцо вареное (1 шт ≈ 50г)", Cal100g = 155, Prot100g = 13, Fat100g = 11, Carb100g = 1.1 },
            new FoodPreset { Title = "Творог 5%", Cal100g = 121, Prot100g = 17, Fat100g = 5, Carb100g = 1.8 },
            new FoodPreset { Title = "Яблоко", Cal100g = 52, Prot100g = 0.3, Fat100g = 0.2, Carb100g = 14 },
            new FoodPreset { Title = "Банан", Cal100g = 89, Prot100g = 1.1, Fat100g = 0.3, Carb100g = 23 }
        };

        public FoodPreset? SelectedPreset { get; set; }
        public string GramsInput { get; set; } = "100";

        private double _waterMl = 0;
        public double WaterMl
        {
            get => _waterMl;
            set { _waterMl = value; OnPropertyChanged(); OnPropertyChanged(nameof(WaterText)); }
        }

        public string WaterText => $"{WaterMl / 1000.0:F2} L / 2.50 L";

        private double _consumedCalories = 0;
        private double _consumedProteins = 0;
        private double _consumedFats = 0;
        private double _consumedCarbs = 0;

        public string CalorieProgressText => $"{_consumedCalories:F0} / {DailyCalorieNorm:F0} ккал";

        public string ProteinText => $"Белки: {_consumedProteins:F1} g";
        public string FatText => $"Жиры: {_consumedFats:F1} g";
        public string CarbsText => $"Углеводы: {_consumedCarbs:F1} g";

        public MainViewModel()
        {
            SelectedPreset = PresetFoods.FirstOrDefault();
            RecalculateAll();
        }

        public void ToggleAuthMode() => IsRegMode = !IsRegMode;

        public void AuthAction()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                AuthStatusMessage = "Введите имя пользователя!";
                return;
            }
            IsLoggedIn = true;
            AuthStatusMessage = "";
        }

        public void Logout() => IsLoggedIn = false;

        public void SelectTab0() => CurrentTab = 0;
        public void SelectTab1() => CurrentTab = 1;
        public void SelectTab2() => CurrentTab = 2;
        public void SelectTab3() => CurrentTab = 3;

        public void RecalculateAll()
        {
            double bmr = 88.36 + (13.4 * Weight) + (4.8 * Height) - (5.7 * Age);
            DailyCalorieNorm = Math.Round(bmr * 1.375);
        }

        public void AddPresetFood()
        {
            if (SelectedPreset != null && double.TryParse(GramsInput, out double grams) && grams > 0)
            {
                double factor = grams / 100.0;
                var item = new LoggedFoodItem
                {
                    Name = SelectedPreset.Title,
                    Grams = grams,
                    Calories = SelectedPreset.Cal100g * factor,
                    Proteins = SelectedPreset.Prot100g * factor,
                    Fats = SelectedPreset.Fat100g * factor,
                    Carbs = SelectedPreset.Carb100g * factor
                };

                LoggedFoods.Add(item);
                UpdateStats();
            }
        }

        public void RemoveSelectedFood()
        {
            if (SelectedLoggedFood != null)
            {
                LoggedFoods.Remove(SelectedLoggedFood);
                UpdateStats();
            }
        }

        private void UpdateStats()
        {
            _consumedCalories = LoggedFoods.Sum(f => f.Calories);
            _consumedProteins = LoggedFoods.Sum(f => f.Proteins);
            _consumedFats = LoggedFoods.Sum(f => f.Fats);
            _consumedCarbs = LoggedFoods.Sum(f => f.Carbs);

            OnPropertyChanged(nameof(CalorieProgressText));
            OnPropertyChanged(nameof(ProteinText));
            OnPropertyChanged(nameof(FatText));
            OnPropertyChanged(nameof(CarbsText));
        }

        public void AddWater250() => WaterMl += 250;
        public void AddWater500() => WaterMl += 500;
        public void ResetWater() => WaterMl = 0;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}