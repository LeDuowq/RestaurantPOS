using BusinessObject;
using System;
using System.Collections.Generic;

namespace Services
{
    public interface IReportService
    {
        List<Order> GetRevenueReport(DateTime fromDate, DateTime toDate);
        List<TopSellingFoodDTO> GetTopSellingFoods(DateTime fromDate, DateTime toDate, bool sortByRevenue = false);
    }
}
