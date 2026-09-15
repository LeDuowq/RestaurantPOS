using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer
{
    public class ReportDAO
    {
        // 1. Hàm lấy danh sách Hóa đơn đã thanh toán trong khoảng thời gian để tính doanh thu
        public static List<Order> GetRevenueReport(DateTime fromDate, DateTime toDate)
        {
            using var context = new RestaurantPosContext();
            
            // Lọc ra các hóa đơn: Đã thanh toán (Status = 1) VÀ ngày thanh toán nằm trong khoảng [fromDate, toDate]
            return context.Orders
                .Include(o => o.Account) // Lấy thêm thông tin người thu ngân
                .Include(o => o.Table)   // Lấy thêm thông tin Bàn
                .Where(o => o.Status == 1 
                         && o.CheckoutDate != null 
                         && o.CheckoutDate.Value.Date >= fromDate.Date 
                         && o.CheckoutDate.Value.Date <= toDate.Date)
                .OrderByDescending(o => o.CheckoutDate) // Mới nhất xếp trên cùng
                .ToList();
        }

        // 2. Hàm thống kê Món bán chạy / Doanh thu cao nhất
        public static List<TopSellingFoodDTO> GetTopSellingFoods(DateTime fromDate, DateTime toDate, bool sortByRevenue = false)
        {
            using var context = new RestaurantPosContext();

            // Kết hợp (JOIN) OrderDetail với Order để kiểm tra ngày giờ thanh toán
            var query = from detail in context.OrderDetails
                        join order in context.Orders on detail.OrderId equals order.OrderId
                        join food in context.FoodItems on detail.FoodId equals food.FoodId
                        where order.Status == 1 
                           && order.CheckoutDate != null 
                           && order.CheckoutDate.Value.Date >= fromDate.Date 
                           && order.CheckoutDate.Value.Date <= toDate.Date
                        group detail by new { food.FoodId, food.FoodName } into g
                        select new TopSellingFoodDTO
                        {
                            FoodId = g.Key.FoodId,
                            FoodName = g.Key.FoodName,
                            TotalQuantitySold = g.Sum(x => x.Quantity),
                            TotalRevenue = g.Sum(x => x.Quantity * x.UnitPrice)
                        };

            if (sortByRevenue)
            {
                return query.OrderByDescending(x => x.TotalRevenue).ToList();
            }
            return query.OrderByDescending(x => x.TotalQuantitySold).ToList();
        }
    }
}
