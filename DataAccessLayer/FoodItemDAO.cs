using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class FoodItemDAO
    {
        public static List<FoodItem> GetFoodItems()
        {
            using var context = new RestaurantPosContext();
            return context.FoodItems.Include(f => f.Category).ToList();
        }

        public static FoodItem GetFoodItemByID(int id)
        {
            using var context = new RestaurantPosContext();
            return context.FoodItems.FirstOrDefault(f => f.FoodId == id);
        }

        public static void AddFoodItem(FoodItem foodItem)
        {
            using var context = new RestaurantPosContext();
            context.FoodItems.Add(foodItem);
            context.SaveChanges();
        }

        public static void UpdateFoodItem(FoodItem foodItem)
        {
            using var context = new RestaurantPosContext();
            var existingFood = context.FoodItems.FirstOrDefault(f => f.FoodId == foodItem.FoodId);
            if (existingFood != null)
            {
                existingFood.FoodName = foodItem.FoodName;
                existingFood.Price = foodItem.Price;
                existingFood.CategoryId = foodItem.CategoryId;
                existingFood.IsAvailable = foodItem.IsAvailable;
                existingFood.Quantity = foodItem.Quantity;
                context.SaveChanges();
            }
        }

        public static void UpdateQuantity(int foodId, int newQuantity)
        {
            using var context = new RestaurantPosContext();
            var existingFood = context.FoodItems.FirstOrDefault(f => f.FoodId == foodId);
            if (existingFood != null)
            {
                existingFood.Quantity = newQuantity;
                context.SaveChanges();
            }
        }

        public static void DeleteFoodItem(int id)
        {
            using var context = new RestaurantPosContext();
            var foodItem = context.FoodItems.FirstOrDefault(f => f.FoodId == id);
            if (foodItem != null)
            {
                context.FoodItems.Remove(foodItem);
                context.SaveChanges();
            }
        }
        public static List<FoodItem> ShowAllFoodItem()
        {
            using var db = new RestaurantPosContext();
            return db.FoodItems.Where(a=>a.IsAvailable).ToList();
        }
        public static List<FoodItem> FilterFoodIitem(int categoryId)
        {
            using var db = new RestaurantPosContext();
            return db.FoodItems.Where(a=>a.CategoryId == categoryId).ToList();
        }
        public static FoodItem GetFoodById(int foodId)
        {
            using var db = new RestaurantPosContext();
            return db.FoodItems.FirstOrDefault(f => f.FoodId == foodId);
        }
    }
}
