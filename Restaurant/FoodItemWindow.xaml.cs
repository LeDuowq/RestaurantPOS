using BusinessObject;
using Microsoft.Win32;
using Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Restaurant
{
    public partial class FoodItemWindow : Window
    {
        private IFoodItemService foodService = new FoodItemService();
        private ICategoriesService categoryService = new CategoriesService();

        public FoodItemWindow()
        {
            InitializeComponent();
            LoadCategories(); // Phải load danh mục vào ComboBox trước
            LoadFoodItems();
        }

        private void LoadCategories()
        {
            cbCategory.ItemsSource = categoryService.GetCategories();
        }

        private void LoadFoodItems()
        {
            dgFoodItems.ItemsSource = foodService.GetFoodItems();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFoodName.Text) || string.IsNullOrWhiteSpace(txtPrice.Text) || cbCategory.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng điền đủ thông tin và chọn danh mục!", "Cảnh báo");
                return;
            }

            try
            {
                int quantity = 0;
                if (!string.IsNullOrWhiteSpace(txtQuantity.Text))
                {
                    int.TryParse(txtQuantity.Text, out quantity);
                }

                FoodItem newItem = new FoodItem
                {
                    FoodName = txtFoodName.Text,
                    Price = decimal.Parse(txtPrice.Text),
                    Quantity = quantity,
                    CategoryId = (int)cbCategory.SelectedValue,
                    Img = string.IsNullOrWhiteSpace(txtImg.Text) ? null : txtImg.Text.Trim(),
                    IsAvailable = chkIsAvailable.IsChecked ?? true
                };

                foodService.AddFoodItem(newItem);
                MessageBox.Show("Thêm món ăn thành công!");
                LoadFoodItems();
                btnClear_Click(null, null); // Tự động làm mới ô nhập
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: Giá tiền và số lượng phải là số hợp lệ. " + ex.Message, "Lỗi");
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFoodId.Text) || string.IsNullOrWhiteSpace(txtFoodName.Text) || string.IsNullOrWhiteSpace(txtPrice.Text) || cbCategory.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn món ăn và điền đầy đủ thông tin!", "Cảnh báo");
                return;
            }

            try
            {
                int quantity = 0;
                if (!string.IsNullOrWhiteSpace(txtQuantity.Text))
                {
                    int.TryParse(txtQuantity.Text, out quantity);
                }

                FoodItem updateItem = new FoodItem
                {
                    FoodId = int.Parse(txtFoodId.Text),
                    FoodName = txtFoodName.Text,
                    Price = decimal.Parse(txtPrice.Text),
                    Quantity = quantity,
                    CategoryId = (int)cbCategory.SelectedValue,
                    Img = string.IsNullOrWhiteSpace(txtImg.Text) ? null : txtImg.Text.Trim(),
                    IsAvailable = chkIsAvailable.IsChecked ?? true
                };

                foodService.UpdateFoodItem(updateItem);
                MessageBox.Show("Cập nhật thành công!");
                LoadFoodItems();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: Giá tiền và số lượng phải là số hợp lệ. " + ex.Message, "Lỗi");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFoodId.Text))
            {
                MessageBox.Show("Vui lòng chọn món ăn cần xóa!", "Cảnh báo");
                return;
            }

            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa món này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                foodService.DeleteFoodItem(int.Parse(txtFoodId.Text));
                MessageBox.Show("Xóa thành công!");
                LoadFoodItems();
                btnClear_Click(null, null);
            }
        }

        private void btnClear_Click(object? sender, RoutedEventArgs? e)
        {
            txtFoodId.Clear();
            txtFoodName.Clear();
            txtPrice.Clear();
            txtQuantity.Clear();
            txtImg.Clear();
            imgPreview.Source = null;
            cbCategory.SelectedItem = null;
            chkIsAvailable.IsChecked = true; // Trả về mặc định
            dgFoodItems.SelectedItem = null;
        }

        private void btnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.webp)|*.png;*.jpg;*.jpeg;*.webp|All Files (*.*)|*.*";
                dialog.Title = "Chọn hình ảnh món ăn";

                if (dialog.ShowDialog() == true)
                {
                    string sourceFilePath = dialog.FileName;
                    string fileName = Path.GetFileName(sourceFilePath);
                    string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

                    if (!Directory.Exists(imagesFolder))
                    {
                        Directory.CreateDirectory(imagesFolder);
                    }

                    string destFilePath = Path.Combine(imagesFolder, fileName);
                    if (!File.Exists(destFilePath))
                    {
                        File.Copy(sourceFilePath, destFilePath, true);
                    }

                    string relativePath = $"/Images/{fileName}";
                    txtImg.Text = relativePath;
                    imgPreview.Source = new BitmapImage(new Uri(destFilePath, UriKind.Absolute));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi chọn ảnh: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnManageCategories_Click(object sender, RoutedEventArgs e)
        {
            CategoryWindow catWin = new CategoryWindow();
            catWin.ShowDialog();
            LoadCategories();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dgFoodItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgFoodItems.SelectedItem is FoodItem selectedItem)
            {
                txtFoodId.Text = selectedItem.FoodId.ToString();
                txtFoodName.Text = selectedItem.FoodName;
                txtPrice.Text = selectedItem.Price.ToString("0.##"); // Bỏ số 0 vô nghĩa ở đuôi
                txtQuantity.Text = selectedItem.Quantity.ToString();
                txtImg.Text = selectedItem.Img ?? "";
                cbCategory.SelectedValue = selectedItem.CategoryId; // Tự động chọn đúng Danh mục trên ComboBox
                chkIsAvailable.IsChecked = selectedItem.IsAvailable;

                if (!string.IsNullOrWhiteSpace(selectedItem.Img))
                {
                    try
                    {
                        imgPreview.Source = new BitmapImage(new Uri(selectedItem.Img, UriKind.RelativeOrAbsolute));
                    }
                    catch
                    {
                        imgPreview.Source = null;
                    }
                }
                else
                {
                    imgPreview.Source = null;
                }
            }
        }
    }
}
