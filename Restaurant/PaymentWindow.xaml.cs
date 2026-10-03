using BusinessObject;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WPF
{
    public partial class PaymentWindow : Window
    {
        private readonly Order _order;
        private readonly DiningTable _table;
        private readonly decimal _finalTotal;
        private readonly List<OrderDetailDTO>? _inProgressItems;
        private readonly IOrderService _orderService;
        private readonly IDiningTableService _diningTableService;
        private readonly IBankingService _bankingService;
        private string _transferContent = "";

        public PaymentWindow(Order order, DiningTable table, decimal finalTotal, List<OrderDetailDTO>? inProgressItems)
        {
            InitializeComponent();
            _order = order;
            _table = table;
            _finalTotal = finalTotal;
            _inProgressItems = inProgressItems;

            _orderService = new OrderService();
            _diningTableService = new DiningTableService();
            _bankingService = new BankingService();

            InitData();
        }

        private void InitData()
        {
            txtHeaderTable.Text = $" {_table.TableName.ToUpper()}";
            txtHeaderOrder.Text = $"Hóa đơn #{_order.OrderId}";

            txtTableName.Text = _table.TableName;
            txtOrderId.Text = $"#{_order.OrderId}";
            txtStaffName.Text = AppSession.CurrentUser?.FullName ?? "Thu ngân";
            txtOrderTime.Text = _order.OrderDate.ToString("dd/MM/yyyy HH:mm");
            txtTotalAmount.Text = $"{_finalTotal:N0} VNĐ";

            var bankingConfig = _bankingService.GetBankingConfig();
            txtBankName.Text = bankingConfig.BankName;
            txtAccountNumber.Text = bankingConfig.AccountNumber;
            txtAccountName.Text = bankingConfig.AccountName;

            // Nội dung chuyển khoản chuẩn hóa, không dấu để ngân hàng xử lý mượt mà
            _transferContent = $"Thanh toan Ban {_table.TableId} HD{_order.OrderId}";
            txtTransferContent.Text = _transferContent;

            // Chỉ Admin mới có quyền đổi tài khoản ngân hàng
            if (AppSession.CurrentUser?.Role != 1)
            {
                btnConfigBank.Visibility = Visibility.Collapsed;
            }

            // Load mã QR
            LoadVietQr();
        }

        private void LoadVietQr()
        {
            try
            {
                txtQrLoading.Visibility = Visibility.Visible;
                txtQrLoading.Text = "Đang tạo mã VietQR...";

                string qrUrl = _bankingService.GenerateVietQrUrl(_finalTotal, _transferContent);

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(qrUrl, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;

                bitmap.DownloadCompleted += (s, e) =>
                {
                    txtQrLoading.Visibility = Visibility.Collapsed;
                };

                bitmap.DownloadFailed += (s, e) =>
                {
                    txtQrLoading.Visibility = Visibility.Visible;
                    txtQrLoading.Text = "Không tải được mã QR. Vui lòng kiểm tra kết nối mạng!";
                };

                bitmap.EndInit();
                imgQrCode.Source = bitmap;
            }
            catch (Exception ex)
            {
                txtQrLoading.Visibility = Visibility.Visible;
                txtQrLoading.Text = "Lỗi tạo mã QR: " + ex.Message;
            }
        }

        private void btnReloadQr_Click(object sender, RoutedEventArgs e)
        {
            LoadVietQr();
        }

        private void btnConfigBank_Click(object sender, RoutedEventArgs e)
        {
            BankingSettingWindow settingWindow = new BankingSettingWindow();
            if (settingWindow.ShowDialog() == true)
            {
                var bankingConfig = _bankingService.GetBankingConfig();
                txtBankName.Text = bankingConfig.BankName;
                txtAccountNumber.Text = bankingConfig.AccountNumber;
                txtAccountName.Text = bankingConfig.AccountName;
                LoadVietQr();
            }
        }

        private void txtCashGiven_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateCashChange();
        }

        private void CalculateCashChange()
        {
            string rawText = txtCashGiven.Text.Replace(".", "").Replace(",", "").Trim();
            if (decimal.TryParse(rawText, out decimal cashGiven))
            {
                decimal change = cashGiven - _finalTotal;
                if (change >= 0)
                {
                    txtChangeAmount.Text = $"{change:N0} VNĐ";
                    txtChangeAmount.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(39, 174, 96)); // Xanh lá
                }
                else
                {
                    txtChangeAmount.Text = $"Còn thiếu {Math.Abs(change):N0} VNĐ";
                    txtChangeAmount.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(231, 76, 60)); // Đỏ
                }
            }
            else
            {
                txtChangeAmount.Text = "0 VNĐ";
                txtChangeAmount.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(127, 140, 141));
            }
        }

        private void btnQuickCash_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string tag)
            {
                if (tag == "EXACT")
                {
                    txtCashGiven.Text = $"{_finalTotal:N0}";
                }
                else if (decimal.TryParse(tag, out decimal val))
                {
                    txtCashGiven.Text = $"{val:N0}";
                }
            }
        }

        private void btnConfirmPayment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra nếu nhập tiền mặt mà chưa đủ
                string rawText = txtCashGiven.Text.Replace(".", "").Replace(",", "").Trim();
                if (!string.IsNullOrEmpty(rawText) && decimal.TryParse(rawText, out decimal cashGiven))
                {
                    if (cashGiven < _finalTotal)
                    {
                        var confirmShort = MessageBox.Show($"Số tiền khách đưa ({cashGiven:N0} VNĐ) chưa đủ tổng tiền ({_finalTotal:N0} VNĐ). Bạn có chắc chắn vẫn muốn xác nhận thanh toán?",
                            "Cảnh báo chưa đủ tiền", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        if (confirmShort != MessageBoxResult.Yes)
                        {
                            return;
                        }
                    }
                }

                // Thông báo món đang chế biến nếu có
                if (_inProgressItems != null && _inProgressItems.Count > 0)
                {
                    string itemsStr = string.Join("\n", _inProgressItems.Select(x => $"• {x.FoodName} (SL: {x.Quantity})"));
                    MessageBox.Show($"⚠️ THÔNG BÁO CHO THU NGÂN:\nCác món sau đây đang được Bếp chế biến:\n{itemsStr}\n\nThu ngân vui lòng báo khách món đang chế biến và hỗ trợ đóng gói mang về (Takeout)!",
                        "Món ăn đang chế biến", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                // Cập nhật trạng thái Order và Bàn
                _order.TotalPrice = _finalTotal;
                _order.Status = 1;
                _order.CheckoutDate = DateTime.Now;

                _orderService.UpdateOrder(_order);
                _diningTableService.UpdateStatus(_table.TableId, 0);

                MessageBox.Show($"Thanh toán thành công cho {_table.TableName}!\nTổng tiền: {_finalTotal:N0} VNĐ", 
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi trong quá trình hoàn tất thanh toán: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
