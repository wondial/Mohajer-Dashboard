using Caliburn.Micro;
using Mohajer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mohajer.Desktop.ViewModels
{
    class SystemTrayViewModel : Screen
    {
        private IWindowManager _windowManager;
        private ShellViewModel _shellViewModel;

        public SystemTrayViewModel(IWindowManager windowManager, ShellViewModel shellViewModel, ISettings settings)
        {
            _windowManager = windowManager;
            _shellViewModel = shellViewModel;

            if (settings.UserName != 0)
            {
                FluentScheduler.JobManager.Initialize(new JobRegistry());
            }

            if (settings.FirstTime)
            {
                ShowWindow();
            }
        }

        public void ShowWindow()
        {
            if (!_shellViewModel.IsActive)
            {
                _windowManager.ShowWindow(_shellViewModel);
            }
        }

        public void Exit()
        {
            Application.Current.Shutdown();
        }
    }
}
