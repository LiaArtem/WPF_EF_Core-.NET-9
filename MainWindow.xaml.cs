using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPF_EF_Core
{
    public class UserData
    {
        private string textValue;
        private int? intValue;
        private double? doubleValue;
        private Boolean? boolValue;
        private DateTime? dateValue;

        public int? Id { get; set; }
        public string TextValue
        {
            get { return textValue; }
            set { textValue = value; OnPropertyChanged("TextValue"); }
        }
        public int? IntValue
        {
            get { return intValue; }
            set { intValue = value; OnPropertyChanged("IntValue"); }
        }
        public double? DoubleValue
        {
            get { return doubleValue; }
            set { doubleValue = value; OnPropertyChanged("DoubleValue"); }
        }
        public Boolean? BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; OnPropertyChanged("BoolValue"); }
        }
        public DateTime? DateValue
        {
            get { return dateValue; }
            set { dateValue = value; OnPropertyChanged("DateValue"); }
        }        

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ApplicationContext db;

        public MainWindow()
        {
            InitializeComponent();

            db = new ApplicationContext();            
            UpdateDatagrid();            
        }
        
        public class ApplicationContext : DbContext
        {
            public DbSet<UserData> UsersData { get; set; }
            public ApplicationContext()
            {
                Database.EnsureCreated();
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=efcoreappdb4;Trusted_Connection=True;");
            }
        }

        private void UpdateDatagrid()
        {            
            DataGrid1.ItemsSource = db.UsersData.ToList();
            this.DataContext = db.UsersData.ToList();

            // Выделить сроку с курсором
            if (DataGrig_Id == null && DataGrid1.Items.Count > 0) DataGrig_Id = 1;

            if (DataGrig_Id != null && DataGrid1.Items.Count > 0) 
            {
                foreach (UserData drv in DataGrid1.ItemsSource)
                {
                    if ( drv.Id == DataGrig_Id)
                    {
                        DataGrid1.SelectedItem = drv;
                        DataGrid1.ScrollIntoView(drv);
                        DataGrid1.Focus();
                        break;
                    }
                }             
            }            
        }

        // изменение типа базы данных
        private void Database_type_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
            String database_type = selectedItem.Content.ToString();
        }

        // добавить запись
        private void Button_insertClick(object sender, RoutedEventArgs e)
        {
            AddWindow addWin = new AddWindow(new UserData());
            if (addWin.ShowDialog() == true)
            {
                UserData ud = addWin.UserDataAdd;
                db.UsersData.Add(ud);
                db.SaveChanges();
                //
                UpdateDatagrid();
            }            
        }

        // изменить запись
        private void Button_updateClick(object sender, RoutedEventArgs e)
        {
            // если ни одного объекта не выделено, выходим
            if (DataGrid1.SelectedItem == null) return;
            // получаем выделенный объект
            UserData ud = DataGrid1.SelectedItem as UserData;

            AddWindow addWin = new AddWindow(new UserData
            {
                Id = ud.Id,
                TextValue = ud.TextValue,
                IntValue = ud.IntValue,
                DoubleValue = ud.DoubleValue,
                BoolValue = ud.BoolValue,
                DateValue = ud.DateValue                
            }); 

            if (addWin.ShowDialog() == true)
            {
                // получаем измененный объект
                ud = db.UsersData.Find(addWin.UserDataAdd.Id);
                if (ud != null)
                {
                    ud.TextValue = addWin.UserDataAdd.TextValue;
                    ud.IntValue = addWin.UserDataAdd.IntValue;
                    ud.DoubleValue = addWin.UserDataAdd.DoubleValue;
                    ud.BoolValue = addWin.UserDataAdd.BoolValue;
                    ud.DateValue = addWin.UserDataAdd.DateValue;                    
                    db.Entry(ud).State = EntityState.Modified;
                    db.SaveChanges();
                    //
                    UpdateDatagrid();
                    MessageBox("Запись обновлена");
                }
            }
        }

        // удалить запись
        private void Button_deleteClick(object sender, RoutedEventArgs e)
        {
            // если ни одного объекта не выделено, выходим
            if (DataGrid1.SelectedItem == null) return;

            MessageBoxResult result = System.Windows.MessageBox.Show("Удалить запись ???", "Сообщение", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    // получаем выделенный объект
                    UserData ud = DataGrid1.SelectedItem as UserData;
                    db.UsersData.Remove(ud);
                    db.SaveChanges();
                    //
                    UpdateDatagrid();
                    MessageBox("Запись удалена");
                    break;
                case MessageBoxResult.No:                    
                    break;
            }
        }

        // обновить запись
        private void Button_selectClick(object sender, RoutedEventArgs e)
        {
            UpdateDatagrid();
        }

        private SolidColorBrush hb = new SolidColorBrush(Colors.MistyRose);
        private SolidColorBrush nb = new SolidColorBrush(Colors.AliceBlue);
        private void DataGrid1_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            //UserData product = (UserData) e.Row.DataContext;
            //if (product.Id % 2 == 0)
            if ((e.Row.GetIndex() + 1) % 2 == 0)
                e.Row.Background = hb;
            else
                e.Row.Background = nb;
        }

        // вывод диалогового окна
        public static void MessageBox(String infoMessage)
        {
            System.Windows.MessageBox.Show(infoMessage, "Сообщение", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        public int? DataGrig_Id;
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var row_list = (UserData)DataGrid1.SelectedItem;
                DataGrig_Id = row_list.Id;                
            }
            catch {
                DataGrig_Id = null;
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            Button_updateClick(sender, e);            
        }

    }
}