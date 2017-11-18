using System.Net;
using System.Threading.Tasks;
using Mohajer.Core.Models;
using System.Collections.Generic;

namespace Mohajer.Core
{
    public interface IFoodController
    {
        Task<RequestResult<float>> GetBalanceAsync();
        Task<RequestResult<Dictionary<MealCost, float>>> GetPricesAsync();
        Task<RequestResult<List<Food>>> GetFoods(int maxWeeks = 2);
    }
}