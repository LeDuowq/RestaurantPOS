using BusinessObject;
using Repositories;
using System.Collections.Generic;
using BusinessObject;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class FoodItemService : IFoodItemService
    {
        private IFoodItemRepository repository;

        public FoodItemService()
        {
            repository = new FoodItemRepository();
        }

        public List<FoodItem> GetFoodItems() => repository.GetFoodItems();

        public FoodItem GetFoodItemByID(int id) => repository.GetFoodItemByID(id);

        public void AddFoodItem(FoodItem foodItem) => repository.AddFoodItem(foodItem);

        public void UpdateFoodItem(FoodItem foodItem) => repository.UpdateFoodItem(foodItem);

        public void DeleteFoodItem(int id) => repository.DeleteFoodItem(id);

        public List<FoodItem> FilterFoodIitem(int categoryId)
        {
            return repository.FilterFoodItem(categoryId);
        }
        public FoodItem GetFoodById(int foodID)
        {
            return repository.GetFoodById(foodID);
        }
        public List<FoodItem> ShowAllFoodItem()
        {
            return repository.ShowAllFoodItem();
        }
        public void UpdateQuantity(int foodId, int newQuantity)
        {
            repository.UpdateQuantity(foodId, newQuantity);
        }
    }
}
