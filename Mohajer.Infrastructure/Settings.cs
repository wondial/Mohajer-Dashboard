using LiteDB;
using Mohajer.Core;
using System;
using System.Collections.Generic;
using System.Net;

namespace Mohajer.Infrastructure
{
    public class Settings : ISettings
    {
        public event EventHandler BalanceChanged = delegate { };

        private LiteDatabase _liteDatabase = new LiteDatabase("MainDb.db");
        private LiteCollection<InternalSettings> _collection;

        private InternalSettings _currentSettings;

        public Settings()
        {
            _collection = _liteDatabase.GetCollection<InternalSettings>();
            Load();
        }

        public int UserName { get => _currentSettings.UserName; set { _currentSettings.UserName = value; } }

        public float Balance { get => _currentSettings.Balance;
            set
            {
                _currentSettings.Balance = value;
                BalanceChanged(this, null);
            }
        }
        public string FullName { get => _currentSettings.FullName; set => _currentSettings.FullName = value; }
        public string StudentCode { get => _currentSettings.StudentCode; set => _currentSettings.StudentCode = value; }

        public float HighFood { get => _currentSettings.HighFood; set => _currentSettings.HighFood = value; }
        public float NormalFood { get => _currentSettings.MediumFood; set => _currentSettings.MediumFood = value; }
        public float LowFood { get => _currentSettings.LowFood; set => _currentSettings.LowFood = value; }

        public string Cookies { get => _currentSettings.Cookies; set => _currentSettings.Cookies = value; }

        public void Clear()
        {
            _currentSettings = new InternalSettings();
            Save();
        }

        public void Load()
        {
            var setting = _collection.FindById(1);

            if (setting == null)
            {
                _currentSettings = new InternalSettings();
                _collection.Insert(_currentSettings);
            }
            else
            {
                _currentSettings = setting;
            }
        }

        public void Save()
        {
            _collection.Update(_currentSettings);
        }

        public void Dispose()
        {
            _liteDatabase.Dispose();
        }

        private class InternalSettings
        {
            public int Id => 1;

            public string Cookies { get; set; }

            public int UserName { get; set; }
            public float Balance { get; set; }
            public string FullName { get; set; }
            public string StudentCode { get; set; }

            public float HighFood { get; set; }
            public float MediumFood { get; set; }
            public float LowFood { get; set; }
        }
    }
}