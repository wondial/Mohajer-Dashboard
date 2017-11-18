using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using Mohajer.Core;
using Mohajer.Core.Models;
using Mohajer.Core.Repositories;
using Mohajer.Desktop.Dialogs;
using Mohajer.Desktop.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mohajer.Desktop.ViewModels
{
    class ShellViewModel : Conductor<Screen>, IHandle<int>
    {
        private ISettings _settings;

        Lazy<MainViewModel> _mainViewModel;
        Lazy<LoginViewModel> _loginViewModel;
        IEventAggregator _eventAggregator;

        private Lazy<IActiveStudent> _activeStudent;

        public ShellViewModel(Lazy<MainViewModel> mainViewModel, Lazy<LoginViewModel> loginViewModel, ISettings settings, IEventAggregator eventAggregator, Lazy<IActiveStudent> activeStudent)
        {
            _activeStudent = activeStudent;

            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            _mainViewModel = mainViewModel;
            _loginViewModel = loginViewModel;

            _settings = settings;

            if (_settings.UserName != 0)
            {
                ActivateItem(_mainViewModel.Value);
                IsLogoutEnabled = true;
            }
            else
            {
                ActivateItem(_loginViewModel.Value);
                IsLogoutEnabled = false;
            }
        }

        private bool _isLogoutEnabled;
        public bool IsLogoutEnabled { get => _isLogoutEnabled; set => Set(ref _isLogoutEnabled, value); }

        public void Handle(int message)
        {

            IsLogoutEnabled = true;
            ActivateItem(_mainViewModel.Value);
        }

        public void Logout()
        {
            _activeStudent.Value.ClearAllInformation();

            ActivateItem(_loginViewModel.Value);
            IsLogoutEnabled = false;
        }

        public void LogList()
        {
            _eventAggregator.PublishOnUIThread(NavigationEnum.LogList);
        }

        public void Home()
        {
            _eventAggregator.PublishOnUIThread(NavigationEnum.Home);
        }

        public void AboutUs()
        {
            try
            {
                DialogHost.Show(new AboutUsView());
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
