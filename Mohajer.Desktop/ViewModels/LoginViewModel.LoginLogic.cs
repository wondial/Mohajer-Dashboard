using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using Mohajer.Core;
using Mohajer.Core.Models;
using Mohajer.Core.Repositories;
using Mohajer.Desktop.Dialogs;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Mohajer.Desktop.ViewModels
{
    public partial class LoginViewModel
    {
        private ILoginController _loginController;
        private IEventAggregator _eventAggregator;
        private IStudentRepository _studentRepository;
        private ISettings _settings;

        private DispatcherTimer _timer = new DispatcherTimer();

        public LoginViewModel(ILoginController loginController, IEventAggregator eventAggregator, ISettings settings, IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;

            _settings = settings;

            _eventAggregator = eventAggregator;

            _loginController = loginController;

            _timer.Interval = TimeSpan.FromSeconds(180);

            _timer.Tick += (s, e) => { RefreshCaptcha(); };
        }

        protected override void OnViewLoaded(object view)
        {
            RefreshCaptcha();
            _timer.Start();
        }

        public bool CanLogin => !(HasErrors || string.IsNullOrEmpty(_password) || string.IsNullOrEmpty(_userName) || string.IsNullOrEmpty(_captchaValue));

        public async void Login()
        {
            var result = await _loginController.Login(_userName, _password, _captchaValue);

            switch (result.ResultStatus)
            {
                case ResultType.WrongUserNameOrPassword:
                    Error = "نام کاربری یا رمز عبور اشتباه است";
                    RefreshCaptcha();
                    break;

                case ResultType.WrongCaptcha:
                    Error = "کد تصویر اشتباه است";
                    RefreshCaptcha();
                    break;

                case ResultType.ConnectionProblem:
                    Error = "خطا در اتصال";
                    break;

                default:
                    {
                        Student newStudent = result.Content;
                        _studentRepository.Insert(newStudent);

                        _settings.UserName = newStudent.UserName;
                        _settings.FullName = newStudent.FullName;
                        _settings.StudentCode = newStudent.StudentCode;
                        _settings.Cookies = string.Join(";", result.Content2.Select(p => $"{p.Name}={p.Value}"));
                        _settings.FirstTime = false;

                        _settings.Save();

                        _eventAggregator.PublishOnUIThread(newStudent.UserName);
                        break;
                    }
            }
        }

        private async void RefreshCaptcha()
        {
            await DialogHost.Show(new BusyProgressView(), async (object s, DialogOpenedEventArgs e) =>
            {
                var captchaResult = await _loginController.GetCaptcha();

                if (captchaResult.ResultStatus == ResultType.ConnectionProblem)
                {
                    Error = "خطا در اتصال";
                }
                else
                {
                    BitmapImage bitmap = null;

                    try
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            var response = await webClient.DownloadDataTaskAsync(new Uri(captchaResult.Content));

                            using (var stream = new MemoryStream(response))
                            {
                                bitmap = new BitmapImage();
                                bitmap.BeginInit();
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                bitmap.StreamSource = stream;
                                bitmap.EndInit();
                                bitmap.Freeze();

                                CaptchaImage = bitmap;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        Error = "خطا در اتصال";
                    }

                }

                e.Session.Close();
            });
        }
    }
}