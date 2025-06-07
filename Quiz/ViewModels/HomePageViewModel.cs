using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Quiz.Models;
using Quiz.Services;
using Quiz.Views;
using Microsoft.Maui.Controls;

namespace Quiz.ViewModels
{
    public class HomePageViewModel : INotifyPropertyChanged
    {
        private string _greeting;
        private string _selectedDifficulty = "VeryEasy";
        private ObservableCollection<QuizCategory> _allCategories;
        private ObservableCollection<QuizCategory> _filteredCategories;
        private QuizCategory _selectedCategory;
        private bool _isPopupVisible;
        private double _popupWidth;
        private double _popupHeight;

        public string Greeting
        {
            get => _greeting;
            set
            {
                if (_greeting != value)
                {
                    _greeting = value;
                    OnPropertyChanged(nameof(Greeting));
                }
            }
        }

        public string SelectedDifficulty
        {
            get => _selectedDifficulty;
            set
            {
                if (_selectedDifficulty != value)
                {
                    _selectedDifficulty = value;
                    OnPropertyChanged(nameof(SelectedDifficulty));
                    FilterCategoriesByDifficulty();
                }
            }
        }

        public ObservableCollection<QuizCategory> AllCategories
        {
            get => _allCategories;
            set
            {
                _allCategories = value;
                OnPropertyChanged(nameof(AllCategories));
            }
        }

        public ObservableCollection<QuizCategory> FilteredCategories
        {
            get => _filteredCategories;
            set
            {
                _filteredCategories = value;
                OnPropertyChanged(nameof(FilteredCategories));
            }
        }

        public QuizCategory SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }

        public bool IsPopupVisible
        {
            get => _isPopupVisible;
            set
            {
                _isPopupVisible = value;
                OnPropertyChanged(nameof(IsPopupVisible));
            }
        }

        public double PopupWidth
        {
            get => _popupWidth;
            set
            {
                _popupWidth = value;
                OnPropertyChanged(nameof(PopupWidth));
            }
        }

        public double PopupHeight
        {
            get => _popupHeight;
            set
            {
                _popupHeight = value;
                OnPropertyChanged(nameof(PopupHeight));
            }
        }

        public ICommand SelectCategoryCommand { get; }
        public ICommand ClosePopupCommand { get; }
        public ICommand StartQuizCommand { get; }

        public HomePageViewModel()
        {
            // Initialize collections
            AllCategories = new ObservableCollection<QuizCategory>();
            FilteredCategories = new ObservableCollection<QuizCategory>();

            // Initialize commands
            SelectCategoryCommand = new Command<QuizCategory>(OnCategorySelected);
            ClosePopupCommand = new Command(OnClosePopup);
            StartQuizCommand = new Command(OnStartQuiz);

            // Calculate popup size (50% of screen)
            var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
            PopupWidth = displayInfo.Width / displayInfo.Density * 0.5;
            PopupHeight = displayInfo.Height / displayInfo.Density * 0.5;

            // Load user greeting
            var person = UserPreferences.LoadPerson();
            if (person != null && !string.IsNullOrEmpty(person.Name))
                Greeting = $"Welcome Back {person.Name}";
            else
                Greeting = "Welcome Back";

            // Load categories
            Task.Run(async () => await LoadCategoriesAsync());
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                FirebaseServices.Init();
                var categories = await FirebaseServices.GetCategoriesByDifficultyAsync(_selectedDifficulty);

                await Application.Current.Dispatcher.DispatchAsync(() =>
                {
                    AllCategories.Clear();
                    foreach (var category in categories)
                    {
                        AllCategories.Add(category);
                    }
                    FilterCategoriesByDifficulty();
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading categories: {ex.Message}");
            }
        }

        private void FilterCategoriesByDifficulty()
        {
            Task.Run(async () => await LoadCategoriesByDifficultyAsync());
        }

        private async Task LoadCategoriesByDifficultyAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Loading categories for difficulty: {_selectedDifficulty}");

                var categories = await FirebaseServices.GetCategoriesByDifficultyAsync(_selectedDifficulty);

                System.Diagnostics.Debug.WriteLine($"Received {categories?.Count ?? 0} categories from Firebase");

                await Application.Current.Dispatcher.DispatchAsync(() =>
                {
                    FilteredCategories.Clear();
                    if (categories != null)
                    {
                        foreach (var category in categories)
                        {
                            System.Diagnostics.Debug.WriteLine($"Adding category: {category.MainTopic} - {category.SubTopic}");
                            FilteredCategories.Add(category);
                        }
                    }
                    System.Diagnostics.Debug.WriteLine($"FilteredCategories now contains {FilteredCategories.Count} items");
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error filtering categories: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        public async Task SearchCategoriesAsync(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                FilterCategoriesByDifficulty();
                return;
            }

            var searchLower = searchText.ToLower();
            var filtered = AllCategories.Where(c =>
                c.MainTopic.ToLower().Contains(searchLower) ||
                c.SubTopic.ToLower().Contains(searchLower) ||
                c.Description.ToLower().Contains(searchLower)
            ).ToList();

            FilteredCategories.Clear();
            foreach (var category in filtered)
            {
                FilteredCategories.Add(category);
            }
        }

        private void OnCategorySelected(QuizCategory category)
        {
            if (category == null) return;

            SelectedCategory = category;
            IsPopupVisible = true;
        }

        private void OnClosePopup()
        {
            IsPopupVisible = false;
            SelectedCategory = null;
        }

        private async void OnStartQuiz()
        {
            if (SelectedCategory == null) return;

            // Close popup first
            IsPopupVisible = false;

            // Navigate to quiz page with parameters
            await Shell.Current.GoToAsync($"QuizPage?categoryId={SelectedCategory.CategoryId}&totalQuestions={SelectedCategory.TotalQuestions}");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}