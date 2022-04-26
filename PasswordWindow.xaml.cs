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

namespace WPF_EF_Core
{
    /// <summary>
    /// Логика взаимодействия для PasswordWindow.xaml
    /// </summary>
    public partial class PasswordWindow : Window
    {
        public UserLogin UserLoginAdd { get; private set; }

        public PasswordWindow(UserLogin ul)
        {
            InitializeComponent();

            UserLoginAdd = ul;
            this.DataContext = UserLoginAdd;
        }

        // Сохранить
        private void Button_saveClick(object sender, RoutedEventArgs e)
        {
            // передаем пароль
            UserLoginAdd.PasswordValue = col_password.Password;            

            this.DialogResult = true;
        }
    }
}
