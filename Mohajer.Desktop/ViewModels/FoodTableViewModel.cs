using Caliburn.Micro;
using MD.PersianDateTime;
using Mohajer.Core;
using Mohajer.Core.Models;
using Mohajer.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Mohajer.Desktop.ViewModels
{
    public partial class FoodTableViewModel : Screen, IHandle<object>
    {
        public ObservableCollection<SelectableFoodViewModel> Foods { get; set; } = new ObservableCollection<SelectableFoodViewModel>();

        private ISettings _settings;
        private IEventAggregator _eventAggregator;
        private IActiveStudent _activeStudent;
        private IFoodRepository _foodRepository;
        private Lazy<IReserver> _reserver;

        private DateTime _startOfWeek = DateTime.Now;

        public DateTime StartOfWeek
        {
            get { return _startOfWeek; }
            set { Set(ref _startOfWeek, value); }
        }

        public DateTime EndOfWeek => StartOfWeek.AddDays(5);

        private bool _helpTextEnabled;

        public bool HelpTextEnabled
        {
            get { return _helpTextEnabled; }
            set { Set(ref _helpTextEnabled, value); }
        }

        public FoodTableViewModel(Lazy<IReserver> reserver ,ISettings settings, IEventAggregator eventAggregator, IActiveStudent activeStudent, IFoodRepository foodRepository)
        {
            _reserver = reserver;
            _activeStudent = activeStudent;
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            _foodRepository = foodRepository;

            _settings = settings;
        }

        private float GetPrice(MealCost mealCost)
        {
            switch (mealCost)
            {
                case MealCost.High:
                    return _settings.HighFood;

                case MealCost.Normal:
                    return _settings.NormalFood;

                case MealCost.Low:
                    return _settings.LowFood;
                default:
                    return 0;
            }
        }

        private void AddToList(IEnumerable<Food> foods)
        {
            Foods.Clear();

            foreach (var item in foods)
            {
                var newEntry = new SelectableFoodViewModel(item)
                {
                    Price = GetPrice(item.MealCost)
                };

                Foods.Add(newEntry);
            }
        }

        public void Handle(object message) => CurrentWeek();

        private bool _canPreviousWeek;
        public bool CanPreviousWeek
        {
            get
            {
                var temp = StartOfWeek.AddDays(-7);
                var result = _foodRepository.GetRange(temp, temp.AddDays(5));

                if (!result.Any())
                {
                    return false;
                }

                return true;
            }

            set { Set(ref _canPreviousWeek, value); }
        }

        public void PreviousWeek() => AddToList(MoveCurrentWeek(-7));
        public void PreviousTwoWeeks() => AddToList(MoveCurrentWeek(-14));

        public void NextWeek()
        {
            var result = MoveCurrentWeek(7);
            if (!result.Any())
            {
                AddToList(CreateUnknownFoods(StartOfWeek));
            }
            else
            {
                AddToList(result);
            }
        }

        public void NextTwoWeek()
        {
            var result = MoveCurrentWeek(14);
            if (!result.Any())
            {
                AddToList(CreateUnknownFoods(StartOfWeek));
            }
            else
            {
                AddToList(result);
            }
        }

        private IEnumerable<Food> MoveCurrentWeek(int days)
        {
            StartOfWeek = StartOfWeek.AddDays(days);
            NotifyOfPropertyChange(() => CanPreviousWeek);
            NotifyOfPropertyChange(() => CanPreviousTwoWeeks);
            return _foodRepository.GetRange(StartOfWeek, EndOfWeek);
        }

        private bool _canPreviousTwoWeeks;
        public bool CanPreviousTwoWeeks
        {
            get
            {
                var temp = StartOfWeek.AddDays(-14);
                var result = _foodRepository.GetRange(temp, temp.AddDays(5));

                if (!result.Any())
                {
                    return false;
                }

                return true;
            }

            set { Set(ref _canPreviousTwoWeeks, value); }
        }

        public void CurrentWeek()
        {
            StartOfWeek = DateTime.Now.StartOfWeek();
            AddToList(_foodRepository.GetRange(StartOfWeek, StartOfWeek.AddDays(5)));
            NotifyOfPropertyChange(() => CanPreviousWeek);
            NotifyOfPropertyChange(() => CanPreviousTwoWeeks);
        }

        public IEnumerable<Food> CreateUnknownFoods(DateTime startOfWeek)
        {
            var dateInPersian = new PersianDateTime(startOfWeek).AddDays(-1);
            var dateInGeorgian = startOfWeek.AddDays(-1);

            List<Food> result = new List<Food>();

            for (int i = 0; i < 5; i++)
            {
                dateInPersian = dateInPersian.AddDays(+1);
                dateInGeorgian = dateInGeorgian.AddDays(+1);

                var newFood = new Food()
                {
                    Date = dateInGeorgian,
                    PersianDate = dateInPersian.ToShortDateInt(),
                };

                result.Add(newFood);
            }

            _foodRepository.Insert(result.ToArray());
            return result;
        }

    }
}