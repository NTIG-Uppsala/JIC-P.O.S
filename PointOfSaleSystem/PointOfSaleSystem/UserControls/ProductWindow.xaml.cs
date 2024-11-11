using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace PointOfSaleSystem.UserControls
{
    public partial class ProductWindow : UserControl
    {
        public ProductWindow()
        {
            InitializeComponent();
            DataContext = this;
            Products = new ObservableCollection<ProductItem>();
            ProductListView.ItemsSource = Products; // Initialize the ListView to use the Products collection as its source
        }

        public ObservableCollection<ProductItem> Products { get; set; }

        public void AddProduct(string productName, int productPrice)
        {
            var existingProduct = Products.FirstOrDefault(p => p.ProductName == productName);
            if (existingProduct != null) // Add to product price and amount if product of this name has already been added
            {
                existingProduct.ProductAmount++;
                existingProduct.ProductPrice += productPrice;
            }
            else
            {
                Products.Add(new ProductItem // Initialize product if not yet added
                {
                    ProductName = productName,
                    ProductAmount = 1,
                    ProductPrice = productPrice
                });
            }
        }
        public void ClearProducts()
        {
            Products.Clear();
        }
    }
    public class ProductItem : INotifyPropertyChanged
    {
        private string productName;
        private int productAmount;
        private int productPrice;

        // Create bindings used for each value in the columns of each row of products in the product window
        public string ProductName
        {
            get => productName;
            set
            {
                productName = value;
            }
        }

        public int ProductAmount
        {
            get => productAmount;
            set
            {
                productAmount = value;
                OnPropertyChanged();
            }
        }

        public int ProductPrice
        {
            get => productPrice;
            set
            {
                productPrice = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged; // Notify subscribers to the ProductItem class when a property value changes

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) // Gets called directly on the name of the property calling the function
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); // If not null raises the event with the name of the property that changed
        }
    }
}