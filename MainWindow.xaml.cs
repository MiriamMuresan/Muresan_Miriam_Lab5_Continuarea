using AutoGeistModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Muresan_Miriam_Lab5
{
    enum ActionState { New, Edit, Delete, Nothing }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ActionState action = ActionState.Nothing;
        AutoGeistEntitiesModel ctx = new AutoGeistEntitiesModel(); 
        CollectionViewSource carViewSource;
        CollectionViewSource customerViewSource;
        CollectionViewSource carOrdersViewSource;

        Binding bodyStyleTextBoxBinding = new Binding(); 
        Binding modelTextBoxBinding = new Binding(); 
        Binding makeTextBoxBinding = new Binding();
        Binding firstNameTextBoxBinding = new Binding();
        Binding lastNameTextBoxBinding = new Binding();
        Binding purchaseDateBinding = new Binding();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            modelTextBoxBinding.Path = new PropertyPath("Model"); 
            makeTextBoxBinding.Path = new PropertyPath("Make"); 
            bodyStyleTextBoxBinding.Path = new PropertyPath("BodyStyle"); 
            modelTextBox.SetBinding(TextBox.TextProperty, modelTextBoxBinding); 
            makeTextBox.SetBinding(TextBox.TextProperty, makeTextBoxBinding); 
            bodyStyleTextBox.SetBinding(TextBox.TextProperty, bodyStyleTextBoxBinding);

            firstNameTextBoxBinding.Path = new PropertyPath("FirstName");
            lastNameTextBoxBinding.Path = new PropertyPath("LastName");
            firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameTextBoxBinding);
            lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameTextBoxBinding);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            System.Windows.Data.CollectionViewSource carViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("carViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // carViewSource.Source = [generic data source]
            System.Windows.Data.CollectionViewSource customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // customerViewSource.Source = [generic data source]

            carViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("carViewSource"))); 
            carViewSource.Source = ctx.Cars.Local; 
            ctx.Cars.Load();

            carViewSource =
           ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            customerViewSource.Source = ctx.Customers.Local;
            ctx.Customers.Load();

            carOrdersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("carOrdersViewSource"))); 
           // carOrdersViewSource.Source = ctx.Orders.Local; 
            ctx.Orders.Load();
            BindDataGrid();
            cbCars.ItemsSource = ctx.Cars.Local; 
            // cbCars.DisplayMemberPath = "Make"; 
            cbCars.SelectedValuePath = "CarId";
            cbCustomers.ItemsSource = ctx.Customers.Local; 
            // cbCustomers.DisplayMemberPath = "FirstName"; 
            cbCustomers.SelectedValuePath = "CustId";
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New; 
            TabItem ti = tbCtrlAutoGeist.SelectedItem as TabItem; 
            switch (ti.Header) 
            { 
                case "Cars": 
                    BindingOperations.ClearBinding(bodyStyleTextBox, TextBox.TextProperty); 
                    BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty); 
                    BindingOperations.ClearBinding(modelTextBox, TextBox.TextProperty); 
                    bodyStyleTextBox.Text = ""; makeTextBox.Text = ""; modelTextBox.Text = ""; 
                    Keyboard.Focus(bodyStyleTextBox); 
                    break; 
                case "Customers": 
                    BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty); 
                    BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty); 
                    firstNameTextBox.Text = ""; 
                    lastNameTextBox.Text = ""; 
                    Keyboard.Focus(firstNameTextBox);
                    break; 
                case "Orders": 
                    break; 
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            carViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            carViewSource.View.MoveCurrentToNext();
        }

        private void SaveCars()
        {
            Car car = null;
            if (action == ActionState.New)
            {
                try
                {
                    car = new Car()
                    {
                        Make = makeTextBox.Text.Trim(),
                        Model = modelTextBox.Text.Trim(),
                        BodyStyle = bodyStyleTextBox.Text.Trim(),
                    };
                    ctx.Cars.Add(car);
                    carViewSource.View.Refresh();
                    carViewSource.View.MoveCurrentTo(car);
                    ctx.SaveChanges();
                }
                // using System.Data;
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                makeTextBox.SetBinding(TextBox.TextProperty, makeTextBoxBinding);
                modelTextBox.SetBinding(TextBox.TextProperty, modelTextBoxBinding);
                bodyStyleTextBox.SetBinding(TextBox.TextProperty, bodyStyleTextBoxBinding);
            }
            else if (action == ActionState.Edit)
            {
                try
                {
                    car = (Car)carDataGrid.SelectedItem;
                    car.Make = makeTextBox.Text.Trim();
                    car.Model = modelTextBox.Text.Trim();
                    car.BodyStyle = bodyStyleTextBox.Text.Trim();
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                carViewSource.View.Refresh();
                carViewSource.View.MoveCurrentTo(car);
                makeTextBox.SetBinding(TextBox.TextProperty, makeTextBoxBinding);
                modelTextBox.SetBinding(TextBox.TextProperty, modelTextBoxBinding);
                bodyStyleTextBox.SetBinding(TextBox.TextProperty, bodyStyleTextBoxBinding);
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    car = (Car)carDataGrid.SelectedItem;
                    ctx.Cars.Remove(car);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                carViewSource.View.Refresh();
                makeTextBox.SetBinding(TextBox.TextProperty, makeTextBoxBinding);
                modelTextBox.SetBinding(TextBox.TextProperty, modelTextBoxBinding);
                bodyStyleTextBox.SetBinding(TextBox.TextProperty, bodyStyleTextBoxBinding);
            }
        }

        private void btnPrev1_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNext1_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToNext();
        }

        private void SaveCustomers()
        {
            Customer customer = null;
            if (action == ActionState.New)
            {
                try
                {
                    customer = new Customer()
                    {
                        FirstName = firstNameTextBox.Text.Trim(),
                        LastName = lastNameTextBox.Text.Trim(),
                        PurchaseDate = purchaseDatePicker.SelectedDate
                    };
                    ctx.Customers.Add(customer);
                    customerViewSource.View.Refresh();
                    customerViewSource.View.MoveCurrentTo(customer);
                    ctx.SaveChanges();
                }
                // using System.Data;
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameTextBoxBinding);
                lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameTextBoxBinding);
                purchaseDatePicker.SetBinding(DatePicker.SelectedDateProperty, purchaseDateBinding);
            }
            else if (action == ActionState.Edit)
            {
                try
                {
                    customer = (Customer)carDataGrid.SelectedItem;
                    customer.FirstName = firstNameTextBox.Text.Trim();
                    customer.LastName = lastNameTextBox.Text.Trim();
                    customer.PurchaseDate = purchaseDatePicker.SelectedDate;
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();
                customerViewSource.View.MoveCurrentTo(customer);
                firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameTextBoxBinding);
                lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameTextBoxBinding);
                purchaseDatePicker.SetBinding(DatePicker.SelectedDateProperty, purchaseDateBinding);
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    customer = (Customer)carDataGrid.SelectedItem;
                    ctx.Customers.Remove(customer);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();
                firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameTextBoxBinding);
                lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameTextBoxBinding);
                purchaseDatePicker.SetBinding(DatePicker.SelectedDateProperty, purchaseDateBinding);
            }
        }

        private void gbOperations_Click(object sender, RoutedEventArgs e)
        {
            Button SelectedButton = (Button)e.OriginalSource;
            Panel panel = (Panel)SelectedButton.Parent;
            foreach (Button B in panel.Children.OfType<Button>())
            {
                if (B != SelectedButton)
                    B.IsEnabled = false;
            }

            gbActions.IsEnabled = true;
        }

        private void ReInitialize()
        {
            Panel panel = gbOperations.Content as Panel;
            foreach (Button B in panel.Children.OfType<Button>())
            {
                B.IsEnabled = true;
            }
            gbActions.IsEnabled = false;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            TabItem ti = tbCtrlAutoGeist.SelectedItem as TabItem;
            switch (ti.Header)
            {
                case "Cars":
                    SaveCars();
                    break;
                case "Customers":
                    SaveCustomers();
                    break;
                case "Orders":
                    break;
            }
            ReInitialize();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ReInitialize();
        }

        private void SaveOrders()
        {
            Order order = null;
            if (action == ActionState.New)
            {
                try
                {
                    Car car = (Car)cbCars.SelectedItem;
                    Customer customer = (Customer)cbCustomers.SelectedItem;
                    //instantiem Order entity
                    order = new Order()
                    {
                        CarId = car.CarId,
                        CustId = customer.CustId
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Orders.Add(order);
                    ctx.SaveChanges();
                    BindDataGrid();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (action == ActionState.Edit)
            {
                dynamic selectedOrder = ordersDataGrid.SelectedItem;
                try
                {
                    int curr_id = selectedOrder.OrderId;
                    var editedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (editedOrder != null)
                    {
                        editedOrder.CarId = Convert.ToInt32(cbCars.SelectedValue.ToString());
                        editedOrder.CustId = Int32.Parse(cbCustomers.SelectedValue.ToString());
                        //salvam modificarile
                        ctx.SaveChanges();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                BindDataGrid();
                // pozitionarea pe item-ul curent
                carOrdersViewSource.View.MoveCurrentTo(selectedOrder);
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    dynamic selectedOrder = ordersDataGrid.SelectedItem;
                    int curr_id = selectedOrder.OrderId;
                    var deletedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (deletedOrder != null)
                    {
                        ctx.Orders.Remove(deletedOrder);
                        ctx.SaveChanges();
                        MessageBox.Show("Order Deleted Successfully", "Message");
                        BindDataGrid();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void BindDataGrid()
        {
            var queryOrder = from ord in ctx.Orders
                             join cust in ctx.Customers on ord.CustId equals cust.CustId
                             join car in ctx.Cars on ord.CarId equals car.CarId
                             select new
                             {
                                 ord.OrderId,
                                 ord.CarId,
                                 ord.CustId,
                                 cust.FirstName,
                                 cust.LastName,
                                 car.Make,
                                 car.Model
                             };
            carOrdersViewSource.Source = queryOrder.ToList();
        }

    }
}
