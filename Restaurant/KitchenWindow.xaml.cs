using BusinessObject;
using Services;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Restaurant
{
    public partial class KitchenWindow : Window
    {
        private readonly IOrderDetailService orderDetailService;
        private readonly IFoodItemService foodItemService;

        public KitchenWindow()
        {
            InitializeComponent();
            orderDetailService = new OrderDetailService();
            foodItemService = new FoodItemService();

            LoadPendingOrders();
            LoadStockItems();
        }

        private void LoadPendingOrders()
        {
            try
            {
                var list = orderDetailService.GetKitchenOrderDetails();
                dgPendingOrders.ItemsSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách chế biến: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadStockItems()
        {
            try
            {
                var foodList = foodItemService.GetFoodItems();
                dgStockItems.ItemsSource = foodList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách món ăn: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRefreshKitchen_Click(object sender, RoutedEventArgs e)
        {
            LoadPendingOrders();
        }

        private void btnStartCook_Click(object sender, RoutedEventArgs e)
        {
            if (dgPendingOrders.SelectedItem is OrderDetailDTO selected)
            {
                try
                {
                    orderDetailService.UpdateStatus(selected.DetailId, 1); // 1: Đang chế biến
                    MessageBox.Show($"Đã chuyển món '{selected.FoodName}' của {selected.TableName} sang trạng thái Đang chế biến!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadPendingOrders();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi cập nhật trạng thái: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn 1 món từ danh sách để bắt đầu chế biến!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnFinishCook_Click(object sender, RoutedEventArgs e)
        {
            if (dgPendingOrders.SelectedItem is OrderDetailDTO selected)
            {
                try
                {
                    orderDetailService.UpdateStatus(selected.DetailId, 2); // 2: Hoàn thành
                    MessageBox.Show($"Món '{selected.FoodName}' của {selected.TableName} đã hoàn thành!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadPendingOrders();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi cập nhật trạng thái: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn 1 món từ danh sách để hoàn thành!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void dgStockItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgStockItems.SelectedItem is FoodItem selected)
            {
                txtSelectedFoodName.Text = $"{selected.FoodName} (Số lượng hiện tại: {selected.Quantity})";
            }
            else
            {
                txtSelectedFoodName.Text = "(Vui lòng chọn món bên dưới)";
            }
        }

        private void btnDecrease_Click(object sender, RoutedEventArgs e)
        {
            if (dgStockItems.SelectedItem is FoodItem item)
            {
                AdjustItemQuantity(item, -1);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn món trong bảng trước!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnIncrease_Click(object sender, RoutedEventArgs e)
        {
            if (dgStockItems.SelectedItem is FoodItem item)
            {
                AdjustItemQuantity(item, 1);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn món trong bảng trước!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnDecreaseRow_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is FoodItem item)
            {
                AdjustItemQuantity(item, -1);
            }
        }

        private void btnIncreaseRow_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is FoodItem item)
            {
                AdjustItemQuantity(item, 1);
            }
        }

        private void AdjustItemQuantity(FoodItem item, int delta)
        {
            int newQty = item.Quantity + delta;
            if (newQty < 0)
            {
                MessageBox.Show("Số lượng suất ăn không thể nhỏ hơn 0!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                item.Quantity = newQty;
                foodItemService.UpdateFoodItem(item);
                dgStockItems.Items.Refresh();
                if (dgStockItems.SelectedItem == item)
                {
                    txtSelectedFoodName.Text = $"{item.FoodName} (Số lượng hiện tại: {item.Quantity})";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật số lượng suất ăn: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSaveStock_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is FoodItem item)
            {
                try
                {
                    foodItemService.UpdateFoodItem(item);
                    MessageBox.Show($"Đã lưu số lượng ({item.Quantity} suất) cho món '{item.FoodName}' thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadStockItems();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi cập nhật suất ăn: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
