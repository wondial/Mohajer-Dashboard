using HtmlAgilityPack;
using Mohajer.Core;
using Mohajer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mohajer.Infrastructure
{
    public class LoginController : ILoginController
    {
        private static HttpClient _httpClient;
        private HttpClientHandler _httpClientHandler;
        private CookieContainer _cookieContainer;

        private HtmlDocument _htmlDocument = new HtmlDocument();

        private string _loginUrl = "StudentLoginForm.aspx?Language=Fa&amp;Status=Students&amp;lr=lang_fa";
        private string _baseUrl = "http://www.mohajertc.ir/";

        public LoginController()
        {
            _cookieContainer = new CookieContainer();

            _httpClientHandler = new HttpClientHandler()
            {
                CookieContainer = _cookieContainer
            };

            _httpClient = new HttpClient(_httpClientHandler)
            {
                BaseAddress = new Uri(_baseUrl)
            };
        }

        public async Task<RequestResult<string>> GetCaptcha()
        {
            string captchaImageUrl = null;

            HttpResponseMessage result = null;

            var loginResult = new RequestResult<string>();

            try
            {
                result = await _httpClient.GetAsync(_loginUrl);

                if (result.IsSuccessStatusCode)
                {
                    _htmlDocument.Load(await result.Content.ReadAsStreamAsync());
                    captchaImageUrl = _baseUrl + _htmlDocument.DocumentNode.SelectSingleNode("//img").Attributes["src"].Value;

                    loginResult.Content = captchaImageUrl;
                    loginResult.ResultStatus = ResultType.Successful;

                    return loginResult;
                }
            }
            catch (Exception)
            {
                loginResult.Content = null;
                loginResult.ResultStatus = ResultType.ConnectionProblem;

                return loginResult;
            }

            loginResult.Content = null;
            loginResult.ResultStatus = ResultType.ConnectionProblem;

            return loginResult;
        }

        public async Task<RequestResultExtended<Student,IEnumerable<Cookie>>> Login(string username, string password, string captchaValue)
        {
            var failedResult = new RequestResultExtended<Student,IEnumerable<Cookie>>() { Content = null, ResultStatus = ResultType.ConnectionProblem };

            var viewState = _htmlDocument.DocumentNode.SelectSingleNode("//input[@id='__VIEWSTATE']").Attributes["value"].Value;
            var viewStateManager = _htmlDocument.DocumentNode.SelectSingleNode("//input[@id='__VIEWSTATEGENERATOR']").Attributes["value"].Value;
            var eventValidation = _htmlDocument.DocumentNode.SelectSingleNode("//input[@id='__EVENTVALIDATION']").Attributes["value"].Value;

            FormUrlEncodedContent parameters = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string,string>("TxtUserName",username),
                    new KeyValuePair<string,string>("a","rdbStudent"),
                    new KeyValuePair<string,string>("Button1","ورود به سیستم"),
                    new KeyValuePair<string,string>("TxtPassword",password),
                    new KeyValuePair<string,string>("CaptchaControl1",captchaValue),
                    new KeyValuePair<string,string>("chkRemember","on"),

                    new KeyValuePair<string,string>("__EVENTTARGET",""),
                    new KeyValuePair<string,string>("__EVENTARGUMENT",""),

                    new KeyValuePair<string,string>("__VIEWSTATE",viewState),
                    new KeyValuePair<string,string>("__VIEWSTATEGENERATOR",viewStateManager),
                    new KeyValuePair<string,string>("__EVENTVALIDATION",eventValidation),
                    new KeyValuePair<string,string>("StandardCode",""),
            });

            try
            {
                var result = await _httpClient.PostAsync(_loginUrl, parameters);

                if (result.IsSuccessStatusCode)
                {
                    _htmlDocument.LoadHtml(await result.Content.ReadAsStringAsync());

                    var loginError = _htmlDocument.DocumentNode.SelectSingleNode("//span[@id='Label2']");

                    if (loginError == null)
                    {
                        var firstName = _htmlDocument.DocumentNode.SelectSingleNode("//span[@id='ctl00_lblName']").InnerText;
                        var lastName = _htmlDocument.DocumentNode.SelectSingleNode("//span[@id='ctl00_lblLastName']").InnerText;
                        var studentCode = _htmlDocument.DocumentNode.SelectSingleNode("//span[@id='ctl00_lblStandardCode']").InnerText;

                        var cookies = _cookieContainer.GetCookies(new Uri(_baseUrl)).Cast<Cookie>();


                        return new RequestResultExtended<Student, IEnumerable<Cookie>>()
                        {
                            Content = new Student() { FullName = $"{firstName} {lastName}", UserName = int.Parse(username), StudentCode = studentCode, Password = password },
                            Content2 = cookies,
                            ResultStatus = ResultType.Successful
                        };
                         
                    }

                    var loginErrorMessage = loginError.InnerText;

                    switch (loginErrorMessage)
                    {
                        case "لطفا جهت دریافت کلمه عبور به مدیر سایت مراجعه نمایید":
                        case "نام کاربری یا کلمه عبور وارد شده، نادرست است ":
                            return new RequestResultExtended<Student,IEnumerable<Cookie>>() { Content = null, ResultStatus = ResultType.WrongUserNameOrPassword };

                        default:
                            return new RequestResultExtended<Student,IEnumerable<Cookie>>() { Content = null, ResultStatus = ResultType.WrongCaptcha };
                    }
                }
            }
            catch (Exception)
            {
            }

            return failedResult;
        }
    }
}