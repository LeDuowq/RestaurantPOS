using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IOrderDetailService
    {
        List<OrderDetail> GetListOrderDetailByOrderId(int orderID);
        bool AddFoodItem(int orderId, FoodItem foodItem);
        List<OrderDetailDTO> GetOrderDetailsDisplay(int orderId);
        decimal CalculateOrderTotal(int orderId);
        void UpdateStatus(int detailId, int newStatus);
        List<OrderDetailDTO> GetKitchenOrderDetails();
        (decimal FinalTotal, List<OrderDetailDTO> InProgressItems) ProcessOrderCheckout(int orderId);
    }
}
