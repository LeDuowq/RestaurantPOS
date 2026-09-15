using BusinessObject;
using Microsoft.Win32;
using Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Restaurant
{
    public partial class ReportWindow : Window
    {
        private readonly IReportService reportService = new ReportService();
        private readonly IOrderDetailService orderDetailService = new OrderDetailService();

        public ReportWindow()
        {
            InitializeComponent();
            // Mặc định chọn thống kê 7 ngày gần nhất
            dpFromDate.SelectedDate = DateTime.Today.AddDays(-7);
            dpToDate.SelectedDate = DateTime.Today;
            
            LoadData();
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void cboSortOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpFromDate.SelectedDate != null && dpToDate.SelectedDate != null)
            {
                LoadTopSellingData();
            }
        }

        private void LoadData()
        {
            if (dpFromDate.SelectedDate == null || dpToDate.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn khoảng thời gian hợp lệ!", "Cảnh báo");
                return;
            }

            DateTime fromDate = dpFromDate.SelectedDate.Value;
            DateTime toDate = dpToDate.SelectedDate.Value;

            if (fromDate > toDate)
            {
                MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc!", "Cảnh báo");
                return;
            }

            // 1. Tải dữ liệu Doanh thu Hóa đơn
            var revenueData = reportService.GetRevenueReport(fromDate, toDate);
            dgRevenue.ItemsSource = revenueData;
            dgOrderDetails.ItemsSource = null; // Reset chi tiết món

            // Tính tổng tiền
            decimal total = revenueData.Sum(o => o.TotalPrice ?? 0);
            txtTotalRevenue.Text = total.ToString("N0") + " VNĐ";

            // 2. Tải dữ liệu Thống kê Món ăn
            LoadTopSellingData();
        }

        private void LoadTopSellingData()
        {
            if (dpFromDate.SelectedDate == null || dpToDate.SelectedDate == null) return;

            DateTime fromDate = dpFromDate.SelectedDate.Value;
            DateTime toDate = dpToDate.SelectedDate.Value;

            bool sortByRevenue = cboSortOption?.SelectedIndex == 1;

            // Lấy danh sách sắp xếp theo doanh thu để tìm Món Doanh Thu Cao Nhất
            var topRevenueList = reportService.GetTopSellingFoods(fromDate, toDate, sortByRevenue: true);
            if (topRevenueList != null && topRevenueList.Count > 0)
            {
                txtTopRevenueFood.Text = topRevenueList[0].FoodName;
                txtTopRevenueAmount.Text = topRevenueList[0].TotalRevenue.ToString("N0") + " VNĐ";
            }
            else
            {
                txtTopRevenueFood.Text = "Chưa có dữ liệu";
                txtTopRevenueAmount.Text = "0 VNĐ";
            }

            // Nạp dữ liệu lên DataGrid theo tiêu chí người dùng chọn
            var foodList = reportService.GetTopSellingFoods(fromDate, toDate, sortByRevenue);
            dgTopSelling.ItemsSource = foodList;
        }

        private void dgRevenue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgRevenue.SelectedItem is Order selectedOrder)
            {
                try
                {
                    var details = orderDetailService.GetOrderDetailsDisplay(selectedOrder.OrderId);
                    dgOrderDetails.ItemsSource = details;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải chi tiết món ăn: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnExportCsv_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSV File|*.csv";
                sfd.Title = "Lưu báo cáo thống kê";
                sfd.FileName = "BaoCaoThongKe_" + DateTime.Now.ToString("ddMMyyyy");

                if (sfd.ShowDialog() == true)
                {
                    StringBuilder csv = new StringBuilder();
                    
                    if (tabMain.SelectedIndex == 0) 
                    {
                        // Tab Doanh thu
                        csv.AppendLine("Ma HD,Ban,Thu ngan,Gio vao,Gio ra,Giam gia,Thanh tien");
                        var data = dgRevenue.ItemsSource as List<Order>;
                        if (data != null)
                        {
                            foreach (var item in data)
                            {
                                string outTime = item.CheckoutDate?.ToString("dd/MM/yyyy HH:mm") ?? "";
                                csv.AppendLine($"{item.OrderId},{item.Table?.TableName},{item.Account?.FullName},{item.OrderDate:dd/MM/yyyy HH:mm},{outTime},{item.Discount},{item.TotalPrice}");
                            }
                        }
                    }
                    else 
                    {
                        // Tab Món bán chạy & Doanh thu
                        csv.AppendLine("Ma Mon,Ten Mon An,So Luong Da Ban,Tong Tien Thu Ve");
                        var data = dgTopSelling.ItemsSource as List<TopSellingFoodDTO>;
                        if (data != null)
                        {
                            foreach (var item in data)
                            {
                                csv.AppendLine($"{item.FoodId},{item.FoodName},{item.TotalQuantitySold},{item.TotalRevenue}");
                            }
                        }
                    }

                    File.WriteAllText(sfd.FileName, csv.ToString(), Encoding.UTF8);
                    MessageBox.Show("Xuất file báo cáo thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất file: " + ex.Message, "Lỗi");
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
