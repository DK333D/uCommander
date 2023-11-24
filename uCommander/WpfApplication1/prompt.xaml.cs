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

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for prompt.xaml
    /// </summary>
    public partial class prompt : Window
    {
        public prompt()
        {
            InitializeComponent();
        }

        private void Anuluj_Click(object sender, RoutedEventArgs e)
        {
            dialogResult.IsChecked = false;
            this.Close();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            dialogResult.IsChecked = true;
            this.Close();
        }
        
    
    }
}
