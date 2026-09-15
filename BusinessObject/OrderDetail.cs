using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class OrderDetail
{
    public int DetailId { get; set; }

    public int OrderId { get; set; }

    public int FoodId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public int Status { get; set; } = 0;


    public virtual FoodItem Food { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
