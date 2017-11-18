using System;
using System.Collections.Generic;
using System.Text;

namespace Mohajer.Core.Models
{
    public class RequestResult<T>
    {
        public ResultType ResultStatus { get; set; }
        public T Content { get; set; }
    }

    public class RequestResultExtended<T1, T2> : RequestResult<T1>
    {
        public T2 Content2 { get; set; }
    }

    public enum ResultType
    {
        WrongUserNameOrPassword,
        WrongCaptcha,
        ConnectionProblem,
        Successful
    }
}
