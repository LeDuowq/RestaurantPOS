using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class FoodItem
{
    public int FoodId { get; set; }

    public string FoodName { get; set; } = null!;

    public decimal Price { get; set; }

    public int CategoryId { get; set; }


    public bool IsAvailable { get; set; } = true;

    public int Quantity { get; set; } = 0;

    public string? Img {  get; set; }


    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
