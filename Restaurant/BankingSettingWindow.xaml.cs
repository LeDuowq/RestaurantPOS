using BusinessObject;
using Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WPF
{
    public partial class BankingSettingWindow : Window
    {
        private readonly IBankingService _bankingService;
        private bool _isLoaded = false;

        public BankingSettingWindow()
        {
            InitializeComponent();
            _bankingService = new BankingService();
            LoadCurrentSettings();
            _isLoaded = true;
            UpdatePreviewQr();
        }

        private void LoadCurrentSettings()
        {
            // Nạp danh sách ngân hàng
            var banks = _bankingService.GetPopularBanks();
            cboBank.ItemsSource = banks;

            // Nạp cấu hình hiện tại
            var config = _bankingService.GetBankingConfig();

            var matchingBank = banks.FirstOrDefault(b => b.BankId.Equals(config.BankId, StringComparison.OrdinalIgnoreCase));
            if (matchingBank != null)
            {
                cboBank.SelectedItem = matchingBank;
            }
            else if (banks.Count > 0)
            {
                cboBank.SelectedIndex = 0;
            }

            txtAccountNumber.Text = config.AccountNumber;
            txtAccountName.Text = config.AccountName;

            // Template
            foreach (ComboBoxItem item in cboTemplate.Items)
            {
                if (item.Tag?.ToString() == config.Template)
                {
                    item.IsSelected = true;
                    break;
                }
            }
        }

        private void UpdatePreviewQr()
        {
            if (!_isLoaded) return;

            try
            {
                string bankId = (cboBank.SelectedItem as BankItem)?.BankId ?? "MB";
                string accountNo = txtAccountNumber.Text.Trim();
                string accountName = txtAccountName.Text.Trim().ToUpper();
                string template = (cboTemplate.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "compact2";

                if (string.IsNullOrWhiteSpace(accountNo))
                {
                    txtPreviewLoading.Visibility = Visibility.Visible;
                    txtPreviewLoading.Text = "Vui lòng nhập số tài khoản";
                    imgPreviewQr.Source = null;
                    return;
                }

                txtPreviewLoading.Visibility = Visibility.Visible;
                txtPreviewLoading.Text = "Đang tạo mã xem trước...";

                string safeName = Uri.EscapeDataString(accountName);
                string previewUrl = $"https://img.vietqr.io/image/{bankId}-{accountNo}-{template}.png?amount=50000&addInfo=Kiem%20tra%20VietQR&accountName={safeName}";

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(previewUrl, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;

                bitmap.DownloadCompleted += (s, e) =>
                {
                    txtPreviewLoading.Visibility = Visibility.Collapsed;
                };

                bitmap.DownloadFailed += (s, e) =>
                {
                    txtPreviewLoading.Visibility = Visibility.Visible;
                    txtPreviewLoading.Text = "Không tải được mã xem trước";
                };

                bitmap.EndInit();
                imgPreviewQr.Source = bitmap;
            }
            catch
            {
                txtPreviewLoading.Visibility = Visibility.Visible;
                txtPreviewLoading.Text = "Lỗi tạo mã xem trước";
            }
        }

        private void cboBank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePreviewQr();
        }

        private void txtAccountNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Chỉ cập nhật khi đã nhập xong một số ký tự
            if (txtAccountNumber.Text.Length >= 6)
            {
                UpdatePreviewQr();
            }
        }

        private void txtAccountName_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Không can thiệp cursor khi người dùng gõ
        }

        private void cboTemplate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePreviewQr();
        }

        private void btnPreviewQr_Click(object sender, RoutedEventArgs e)
        {
            // Chuẩn hóa tên viết hoa
            txtAccountName.Text = txtAccountName.Text.Trim().ToUpper();
            UpdatePreviewQr();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var selectedBank = cboBank.SelectedItem as BankItem;
            if (selectedBank == null)
            {
                MessageBox.Show("Vui lòng chọn ngân hàng thụ hưởng!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                cboBank.Focus();
                return;
            }

            string accountNo = txtAccountNumber.Text.Trim();
            if (string.IsNullOrWhiteSpace(accountNo))
            {
                MessageBox.Show("Vui lòng nhập số tài khoản ngân hàng!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtAccountNumber.Focus();
                return;
            }

            string accountName = txtAccountName.Text.Trim().ToUpper();
            if (string.IsNullOrWhiteSpace(accountName))
            {
                MessageBox.Show("Vui lòng nhập tên chủ tài khoản!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtAccountName.Focus();
                return;
            }

            string template = (cboTemplate.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "compact2";

            var newConfig = new BankingConfig
            {
                BankId = selectedBank.BankId,
                BankName = selectedBank.BankName,
                AccountNumber = accountNo,
                AccountName = accountName,
                Template = template
            };

            bool success = _bankingService.SaveBankingConfig(newConfig);
            if (success)
            {
                MessageBox.Show($"Đã lưu thành công cấu hình tài khoản ngân hàng!\n• Ngân hàng: {selectedBank.BankName}\n• STK: {accountNo}\n• Chủ TK: {accountName}\n\nMã VietQR trên màn hình thanh toán sẽ được cập nhật tự động ngay lập tức.", 
                    "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Không thể lưu cấu hình vào file appsettings.json. Vui lòng kiểm tra quyền ghi tập tin!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
