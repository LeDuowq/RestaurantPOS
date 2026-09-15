using BusinessObject;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        public bool AddFoodItem(int orderId, FoodItem foodItem)
        {
            return OrderDetailDAO.AddFoodItem(orderId, foodItem);
        }

        public decimal CalculateOrderTotal(int orderId)
        {
            return OrderDetailDAO.CalculateOrderTotal(orderId);
        }

        public List<OrderDetail> GetListOrderDetailByOrderId(int orderID)
        {
           return OrderDetailDAO.GetListOrderDetailByOrderId(orderID);
        }

        public void UpdateStatus(int detailId, int newStatus)
        {
            OrderDetailDAO.UpdateStatus(detailId, newStatus);
        }

        public List<OrderDetailDTO> GetKitchenOrderDetails()
        {
            return OrderDetailDAO.GetKitchenOrderDetails();
        }

        public (decimal FinalTotal, List<OrderDetailDTO> InProgressItems) ProcessOrderCheckout(int orderId)
        {
            return OrderDetailDAO.ProcessOrderCheckout(orderId);
        }
    }
}
