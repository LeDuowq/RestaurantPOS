using BusinessObject; 
using Services;      
using System;
using System.Windows;
using System.Windows.Controls;

namespace WPF
{
    public partial class OrderWindow : Window
    {
        private DiningTable currentTable;
        private Order currentOrder;
        private readonly IFoodItemService foodItemService;
        private readonly ICategoryService categoryService;
        private readonly IDiningTableService ingoutTableService;
        private readonly IOrderService orderService;
        private readonly IOrderDetailService orderDetailService;
        public OrderWindow(DiningTable table)
        {
            InitializeComponent();
            currentTable = table;
            txtTableName.Text = $"Hóa đơn - {currentTable.TableName}";
            categoryService = new CategoryService();
            foodItemService = new FoodItemService();
            ingoutTableService = new DiningTableService();
            orderService = new OrderService();
            orderDetailService = new OrderDetailService();

            LoadMenu();
            SetupOrder();
            LoadCategory();
            ApplyRolePermissions();
        }

        private void ApplyRolePermissions()
        {
            var user = AppSession.CurrentUser;
            if (user == null) return;

            if (user.Role == 3) // Phục vụ / Bồi bàn
            {
                btnPayment.Visibility = Visibility.Collapsed;
                icMenu.IsEnabled = false; // Vô hiệu hóa vùng chọn món thực đơn
                borderWaitstaffNotice.Visibility = Visibility.Visible;
            }
            else // Admin (1) & Thu ngân (2)
            {
                btnPayment.Visibility = Visibility.Visible;
                icMenu.IsEnabled = true;
                borderWaitstaffNotice.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadCategory()
        {
            try
            {
                var listCategory = categoryService.GetAllCategory();
                listCategory.Insert(0, new Category { CategoryId = 0, CategoryName = "--- Tất cả ---" });
                cboCategory.ItemsSource = listCategory;
                cboCategory.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải category: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void cboCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboCategory.SelectedValue != null)
            {
                if (int.TryParse(cboCategory.SelectedValue.ToString(), out int selectedCategoryId))
                {
                    if (selectedCategoryId == 0)
                    {
                        LoadMenu();
                    }
                    else
                    {
                        icMenu.ItemsSource = foodItemService.FilterFoodIitem(selectedCategoryId);
                    }
                }
            }
        }


        private void LoadMenu()
        {
            try
            {
                var menuItems = foodItemService.ShowAllFoodItem();
                icMenu.ItemsSource = menuItems;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải thực đơn: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetupOrder()
        {
            currentOrder = orderService.GetOrder(currentTable.TableId,0);

            if (currentOrder == null)
            {
                currentOrder = new Order
                {
                    TableId = currentTable.TableId,
                    Status = 0,
                    AccountId = AppSession.CurrentUser.AccountId,
                    OrderDate = DateTime.Now,
                    TotalPrice = 0
                };
                orderService.AddOrder(currentOrder);
            }
            else
            {
                LoadOrderDetails();
            }
        }
        private void LoadOrderDetails()
        {
            var displayList = orderDetailService.GetOrderDetailsDisplay(currentOrder.OrderId);
            dgOrderDetails.ItemsSource = displayList;
            decimal total = orderDetailService.CalculateOrderTotal(currentOrder.OrderId);
            txtTotalPrice.Text = $"{total:N0} VNĐ";
        }
        private void btnPayment_Click(object sender, RoutedEventArgs e)
        {
            if (AppSession.CurrentUser?.Role == 3)
            {
                MessageBox.Show("Bồi bàn không có quyền thanh toán hóa đơn!", "Cảnh báo phân quyền", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                var result = MessageBox.Show($"Bạn có chắc chắn muốn thanh toán hóa đơn cho {currentTable.TableName} không?", 
                    "Xác nhận thanh toán", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
                if (result == MessageBoxResult.Yes)
                {
                    var (finalTotal, inProgressItems) = orderDetailService.ProcessOrderCheckout(currentOrder.OrderId);

                    if (inProgressItems != null && inProgressItems.Count > 0)
                    {
                        string itemsStr = string.Join("\n", inProgressItems.Select(x => $"• {x.FoodName} (SL: {x.Quantity})"));
                        MessageBox.Show($"⚠️ THÔNG BÁO CHO THU NGÂN:\nCác món sau đây đang được Bếp chế biến:\n{itemsStr}\n\nThu ngân vui lòng báo khách món đang chế biến và hỗ trợ đóng gói mang về (Takeout)!", 
                            "Món ăn đang chế biến", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    currentOrder.TotalPrice = finalTotal;
                    currentOrder.Status = 1;
                    currentOrder.CheckoutDate = DateTime.Now;

                    orderService.UpdateOrder(currentOrder);
                    ingoutTableService.UpdateStatus(currentTable.TableId, 0);

                    MessageBox.Show($"Thanh toán thành công!\nTổng tiền: {finalTotal:N0} VNĐ", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi trong quá trình thanh toán: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnFoodItem_Click(object sender, RoutedEventArgs e)
        {
            if (AppSession.CurrentUser?.Role == 3)
            {
                MessageBox.Show("Bồi bàn chỉ có quyền xem danh sách món đã order!", "Cảnh báo phân quyền", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Button? clickedButton = sender as Button;
            if (clickedButton?.Tag is FoodItem selectedFood)
            {
                bool success = orderDetailService.AddFoodItem(currentOrder.OrderId, selectedFood);
                if (!success)
                {
                    MessageBox.Show($"Món '{selectedFood.FoodName}' đã hết số lượng suất ăn phục vụ!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                LoadOrderDetails();
                LoadMenu(); // Cập nhật số suất còn lại trên giao diện thực đơn
                if (currentTable.Status == 0)
                {
                    ingoutTableService.UpdateStatus(currentTable.TableId, 1);
                    currentTable.Status = 1;
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}