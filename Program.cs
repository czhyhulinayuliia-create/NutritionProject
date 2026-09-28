using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Themes.Simple;
using System;

namespace DietAppWorking
{
    public class App : Application
    {
        public override void Initialize()
        {
            Styles.Add(new SimpleTheme());
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = CreateMainWindow();
            }
            base.OnFrameworkInitializationCompleted();
        }

        private Window CreateMainWindow()
        {
            var vm = new MainViewModel();
            var window = new Window
            {
                Title = "Diet Tracker Pro",
                Width = 900,
                Height = 600,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                DataContext = vm
            };

            var rootGrid = new Grid { Background = Brush.Parse("#0F172A") };

            // Авторизация
            var authBorder = new Border
            {
                Background = Brush.Parse("#1E293B"),
                CornerRadius = new CornerRadius(16),
                Padding = new Thickness(35),
                Width = 400,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                [!Visual.IsVisibleProperty] = new Binding("IsLoggedOut")
            };

            var authStack = new StackPanel { Spacing = 12 };
            authStack.Children.Add(new TextBlock { Text = "Diet Tracker Pro", Foreground = Brush.Parse("#10B981"), FontSize = 24, FontWeight = FontWeight.Bold, HorizontalAlignment = HorizontalAlignment.Center });
            authStack.Children.Add(new TextBlock { Text = "Авторизация", Foreground = Brush.Parse("#94A3B8"), HorizontalAlignment = HorizontalAlignment.Center });

            var txtUser = new TextBox();
            txtUser.Bind(TextBox.TextProperty, new Binding("Username"));
            authStack.Children.Add(new TextBlock { Text = "Имя пользователя", Foreground = Brush.Parse("#CBD5E1") });
            authStack.Children.Add(txtUser);

            var txtPass = new TextBox { PasswordChar = '*' };
            txtPass.Bind(TextBox.TextProperty, new Binding("Password"));
            authStack.Children.Add(new TextBlock { Text = "Пароль", Foreground = Brush.Parse("#CBD5E1") });
            authStack.Children.Add(txtPass);

            var btnAuth = new Button
            {
                Background = Brush.Parse("#10B981"),
                Foreground = Brushes.White,
                FontWeight = FontWeight.Bold,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Padding = new Thickness(12),
                CornerRadius = new CornerRadius(8),
                Command = new DelegateCommand(() => vm.AuthAction())
            };
            btnAuth.Bind(ContentControl.ContentProperty, new Binding("AuthBtnText"));
            authStack.Children.Add(btnAuth);

            var btnToggle = new Button
            {
                Background = Brushes.Transparent,
                Foreground = Brush.Parse("#38BDF8"),
                HorizontalAlignment = HorizontalAlignment.Center,
                Command = new DelegateCommand(() => vm.ToggleAuthMode())
            };
            btnToggle.Bind(ContentControl.ContentProperty, new Binding("ToggleAuthModeText"));
            authStack.Children.Add(btnToggle);

            authBorder.Child = authStack;
            rootGrid.Children.Add(authBorder);

            
            var mainGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions("240,*"),
                [!Visual.IsVisibleProperty] = new Binding("IsLoggedIn")
            };

            var sidebar = new Border { Background = Brush.Parse("#1E293B"), Padding = new Thickness(20) };
            Grid.SetColumn(sidebar, 0);

            var sideStack = new StackPanel { Spacing = 10 };
            sideStack.Children.Add(new TextBlock { Text = "🥑 Diet Tracker", Foreground = Brush.Parse("#10B981"), FontSize = 20, FontWeight = FontWeight.Bold });

            Action<string, Action> addNav = (title, act) =>
            {
                var b = new Button
                {
                    Content = title,
                    Background = Brush.Parse("#334155"),
                    Foreground = Brushes.White,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Padding = new Thickness(10),
                    CornerRadius = new CornerRadius(8),
                    Command = new DelegateCommand(act)
                };
                sideStack.Children.Add(b);
            };

            addNav("📊 Обзор", () => vm.SelectTab0());
            addNav("🍽️ Дневник", () => vm.SelectTab1());
            addNav("💧 Вода", () => vm.SelectTab2());
            addNav("⚙️ Профиль", () => vm.SelectTab3());

            var btnLogout = new Button
            {
                Content = "Выйти",
                Background = Brush.Parse("#EF4444"),
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Padding = new Thickness(8),
                Margin = new Thickness(0, 20, 0, 0),
                CornerRadius = new CornerRadius(8),
                Command = new DelegateCommand(() => vm.Logout())
            };
            sideStack.Children.Add(btnLogout);
            sidebar.Child = sideStack;
            mainGrid.Children.Add(sidebar);

            
            var contentArea = new Grid { Background = Brush.Parse("#0F172A") };
            Grid.SetColumn(contentArea, 1);

            var infoStack = new StackPanel { Padding = new Thickness(30), Spacing = 15 };
            
            var calText = new TextBlock { Foreground = Brush.Parse("#10B981"), FontSize = 22, FontWeight = FontWeight.Bold };
            calText.Bind(TextBlock.TextProperty, new Binding("CalorieProgressText"));

            var protText = new TextBlock { Foreground = Brushes.White, FontSize = 16 };
            protText.Bind(TextBlock.TextProperty, new Binding("ProteinText"));

            var fatText = new TextBlock { Foreground = Brushes.White, FontSize = 16 };
            fatText.Bind(TextBlock.TextProperty, new Binding("FatText"));

            var carbText = new TextBlock { Foreground = Brushes.White, FontSize = 16 };
            carbText.Bind(TextBlock.TextProperty, new Binding("CarbsText"));

            infoStack.Children.Add(new TextBlock { Text = "Сводка за сегодня", FontSize = 24, FontWeight = FontWeight.Bold, Foreground = Brushes.White });
            infoStack.Children.Add(calText);
            infoStack.Children.Add(protText);
            infoStack.Children.Add(fatText);
            infoStack.Children.Add(carbText);

            contentArea.Children.Add(infoStack);
            mainGrid.Children.Add(contentArea);

            rootGrid.Children.Add(mainGrid);
            window.Content = rootGrid;
            return window;
        }
    }

    public class DelegateCommand : System.Windows.Input.ICommand
    {
        private readonly Action _execute;
        public DelegateCommand(Action execute) => _execute = execute;
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => _execute();
        public event EventHandler? CanExecuteChanged { add { } remove { } }
    }

    internal class Program
    {
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();
    }
}