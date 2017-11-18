using MaterialDesignThemes.Wpf;
using Mohajer.Core.Models;
using Mohajer.Desktop.Dialogs;
using System;
using System.Collections.Generic;

namespace Mohajer.Desktop.ViewModels
{
    public partial class FoodTableViewModel
    {
        public async void Save()
        {
            await DialogHost.Show(new BusyProgressView(), async (object s, DialogOpenedEventArgs e) =>
            {
                List<Food> updatedFoods = new List<Food>();

                List<SelectableFoodViewModel> full = new List<SelectableFoodViewModel>();

                foreach (var item in Foods)
                {
                    if (!item.IsSelected && item.Food.Status == Core.Models.FoodStatus.ReserverdAndChangeable)
                    {
                        item.Food.Status = Core.Models.FoodStatus.ToBeUnreserved;
                        updatedFoods.Add(item.Food);
                    }
                    else if (!item.IsSelected && item.Food.Status == FoodStatus.ToBeReserved)
                    {
                        item.Food.Status = FoodStatus.Reservable;
                        updatedFoods.Add(item.Food);
                    }
                    else if (item.IsSelected && (item.Food.Status == Core.Models.FoodStatus.Reservable || item.Food.Status == Core.Models.FoodStatus.Unknown))
                    {
                        item.Food.Status = Core.Models.FoodStatus.ToBeReserved;
                        updatedFoods.Add(item.Food);
                    }

                    full.Add(item);
                }

                AddToList(full);

                _foodRepository.Update(updatedFoods.ToArray());

                await _reserver.Value.Run();

                e.Session.Close();
            });
        }

        private void AddToList(IEnumerable<SelectableFoodViewModel> selectableFoodViewModels)
        {
            Foods.Clear();
            foreach (var item in selectableFoodViewModels)
            {
                Foods.Add(item);
            }
        }
    }
}