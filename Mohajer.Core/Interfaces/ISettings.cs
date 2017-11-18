using System;
using System.Collections.Generic;
using System.Net;

namespace Mohajer.Core
{
    public interface ISettings : IDisposable
    {
        event EventHandler BalanceChanged;

        int UserName { get; set; }
        float Balance { get; set; }
        string FullName { get; set; }
        string StudentCode { get; set; }

        bool FirstTime { get; set; }

        string Cookies { get; set; }

        float HighFood { get; set; }
        float NormalFood { get; set; }
        float LowFood { get; set; }

        void Save();
        void Load();
        void Clear();
    }
}