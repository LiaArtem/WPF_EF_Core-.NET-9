using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging.Console;
using Microsoft.EntityFrameworkCore.Design;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    //[NotMapped] - атрибут чтобы не попадал в миграцию
    //[Table("Mobiles")] - сопоставление с таблицей
    public class Country
    {
        public int Id { get; set; }
        //[Required] // Аннотация, которая указывает, что свойство обязательно должно иметь значение.      
        //[NotMapped] // Не будет создавать объект в базе данных (столбец)
        //[Key] // Указание что это первичный ключ
        //[MaxLength] [MinLength] // устанавливают максимальное и минимальное количество символов в строке-свойстве
        //[Column("ModelName")] // сопоставление столбцов
        //[ForeignKey("CompId")] // установка внешнего ключа
        //[Index] // для установки индекса
        //[ConcurrencyCheck] // позволяет решить проблему параллелизма, когда с одной и той же записью в таблице могут работать одновременно несколько пользователей
        public string Name { get; set; }
    }

    public class ApplicationContext : DbContext
    {
        public DbSet<UserData> UsersData { get; set; }
        
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();

            //Database.EnsureDeleted();   // удаляем бд со старой схемой
            //Database.EnsureCreated();   // создаем бд с новой схемой
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>();  // автоматическое добавление таблиц по классам, которых нет в связях ПРИ МИГРАЦИИ
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ApplicationContext db;
        Boolean is_initialize = true;

        public MainWindow()
        {
            InitializeComponent();
         
            is_initialize = false;
            UpdateDatagrid();
        }

        // загрузить NuGet
        // Microsoft.Extensions.Configuration.FileExtensions;
        // Microsoft.Extensions.Configuration.Json;
        public static DbContextOptions<ApplicationContext> LoadConfiguration(string database_type)
        {
            var builder = new ConfigurationBuilder() 
                                    .SetBasePath(Directory.GetCurrentDirectory()) // установка пути к текущему каталогу
                                    .AddJsonFile("appsettings.json"); // получаем конфигурацию из файла appsettings.json                                    
            // создаем конфигурацию
            var config = builder.Build();
            // получаем строку подключения
            string conn_string = "";
            if (database_type == "MS SQL Server") conn_string = "DefaultConnection";
            else if (database_type == "Oracle") conn_string = "DefaultConnection";
            else if (database_type == "MySQL") conn_string = "DefaultConnection";
            else if (database_type == "SQLite") conn_string = "DefaultConnection";            
           
            // строка подключения
            string connectionString = config.GetConnectionString(conn_string);
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            var options = optionsBuilder
                .UseSqlServer(connectionString)
                .UseLoggerFactory(MyLoggerFactory)
                .Options;

            return options;
        }

        // устанавливаем фабрику логгера
        public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder =>
        {
            //Настройка логгирования (свой провайдер)
            //Логгирование позволяет получить разнообразную информацию, связанную с операциями с данными, но не вся эта информация может быть нам нужна.Для фильтрации этой информации EF Core предоставляет класс DbLoggerCategory, который позволяет задать нужные категории логгирования:
            //Database.Command: категория для выполняемых команд, позволяет получить выполняемый код SQL
            //Database.Connection : категория для операций подключения к БД
            //Database.Transaction : категория для транзакций с бд
            //Database.Migration: категория для миграций
            //Database.Model: категория для действий, совершаемых при привязке модели
            //Database.Query: категория для запросов за исключением тех, что генерируют исполняемый код SQL
            //Database.Scaffolding: категория для действий, выполняемых в поцессе обратного инжиниринга(то есть когда по базе данных генерируются классы и класс контекста)
            //Database.Update: категория для сообщений вызова DbContext.SaveChanges()
            //Database.Infrastructure: категория для всех остальных сообщений
            
            builder.AddFilter((category, level) => category == DbLoggerCategory.Database.Command.Name
                                    && level == LogLevel.Information)
                    .AddProvider(new MyLoggerProvider()); // указываем наш провайдер логгирования
            

            // или стандартный от Microsoft
            // NuGet - Microsoft.Extensions.Logging.Console
            //builder.AddConsole();
        });

        private void UpdateDatagrid()
        {
            if (is_initialize == true) return;

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
             
            db = new ApplicationContext(LoadConfiguration(database_type));
            UpdateDatagrid();
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

        private readonly SolidColorBrush hb = new SolidColorBrush(Colors.MistyRose);
        private readonly SolidColorBrush nb = new SolidColorBrush(Colors.AliceBlue);
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
                if (row_list != null)  
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

        // логирование действий с базой данных -> log.txt (свой провайдер)        
        public class MyLoggerProvider : ILoggerProvider
        {
            public ILogger CreateLogger(string categoryName)
            {
                return new MyLogger();
            }

            public void Dispose() { }

            private class MyLogger : ILogger
            {
                public IDisposable BeginScope<TState>(TState state)
                {
                    return null;
                }

                public bool IsEnabled(LogLevel logLevel)
                {
                    return true;
                }

                //BeginScope: этот метод возвращает объект IDisposable, который представляет некоторую область видимости для логгера.В данном случае нам этот метод не важен, поэтому возвращаем значение null
                //IsEnabled: возвращает значения true или false, которые указывает, доступен ли логгер для использования.Здесь можно здать различную логику. В частности, в этот метод передается объект LogLevel, и мы можем, к примеру, задействовать логгер в зависимости от значения этого объекта. Но в данном случае просто возвращаем true, то есть логгер доступен всегда.
                //Log: этот метод предназначен для выполнения логгирования. Он принимает пять параметров:
                    //LogLevel: уровень детализации текущего сообщения
                    //EventId: идентификатор события
                    //TState: некоторый объект состояния, который хранит сообщение
                    //Exception: информация об исключении
                    //formatter: функция форматирования, которая с помощью двух предыдущих параметров позволяет получить собственно сообщение для логгирования               
                public void Log<TState>(LogLevel logLevel, EventId eventId,
                        TState state, Exception exception, Func<TState, Exception, string> formatter)
                {
                    File.AppendAllText("log.txt", formatter(state, exception));
                    Console.WriteLine(formatter(state, exception));
                }
            }
        }
    }

    // Этот класс формально нигде не вызывается и никак не используется, фактически он вызывается 
    // инфраструктурой Entity Framework при создании миграции
    // выбор базы данных для подключения
    public class SampleContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
    {
        public ApplicationContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();

            // получаем конфигурацию из файла appsettings.json
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            IConfigurationRoot config = builder.Build();

            // получаем строку подключения из файла appsettings.json
            string connectionString = config.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString, opts => opts.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds));
            return new ApplicationContext(optionsBuilder.Options);
        }
    }
}