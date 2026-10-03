using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF;

namespace Restaurant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CheckUserRole();
        }

        private void CheckUserRole()
        {
            var user = AppSession.CurrentUser;
            if (user == null) return;
            if (user.Role == 1) // Admin
            {
                btnBankSetting.Visibility = Visibility.Visible;
                btnManagerAccount.Visibility = Visibility.Visible;
                btnFoodItems.Visibility = Visibility.Visible;
                btnCategories.Visibility = Visibility.Visible;
                btnDiningTables.Visibility = Visibility.Visible;
                btnReports.Visibility = Visibility.Visible;
                btnSales.Visibility = Visibility.Visible;
                btnKitchen.Visibility = Visibility.Visible;
            }
            else if (user.Role == 4) // Đầu bếp
            {
                btnBankSetting.Visibility = Visibility.Collapsed;
                btnManagerAccount.Visibility = Visibility.Collapsed;
                btnFoodItems.Visibility = Visibility.Collapsed;
                btnCategories.Visibility = Visibility.Collapsed;
                btnDiningTables.Visibility = Visibility.Collapsed;
                btnReports.Visibility = Visibility.Collapsed;
                btnSales.Visibility = Visibility.Collapsed;
                btnKitchen.Visibility = Visibility.Visible;
            }
            else // Thu ngân (2), Phục vụ (3)
            {
                btnBankSetting.Visibility = Visibility.Collapsed;
                btnManagerAccount.Visibility = Visibility.Collapsed;
                btnFoodItems.Visibility = Visibility.Collapsed;
                btnCategories.Visibility = Visibility.Collapsed;
                btnDiningTables.Visibility = Visibility.Collapsed;
                btnReports.Visibility = Visibility.Collapsed;
                btnSales.Visibility = Visibility.Visible;
                btnKitchen.Visibility = Visibility.Collapsed;
            }
        }

        private void OpenChildWindow(Window window)
        {
            window.Owner = this;
            window.Loaded += (s, e) => this.Hide();
            window.ShowDialog();
            this.Show();
        }

        private void btnSales_Click(object sender, RoutedEventArgs e)
        {
            OpenChildWindow(new SaleWindow());
        }

        private void btnKitchen_Click(object sender, RoutedEventArgs e)
        {
            OpenChildWindow(new KitchenWindow());
        }

        private void btnFoodItems_Click(object sender, RoutedEventArgs e)
        {
            OpenChildWindow(new FoodItemWindow());
        }

        private void btnCategories_Click(object sender, RoutedEventArgs e)
        {
            OpenChildWindow(new CategoryWindow());
        }

        private void btnDiningTables_Click(object sender, RoutedEventArgs e)
        {
            OpenChildWindow(new DiningTableWindow());
        }

        private void btnReports_Click(object sender, RoutedEventArgs e)
        {
            OpenChildWindow(new ReportWindow());
        }

        private void btnManagerAccount_Click(object sender, RoutedEventArgs e)
        {
            OpenChildWindow(new ManagerAccountWindow());
        }

        private void btnProfile_Click(object sender, RoutedEventArgs e)
        {
            ProfileWindow profileWin = new ProfileWindow();
            profileWin.Owner = this;
            profileWin.ShowDialog();
        }

        private void btnBankSetting_Click(object sender, RoutedEventArgs e)
        {
            BankingSettingWindow settingWindow = new BankingSettingWindow();
            settingWindow.Owner = this;
            settingWindow.ShowDialog();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            AppSession.CurrentUser = null;
            Login loginWindow = new Login(); 
            loginWindow.Show();
            this.Close();
        }
    }
}