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
    /// Logika interakcji dla klasy NewGroup.xaml
    /// </summary>
    public partial class Group : Window
    {
        public Group()
        {
            InitializeComponent();
            GroupName.Focus();
        }
        private void Create_Click(object sender, RoutedEventArgs e)
        {
            UsersAndGroupsCommander.AddGroup(GroupName.Text);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void GroupName_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                Create.Focus();
        }

        private void Create_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                Cancel.Focus();
        }

        private void Cancel_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                GroupName.Focus();

        }

        private void GroupName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (GroupName.Text.Length > 0)
            {
                Create.IsEnabled = true;
                //Delete.IsEnabled = true;
            }
            else
            {
                Create.IsEnabled = false;
                //Delete.IsEnabled = false;
            }
        }
        /*
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            string toDeleteGroup = GroupName.Text;
            List<string> toDeleteGroupsList = new List<string>()
            {
                toDeleteGroup
            };

            if(!UsersAndGroupsCommander.DoesExist(toDeleteGroup))
            {
                MessageBox.Show("Grupa o takiej nazwie nie istnieje.");
            }
            
            UsersAndGroupsCommander.DeleteGroups(toDeleteGroupsList);
        }*/
    }
}
