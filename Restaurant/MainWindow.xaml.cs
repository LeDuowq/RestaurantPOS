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
                btnManagerAccount.Visibility = Visibility.Collapsed;
                btnFoodItems.Visibility = Visibility.Collapsed;
                btnCategories.Visibility = Visibility.Collapsed;
                btnDiningTables.Visibility = Visibility.Collapsed;
                btnReports.Visibility = Visibility.Collapsed;
                btnSales.Visibility = Visibility.Visible;
                btnKitchen.Visibility = Visibility.Collapsed;
            }
        }

        private void btnSales_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            SaleWindow saleWindow = new SaleWindow();
            saleWindow.ShowDialog();
            this.Show();
        }

        private void btnKitchen_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            KitchenWindow kitchenWindow = new KitchenWindow();
            kitchenWindow.ShowDialog();
            this.Show();
        }

        private void btnFoodItems_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            FoodItemWindow foodWindow = new FoodItemWindow();
            foodWindow.ShowDialog();
            this.Show();
        }

        private void btnCategories_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            CategoryWindow categoryWindow = new CategoryWindow();
            categoryWindow.ShowDialog();
            this.Show();
        }

        private void btnDiningTables_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            DiningTableWindow tableWindow = new DiningTableWindow();
            tableWindow.ShowDialog();
            this.Show();
        }

        private void btnReports_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            ReportWindow reportWindow = new ReportWindow();
            reportWindow.ShowDialog();
            this.Show();
        }

        private void btnManagerAccount_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            ManagerAccountWindow manageWindow = new ManagerAccountWindow();
            manageWindow.ShowDialog();
            this.Show();
        }

        private void btnProfile_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            ProfileWindow profileWin = new ProfileWindow();
            profileWin.ShowDialog();
            this.Show();
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