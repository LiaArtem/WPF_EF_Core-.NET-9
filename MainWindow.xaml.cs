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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_EF_Core
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
            AddWindow ww = new AddWindow();
            ww.Show();
        }

        // изменить запись
        private void Button_updateClick(object sender, RoutedEventArgs e)
        {
        }

        // удалить запись
        private void Button_deleteClick(object sender, RoutedEventArgs e)
        {
        }

        // обновить запись
        private void Button_selectClick(object sender, RoutedEventArgs e)
        {
        }

    }
}