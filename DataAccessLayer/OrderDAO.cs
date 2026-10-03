using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class OrderDAO
    {
        public static Order GetOrder(int tableID, int status)
        {
            using var db = new RestaurantPosContext();
            return db.Orders.OrderByDescending(a => a.OrderId)
                            .FirstOrDefault(a => a.TableId == tableID && a.Status == status);
        }
        public static void AddOrder(Order order)
        {
            using var db = new RestaurantPosContext();
            db.Orders.Add(order);
            db.SaveChanges();
        }
        public static void UpdateOrder(Order order)
        {
            using var db = new RestaurantPosContext();
            db.Orders.Update(order);
            db.SaveChanges();
        }
    }
}
