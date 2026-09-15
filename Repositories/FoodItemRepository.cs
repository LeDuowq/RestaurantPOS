using BusinessObject;
using DataAccessLayer;
using System.Collections.Generic;

namespace Repositories
{
    public class FoodItemRepository : IFoodItemRepository
    {
        public List<FoodItem> GetFoodItems() => FoodItemDAO.GetFoodItems();
        
        public FoodItem GetFoodItemByID(int id) => FoodItemDAO.GetFoodItemByID(id);
        
        public void AddFoodItem(FoodItem foodItem) => FoodItemDAO.AddFoodItem(foodItem);
        
        public void UpdateFoodItem(FoodItem foodItem) => FoodItemDAO.UpdateFoodItem(foodItem);
        
        public void DeleteFoodItem(int id) => FoodItemDAO.DeleteFoodItem(id);

        public List<FoodItem> FilterFoodItem(int categoryId)
        {
            return FoodItemDAO.FilterFoodIitem(categoryId);
        }
        public FoodItem GetFoodById(int foodID)
        {
            return FoodItemDAO.GetFoodById(foodID);
        }
        public List<FoodItem> ShowAllFoodItem()
        {
            return FoodItemDAO.ShowAllFoodItem();
        }
        public void UpdateQuantity(int foodId, int newQuantity)
        {
            FoodItemDAO.UpdateQuantity(foodId, newQuantity);
        }
    }
}
