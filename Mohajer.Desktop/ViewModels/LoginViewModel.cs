using Mohajer.Desktop.Validation;
using System.Linq;

namespace Mohajer.Desktop.ViewModels
{
    public partial class LoginViewModel : ScreenWithValidation
    {
        #region form properties

        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set
            {
                ClearErrors();
                if (value.Length != 5)
                {
                    AddError(errors: "نام کاربری باید پنج رقم باشد");
                }

                if (value.Any(p => !char.IsDigit(p)))
                {
                    AddError(errors: "نام کاربری باید عدد باشد");
                }

                _userName = value;
            }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set
            {
                ClearErrors();
                if (string.IsNullOrEmpty(value))
                {
                    AddError(errors: "رمز عبور نمی تواند خالی باشد");
                }

                if (value.Length < 6)
                {
                    AddError(errors: "رمز عبور حداقل باید شش کاراکتر باشد");
                }

                NotifyOfPropertyChange(() => CanLogin);
                _password = value;
            }
        }

        private string _captchaValue;

        public string CaptchaValue
        {
            get { return _captchaValue; }
            set
            {
                ClearErrors();
                if (value.Length != 5)
                {
                    AddError(errors: "مقدار وارد شده باید پنج رقم باشد");
                }

                if (value.Any(p => !char.IsDigit(p)))
                {
                    AddError(errors: "مقدار وارد شده باید عدد باشد");
                }

                NotifyOfPropertyChange(() => CanLogin);
                _captchaValue = value;
            }
        }

        private string _captchaImageUrl;

        public string CaptchaImageUrl
        {
            get => _captchaImageUrl;
            set { Set(ref _captchaImageUrl, value); _captchaValue = string.Empty; NotifyOfPropertyChange(() => CaptchaValue); }
        }


        private string _error;

        public string Error
        {
            get => _error;
            private set
            {
                Set(ref _error, value);
                NotifyOfPropertyChange(() => IsErrorActive);
            }
        }

        public bool IsErrorActive => !string.IsNullOrEmpty(_error);

        #endregion form properties
    }
}