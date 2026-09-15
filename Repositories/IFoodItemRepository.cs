using BusinessObject;
using System.Collections.Generic;

namespace Repositories
{
    public interface IFoodItemRepository
    {
        List<FoodItem> GetFoodItems();
        FoodItem GetFoodItemByID(int id);
        void AddFoodItem(FoodItem foodItem);
        void UpdateFoodItem(FoodItem foodItem);
        void DeleteFoodItem(int id);
        List<FoodItem> FilterFoodItem(int categoryId);
        FoodItem GetFoodById(int foodID);
        List<FoodItem> ShowAllFoodItem();
        void UpdateQuantity(int foodId, int newQuantity);
    }
}
