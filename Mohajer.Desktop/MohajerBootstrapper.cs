using Caliburn.Micro;
using FluentScheduler;
using LightInject;
using Mohajer.Desktop.Validation;
using Mohajer.Desktop.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace Mohajer.Desktop
{
    public class MohajerBootstrapper : BootstrapperBase
    {
        private ServiceContainer _container;

        public MohajerBootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            if (Utilities.StartUpManager.IsUserAdministrator())
            {
                Utilities.StartUpManager.AddApplicationToAllUserStartup();
            }
            else
            {
                Utilities.StartUpManager.AddApplicationToCurrentUserStartup();
            }

            Mohajer.Infrastructure.ConfigureLiteDb.Configure();

            DisplayRootViewFor<SystemTrayViewModel>();



            
        }

        protected override void Configure()
        {
            _container = new ServiceContainer();

            _container.Register<IWindowManager, WindowManager>(new PerContainerLifetime());
            _container.Register<IEventAggregator, EventAggregator>(new PerContainerLifetime());

            _container.Register<ShellViewModel>();
            _container.Register<LoginViewModel>();
            _container.Register<MainViewModel>();
            _container.Register<InfoViewModel>();
            _container.Register<FoodTableViewModel>();
            _container.Register<LogListViewModel>();
            _container.Register<SystemTrayViewModel>();

            _container.Register<Mohajer.Core.Repositories.IStudentRepository, Mohajer.Infrastructure.Repositories.StudentRepository>();
            _container.Register<Mohajer.Core.Repositories.IFoodRepository, Mohajer.Infrastructure.Repositories.FoodRepository>();
            _container.Register<Mohajer.Core.Repositories.IActiveStudent, Mohajer.Infrastructure.Repositories.ActiveStudent>();
            _container.Register<Mohajer.Core.Repositories.IReserveLogRepository, Mohajer.Infrastructure.Repositories.ReserveLogRepository>();

            _container.Register<Mohajer.Core.ILoginController, Mohajer.Infrastructure.LoginController>();
            _container.Register<Mohajer.Core.IFoodController, Mohajer.Infrastructure.FoodController>();
            _container.Register<Mohajer.Core.IReserver, Mohajer.Infrastructure.Reserver>();
            _container.Register<Mohajer.Core.ISettings, Mohajer.Infrastructure.Settings>(new PerContainerLifetime());
        }

        protected override object GetInstance(Type service, string key)
        {
            return (string.IsNullOrEmpty(key)) ? _container.GetInstance(service) : _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }
    }
}