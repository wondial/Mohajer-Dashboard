using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using Mohajer.Core;

namespace Mohajer.Desktop.ViewModels
{
    internal class InfoViewModel : Screen
    {
        private readonly ISettings _settings;


        public InfoViewModel(ISettings settings)
        {
            _settings = settings;
            _settings.BalanceChanged += OnBalanceChanged;
        }

        private void OnBalanceChanged(object sender, System.EventArgs e)
        {
            NotifyOfPropertyChange(() => Balance);
        }

        public string FullName => _settings.FullName;
        public string StudentCode => _settings.StudentCode;
        public float Balance => _settings.Balance;

        public void AddPicture()
        {
            try
            {
                DialogHost.Show(new Dialogs.NowImplementedYetView());
            }
            catch (System.Exception)
            {
                return;
            }
        }

    }


}