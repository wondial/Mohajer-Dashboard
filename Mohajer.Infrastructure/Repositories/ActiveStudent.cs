using Mohajer.Core;
using Mohajer.Core.Models;
using Mohajer.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mohajer.Infrastructure.Repositories
{
    public class ActiveStudent : IActiveStudent
    {
        private ISettings _settings;
        private IStudentRepository _studentRepository;
        private IFoodRepository _foodRepository;

        Lazy<IReserveLogRepository> _reserveLogRepository;

        public ActiveStudent(ISettings settings, IStudentRepository studentRepository, IFoodRepository foodRepository, Lazy<IReserveLogRepository> reserveLogRepository)
        {
            _reserveLogRepository = reserveLogRepository;
            _foodRepository = foodRepository;
            _settings = settings;
            _studentRepository = studentRepository;
        }

        public Student Current => _studentRepository.FindOne(_settings.UserName);
        public IEnumerable<Food> Foods => _studentRepository.GetFoods(_settings.UserName);

        public void ClearAllInformation()
        {
            _settings.Clear();
            _reserveLogRepository.Value.Clear();
            _foodRepository.Clear();
            _studentRepository.Clear();
        }

        public IEnumerable<Food> GetRange(DateTime start, DateTime end) => Foods.Where(p => p.Date >= start && p.Date <= end).ToList();

        public void InsertFood(IEnumerable<Food> foods)
        {
            var currentStudent = Current;

            List<Food> newOnes = new List<Food>();

            foreach (var item in foods)
            {
                var existingOne = _foodRepository.FindOne(item.PersianDate);

                if (existingOne != null)
                {
                    if (existingOne.Title == "نامشخص" && existingOne.MealCost == MealCost.None && !existingOne.IsHoliday)
                    {
                        existingOne.Title = item.Title;
                        existingOne.SideDishTitle = item.SideDishTitle;
                        existingOne.MealCost = item.MealCost;
                    }

                    if ((existingOne.Status == FoodStatus.ToBeReserved || existingOne.Status == FoodStatus.ToBeUnreserved)
                    && item.Status == FoodStatus.Reservable
                    || item.Status == FoodStatus.Unknown)
                        continue;

                    existingOne.Status = item.Status;
                    _foodRepository.Update(existingOne);
                }
                else
                {
                    newOnes.Add(item);
                }
            }

            _foodRepository.Insert(newOnes.ToArray());
            currentStudent.Foods.AddRange(newOnes);
            _studentRepository.Update(currentStudent);
        }
    }
}