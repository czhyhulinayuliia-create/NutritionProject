using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FinalNutritionProject.Models;
using FinalNutritionProject.Services;

namespace FinalNutritionProject
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;
        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }
        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute();
        public void Execute(object? parameter) => _execute();
        public event EventHandler? CanExecuteChanged;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public class MainDashboardViewModel : INotifyPropertyChanged
    {
        private readonly DataRepository _repository = new DataRepository();
        private List<UserAccount> _users = new List<UserAccount>();
        private UserAccount? _currentUser;

        private bool _isLoggedIn = false;
        public bool IsLoggedIn { get => _isLoggedIn; set { SetField(ref _isLoggedIn, value); OnPropertyChanged(nameof(IsLoggedOut)); } }
        public bool IsLoggedOut => !IsLoggedIn;

        public string AuthUsername { get; set; } = "";
        public string AuthPassword { get; set; } = "";
        private string _authMessage = "";
        public string AuthMessage { get => _authMessage; set => SetField(ref _authMessage, value); }

        public string InputWeight { get; set; } = "75";
        public string InputHeight { get; set; } = "178";
        public string InputAge { get; set; } = "25";
        public int SelectedGenderIndex { get; set; } = 0;
        public int SelectedActivityIndex { get; set; } = 1;
        public int SelectedGoalIndex { get; set; } = 1;
        public string InputAllergies { get; set; } = "";

        private string _validationError = "";
        public string ValidationError { get => _validationError; set => SetField(ref _validationError, value); }

        private string _targetSummary = "0 ккал";
        public string TargetSummary { get => _targetSummary; set => SetField(ref _targetSummary, value); }
        private string _actualSummary = "0 ккал";
        public string ActualSummary { get => _actualSummary; set => SetField(ref _actualSummary, value); }
        private string _targetMacros = "Б:0г Ж:0г В:0г";
        public string TargetMacros { get => _targetMacros; set => SetField(ref _targetMacros, value); }
        private string _actualMacros = "Б:0г Ж:0г В:0г";
        public string ActualMacros { get => _actualMacros; set => SetField(ref _actualMacros, value); }

        private string _searchQuery = "";
        public string SearchQuery { get => _searchQuery; set { SetField(ref _searchQuery, value); FilterCatalog(); } }

        public ObservableCollection<DishItem> CatalogCollection { get; set; } = new ObservableCollection<DishItem>();
        public ObservableCollection<DishItem> FilteredCatalogCollection { get; set; } = new ObservableCollection<DishItem>();
        public ObservableCollection<DishItem> ActiveDietPlan { get; set; } = new ObservableCollection<DishItem>();

        public DishItem? SelectedCatalogItem { get; set; }
        public DishItem? SelectedPlanItem { get; set; }

        public string FormTitle { get; set; } = "";
        public int FormCategoryIndex { get; set; } = 0;
        public string FormCalories { get; set; } = "300";
        public string FormProtein { get; set; } = "15";
        public string FormFat { get; set; } = "10";
        public string FormCarbs { get; set; } = "30";
        public string FormTags { get; set; } = "";

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand SaveProfileAndCalculateCommand { get; }
        public ICommand AddSelectedToPlanCommand { get; }
        public ICommand RemoveFromPlanCommand { get; }
        public ICommand ExportReportCommand { get; }
        public ICommand CreateDishCommand { get; }
        public ICommand DeleteDishCommand { get; }

        public MainDashboardViewModel()
        {
            LoginCommand = new RelayCommand(ExecuteLogin);
            RegisterCommand = new RelayCommand(ExecuteRegister);
            LogoutCommand = new RelayCommand(ExecuteLogout);
            SaveProfileAndCalculateCommand = new RelayCommand(ExecuteSaveProfileAndCalculate);
            AddSelectedToPlanCommand = new RelayCommand(ExecuteAddSelectedToPlan);
            RemoveFromPlanCommand = new RelayCommand(ExecuteRemoveFromPlan);
            ExportReportCommand = new RelayCommand(ExecuteExportReport);
            CreateDishCommand = new RelayCommand(ExecuteCreateDish);
            DeleteDishCommand = new RelayCommand(ExecuteDeleteDish);

            _users = _repository.LoadUsers();
            var dishes = _repository.LoadDishes();
            foreach (var d in dishes) CatalogCollection.Add(d);
            FilterCatalog();
        }

        private void ExecuteLogin()
        {
            var user = _users.FirstOrDefault(u => u.Username.Equals(AuthUsername, StringComparison.OrdinalIgnoreCase) && u.PasswordHash == AuthPassword);
            if (user != null)
            {
                _currentUser = user;
                IsLoggedIn = true;
                AuthMessage = "";
                LoadUserProfile(user.Profile);
                ExecuteSaveProfileAndCalculate();
                Logger.Log($"User logged in: {user.Username}");
            }
            else
            {
                AuthMessage = "Невірний логін або пароль!";
            }
        }

        private void ExecuteRegister()
        {
            if (string.IsNullOrWhiteSpace(AuthUsername) || string.IsNullOrWhiteSpace(AuthPassword))
            {
                AuthMessage = "Заповніть логін і пароль!";
                return;
            }
            if (_users.Any(u => u.Username.Equals(AuthUsername, StringComparison.OrdinalIgnoreCase)))
            {
                AuthMessage = "Користувач вже існує!";
                return;
            }

            var newUser = new UserAccount { Username = AuthUsername, PasswordHash = AuthPassword };
            _users.Add(newUser);
            _repository.SaveUsers(_users);
            _currentUser = newUser;
            IsLoggedIn = true;
            AuthMessage = "";
            ExecuteSaveProfileAndCalculate();
            Logger.Log($"New user registered: {newUser.Username}");
        }

        private void ExecuteLogout()
        {
            Logger.Log($"User logged out: {_currentUser?.Username}");
            _currentUser = null;
            IsLoggedIn = false;
            ActiveDietPlan.Clear();
        }

        private void LoadUserProfile(UserProfile p)
        {
            InputWeight = p.WeightKg.ToString();
            InputHeight = p.HeightCm.ToString();
            InputAge = p.Age.ToString();
            SelectedGenderIndex = p.UserGender == Gender.Male ? 0 : 1;
            SelectedActivityIndex = p.ActivityLevelIndex;
            SelectedGoalIndex = p.GoalIndex;
            InputAllergies = p.AllergyKeywords;
            OnPropertyChanged(nameof(InputWeight));
            OnPropertyChanged(nameof(InputHeight));
            OnPropertyChanged(nameof(InputAge));
            OnPropertyChanged(nameof(SelectedGenderIndex));
            OnPropertyChanged(nameof(SelectedActivityIndex));
            OnPropertyChanged(nameof(SelectedGoalIndex));
            OnPropertyChanged(nameof(InputAllergies));
        }

        private void ExecuteSaveProfileAndCalculate()
        {
            ValidationError = "";
            if (!double.TryParse(InputWeight, out double w) || w < 20 || w > 300)
            {
                ValidationError = "Помилка: Введіть некоректну вагу (20 - 300 кг).";
                return;
            }
            if (!double.TryParse(InputHeight, out double h) || h < 50 || h > 250)
            {
                ValidationError = "Помилка: Введіть некоректний ріст (50 - 250 см).";
                return;
            }
            if (!int.TryParse(InputAge, out int a) || a < 10 || a > 120)
            {
                ValidationError = "Помилка: Введіть некоректний вік (10 - 120 років).";
                return;
            }

            var p = new UserProfile
            {
                WeightKg = w,
                HeightCm = h,
                Age = a,
                UserGender = SelectedGenderIndex == 0 ? Gender.Male : Gender.Female,
                ActivityLevelIndex = SelectedActivityIndex,
                GoalIndex = SelectedGoalIndex,
                AllergyKeywords = InputAllergies ?? ""
            };

            if (_currentUser != null)
            {
                _currentUser.Profile = p;
                _repository.SaveUsers(_users);
            }

            var targets = NutritionCalculator.CalculateTargets(p);
            TargetSummary = $"{targets.targetCalories} ккал";
            TargetMacros = $"Б:{targets.protein}г Ж:{targets.fat}г В:{targets.carbs}г";

            GenerateSmartDietPlan(targets.targetCalories, p.AllergyKeywords);
        }

        private void GenerateSmartDietPlan(int targetCalories, string allergies)
        {
            ActiveDietPlan.Clear();
            var stopWords = allergies.ToLower().Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var validDishes = CatalogCollection.Where(d =>
            {
                string combined = (d.Title + " " + d.Tags).ToLower();
                return !stopWords.Any(sw => combined.Contains(sw));
            }).ToList();

            if (!validDishes.Any()) return;

            var rand = new Random();
            var categories = new[] { "Ранок", "Обід", "Перекус", "Вечір" };
            foreach (var cat in categories)
            {
                var match = validDishes.Where(d => d.Category == cat).OrderBy(_ => rand.Next()).FirstOrDefault();
                if (match != null) ActiveDietPlan.Add(match);
            }

            RecalculateActualTotals();
        }

        private void RecalculateActualTotals()
        {
            int totalCal = ActiveDietPlan.Sum(d => d.Calories);
            double totalP = ActiveDietPlan.Sum(d => d.Protein);
            double totalF = ActiveDietPlan.Sum(d => d.Fat);
            double totalC = ActiveDietPlan.Sum(d => d.Carbs);

            ActualSummary = $"{totalCal} ккал";
            ActualMacros = $"Б:{Math.Round(totalP)}г Ж:{Math.Round(totalF)}г В:{Math.Round(totalC)}г";
        }

        private void FilterCatalog()
        {
            FilteredCatalogCollection.Clear();
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                foreach (var item in CatalogCollection) FilteredCatalogCollection.Add(item);
                return;
            }

            string q = SearchQuery.ToLower().Trim();
            var matches = CatalogCollection.Where(d => 
                d.Title.ToLower().Contains(q) || 
                d.Category.ToLower().Contains(q) ||
                d.Tags.ToLower().Contains(q) ||
                StringDistance.LevenshteinDistance(d.Title.ToLower(), q) <= 3
            ).ToList();

            foreach (var item in matches) FilteredCatalogCollection.Add(item);
        }

        private void ExecuteAddSelectedToPlan()
        {
            if (SelectedCatalogItem != null)
            {
                ActiveDietPlan.Add(SelectedCatalogItem);
                RecalculateActualTotals();
            }
        }

        private void ExecuteRemoveFromPlan()
        {
            if (SelectedPlanItem != null)
            {
                ActiveDietPlan.Remove(SelectedPlanItem);
                RecalculateActualTotals();
            }
        }

        private void ExecuteCreateDish()
        {
            if (string.IsNullOrWhiteSpace(FormTitle)) return;
            int.TryParse(FormCalories, out int cal);
            double.TryParse(FormProtein, out double p);
            double.TryParse(FormFat, out double f);
            double.TryParse(FormCarbs, out double c);

            string cat = FormCategoryIndex switch { 0 => "Ранок", 1 => "Обід", 2 => "Перекус", _ => "Вечір" };

            var newDish = new DishItem
            {
                Title = FormTitle,
                Category = cat,
                Calories = cal > 0 ? cal : 200,
                Protein = p,
                Fat = f,
                Carbs = c,
                Tags = FormTags ?? ""
            };

            CatalogCollection.Add(newDish);
            _repository.SaveDishes(CatalogCollection.ToList());
            FilterCatalog();

            FormTitle = "";
            OnPropertyChanged(nameof(FormTitle));
            Logger.Log($"Created new dish: {newDish.Title}");
        }

        private void ExecuteDeleteDish()
        {
            if (SelectedCatalogItem != null)
            {
                string name = SelectedCatalogItem.Title;
                CatalogCollection.Remove(SelectedCatalogItem);
                _repository.SaveDishes(CatalogCollection.ToList());
                FilterCatalog();
                Logger.Log($"Deleted dish: {name}");
            }
        }

        private void ExecuteExportReport()
        {
            try
            {
                string file = "diet_export_report.txt";
                using var sw = new StreamWriter(file);
                sw.WriteLine("=== NutriLife - Звіт з раціону ===");
                sw.WriteLine($"Користувач: {_currentUser?.Username ?? "Гість"}");
                sw.WriteLine($"Ціль: {TargetSummary} ({TargetMacros})");
                sw.WriteLine($"Фактично в раціоні: {ActualSummary} ({ActualMacros})");
                sw.WriteLine("------------------------------------------");
                foreach (var item in ActiveDietPlan)
                {
                    sw.WriteLine($"[{item.Category}] {item.Title} - {item.Calories} ккал (Б:{item.Protein}г, Ж:{item.Fat}г, В:{item.Carbs}г)");
                }
                Logger.Log("Report exported successfully.");
                ValidationError = "Успіх: Звіт збережено у diet_export_report.txt!";
            }
            catch (Exception ex)
            {
                Logger.Log($"Export failed: {ex.Message}");
                ValidationError = "Помилка збереження файлу!";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}