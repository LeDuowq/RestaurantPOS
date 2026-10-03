using BusinessObject;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPF;

namespace Restaurant
{
    /// <summary>
    /// Interaction logic for SaleWindow.xaml
    /// </summary>
    public partial class SaleWindow : Window
    {
        private readonly IDiningTableService tableService;

        public SaleWindow()
        {
            InitializeComponent();
            tableService = new DiningTableService();
            LoadTables();
        }
        private void LoadTables()
        {
            try
            {
                icTables.ItemsSource = tableService.GetAllDiningTable();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách bàn: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnTable_Click(object sender, RoutedEventArgs e)
        {
            Button? clickedButton = sender as Button;

            if (clickedButton?.Tag is DiningTable selectedTable)
            {
                OrderWindow orderWindow = new OrderWindow(selectedTable);
                orderWindow.Owner = this;
                orderWindow.Loaded += (s, ev) => this.Hide();
                orderWindow.ShowDialog();
                this.Show();
                LoadTables();
            }
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            if (this.Owner != null)
            {
                this.Owner.Show();
            }
        }
    }
}
