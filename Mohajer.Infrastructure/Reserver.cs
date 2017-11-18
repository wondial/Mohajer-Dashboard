using HtmlAgilityPack;
using Mohajer.Core;
using Mohajer.Core.Models;
using Mohajer.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mohajer.Infrastructure
{
    public class Reserver : IReserver, IDisposable
    {
        private IFoodController _foodController;
        private IFoodRepository _foodRepository;
        private IReserveLogRepository _reserveLogRepository;

        private static HttpClient _httpClient = new HttpClient();
        private HtmlDocument _htmlDocument = new HtmlDocument();

        private Dictionary<string, ReserveLog> _logs = new Dictionary<string, ReserveLog>();

        private List<Food> _foods;

        public Reserver(IFoodController foodController, IFoodRepository foodRepository, ISettings settings, IReserveLogRepository reserveLogRepository)
        {
            _reserveLogRepository = reserveLogRepository;
            _foodController = foodController;
            _foodRepository = foodRepository;

            var httpClientHandler = new HttpClientHandler() { UseCookies = false, };
            _httpClient = new HttpClient(httpClientHandler);
            _httpClient.DefaultRequestHeaders.Add("Cookie", settings.Cookies);
        }

        public async Task Run()
        {
            try
            {
                var response = await _httpClient.GetAsync("http://mohajertc.ir/StudentReserveFood.aspx?Language=Fa&Status=Students&lr=lang_fa");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    _htmlDocument.LoadHtml(await response.Content.ReadAsStringAsync());

                    _foods = new List<Food>(_foodRepository.CurrentWeekFoods());

                    await SendForm(_foods, "0");

                    CheckResult();

                    await GoToNextWeek(_foods);

                    _foods = new List<Food>(_foodRepository.NextWeekFoods());

                    await SendForm(_foods, "1");
                    CheckResult();
                }
            }
            catch (Exception)
            {
                _reserveLogRepository.Insert(_logs.Values.ToArray());
            }
        }

        private async Task SendForm(IEnumerable<Food> foods, string weekNum)
        {
            FormUrlEncodedContent parameters = null;

            parameters = FillParameters(weekNum, DetectEachDayStatusWithLog(foods));

            var reserveResponse = await _httpClient.PostAsync("http://mohajertc.ir/StudentReserveFood.aspx?Language=Fa&Status=Students&lr=lang_fa", parameters);

            _htmlDocument.LoadHtml(await reserveResponse.Content.ReadAsStringAsync());
        }

        private FormUrlEncodedContent FillParameters(string weekNum, List<KeyValuePair<string, string>> foods)
        {
            var viewState = _htmlDocument.DocumentNode.SelectSingleNode("//input[@id='__VIEWSTATE']").Attributes["value"].Value;
            var viewStateManager = _htmlDocument.DocumentNode.SelectSingleNode("//input[@id='__VIEWSTATEGENERATOR']").Attributes["value"].Value;
            var eventValidation = _htmlDocument.DocumentNode.SelectSingleNode("//input[@id='__EVENTVALIDATION']").Attributes["value"].Value;

            var Parameters = new List<KeyValuePair<string, string>>
            {
                    new KeyValuePair<string,string>("__VIEWSTATE",viewState),
                    new KeyValuePair<string,string>("__VIEWSTATEGENERATOR",viewStateManager),
                    new KeyValuePair<string,string>("__EVENTVALIDATION",eventValidation),
                    new KeyValuePair<string,string>("ctl00$RadMenu2",""),
                    new KeyValuePair<string,string>("__LASTFOCUS",""),
                    new KeyValuePair<string,string>("__EVENTARGUMENT",""),
                    new KeyValuePair<string,string>("ctl00$ContentPlaceHolder1$_drpSelectPeriod",weekNum),
                    new KeyValuePair<string,string>("ctl00$ContentPlaceHolder1$Button1","ذخیره جدول زمانبندی"),
            };

            Parameters.AddRange(foods);

            FormUrlEncodedContent parameters = new FormUrlEncodedContent(Parameters);

            return parameters;
        }

        private async Task GoToNextWeek(IEnumerable<Food> foods)
        {
            var foodParameters = DetectEachDayStatus(foods);

            var viewState = _htmlDocument.DocumentNode.SelectSingleNode("//input[@id='__VIEWSTATE']").Attributes["value"].Value;
            var viewStateManager = _htmlDocument.DocumentNode.SelectSingleNode("//input[@id='__VIEWSTATEGENERATOR']").Attributes["value"].Value;
            var eventValidation = _htmlDocument.DocumentNode.SelectSingleNode("//input[@id='__EVENTVALIDATION']").Attributes["value"].Value;

            var technicalParameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string,string>("__LASTFOCUS",""),
                new KeyValuePair<string,string>("__EVENTARGUMENT",""),
                new KeyValuePair<string,string>("ctl00$RadMenu2",""),
                new KeyValuePair<string,string>("__VIEWSTATE",viewState),
                new KeyValuePair<string,string>("__EVENTVALIDATION",eventValidation),
                new KeyValuePair<string,string>("__EVENTTARGET","ctl00$ContentPlaceHolder1$_drpSelectPeriod"),
                new KeyValuePair<string,string>("__VIEWSTATEGENERATOR",viewStateManager),
                new KeyValuePair<string,string>("ctl00$ContentPlaceHolder1$_drpSelectPeriod","1"),
            };

            technicalParameters.AddRange(foodParameters);

            FormUrlEncodedContent parameters = new FormUrlEncodedContent(technicalParameters);

            var reserveResponse = await _httpClient.PostAsync("http://mohajertc.ir/StudentReserveFood.aspx?Language=Fa&Status=Students&lr=lang_fa", parameters);

            _htmlDocument.LoadHtml(await reserveResponse.Content.ReadAsStringAsync());
        }

        private List<KeyValuePair<string, string>> DetectEachDayStatusWithLog(IEnumerable<Food> foods)
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();

            foreach (var item in foods)
            {
                string headerValue = null;
                string headerLabel = null;

                var dayNumber = item.Date.DayOfWeekNumber1Based();
                headerLabel = $"ctl00$ContentPlaceHolder1$drp2{dayNumber}";

                if (item.Status == FoodStatus.Reservable || item.Status == FoodStatus.ToBeUnreserved)
                {
                    headerValue = "-1";

                    if (item.Status == FoodStatus.ToBeUnreserved)
                    {
                        var log = new ReserveLog() { Food = item, Operation = ReserveOperation.Unreserve, Result = ReserveResult.ConnectionProblem, TimeStamp = DateTime.Now };
                        _logs.Add(headerLabel, log);
                    }
                }

                if (item.Status == FoodStatus.ToBeReserved || item.Status == FoodStatus.ReserverdAndChangeable)
                {
                    headerValue = ((int)item.MealCost).ToString();

                    if (item.Status == FoodStatus.ToBeReserved)
                    {
                        var log = new ReserveLog() { Food = item, Operation = ReserveOperation.Reserve, Result = ReserveResult.ConnectionProblem, TimeStamp = DateTime.Now };
                        _logs.Add(headerLabel, log);
                    }
                }

                if (headerValue != null && headerLabel != null)
                    result.Add(new KeyValuePair<string, string>(headerLabel, headerValue));
            }

            return result;
        }

        private List<KeyValuePair<string, string>> DetectEachDayStatus(IEnumerable<Food> foods)
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();

            foreach (var item in foods)
            {
                string headerValue = null;
                string headerLabel = null;

                var dayNumber = item.Date.DayOfWeekNumber1Based();
                headerLabel = $"ctl00$ContentPlaceHolder1$drp2{dayNumber}";

                if (item.Status == FoodStatus.Reservable || item.Status == FoodStatus.ToBeUnreserved)
                {
                    headerValue = "-1";
                }

                if (item.Status == FoodStatus.ToBeReserved || item.Status == FoodStatus.ReserverdAndChangeable)
                {
                    headerValue = ((int)item.MealCost).ToString();
                }

                if (headerValue != null && headerLabel != null)
                    result.Add(new KeyValuePair<string, string>(headerLabel, headerValue));
            }

            return result;
        }

        private void CheckResult()
        {
            var rows = _htmlDocument.DocumentNode.SelectNodes("//select[starts-with(@id,'ctl00_ContentPlaceHolder1_drp')]");

            foreach (var row in rows)
            {
                var lableName = row.Attributes["name"].Value;
                var selectedValue = row.SelectSingleNode("//option[@selected]").Attributes["value"].Value;

                if (selectedValue == "1" || selectedValue == "-1")
                {
                    if (_logs.ContainsKey(lableName))
                    {
                        var food = _logs[lableName].Food;

                        if (food.Status == FoodStatus.ToBeUnreserved)
                        {
                            _logs[lableName].Result = ReserveResult.Successful;
                            _logs[lableName].Food.Status = FoodStatus.Reservable;
                        }
                    }
                }
                else
                {
                    if (_logs.ContainsKey(lableName))
                    {
                        var food = _logs[lableName].Food;

                        if (food.Status == FoodStatus.ToBeReserved)
                        {
                            _logs[lableName].Result = ReserveResult.Successful;
                            _logs[lableName].Food.Status = FoodStatus.ReserverdAndChangeable;
                        }
                        else
                        {
                            _logs[lableName].Result = ReserveResult.NotEnoughMoney;
                        }
                    }
                }

            }

            _reserveLogRepository.Insert(_logs.Values.ToArray());
            _logs.Clear();
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}