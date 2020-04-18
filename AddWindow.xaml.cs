using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        public AddWindow()
        {
            InitializeComponent();

            // подписываем textBox на событие PreviewTextInput, с помощью которого можно обрабатывать вводимый текст
            col_int.PreviewTextInput += new TextCompositionEventHandler(TextBox_PreviewTextInput_Int);
            col_num.PreviewTextInput += new TextCompositionEventHandler(TextBox_PreviewTextInput_Float);
            col_double.PreviewTextInput += new TextCompositionEventHandler(TextBox_PreviewTextInput_Float);
        }

        // Сохранить
        private void Button_saveClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Отменить
        private void Button_cancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
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
