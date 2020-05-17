using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
        public string ColorTypeValue { get; set; }

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
                DateValue = ud.DateValue,
                ColorTypeValue = ud.ColorTypeValue
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
                }
            }
        }

        // удалить запись
        private void Button_deleteClick(object sender, RoutedEventArgs e)
        {
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
            UserData product = (UserData) e.Row.DataContext;

            if (product.Id % 2 == 0)
                e.Row.Background = hb;
            else
                e.Row.Background = nb;
        }

    }
}