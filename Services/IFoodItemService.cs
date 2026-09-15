using BusinessObject;
using System.Collections.Generic;

namespace Services
{
    public interface IFoodItemService
    {
        List<FoodItem> GetFoodItems();
        FoodItem GetFoodItemByID(int id);
        void AddFoodItem(FoodItem foodItem);
        void UpdateFoodItem(FoodItem foodItem);
        void DeleteFoodItem(int id);
        List<FoodItem> ShowAllFoodItem();
        List<FoodItem> FilterFoodIitem(int categoryId);
        FoodItem GetFoodById(int foodID);
        void UpdateQuantity(int foodId, int newQuantity);
    }
}
