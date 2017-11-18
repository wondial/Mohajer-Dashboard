using System.Net;
using System.Threading.Tasks;
using Mohajer.Core.Models;
using System.Collections.Generic;

namespace Mohajer.Core
{
    public interface ILoginController
    {
        Task<RequestResult<string>> GetCaptcha();
        Task<RequestResultExtended<Student, IEnumerable<Cookie>>> Login(string username, string password, string captchaValue);
    }
}