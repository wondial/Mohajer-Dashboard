using Caliburn.Micro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Mohajer.Desktop.Validation
{
    public abstract class ScreenWithValidation : Screen,INotifyDataErrorInfo
    {
        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        public bool HasErrors => _errors.Any();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return _errors.Values;
            else
                return _errors.ContainsKey(propertyName) ? _errors[propertyName] : null;
        }

        protected void OnErrorsChanged(string propertyName) => ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

        protected void ClearErrors([CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrEmpty(propertyName) && !_errors.ContainsKey(propertyName))
                _errors.Clear();
            else
                _errors.Remove(propertyName);

            OnErrorsChanged(propertyName);
        }

        protected void AddError([CallerMemberName] string propertyName = null, params string[] errors)
        {
            if (!_errors.ContainsKey(propertyName))
            {
                _errors.Add(propertyName, new List<string>(errors));
                OnErrorsChanged(propertyName);
                return;
            }

            var inputs = errors.Except(_errors[propertyName]);
            _errors[propertyName].AddRange(inputs);

            if (inputs.Any())
                OnErrorsChanged(propertyName);
        }
    }
}