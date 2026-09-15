using BusinessObject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class OrderDetailDAO
    {
        public static List<OrderDetail> GetListOrderDetailByOrderId(int orderID)
        {
            using var db = new RestaurantPosContext();
            return db.OrderDetails.Where(od => od.OrderId == orderID).ToList();
        }

        public static bool AddFoodItem(int orderId, FoodItem foodItem)
        {
            using var db = new RestaurantPosContext();
            var targetFood = db.FoodItems.FirstOrDefault(f => f.FoodId == foodItem.FoodId);
            if (targetFood == null || targetFood.Quantity <= 0 || !targetFood.IsAvailable)
            {
                return false;
            }

            var existingDetail = db.OrderDetails
                .FirstOrDefault(od => od.OrderId == orderId && od.FoodId == foodItem.FoodId);

            if (existingDetail != null)
            {
                existingDetail.Quantity += 1;
            }
            else
            {
                var newDetail = new OrderDetail
                {
                    OrderId = orderId,
                    FoodId = foodItem.FoodId,
                    Quantity = 1,
                    UnitPrice = targetFood.Price,
                    Status = 0
                };
                db.OrderDetails.Add(newDetail);
            }

            targetFood.Quantity -= 1;
            db.SaveChanges();
            return true;
        }

        public static decimal CalculateOrderTotal(int orderId)
        {
            using var db = new RestaurantPosContext();
            var details = GetListOrderDetailByOrderId(orderId);
            decimal total = details.Sum(d => d.Quantity * d.UnitPrice);
            return total;
        }

        public static void UpdateStatus(int detailId, int newStatus)
        {
            using var db = new RestaurantPosContext();
            var detail = db.OrderDetails.FirstOrDefault(d => d.DetailId == detailId);
            if (detail != null)
            {
                detail.Status = newStatus;
                db.SaveChanges();
            }
        }

        public static List<OrderDetailDTO> GetKitchenOrderDetails()
        {
            using var db = new RestaurantPosContext();
            var query = from od in db.OrderDetails
                        join o in db.Orders on od.OrderId equals o.OrderId
                        join t in db.DiningTables on o.TableId equals t.TableId
                        join f in db.FoodItems on od.FoodId equals f.FoodId
                        where o.Status == 0 && od.Status < 2 // 0: Chờ chế biến, 1: Đang chế biến
                        orderby od.DetailId descending
                        select new OrderDetailDTO
                        {
                            DetailId = od.DetailId,
                            OrderId = od.OrderId,
                            TableName = t.TableName,
                            FoodName = f.FoodName,
                            Quantity = od.Quantity,
                            UnitPrice = od.UnitPrice,
                            TotalPrice = od.Quantity * od.UnitPrice,
                            Status = od.Status
                        };
            return query.ToList();
        }

        public static (decimal FinalTotal, List<OrderDetailDTO> InProgressItems) ProcessOrderCheckout(int orderId)
        {
            using var db = new RestaurantPosContext();
            var details = db.OrderDetails.Where(od => od.OrderId == orderId).ToList();

            // 1. Món Status == 0 (Chờ chế biến): Hủy món, hoàn lại số lượng tồn kho
            var unservedDetails = details.Where(od => od.Status == 0).ToList();
            foreach (var unserved in unservedDetails)
            {
                var food = db.FoodItems.FirstOrDefault(f => f.FoodId == unserved.FoodId);
                if (food != null)
                {
                    food.Quantity += unserved.Quantity; // Hoàn lại kho
                }
                db.OrderDetails.Remove(unserved);
            }

            // 2. Món Status == 1 (Đang chế biến): Nhặt ra danh sách để báo thu ngân nhắc khách gói mang về
            var inProgressDetails = details.Where(od => od.Status == 1).ToList();
            var inProgressList = inProgressDetails.Select(od => new OrderDetailDTO
            {
                DetailId = od.DetailId,
                OrderId = od.OrderId,
                FoodName = db.FoodItems.FirstOrDefault(f => f.FoodId == od.FoodId)?.FoodName ?? "Không xác định",
                Quantity = od.Quantity,
                UnitPrice = od.UnitPrice,
                TotalPrice = od.Quantity * od.UnitPrice,
                Status = od.Status
            }).ToList();

            // 3. Tính tổng tiền cho các món Status >= 1 (Đang chế biến hoặc Hoàn thành)
            var billedDetails = details.Where(od => od.Status >= 1).ToList();
            decimal finalTotal = billedDetails.Sum(od => od.Quantity * od.UnitPrice);

            db.SaveChanges();
            return (finalTotal, inProgressList);
        }
    }
}
