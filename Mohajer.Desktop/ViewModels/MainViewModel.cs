using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using Mohajer.Core;
using Mohajer.Core.Models;
using Mohajer.Core.Repositories;
using Mohajer.Desktop.Dialogs;
using System;
using System.Threading.Tasks;

namespace Mohajer.Desktop.ViewModels
{
    internal class MainViewModel : Conductor<Screen>.Collection.AllActive, IHandle<NavigationEnum>
    {
        private IScreen _rightScreen;
        public IScreen RightScreen { get => _rightScreen; set => Set(ref _rightScreen, value); }

        public IScreen LeftScreen { get; set; }

        private InfoViewModel _studentInfoViewModel;
        private FoodTableViewModel _foodTableViewModel;
        private Lazy<LogListViewModel> _logListViewModel;

        private IFoodController _foodController;
        private IFoodRepository _foodRepository;
        private IEventAggregator _eventAggregator;
        private ISettings _settings;
        private IActiveStudent _activeStudent;

        public MainViewModel(IFoodRepository foodRepository, Lazy<LogListViewModel> logListViewModel, IFoodController studentManager, IActiveStudent activateStudent, ISettings settings, InfoViewModel studentInfoViewModel, FoodTableViewModel foodTableViewModel, IEventAggregator eventAggregator)
        {
            _foodRepository = foodRepository;
            _logListViewModel = logListViewModel;
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            _foodController = studentManager;
            _settings = settings;
            _activeStudent = activateStudent;

            _studentInfoViewModel = studentInfoViewModel;
            _foodTableViewModel = foodTableViewModel;

            ActivateItem(_foodTableViewModel);
            RightScreen = _foodTableViewModel;

            ActivateItem(_studentInfoViewModel);
            LeftScreen = _studentInfoViewModel;
        }

        protected override void OnViewLoaded(object view)
        {
            Preparation();
        }

        public async void Preparation(DialogSession session = null)
        {
            bool balance = false, price = false, food = false;
            do
            {
                try
                {
                    await DialogHost.Show(new BusyProgressView(), async (object s, DialogOpenedEventArgs e) =>
                    {
                        balance = await GetBalance();
                        price = await GetPrices();
                        food = await UpdateFoods();

                        e.Session.Close();
                    });

                    if (balance && price && food)
                    {
                        await _eventAggregator.PublishOnUIThreadAsync(new object());
                        return;
                    }
                    else
                    {
                        await DialogHost.Show(new ConnectionErrorView());
                    }
                }
                catch (Exception)
                {
                    return;
                }

            } while (!(balance && price && food));
        }

        private async Task<bool> GetBalance()
        {
            var result = await _foodController.GetBalanceAsync();

            if (result.ResultStatus == ResultType.Successful)
            {
                _settings.Balance = result.Content;
                _settings.Save();

                return true;
            }

            return false;
        }

        private async Task<bool> GetPrices()
        {
            if (_settings.HighFood == 0 || _settings.NormalFood == 0 || _settings.LowFood == 0)
            {
                var result = await _foodController.GetPricesAsync();

                if (result.ResultStatus == ResultType.Successful)
                {
                    _settings.HighFood = result.Content[MealCost.High];
                    _settings.NormalFood = result.Content[MealCost.Normal];
                    _settings.LowFood = result.Content[MealCost.Low];

                    _settings.Save();

                    return true;
                }

                return false;
            }

            return true;
        }

        private async Task<bool> UpdateFoods()
        {
            var response = await _foodController.GetFoods();

            if (response.ResultStatus == ResultType.Successful)
            {
                var foods = response.Content;

                _activeStudent.InsertFood(foods);

                return true;
            }

            return false;
        }

        public void Handle(NavigationEnum message)
        {
            switch (message)
            {
                case NavigationEnum.Home:
                    {
                        RightScreen = _foodTableViewModel;
                        break;
                    }

                case NavigationEnum.LogList:
                    {
                        var value = _logListViewModel.Value;

                        ActivateItem(value);
                        RightScreen = value;
                        break;
                    }

                case NavigationEnum.Refresh:
                    Preparation();
                    break;

                default:
                    break;
            }
        }
    }
}