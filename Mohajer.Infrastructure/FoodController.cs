using HtmlAgilityPack;
using Mohajer.Core;
using Mohajer.Core.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mohajer.Infrastructure
{
    public class FoodController : IFoodController
    {
        private HtmlDocument _htmlDocument = new HtmlDocument();

        private static HttpClient _httpClient;
        private HttpClientHandler _httpClientHandler;

        private ISettings _settings;

        public FoodController(ISettings settings)
        {
            _settings = settings;

            _httpClientHandler = new HttpClientHandler() { UseCookies = false,  };
            _httpClient = new HttpClient(_httpClientHandler);
            _httpClient.DefaultRequestHeaders.Add("Cookie", _settings.Cookies);

        }

        public async Task<RequestResult<float>> GetBalanceAsync()
        {
            RequestResult<float> result = new RequestResult<float>() { ResultStatus = ResultType.ConnectionProblem };

            try
            {
                var response = await _httpClient.GetAsync("http://mohajertc.ir/StudentReserveFood.aspx?Language=Fa&Status=Students&lr=lang_fa");

                if (response.IsSuccessStatusCode)
                {
                    _htmlDocument.LoadHtml(await response.Content.ReadAsStringAsync());

                    var balance = _htmlDocument.DocumentNode.SelectSingleNode("//span[@id='ctl00_ContentPlaceHolder1_lblcredit']").InnerText;

                    result.Content = float.Parse(balance);
                    result.ResultStatus = ResultType.Successful;

                    return result;
                }
                else
                {
                    return result;
                }
            }
            catch (Exception)
            {
                return result;
            }
        }
        public async Task<RequestResult<Dictionary<MealCost, float>>> GetPricesAsync()
        {
            var failedResult = new RequestResult<Dictionary<MealCost, float>>()
            {
                Content = null,
                ResultStatus = ResultType.ConnectionProblem
            };

            try
            {
                FormUrlEncodedContent parameters;

                var highFoodResponse = await _httpClient.GetAsync("http://mohajertc.ir/FoodMenu.aspx?Language=Fa&Status=Students&lr=lang_fa");
                _htmlDocument.LoadHtml(await highFoodResponse.Content.ReadAsStringAsync());

                var highPrice = float.Parse(_htmlDocument.DocumentNode.SelectSingleNode("//table[@id='ctl00_ContentPlaceHolder1_gvwMenu']/tr[@class='normal']/td").InnerText);

                parameters = FillParameterForFoodMenu("19");

                var normalFoodResponse = await _httpClient.PostAsync("http://mohajertc.ir/FoodMenu.aspx?Language=Fa&Status=Students&lr=lang_fa", parameters);
                _htmlDocument.LoadHtml(await normalFoodResponse.Content.ReadAsStringAsync());

                var normalPrice = float.Parse(_htmlDocument.DocumentNode.SelectSingleNode("//table[@id='ctl00_ContentPlaceHolder1_gvwMenu']/tr[@class='normal']/td").InnerText);

                parameters = FillParameterForFoodMenu("18");

                var lowFoodResponse = await _httpClient.PostAsync("http://mohajertc.ir/FoodMenu.aspx?Language=Fa&Status=Students&lr=lang_fa", parameters);
                _htmlDocument.LoadHtml(await lowFoodResponse.Content.ReadAsStringAsync());

                var lowPrice = float.Parse(_htmlDocument.DocumentNode.SelectSingleNode("//table[@id='ctl00_ContentPlaceHolder1_gvwMenu']/tr[@class='normal']/td").InnerText);

                return new RequestResult<Dictionary<MealCost, float>>()
                {
                    Content = new Dictionary<MealCost, float> { { MealCost.High, highPrice }, { MealCost.Normal, normalPrice }, { MealCost.Low, lowPrice }, },
                    ResultStatus = ResultType.Successful
                };
            }
            catch (Exception)
            {
                return failedResult;
            }
        }
        private FormUrlEncodedContent FillParameterForFoodMenu(string mealCostNum)
        {
            var viewState = _htmlDocument.DocumentNode.SelectSingleNode("//input[@id='__VIEWSTATE']").Attributes["value"].Value;
            var viewStateManager = _htmlDocument.DocumentNode.SelectSingleNode("//input[@id='__VIEWSTATEGENERATOR']").Attributes["value"].Value;
            var eventValidation = _htmlDocument.DocumentNode.SelectSingleNode("//input[@id='__EVENTVALIDATION']").Attributes["value"].Value;

            FormUrlEncodedContent parameters = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string,string>("__LASTFOCUS",""),
                    new KeyValuePair<string,string>("__EVENTTARGET","ctl00$ContentPlaceHolder1$drpMenuprice"),
                    new KeyValuePair<string,string>("__EVENTARGUMENT",""),
                    new KeyValuePair<string,string>("__VIEWSTATE",viewState),
                    new KeyValuePair<string,string>("__VIEWSTATEGENERATOR",viewStateManager),
                    new KeyValuePair<string,string>("__EVENTVALIDATION",eventValidation),
                    new KeyValuePair<string,string>("ctl00$RadMenu2",""),
                    new KeyValuePair<string,string>("ctl00$ContentPlaceHolder1$drpMenuprice",mealCostNum),
            });

            return parameters;
        }
        private FormUrlEncodedContent FillParameterForFoodTable(string week, List<KeyValuePair<string, string>?> foodParams)
        {
            var viewStateManager = _htmlDocument.DocumentNode.SelectSingleNode("//input[@id='__VIEWSTATEGENERATOR']").Attributes["value"].Value;
            var eventValidation = _htmlDocument.DocumentNode.SelectSingleNode("//input[@id='__EVENTVALIDATION']").Attributes["value"].Value;
            var viewState = _htmlDocument.DocumentNode.SelectSingleNode("//input[@id='__VIEWSTATE']").Attributes["value"].Value;

            var temp = new[] {
                    new KeyValuePair<string,string>("__LASTFOCUS",""),
                    new KeyValuePair<string,string>("__EVENTARGUMENT",""),
                    new KeyValuePair<string,string>("ctl00$RadMenu2",""),
                    new KeyValuePair<string,string>("__VIEWSTATE", viewState),
                    new KeyValuePair<string,string>("__EVENTVALIDATION",eventValidation),
                    new KeyValuePair<string,string>("ctl00$ContentPlaceHolder1$_drpSelectPeriod",week),
                    new KeyValuePair<string,string>("__EVENTTARGET","ctl00$ContentPlaceHolder1$_drpSelectPeriod"),
                    new KeyValuePair<string,string>("__VIEWSTATEGENERATOR",viewStateManager),};

            foodParams.RemoveAll(p => p == null);

            var finalParams = foodParams.Cast<KeyValuePair<string, string>>().ToList();

            finalParams.AddRange(temp);

            FormUrlEncodedContent parameters = new FormUrlEncodedContent(finalParams);

            return parameters;
        }
        public async Task<RequestResult<List<Food>>> GetFoods(int maxWeeks = 2)
        {
            //_httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9");
            //_httpClient.DefaultRequestHeaders.Add("Referer", "http://mohajertc.ir/StudentReserveFood.aspx?Language=Fa&Status=Students&lr=lang_fa");
            //_httpClient.DefaultRequestHeaders.Add("Origin", "http://mohajertc.ir");

            RequestResult<List<Food>> requestResult = new RequestResult<List<Food>>()
            {
                Content = null,
                ResultStatus = ResultType.ConnectionProblem
            };

            List<Food> foods = new List<Food>();

            try
            {
                var currentWeekResponse = await _httpClient.GetAsync("http://mohajertc.ir/StudentReserveFood.aspx?Language=Fa&Status=Students&lr=lang_fa");

                if (currentWeekResponse.StatusCode != HttpStatusCode.OK) throw new Exception();

                _htmlDocument.LoadHtml(await currentWeekResponse.Content.ReadAsStringAsync());

                var foodScrppedResult = ScrapeFoodTable();

                foods.AddRange(foodScrppedResult.Keys);

                for (int i = 1; i <= maxWeeks; i++)
                {
                    var parameters = FillParameterForFoodTable(i.ToString(), foodScrppedResult.Values.ToList());

                    currentWeekResponse = await _httpClient.PostAsync("http://mohajertc.ir/StudentReserveFood.aspx?Language=Fa&Status=Students&lr=lang_fa", parameters);

                    if (!currentWeekResponse.IsSuccessStatusCode) throw new Exception();

                    _htmlDocument.LoadHtml(await currentWeekResponse.Content.ReadAsStringAsync());

                    foodScrppedResult = ScrapeFoodTable();

                    foods.AddRange(foodScrppedResult.Keys);
                }

                requestResult.Content = foods;
                requestResult.ResultStatus = ResultType.Successful;

                return requestResult;
            }
            catch (Exception)
            {
                return requestResult;
            }
        }
        private Dictionary<Food, KeyValuePair<string, string>?> ScrapeFoodTable()
        {
            Dictionary<Food, KeyValuePair<string, string>?> result = new Dictionary<Food, KeyValuePair<string, string>?>();

            PersianCalendar persianCalendar = new PersianCalendar();

            var startToEndOfWeekDate = _htmlDocument.DocumentNode.SelectSingleNode("//span[@id='ctl00_ContentPlaceHolder1_lbldate']").InnerText;
            startToEndOfWeekDate = startToEndOfWeekDate.Substring(9, 10);

            int year = int.Parse(startToEndOfWeekDate.Substring(0, 4));
            int month = int.Parse(startToEndOfWeekDate.Substring(5, 2));
            int day = int.Parse(startToEndOfWeekDate.Substring(8, 2));

            var weekStartDate = new DateTime(year, month, day, persianCalendar).AddDays(-1);

            var tomorrow = (DateTime.Now).AddDays(1);

            for (int i = 21; i <= 25; i++)
            {
                weekStartDate = weekStartDate.AddDays(1);

                string fixedMonth = (persianCalendar.GetMonth(weekStartDate)).ToString();
                fixedMonth = fixedMonth.Length == 1 ? fixedMonth.Insert(0, "0") : fixedMonth;

                string fixedDay = persianCalendar.GetDayOfMonth(weekStartDate).ToString();
                fixedDay = fixedDay.Length == 1 ? fixedDay.Insert(0, "0") : fixedDay;

                int persianDate = int.Parse($"{persianCalendar.GetYear(weekStartDate)}{fixedMonth}{fixedDay}");


                Food newFood = new Food();

                var labelName = $@"ctl00$ContentPlaceHolder1$drp{i}";

                var foodRow = _htmlDocument.DocumentNode.SelectSingleNode($"//select[@name='{labelName}']");

                if (foodRow == null)
                {
                    newFood.IsHoliday = true;
                    newFood.MealCost = MealCost.None;
                    newFood.Date = weekStartDate;
                    newFood.PersianDate = persianDate;
                    newFood.Status = FoodStatus.NotReservable;

                    result.Add(newFood, null);

                    continue;
                }


                var foodOptionElement = foodRow.ChildNodes.Count >= 5 ? foodRow.ChildNodes[4] : null;

                if (foodOptionElement == null)
                {
                    newFood.Date = weekStartDate;

                    if (newFood.Date <= tomorrow)
                    {
                        newFood.Status = FoodStatus.NotReservable;
                    }

                    newFood.MealCost = MealCost.None;
                    newFood.PersianDate = persianDate;
                    result.Add(newFood, null);

                    continue;
                }

                var foodTitle = foodRow.ChildNodes[5].InnerText;

                foodTitle = foodTitle.Substring(foodTitle.LastIndexOf('-') + 1);

                if (foodTitle.Contains('+'))
                {
                    var plusIndex = foodTitle.IndexOf('+');

                    var subdish = foodTitle.Substring(plusIndex + 1);

                    newFood.SideDishTitle = subdish.Trim();

                    foodTitle = foodTitle.Substring(0, plusIndex);
                }

                newFood.Title = foodTitle.Trim();

                var optionElementValue = foodOptionElement.Attributes["value"].Value;

                newFood.MealCost = (MealCost)Enum.Parse(typeof(MealCost), optionElementValue);

                newFood.PersianDate = int.Parse(foodRow.Attributes["title"].Value);

                newFood.Date = weekStartDate;

                KeyValuePair<string, string>? labelValue = null;

                if (foodOptionElement.Attributes.Contains("selected"))
                {
                    if (newFood.Date <= tomorrow)
                    {
                        newFood.Status = FoodStatus.ReserverdAndUnchangeable;
                    }
                    else
                    {
                        newFood.Status = FoodStatus.ReserverdAndChangeable;
                        labelValue = new KeyValuePair<string, string>(labelName, optionElementValue);
                    }
                }
                else
                {
                    if (newFood.Date <= tomorrow)
                    {
                        newFood.Status = FoodStatus.NotReservable;
                    }
                    else
                    {
                        newFood.Status = FoodStatus.Reservable;
                        labelValue = new KeyValuePair<string, string>(labelName, "-1");
                    }
                }

                result.Add(newFood, labelValue);
            }

            return result;
        }

        
    }
}