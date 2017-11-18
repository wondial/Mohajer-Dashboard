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

        private void AddToList(IEnumerable<ReserveLog> reserveLogs)
        {
            foreach (var log in reserveLogs)
            {
                ReserveLogs.Add(log);
            }
        }
    }
}