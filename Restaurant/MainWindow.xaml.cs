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
            if (user.Role == 1)
            {
                btnManagerAccount.Visibility = Visibility.Visible;
                btnFoodItems.Visibility = Visibility.Visible;
                btnDiningTables.Visibility = Visibility.Visible;
                btnReports.Visibility = Visibility.Visible;
            }
            else
            {
                btnManagerAccount.Visibility = Visibility.Collapsed;
                btnFoodItems.Visibility = Visibility.Collapsed;
                btnDiningTables.Visibility = Visibility.Collapsed;
                btnReports.Visibility = Visibility.Collapsed;
            }
        }

        private void btnSales_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            SaleWindow saleWindow = new SaleWindow();
            saleWindow.ShowDialog();
            this.Show();
        }

        private void btnFoodItems_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            FoodItemWindow foodWindow = new FoodItemWindow();
            foodWindow.ShowDialog();
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
            ManagerAccountWindow manageWindow = new ManagerAccountWindow();
            manageWindow.Owner = this;
            this.Hide();
            manageWindow.Show();
            this.Show();
        }

        private void btnProfile_Click(object sender, RoutedEventArgs e)
        {
            ProfileWindow profileWin = new ProfileWindow();
            profileWin.Owner = this;
            this.Hide();
            profileWin.Show();
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