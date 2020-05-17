using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace WPF_EF_Core
{
    /// <summary>
    /// Логика взаимодействия для AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        public UserData UserDataAdd { get; private set; }

        public AddWindow(UserData ud)
        {
            InitializeComponent();

            UserDataAdd = ud;                        
            // если класс пустой
            if (UserDataAdd.DateValue == null) 
            {                
                this.col_date.SelectedDate = DateTime.Today;
                UserDataAdd.DateValue = DateTime.Today;
            }
            // если класс пустой
            if (UserDataAdd.BoolValue == null)
            {                
                this.col_yes_no.IsChecked = false;
                UserDataAdd.BoolValue = false;
            }
            this.DataContext = UserDataAdd;

            // подписываем textBox на событие PreviewTextInput, с помощью которого можно обрабатывать вводимый текст
            col_int.PreviewTextInput += new TextCompositionEventHandler(TextBox_PreviewTextInput_Int);
            col_num.PreviewTextInput += new TextCompositionEventHandler(TextBox_PreviewTextInput_Float);
        }

        // Сохранить
        private void Button_saveClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
       
        private void TextBox_PreviewTextInput_Float(object sender, TextCompositionEventArgs e)
        {
            string inputSymbol = e.Text.ToString(); // можно вводить цифры и точку
            if (!Regex.Match(inputSymbol, @"[0-9]|\.").Success)
            {
                e.Handled = true;
            }
        }
        private void TextBox_PreviewTextInput_Int(object sender, TextCompositionEventArgs e)
        {
            string inputSymbol = e.Text.ToString(); // можно вводить цифры
            if (!Regex.Match(inputSymbol, @"[0-9]").Success)
            {
                e.Handled = true;
            }
        }
    }
}
