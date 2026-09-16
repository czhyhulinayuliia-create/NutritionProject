using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;

namespace FinalNutritionProject
{
    public class UserProfile
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public double WeightKg { get; set; } = 70;
        public double HeightCm { get; set; } = 175;
        public int Age { get; set; } = 25;
        public int GenderIndex { get; set; } = 0;
        public int ActivityIndex { get; set; } = 0;
        public int GoalIndex { get; set; } = 1;
        public string Allergies { get; set; } = "";
    }

    public class DishItem
    {
        public string Title { get; set; } = "";
        public string Category { get; set; } = "Загальне";
        public int Calories { get; set; }
        public double Protein { get; set; }
        public double Fat { get; set; }
        public double Carbs { get; set; }
        public double ServingSizeGrams { get; set; } = 100;
        public string Tags { get; set; } = "";
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        public RelayCommand(Action execute) => _execute = execute;
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => _execute();
        public event EventHandler? CanExecuteChanged;
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly string _usersFilePath = "users_data.json";
        private readonly string _dishesFilePath = "dishes_data.json";

        private List<UserProfile> _registeredUsers = new();

        private bool _isLoggedIn;
        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set { _isLoggedIn = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsLoggedOut)); }
        }
        public bool IsLoggedOut => !IsLoggedIn;

        private string _authUsername = "";
        public string AuthUsername
        {
            get => _authUsername;
            set { _authUsername = value; OnPropertyChanged(); }
        }

        private string _authPassword = "";
        public string AuthPassword
        {
            get => _authPassword;
            set { _authPassword = value; OnPropertyChanged(); }
        }

        private string _inputWeight = "70";
        public string InputWeight
        {
            get => _inputWeight;
            set { _inputWeight = value; OnPropertyChanged(); }
        }

        private string _inputHeight = "175";
        public string InputHeight
        {
            get => _inputHeight;
            set { _inputHeight = value; OnPropertyChanged(); }
        }

        private string _inputAge = "25";
        public string InputAge
        {
            get => _inputAge;
            set { _inputAge = value; OnPropertyChanged(); }
        }

        private int _selectedGenderIndex;
        public int SelectedGenderIndex
        {
            get => _selectedGenderIndex;
            set { _selectedGenderIndex = value; OnPropertyChanged(); }
        }

        private int _selectedActivityIndex;
        public int SelectedActivityIndex
        {
            get => _selectedActivityIndex;
            set { _selectedActivityIndex = value; OnPropertyChanged(); }
        }

        private int _selectedGoalIndex = 1;
        public int SelectedGoalIndex
        {
            get => _selectedGoalIndex;
            set { _selectedGoalIndex = value; OnPropertyChanged(); }
        }

        private string _inputAllergies = "";
        public string InputAllergies
        {
            get => _inputAllergies;
            set { _inputAllergies = value; OnPropertyChanged(); }
        }

        private string _calculationResultText = "Введіть свої дані та натисніть «Розрахувати добову норму».";
        public string CalculationResultText
        {
            get => _calculationResultText;
            set { _calculationResultText = value; OnPropertyChanged(); }
        }

        private ObservableCollection<DishItem> _allDishes = new();
        public ObservableCollection<DishItem> Dishes { get; set; } = new();

        private DishItem? _selectedDish;
        public DishItem? SelectedDish
        {
            get => _selectedDish;
            set { _selectedDish = value; OnPropertyChanged(); }
        }

        private string _searchQuery = "";
        public string SearchQuery
        {
            get => _searchQuery;
            set { _searchQuery = value; OnPropertyChanged(); FilterDishes(); }
        }

        public string NewDishTitle { get; set; } = "";
        public string NewDishCategory { get; set; } = "";
        public string NewDishCalories { get; set; } = "";
        public string NewDishProtein { get; set; } = "";
        public string NewDishFat { get; set; } = "";
        public string NewDishCarbs { get; set; } = "";
        public string NewDishServingSize { get; set; } = "";
        public string NewDishTags { get; set; } = "";

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand SaveProfileAndCalculateCommand { get; }
        public ICommand CreateDishCommand { get; }

        public MainViewModel()
        {
            LoginCommand = new RelayCommand(Login);
            RegisterCommand = new RelayCommand(Register);
            SaveProfileAndCalculateCommand = new RelayCommand(CalculateNorms);
            CreateDishCommand = new RelayCommand(CreateDish);

            LoadUsersData();
            LoadDishesData();
        }

        private void Login()
        {
            var user = _registeredUsers.FirstOrDefault(u => u.Username == AuthUsername && u.Password == AuthPassword);
            if (user != null)
            {
                InputWeight = user.WeightKg.ToString();
                InputHeight = user.HeightCm.ToString();
                InputAge = user.Age.ToString();
                SelectedGenderIndex = user.GenderIndex;
                SelectedActivityIndex = user.ActivityIndex;
                SelectedGoalIndex = user.GoalIndex;
                InputAllergies = user.Allergies;

                IsLoggedIn = true;
                CalculateNorms();
            }
            else
            {
                CalculationResultText = "Неправильний логін або пароль!";
            }
        }

        private void Register()
        {
            if (string.IsNullOrWhiteSpace(AuthUsername) || string.IsNullOrWhiteSpace(AuthPassword)) return;

            if (_registeredUsers.Any(u => u.Username == AuthUsername)) return;

            var newUser = new UserProfile { Username = AuthUsername, Password = AuthPassword };
            _registeredUsers.Add(newUser);
            SaveUsersData();
            IsLoggedIn = true;
        }

        private void CalculateNorms()
        {
            if (!double.TryParse(InputWeight, out double weight) ||
                !double.TryParse(InputHeight, out double height) ||
                !int.TryParse(InputAge, out int age)) return;

            double bmr = (SelectedGenderIndex == 0)
                ? (10 * weight) + (6.25 * height) - (5 * age) + 5
                : (10 * weight) + (6.25 * height) - (5 * age) - 161;

            double[] mults = { 1.2, 1.375, 1.55, 1.725 };
            double tdee = bmr * mults[Math.Clamp(SelectedActivityIndex, 0, 3)];

            if (SelectedGoalIndex == 0) tdee *= 0.85;
            else if (SelectedGoalIndex == 2) tdee *= 1.15;

            int calories = (int)Math.Round(tdee);
            double protein = Math.Round(weight * 2, 1);
            double fat = Math.Round(weight * 1, 1);
            double carbs = Math.Round((calories - (protein * 4 + fat * 9)) / 4, 1);

            CalculationResultText = $"Ваша добова норма:\n• Калорії: {calories} ккал\n• Білки: {protein} г\n• Жири: {fat} г\n• Вуглеводи: {carbs} г";
        }

        private void CreateDish()
        {
            if (string.IsNullOrWhiteSpace(NewDishTitle)) return;

            int.TryParse(NewDishCalories, out int cal);
            double.TryParse(NewDishProtein, out double p);
            double.TryParse(NewDishFat, out double f);
            double.TryParse(NewDishCarbs, out double c);
            double.TryParse(NewDishServingSize, out double s);

            var dish = new DishItem
            {
                Title = NewDishTitle,
                Category = string.IsNullOrWhiteSpace(NewDishCategory) ? "Загальне" : NewDishCategory,
                Calories = cal,
                Protein = p,
                Fat = f,
                Carbs = c,
                ServingSizeGrams = s <= 0 ? 100 : s,
                Tags = NewDishTags
            };

            _allDishes.Add(dish);
            FilterDishes();
            SaveDishesData();

            // Очищення полів
            NewDishTitle = "";
            NewDishCategory = "";
            NewDishCalories = "";
            NewDishProtein = "";
            NewDishFat = "";
            NewDishCarbs = "";
            NewDishServingSize = "";
            NewDishTags = "";
            OnPropertyChanged(nameof(NewDishTitle));
            OnPropertyChanged(nameof(NewDishCategory));
            OnPropertyChanged(nameof(NewDishCalories));
            OnPropertyChanged(nameof(NewDishProtein));
            OnPropertyChanged(nameof(NewDishFat));
            OnPropertyChanged(nameof(NewDishCarbs));
            OnPropertyChanged(nameof(NewDishServingSize));
            OnPropertyChanged(nameof(NewDishTags));
        }

        private void FilterDishes()
        {
            Dishes.Clear();
            var filtered = string.IsNullOrWhiteSpace(SearchQuery)
                ? _allDishes
                : _allDishes.Where(d => d.Title.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                                        d.Tags.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));
            foreach (var item in filtered) Dishes.Add(item);
        }

        private void LoadUsersData()
        {
            if (File.Exists(_usersFilePath))
            {
                try { _registeredUsers = JsonSerializer.Deserialize<List<UserProfile>>(File.ReadAllText(_usersFilePath)) ?? new(); }
                catch { _registeredUsers = new(); }
            }
        }

        private void SaveUsersData()
        {
            File.WriteAllText(_usersFilePath, JsonSerializer.Serialize(_registeredUsers));
        }

        private void LoadDishesData()
        {
            if (File.Exists(_dishesFilePath))
            {
                try
                {
                    var items = JsonSerializer.Deserialize<List<DishItem>>(File.ReadAllText(_dishesFilePath));
                    if (items != null && items.Count > 0)
                    {
                        _allDishes = new ObservableCollection<DishItem>(items);
                        FilterDishes();
                        return;
                    }
                }
                catch { }
            }

            // Початкова база з 10 традиційних страв
            _allDishes = new ObservableCollection<DishItem>
            {
                new DishItem { Title = "Український борщ із яловичиною", Category = "Обід", Calories = 235, Protein = 12.5, Fat = 9.0, Carbs = 26.0, ServingSizeGrams = 350, Tags = "суп, борщ, м'ясо, овочі" },
                new DishItem { Title = "Вівсяна каша з бананом та ягодами", Category = "Ранок", Calories = 280, Protein = 8.5, Fat = 4.5, Carbs = 51.0, ServingSizeGrams = 250, Tags = "сніданок, каша, фрукти, корисне" },
                new DishItem { Title = "Запечена куряча грудка з гречкою", Category = "Обід", Calories = 410, Protein = 38.0, Fat = 6.5, Carbs = 48.0, ServingSizeGrams = 300, Tags = "фітнес, білок, курка, гречка" },
                new DishItem { Title = "Сирники з знежиреного сиру з медом", Category = "Ранок", Calories = 320, Protein = 24.0, Fat = 7.0, Carbs = 39.0, ServingSizeGrams = 200, Tags = "десерт, сир, сніданок, білок" },
                new DishItem { Title = "Вареники з картоплею та цибулею", Category = "Обід", Calories = 360, Protein = 9.0, Fat = 8.5, Carbs = 62.0, ServingSizeGrams = 250, Tags = "вареники, традиційне, обід" },
                new DishItem { Title = "Овочевий салат з грецьким сиром та оливковою олією", Category = "Перекус", Calories = 195, Protein = 6.0, Fat = 15.0, Carbs = 9.0, ServingSizeGrams = 200, Tags = "салат, овочі, легке, перекус" },
                new DishItem { Title = "Запечений лосось із брокколі", Category = "Вечеря", Calories = 450, Protein = 34.0, Fat = 26.0, Carbs = 10.0, ServingSizeGrams = 280, Tags = "риба, омега3, вечеря, здорове" },
                new DishItem { Title = "Омлет із трьох яєць із шпинатом", Category = "Ранок", Calories = 260, Protein = 19.0, Fat = 18.0, Carbs = 3.5, ServingSizeGrams = 180, Tags = "яйця, сніданок, протеїн" },
                new DishItem { Title = "Картопляне пюре з котлетою по-київськи", Category = "Обід", Calories = 520, Protein = 26.0, Fat = 24.0, Carbs = 49.0, ServingSizeGrams = 320, Tags = "курка, картопля, ситне" },
                new DishItem { Title = "Грецький йогурт із горіхами та медом", Category = "Перекус", Calories = 210, Protein = 14.0, Fat = 10.0, Carbs = 16.0, ServingSizeGrams = 150, Tags = "йогурт, перекус, десерт" }
            };

            FilterDishes();
            SaveDishesData();
        }

        private void SaveDishesData()
        {
            File.WriteAllText(_dishesFilePath, JsonSerializer.Serialize(_allDishes));
        }
    }
}