using Caliburn.Micro;
using Mohajer.Core.Models;
using Mohajer.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Mohajer.Desktop.ViewModels
{
    internal class LogListViewModel : Screen
    {
        private IReserveLogRepository _reserveLogRepository;

        public ObservableCollection<ReserveLog> ReserveLogs { get; set; } = new ObservableCollection<ReserveLog>();

        public LogListViewModel(IReserveLogRepository reserveLogRepository)
        {
            _reserveLogRepository = reserveLogRepository;

            var logs = _reserveLogRepository.Get(6);

            AddToList(logs);
        }

        public LogListViewModel()
        {
            var FakeData = new List<ReserveLog>()
            {
                new ReserveLog()
                {
                    Food = new Food() {Title = "چلو خورشت قیمه بادمجان", MealCost = MealCost.High, Date = DateTime.Now},
                    Operation = ReserveOperation.Reserve,
                    Result = ReserveResult.Successful,
                    TimeStamp = DateTime.Now.AddMinutes(-15)
                },

                                new ReserveLog()
                {
                    Food = new Food() {Title = "چلو خورشت قیمه بادمجان", MealCost = MealCost.High, Date = DateTime.Now},
                    Operation = ReserveOperation.Reserve,
                    Result = ReserveResult.NotEnoughMoney,
                    TimeStamp = DateTime.Now.AddMinutes(-15)
                },

                                                new ReserveLog()
                {
                    Food = new Food() {Title = "چلو خورشت قیمه بادمجان", MealCost = MealCost.High, Date = DateTime.Now},
                    Operation = ReserveOperation.Reserve,
                    Result = ReserveResult.ConnectionProblem,
                    TimeStamp = DateTime.Now.AddMinutes(-15)
                },

                new ReserveLog()
                {
                    Food = new Food() {Title = "چلو خورشت قیمه بادمجان", MealCost = MealCost.High, Date = DateTime.Now},
                    Operation = ReserveOperation.Reserve,
                    Result = ReserveResult.NotEnoughMoney,
                    TimeStamp = DateTime.Now.AddMinutes(-15)
                },

                new ReserveLog()
                {
                    Food = new Food() {Title = "چلو خورشت قیمه بادمجان", MealCost = MealCost.High, Date = DateTime.Now},
                    Operation = ReserveOperation.Reserve,
                    Result = ReserveResult.NotEnoughMoney,
                    TimeStamp = DateTime.Now.AddMinutes(-15)
                },
            };

            AddToList(FakeData);
        }

        private void AddToList(IEnumerable<ReserveLog> reserveLogs)
        {
            foreach (var log in reserveLogs)
            {
                ReserveLogs.Add(log);
            }
        }
    }
}