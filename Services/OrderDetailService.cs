using BusinessObject;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly OrderDetailRepository orderDetailRepository;
        private readonly FoodItemRepository foodItemRepository;
        public OrderDetailService()
        {
            orderDetailRepository = new OrderDetailRepository();
            foodItemRepository = new FoodItemRepository();
        }
        public bool AddFoodItem(int orderId, FoodItem foodItem)
        {
            return orderDetailRepository.AddFoodItem(orderId, foodItem);
        }

        public decimal CalculateOrderTotal(int orderId)
        {
            return orderDetailRepository.CalculateOrderTotal(orderId);
        }

        public List<OrderDetail> GetListOrderDetailByOrderId(int orderID)
        {
            return orderDetailRepository.GetListOrderDetailByOrderId(orderID);
        }

        public List<OrderDetailDTO> GetOrderDetailsDisplay(int orderId)
        {
            var details = orderDetailRepository.GetListOrderDetailByOrderId(orderId);
            return details.Select(d => new OrderDetailDTO
            {
                DetailId = d.DetailId,
                OrderId = d.OrderId,
                FoodName = foodItemRepository.GetFoodById(d.FoodId)?.FoodName ?? "Không xác định",
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                TotalPrice = d.Quantity * d.UnitPrice,
                Status = d.Status
            }).ToList();
        }

        public void UpdateStatus(int detailId, int newStatus)
        {
            orderDetailRepository.UpdateStatus(detailId, newStatus);
        }

        public List<OrderDetailDTO> GetKitchenOrderDetails()
        {
            return orderDetailRepository.GetKitchenOrderDetails();
        }

        public (decimal FinalTotal, List<OrderDetailDTO> InProgressItems) ProcessOrderCheckout(int orderId)
        {
            return orderDetailRepository.ProcessOrderCheckout(orderId);
        }
    }
}
