using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IOrderDetailRepository
    {
        public List<OrderDetail> GetListOrderDetailByOrderId(int orderID);
        public bool AddFoodItem(int orderId, FoodItem foodItem);
        public decimal CalculateOrderTotal(int orderId);
        public void UpdateStatus(int detailId, int newStatus);
        public List<OrderDetailDTO> GetKitchenOrderDetails();
        public (decimal FinalTotal, List<OrderDetailDTO> InProgressItems) ProcessOrderCheckout(int orderId);
    }
}
