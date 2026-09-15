using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace FinalNutritionProject
{
    public class DisplayDish
    {
        public string Title { get; set; } = "";
        public int EnergyValue { get; set; }
        public string GroupLabel { get; set; } = "";
        public string Tags { get; set; } = "";
    }

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
        private string _systemStatus = "Доступ: Обмежено";
        private string _authLogin = "";
        private string _authToken = "";
        private string _userWeight = "80";
        private string _userHeight = "180";
        private string _userAge = "30";
        private int _activityRatingIndex = 1;
        private int _strategyTargetIndex = 1;
        private string _stopWordsInput = "";
        private string _energySummaryText = "0 ккал";
        private string _distributionSummaryText = "Б:0г Ж:0г В:0г";
        private string _formTitle = "";
        private string _formEnergy = "";
        private int _formTypeIndex = 0;
        private string _formTags = "";
        private string _filterQuery = "";
        private DisplayDish? _targetSelectedDish;

        public ObservableCollection<DisplayDish> GlobalDishesDatabase { get; set; } = new();
        public ObservableCollection<DisplayDish> ActiveDietCollection { get; set; } = new();

        public string SystemStatus { get => _systemStatus; set => SetField(ref _systemStatus, value); }
        public string AuthLogin { get => _authLogin; set => SetField(ref _authLogin, value); }
        public string AuthToken { get => _authToken; set => SetField(ref _authToken, value); }
        public string UserWeight { get => _userWeight; set => SetField(ref _userWeight, value); }
        public string UserHeight { get => _userHeight; set => SetField(ref _userHeight, value); }
        public string UserAge { get => _userAge; set => SetField(ref _userAge, value); }
        public int ActivityRatingIndex { get => _activityRatingIndex; set => SetField(ref _activityRatingIndex, value); }
        public int StrategyTargetIndex { get => _strategyTargetIndex; set => SetField(ref _strategyTargetIndex, value); }
        public string StopWordsInput { get => _stopWordsInput; set => SetField(ref _stopWordsInput, value); }
        public string EnergySummaryText { get => _energySummaryText; set => SetField(ref _energySummaryText, value); }
        public string DistributionSummaryText { get => _distributionSummaryText; set => SetField(ref _distributionSummaryText, value); }
        public string FormTitle { get => _formTitle; set => SetField(ref _formTitle, value); }
        public string FormEnergy { get => _formEnergy; set => SetField(ref _formEnergy, value); }
        public int FormTypeIndex { get => _formTypeIndex; set => SetField(ref _formTypeIndex, value); }
        public string FormTags { get => _formTags; set => SetField(ref _formTags, value); }
        public string FilterQuery
        {
            get => _filterQuery;
            set
            {
                SetField(ref _filterQuery, value);
                ApplyFilter();
            }
        }
        public DisplayDish? TargetSelectedDish { get => _targetSelectedDish; set => SetField(ref _targetSelectedDish, value); }

        public ICommand ProcessAuthCommand { get; }
        public ICommand BuildDietStructureCommand { get; }
        public ICommand FileExportCommand { get; }
        public ICommand RemoveDishCommand { get; }
        public ICommand CreateDishCommand { get; }

        public MainDashboardViewModel()
        {
            ProcessAuthCommand = new RelayCommand(ExecuteAuth);
            BuildDietStructureCommand = new RelayCommand(ExecuteCalculate);
            FileExportCommand = new RelayCommand(ExecuteExport);
            RemoveDishCommand = new RelayCommand(ExecuteRemoveDish);
            CreateDishCommand = new RelayCommand(ExecuteCreateDish);

            SeedDatabase();
            ExecuteCalculate();
        }

        private void SeedDatabase()
        {
            GlobalDishesDatabase.Clear();
            var items = new List<DisplayDish>
            {
                new DisplayDish { Title = "Сніданок: Сирники з медом та сметаною", EnergyValue = 520, GroupLabel = "Ранок", Tags = "сирники, кисломолочний сир, лактоза" },
                new DisplayDish { Title = "Сніданок: Вівсяна каша з ягодами та мигдалем", EnergyValue = 420, GroupLabel = "Ранок", Tags = "вівсянка, каша, горіхи" },
                new DisplayDish { Title = "Сніданок: Омлет із трьох яєць з томатами та зеленню", EnergyValue = 480, GroupLabel = "Ранок", Tags = "омлет, яйця, овочі" },
                new DisplayDish { Title = "Сніданок: Авокадо-тост зі слабосолоним лососем", EnergyValue = 510, GroupLabel = "Ранок", Tags = "авокадо, тост, риба" },
                new DisplayDish { Title = "Сніданок: Млинці з яблуками та корицею", EnergyValue = 490, GroupLabel = "Ранок", Tags = "млинці, фрукти, яблуко" },
                new DisplayDish { Title = "Сніданок: Гранола з грецьким йогуртом", EnergyValue = 430, GroupLabel = "Ранок", Tags = "гранола, йогурт, горіхи" },
                new DisplayDish { Title = "Сніданок: Яєчня з беконом та червоною квасолею", EnergyValue = 580, GroupLabel = "Ранок", Tags = "яєчня, бекон, квасоля" },
                new DisplayDish { Title = "Сніданок: Рисова каша на кокосовому молоці з манго", EnergyValue = 460, GroupLabel = "Ранок", Tags = "каша, рис, манго" },
                new DisplayDish { Title = "Сніданок: Шакшука з соковитими томатами та солодким перцем", EnergyValue = 500, GroupLabel = "Ранок", Tags = "шакшука, яйця, перець" },
                new DisplayDish { Title = "Сніданок: Сендвіч із індичкою, сиром та листям салату", EnergyValue = 470, GroupLabel = "Ранок", Tags = "сендвіч, індичка, сир" },

                new DisplayDish { Title = "Обід: Борщ український з яловичиною та пампушками", EnergyValue = 580, GroupLabel = "Обід", Tags = "борщ, суп, яловичина" },
                new DisplayDish { Title = "Обід: Курячий суп із локшиною та зеленню", EnergyValue = 440, GroupLabel = "Обід", Tags = "суп, курятина, локшина" },
                new DisplayDish { Title = "Обід: Крем-суп із печериць з вершками та грінками", EnergyValue = 420, GroupLabel = "Обід", Tags = "суп, гриби, вершки" },
                new DisplayDish { Title = "Обід: Гречана каша з соковитою телячою котлетою", EnergyValue = 610, GroupLabel = "Обід", Tags = "гречка, котлета, телятина" },
                new DisplayDish { Title = "Обід: Стейк із лосося на пару з диким рисом", EnergyValue = 680, GroupLabel = "Обід", Tags = "лосось, риба, рис" },
                new DisplayDish { Title = "Обід: Паста Болоньєзе з соковитим фаршем та пармезаном", EnergyValue = 720, GroupLabel = "Обід", Tags = "паста, фарш, пармезан" },
                new DisplayDish { Title = "Обід: Боул із філе індички, кіноа та авокадо", EnergyValue = 620, GroupLabel = "Обід", Tags = "боул, індичка, кіноа" },

                new DisplayDish { Title = "Перекус: Протеїновий коктейль із бананом та молоком", EnergyValue = 300, GroupLabel = "Перекус", Tags = "протеїн, банан, молоко" },
                new DisplayDish { Title = "Перекус: Жменя мигдалю, кеш'ю та свіже яблуко", EnergyValue = 270, GroupLabel = "Перекус", Tags = "горіхи, яблуко, кеш'ю" },
                new DisplayDish { Title = "Перекус: Сендвіч із тунцем, огірком та салатом", EnergyValue = 350, GroupLabel = "Перекус", Tags = "тунець, сендвіч, огірок" },
                new DisplayDish { Title = "Перекус: Кисломолочний сир із соковитою лохиною", EnergyValue = 260, GroupLabel = "Перекус", Tags = "сир, ягоди, лохина" },
                new DisplayDish { Title = "Перекус: Запечене яблуко з корицею, медом та горіхами", EnergyValue = 230, GroupLabel = "Перекус", Tags = "яблуко, кориця, десерт" },

                new DisplayDish { Title = "Вечеря: Запечена тріска з броколі та цвітною капустою", EnergyValue = 390, GroupLabel = "Вечір", Tags = "тріска, броколі, риба" },
                new DisplayDish { Title = "Вечеря: Салат Цезар із тигровими креветками", EnergyValue = 450, GroupLabel = "Вечір", Tags = "салат, креветки, морепродукти" },
                new DisplayDish { Title = "Вечеря: Куряче філе на грилі зі спаржею та лимоном", EnergyValue = 460, GroupLabel = "Вечір", Tags = "курка, спаржа, гриль" },
                new DisplayDish { Title = "Вечеря: Філе індички з тушкованими кабачками та томатами", EnergyValue = 480, GroupLabel = "Вечір", Tags = "індичка, кабачки, овочі" },
                new DisplayDish { Title = "Вечеря: Соковитий стейк із тунця з мікс-салатом", EnergyValue = 430, GroupLabel = "Вечір", Tags = "тунець, салат, риба" }
            };

            foreach (var item in items)
            {
                GlobalDishesDatabase.Add(item);
            }
        }

        private void ExecuteAuth()
        {
            if (!string.IsNullOrWhiteSpace(AuthLogin) && !string.IsNullOrWhiteSpace(AuthToken))
            {
                SystemStatus = $"Доступ: VIP ({AuthLogin})";
            }
            else
            {
                SystemStatus = "Доступ: Обмежено";
            }
        }

        private void ExecuteCalculate()
        {
            double.TryParse(UserWeight, out double w);
            double.TryParse(UserHeight, out double h);
            double.TryParse(UserAge, out double a);

            if (w <= 0) w = 80;
            if (h <= 0) h = 180;
            if (a <= 0) a = 30;

            double bmr = 10 * w + 6.25 * h - 5 * a + 5;

            double actMult = ActivityRatingIndex switch
            {
                0 => 1.2,
                1 => 1.375,
                2 => 1.55,
                _ => 1.375
            };

            double tdee = bmr * actMult;

            double targetCalories = StrategyTargetIndex switch
            {
                0 => tdee - 500,
                1 => tdee,
                2 => tdee + 400,
                _ => tdee
            };

            int finalCal = (int)Math.Round(targetCalories);
            EnergySummaryText = $"{finalCal} ккал";

            int p = (int)Math.Round((finalCal * 0.30) / 4);
            int f = (int)Math.Round((finalCal * 0.30) / 9);
            int c = (int)Math.Round((finalCal * 0.40) / 4);

            DistributionSummaryText = $"Б:{p}г Ж:{f}г В:{c}г";

            GenerateMenuForTarget(finalCal);
        }

        private void GenerateMenuForTarget(int targetCalories)
        {
            ActiveDietCollection.Clear();

            var stops = StopWordsInput.ToLower().Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var available = GlobalDishesDatabase.Where(d =>
            {
                string tagTitle = (d.Title + " " + d.Tags).ToLower();
                return !stops.Any(s => tagTitle.Contains(s));
            }).ToList();

            if (!available.Any()) return;

            var rand = new Random();

            var breakfastList = available.Where(x => x.GroupLabel == "Ранок").OrderBy(_ => rand.Next()).ToList();
            var lunchList = available.Where(x => x.GroupLabel == "Обід").OrderBy(_ => rand.Next()).ToList();
            var snackList = available.Where(x => x.GroupLabel == "Перекус").OrderBy(_ => rand.Next()).ToList();
            var dinnerList = available.Where(x => x.GroupLabel == "Вечір").OrderBy(_ => rand.Next()).ToList();

            var breakfast = breakfastList.FirstOrDefault() ?? available[0];
            ActiveDietCollection.Add(breakfast);

            bool IsSimilar(DisplayDish candidate, IEnumerable<DisplayDish> currentList)
            {
                foreach (var item in currentList)
                {
                    var words1 = item.Title.ToLower().Split(new[] { ' ', ':', ',', '-' }, StringSplitOptions.RemoveEmptyEntries);
                    var words2 = candidate.Title.ToLower().Split(new[] { ' ', ':', ',', '-' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var w in words1)
                    {
                        if (w.Length > 3 && w != "сніданок" && w != "обід" && w != "вечеря" && w != "перекус" && words2.Contains(w))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            var lunch = lunchList.FirstOrDefault(l => !IsSimilar(l, ActiveDietCollection)) ?? lunchList.FirstOrDefault() ?? available[0];
            ActiveDietCollection.Add(lunch);

            var firstSnack = snackList.FirstOrDefault(s => !IsSimilar(s, ActiveDietCollection)) ?? snackList.FirstOrDefault();
            if (firstSnack != null)
            {
                ActiveDietCollection.Add(firstSnack);
            }

            if (targetCalories >= 2500)
            {
                var secondSnack = snackList.FirstOrDefault(s => !ActiveDietCollection.Contains(s) && !IsSimilar(s, ActiveDietCollection));
                if (secondSnack != null)
                {
                    ActiveDietCollection.Add(secondSnack);
                }
            }

            var dinner = dinnerList.FirstOrDefault(d => !IsSimilar(d, ActiveDietCollection)) ?? dinnerList.FirstOrDefault() ?? available[0];
            ActiveDietCollection.Add(dinner);
        }

        private void ExecuteExport()
        {
            try
            {
                using var sw = new StreamWriter("diet_export.txt");
                sw.WriteLine("=== NutriLife Diet Export ===");
                sw.WriteLine($"Норма: {EnergySummaryText}");
                sw.WriteLine($"Нутрієнти: {DistributionSummaryText}");
                sw.WriteLine("-----------------------------");
                foreach (var dish in ActiveDietCollection)
                {
                    sw.WriteLine($"[{dish.GroupLabel}] {dish.Title} - {dish.EnergyValue} ккал ({dish.Tags})");
                }
            }
            catch { }
        }

        private void ExecuteRemoveDish()
        {
            if (TargetSelectedDish != null)
            {
                ActiveDietCollection.Remove(TargetSelectedDish);
            }
        }

        private void ExecuteCreateDish()
        {
            if (string.IsNullOrWhiteSpace(FormTitle)) return;
            int.TryParse(FormEnergy, out int kcal);
            if (kcal <= 0) kcal = 300;

            string grp = FormTypeIndex switch
            {
                0 => "Ранок",
                1 => "Обід",
                2 => "Вечір",
                _ => "Обід"
            };

            var newDish = new DisplayDish
            {
                Title = FormTitle,
                EnergyValue = kcal,
                GroupLabel = grp,
                Tags = FormTags
            };

            GlobalDishesDatabase.Add(newDish);
            FormTitle = "";
            FormEnergy = "";
            FormTags = "";
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(FilterQuery)) return;

            var match = GlobalDishesDatabase
                .FirstOrDefault(d => d.Title.ToLower().Contains(FilterQuery.ToLower()));

            if (match != null && !ActiveDietCollection.Contains(match))
            {
                ActiveDietCollection.Add(match);
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