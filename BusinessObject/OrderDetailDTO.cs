using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject
{
    public class OrderDetailDTO
    {
        public int DetailId { get; set; }
        public int OrderId { get; set; }
        public string TableName { get; set; } = string.Empty;
        public string FoodName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public int Status { get; set; }
        public string StatusText => Status switch
        {
            0 => "Chờ chế biến",
            1 => "Đang chế biến",
            2 => "Hoàn thành",
            _ => "Không xác định"
        };
    }
}
