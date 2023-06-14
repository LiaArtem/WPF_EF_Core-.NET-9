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
using Microsoft.EntityFrameworkCore.Design;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using IBM.EntityFrameworkCore;
using IBM.EntityFrameworkCore.Storage.Internal;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System.Collections.Generic;
using System.Windows.Documents;

namespace WPF_EF_Core
{    
    public class UserData
    {
        [Key]
        public int? Id { get; set; }
        private string textValue;
        private int? intValue;
        private double? doubleValue;
        private Boolean? boolValue;
        private DateTime? dateValue;
        [Timestamp]
        public byte[] RowVersion { get; set; }
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
        [Key]
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

    public class UserLogin
    {
        private string loginValue;
        private string passwordValue;
        public string LoginValue
        {
            get { return loginValue; }
            set { loginValue = value; OnPropertyChanged("LoginValue"); }
        }
        public string PasswordValue
        {
            get { return passwordValue; }
            set { passwordValue = value; OnPropertyChanged("PasswordValue"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }

    public class ApplicationContext : DbContext
    {
        public DbSet<UserData> UsersData { get; set; }
        
        public ApplicationContext(DbContextOptions<ApplicationContext> options, String p_database_type) : base(options)
        {
            if (p_database_type == "Oracle")
            {
                try
                {
                    Database.EnsureCreated();
                }
                catch (Oracle.ManagedDataAccess.Client.OracleException e)
                {
                    if (!e.Message.StartsWith("ORA-00955:"))  //  имя уже задействовано для существующего объекта
                    {
                        throw new ArgumentException(e.Message);
                    }                        
                }
                } 
            else if (p_database_type == "Azure SQL Database" || p_database_type == "IBM DB2" || p_database_type == "IBM Informix") { /*Ничего не делаем база уже создана*/ }
            else 
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
        readonly bool is_initialize = true;
        bool is_filter = false;
        static string ConnectionStringGlobal = "";        

        public MainWindow()
        {
            InitializeComponent();

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);            

            is_initialize = false;
            string database_type = "SQLite";
            try 
            { 
                using ApplicationContext db = new(LoadConfiguration(database_type), database_type);
                UpdateDatagrid(db);
            }
            catch (Exception ex)
            {
                MessageBox(ex.Message, System.Windows.MessageBoxImage.Error);
                DataGrid1.ItemsSource = null;
            }
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
            if (database_type == "SQLite") conn_string = "DefaultConnectionSQLite";
            else if (database_type == "MS SQL Server Local") conn_string = "DefaultConnectionMSSQLLocal";
            else if (database_type == "MS SQL Server") conn_string = "DefaultConnectionMSSQL";
            else if (database_type == "Azure SQL Database") conn_string = "DefaultConnectionAzureSQL";
            else if (database_type == "Oracle") conn_string = "DefaultConnectionOracle";
            else if (database_type == "PostgreSQL") conn_string = "DefaultConnectionPostgreSQL";
            else if (database_type == "MySQL") conn_string = "DefaultConnectionMySQL";
            else if (database_type == "MariaDB") conn_string = "DefaultConnectionMariaDB";
            else if (database_type == "IBM DB2") conn_string = "DefaultConnectionIBMDB2";
            else if (database_type == "IBM Informix") conn_string = "DefaultConnectionIBMInformix";
            else if (database_type == "Firebird") conn_string = "DefaultConnectionFirebird";

            // строка подключения
            string connectionString = config.GetConnectionString(conn_string);
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            
            // разбираем строку подключения
            string[] parts = connectionString.Split(";");
            string LoginValue = ""; string LoginText = ""; string LoginName = "";
            string PasswordValue = ""; string PasswordText = ""; string PasswordName = "";
            foreach (string part in parts)
            {
                if (string.IsNullOrEmpty(part)) continue;                
                string partn = part.Replace(" ", "").ToLower();

                if ((conn_string == "DefaultConnectionSQLite") || (partn.Contains("trusted_connection=true")))
                {
                    LoginValue = "+"; PasswordValue = "+";
                    break;
                }                                        

                if (partn.Contains("userid=")) {
                    LoginText = part;
                    LoginName = part[0..(part.IndexOf("="))];
                    LoginValue = part[(part.IndexOf("=") + 1)..].Replace(" ","");
                }
                if (partn.Contains("user=")) {
                    LoginText = part;
                    LoginName = part[0..(part.IndexOf("="))];
                    LoginValue = part[(part.IndexOf("=") + 1)..].Replace(" ", "");
                }
                if (partn.Contains("username=")) {
                    LoginText = part;
                    LoginName = part[0..(part.IndexOf("="))];
                    LoginValue = part[(part.IndexOf("=") + 1)..].Replace(" ", "");
                }
                if (partn.Contains("uid="))
                {
                    LoginText = part;
                    LoginName = part[0..(part.IndexOf("="))];
                    LoginValue = part[(part.IndexOf("=") + 1)..].Replace(" ", "");
                }
                if (partn.Contains("Password=")) {
                    PasswordText = part;
                    PasswordName = part[0..(part.IndexOf("="))];
                    PasswordValue = part[(part.IndexOf("=") + 1)..].Replace(" ", "");
                }
                if (partn.Contains("password=")) {
                    PasswordText = part;
                    PasswordName = part[0..(part.IndexOf("="))];
                    PasswordValue = part[(part.IndexOf("=") + 1)..].Replace(" ", "");
                }
                if (partn.Contains("pwd="))
                {
                    PasswordText = part;
                    PasswordName = part[0..(part.IndexOf("="))];
                    PasswordValue = part[(part.IndexOf("=") + 1)..].Replace(" ", "");
                }
            }

            // вызываем если нужно форму для ввода логина и пароля
            if ((LoginValue == "" || PasswordValue == "") && ConnectionStringGlobal == "")
            {
                PasswordWindow passWin = new(new UserLogin { LoginValue = LoginValue, PasswordValue = PasswordValue});
                if (passWin.ShowDialog() == true)
                {
                    UserLogin ul = passWin.UserLoginAdd;
                    if (ul != null)
                    {
                        if (LoginValue == "")
                        {
                            connectionString = connectionString.Replace(LoginText, LoginName + "=" + ul.LoginValue);
                            ConnectionStringGlobal = connectionString;
                        }
                        if (PasswordValue == "")
                        {
                            connectionString = connectionString.Replace(PasswordText, PasswordName + "=" + ul.PasswordValue);
                            ConnectionStringGlobal = connectionString;
                        }                        
                    }
                }
            }

            // сохраняем строку подключения
            if (ConnectionStringGlobal == "") { ConnectionStringGlobal = connectionString; }
            if (ConnectionStringGlobal != "") { connectionString = ConnectionStringGlobal; }

            // MS SQL Server Local по умолчанию
            var options = optionsBuilder
                    .UseSqlServer(connectionString)
                    .UseLoggerFactory(MyLoggerFactory)                    
                    .Options;

            if (database_type == "MS SQL Server Local")
            {
                optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
                options = optionsBuilder
                    .UseSqlServer(connectionString)
                    .UseLoggerFactory(MyLoggerFactory)
                    .Options;
            }
            else if (database_type == "MS SQL Server")
            {
                optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
                options = optionsBuilder
                    .UseSqlServer(connectionString)
                    .UseLoggerFactory(MyLoggerFactory)
                    .Options;
            }
            else if (database_type == "SQLite")
            {
                optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
                options = optionsBuilder
                    .UseSqlite(connectionString)
                    .UseLoggerFactory(MyLoggerFactory)
                    .Options;
            }
            else if (database_type == "Oracle")
            {
                optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
                options = optionsBuilder
                    .UseOracle(connectionString)
                    .UseLoggerFactory(MyLoggerFactory)
                    .Options;
            }
            else if (database_type == "MySQL")
            {
                optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
                options = optionsBuilder
                    .UseMySQL(connectionString)
                    .UseLoggerFactory(MyLoggerFactory)
                    .Options;
            }
            else if (database_type == "PostgreSQL")
            {
                optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
                options = optionsBuilder
                    .UseNpgsql(connectionString)
                    .UseLoggerFactory(MyLoggerFactory)                                        
                    .Options;                
            }
            else if (database_type == "Azure SQL Database")
            {
                optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
                options = optionsBuilder
                    .UseSqlServer(connectionString)
                    .UseLoggerFactory(MyLoggerFactory)
                    .Options;
            }
            else if (database_type == "IBM DB2")
            {
                optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
                options = optionsBuilder
                    .UseDb2(connectionString, p => p.SetServerInfo(IBMDBServerType.LUW))
                    .UseLoggerFactory(MyLoggerFactory)
                    .Options;
            }
            else if (database_type == "MariaDB")
            {
                optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
                options = optionsBuilder
                    .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                    .UseLoggerFactory(MyLoggerFactory)
                    .Options;
            }
            else if (database_type == "Firebird")
            {
                optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
                options = optionsBuilder
                    .UseFirebird(connectionString)
                    .UseLoggerFactory(MyLoggerFactory)
                    .Options;
            }
            else if (database_type == "IBM Informix")
            {
                optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
                options = optionsBuilder
                    .UseDb2(connectionString, p => p.SetServerInfo(IBMDBServerType.IDS))
                    .UseLoggerFactory(MyLoggerFactory)
                    .Options;
            }

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

        private void UpdateDatagrid(ApplicationContext db)                
        {
            if (is_initialize == true) return;
            if (is_filter == false)
            {
                DataGrid1.ItemsSource = db.UsersData.ToList();                 
            }
            else
            {
                String m_value1 = value1.Text.ToString();
                String m_value2 = value2.Text.ToString();
                int m_value1_int; int m_value2_int;
                bool m_value1_bool;
                bool m_er;

                if (value_type.Text == "id")
                {
                    m_er = int.TryParse(m_value1, out m_value1_int);
                    m_er = int.TryParse(m_value2, out m_value2_int);
                    DataGrid1.ItemsSource = db.UsersData.ToList().Where(p => p.Id >= m_value1_int && p.Id <= m_value2_int);
                }
                else if (value_type.Text == "text")
                {
                    DataGrid1.ItemsSource = db.UsersData.ToList().Where(p => EF.Functions.Like(p.TextValue, "%" + m_value1 + "%"));
                }
                else if (value_type.Text == "int")
                {
                    m_er = int.TryParse(m_value1, out m_value1_int);
                    m_er = int.TryParse(m_value2, out m_value2_int);
                    DataGrid1.ItemsSource = db.UsersData.ToList().Where(p => p.IntValue >= m_value1_int && p.IntValue <= m_value2_int);
                }
                else if (value_type.Text == "double")
                {
                    m_er = double.TryParse(m_value1, out double m_value1_dbl);
                    m_er = double.TryParse(m_value2, out double m_value2_dbl);
                    DataGrid1.ItemsSource = db.UsersData.ToList().Where(p => p.DoubleValue >= m_value1_dbl && p.DoubleValue <= m_value2_dbl);
                }
                else if (value_type.Text == "bool")
                {
                    m_value1_bool = false;
                    if (m_value1.ToUpper() == "T" || m_value1.ToLower() == "true") m_value1_bool = true;
                    DataGrid1.ItemsSource = db.UsersData.ToList().Where(p => p.BoolValue == m_value1_bool);
                }
                else if (value_type.Text == "date")
                {
                    m_er = DateTime.TryParseExact(m_value1, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime m_value1_dat);
                    m_er = DateTime.TryParseExact(m_value2, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime m_value2_dat);
                    DataGrid1.ItemsSource = db.UsersData.ToList().Where(p => p.DateValue >= m_value1_dat && p.DateValue <= m_value2_dat);
                }                
            }
            this.DataContext = DataGrid1.ItemsSource; //db.UsersData.ToList();

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
            ConnectionStringGlobal = "";

            // пока не поддерживает .net 7.0            
            if (database_type == "IBM Informix" || database_type == "Firebird")
            {
                MessageBox("Пока не поддерживает .NET 7.0", System.Windows.MessageBoxImage.Error);
                DataGrid1.ItemsSource = null;
                return;
            }            
            //
            try
            {
                using ApplicationContext db = new(LoadConfiguration(database_type), database_type);
                UpdateDatagrid(db);
            }
            catch (Exception ex)
            {
                MessageBox(ex.Message, System.Windows.MessageBoxImage.Error);
                DataGrid1.ItemsSource = null;
            }            
        }

        // добавить запись
        private void Button_insertClick(object sender, RoutedEventArgs e)
        {
            AddWindow addWin = new(new UserData());
            if (addWin.ShowDialog() == true)
            {                
                UserData ud = addWin.UserDataAdd;
                string database_type = this.database_type.Text.ToString();
                try
                {
                    using ApplicationContext db = new(LoadConfiguration(database_type), database_type);
                    db.UsersData.Add(ud);
                    db.SaveChanges();
                    UpdateDatagrid(db);
                }
                catch (Exception ex)
                {
                    MessageBox(ex.Message, System.Windows.MessageBoxImage.Error);
                    DataGrid1.ItemsSource = null;
                }
            }            
        }

        // изменить запись
        private void Button_updateClick(object sender, RoutedEventArgs e)
        {
            // если ни одного объекта не выделено, выходим
            if (DataGrid1.SelectedItem == null) return;
            // получаем выделенный объект
            UserData ud = DataGrid1.SelectedItem as UserData;

            AddWindow addWin = new(new UserData
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
                string database_type = this.database_type.Text.ToString();
                try 
                {
                    using ApplicationContext db = new(LoadConfiguration(database_type), database_type);
                    ud = db.UsersData.Find(addWin.UserDataAdd.Id);
                    if (ud != null)
                    {
                        ud.TextValue = addWin.UserDataAdd.TextValue;
                        ud.IntValue = addWin.UserDataAdd.IntValue;
                        ud.DoubleValue = addWin.UserDataAdd.DoubleValue;
                        ud.BoolValue = addWin.UserDataAdd.BoolValue;
                        ud.DateValue = addWin.UserDataAdd.DateValue;

                        try
                        {
                            db.Entry(ud).State = EntityState.Modified;
                            db.SaveChanges();
                            UpdateDatagrid(db);                        
                            MessageBox("Запись обновлена");
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            MessageBox("Запись заблокирована другим пользователем", System.Windows.MessageBoxImage.Warning);
                            UpdateDatagrid(db);                        
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox(ex.Message, System.Windows.MessageBoxImage.Error);
                    DataGrid1.ItemsSource = null;
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
                    string database_type = this.database_type.Text.ToString();
                    try 
                    {
                        using ApplicationContext db = new(LoadConfiguration(database_type), database_type);
                        db.UsersData.Remove(ud);
                        db.SaveChanges();
                        UpdateDatagrid(db);
                    }
                    catch (Exception ex)
                    {
                        MessageBox(ex.Message, System.Windows.MessageBoxImage.Error);
                        DataGrid1.ItemsSource = null;
                    }
                    MessageBox("Запись удалена");                    
                    break;
                case MessageBoxResult.No:                    
                    break;
            }
        }

        // обновить запись
        private void Button_selectClick(object sender, RoutedEventArgs e)
        {
            string database_type = this.database_type.Text.ToString();
            try 
            { 
                using ApplicationContext db = new(LoadConfiguration(database_type), database_type);
                UpdateDatagrid(db);
            }
            catch (Exception ex)
            {
                MessageBox(ex.Message, System.Windows.MessageBoxImage.Error);
                DataGrid1.ItemsSource = null;
            }
        }

        private readonly SolidColorBrush hb = new(Colors.MistyRose);
        private readonly SolidColorBrush nb = new(Colors.AliceBlue);
        private void DataGrid1_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            //UserData product = (UserData) e.Row.DataContext;
            //if (product.Id % 2 == 0)
            if ((e.Row.GetIndex() + 1) % 2 == 0)
                e.Row.Background = hb;
            else
                e.Row.Background = nb;

            // А можно в WPF установить - RowBackground - для нечетных строк и AlternatingRowBackground
        }

        // вывод диалогового окна
        public static void MessageBox(String infoMessage, MessageBoxImage mImage = System.Windows.MessageBoxImage.Information)
        {
            System.Windows.MessageBox.Show(infoMessage, "Сообщение", System.Windows.MessageBoxButton.OK, mImage);
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

        // применить фильтр
        private void Button_findClick(object sender, RoutedEventArgs e)
        {
            is_filter = true;
            string database_type = this.database_type.Text.ToString();
            try 
            { 
                using ApplicationContext db = new(LoadConfiguration(database_type), database_type);
                UpdateDatagrid(db);
            }
            catch (Exception ex)
            {
                MessageBox(ex.Message, System.Windows.MessageBoxImage.Error);
                DataGrid1.ItemsSource = null;
            }
        }

        // отменить фильтр
        private void Button_find_cancelClick(object sender, RoutedEventArgs e)
        {
            is_filter = false;
            value1.Text = "";
            value2.Text = "";
            string database_type = this.database_type.Text.ToString();
            try 
            { 
                using ApplicationContext db = new(LoadConfiguration(database_type), database_type);
                UpdateDatagrid(db);
            }
            catch (Exception ex)
            {
                MessageBox(ex.Message, System.Windows.MessageBoxImage.Error);
                DataGrid1.ItemsSource = null;
            }
        }      

        // изменение типа данных
        private void Value_type_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (is_initialize == true) return;

            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
            String value_type = selectedItem.Content.ToString();

            if (value_type == "id") value2.IsEnabled = true;
            else if (value_type == "text") {value2.IsEnabled = false; value2.Text = ""; }
            else if (value_type == "int") value2.IsEnabled = true;
            else if (value_type == "double") value2.IsEnabled = true;
            else if (value_type == "bool") { value2.IsEnabled = false; value2.Text = ""; }
            else if (value_type == "date") value2.IsEnabled = true;            
        }

        // изменение фокуса на value2
        private void Value2_GotKeyboardFocus(object sender, EventArgs e)
        {
            if (value1.Text != "") value2.Text = value1.Text;
        }        

        // логирование действий с базой данных -> log.txt (свой провайдер)        
        public class MyLoggerProvider : ILoggerProvider
        {
            public ILogger CreateLogger(string categoryName)
            {
                return new MyLogger();
            }

            public void Dispose() { GC.SuppressFinalize(this); }

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
            ConfigurationBuilder builder = new();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            IConfigurationRoot config = builder.Build();

            // получаем строку подключения из файла appsettings.json
            string connectionString = config.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString, opts => opts.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds));
            return new ApplicationContext(optionsBuilder.Options, "");
        }
    }
}