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
using System.Collections.ObjectModel;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public class Element
    {

        public string Etype { get; set; }
        public string Ename { get; set; }
        
        public Element(string _type, string _name)
        {
            Etype = _type;
            Ename = _name;
        }
    }

    public class Str2
    {
        public string Day { get; set; }
        public string Times { get; set; }

        public Str2(string _day, string _times)
        {
            Day = _day;
            Times = _times;
        }
    }

    
    public partial class MainWindow : Window
    {

        //------------------------------------------------------------
        


        public List<Element> list = new List<Element>();
        public WindowCommander leftWindowCommander;
        public WindowCommander rightWindowCommander;
        public string selectedItemName = "";
        //public bool Direction_LR = false;




        //------------------------------------------------------------

        //CommanderWindow LeftWindowCommander;

        public MainWindow()
        {
            InitializeComponent();

            this.LeftWindow.Items.Clear();
            leftWindowCommander = new WindowCommander(ref LeftWindow, ref now_l, ref Delete_U, ref Delete_G,  ref DisplayGroups_l, ref ImgGroups_l)
            {
                mode = Mode.groups,
                direction = Direction.from
            };

            this.RightWindow.Items.Clear();
            rightWindowCommander = new WindowCommander(ref RightWindow, ref now_r, ref VirtualDeleteButton, ref VirtualDeleteButton, ref DisplayGroups_r, ref ImgGroups_r)
            {
                mode = Mode.groups,
                direction = Direction.to
            };

            leftWindowCommander.Refresh();
            rightWindowCommander.Refresh();
      

        }
        
        private void DisplayGroups_l_Click(object sender, RoutedEventArgs e)
        {
            
            this.leftWindowCommander.DisplayGroups();
            DoMeetTheConditionsLR();

            //DoEnable(sender, e);


            //-------------------------------------------------------------------------------------

            /*
            list.Add(new Element() { type = "G", name = "grupa4" });
            list.Add(new Element() { type = "G", name = "grupa2" });
            list.Add(new Element() { type = "G", name = "grupa3" });
            list.Add(new Element() { type = "G", name = "grupa1" });

            
            this.leftWindow.ItemsSource = list;
            List<Element> orderedList = list.OrderBy(o=>o.name).ToList();
//-------------------------------------------------------------------------------------------
          */
        }

        private void DisplayGroups_r_Click(object sender, RoutedEventArgs e)
        {

            this.rightWindowCommander.DisplayGroups();
            DoMeetTheConditionsLR();
            //DoEnable(sender, e);
            
        }




        private void DisplayUsers_Click(object sender, RoutedEventArgs e)
        {
            //LeftWindow.UnselectAll();
            leftWindowCommander.DisplayUsers();
            //DoEnable(sender, e);
        }


        public bool AtLeastOneMember(WindowCommander WindowC) // czy zaznaczone grupy mają choć jednego użytkownika
        {
            for(int i = 0; i < WindowC.selectedItemsList.Count; i++)
            {
                if (UsersAndGroupsCommander.UsersofGroupList(WindowC.selectedItemsList[i]).Count > 0) return true;                   
            }
            return false;      
        }
        
        private void Delete_l_Click(object sender, RoutedEventArgs e)
        {
            leftWindowCommander.Delete_Click();
            BothRefresh();
        }
        
        private void Delete_G_Click(object sender, RoutedEventArgs e)
        {
            if(leftWindowCommander.mode == Mode.groups)
            {
                UsersAndGroupsCommander.DeleteGroups(leftWindowCommander.selectedItemsList);
            }
            BothRefresh(); 
        }
        

        public void AddUsers_Click(object sender, RoutedEventArgs e)
        {
            NewUser nu = new NewUser
            {
                Gs_l = leftWindowCommander.ForTo(),
                areGroups = leftWindowCommander.areGroups
                
            };
            nu.ShowDialog();

            BothRefresh();
        }
        


        private void AddUs_toGs_Click(object sender, RoutedEventArgs e)
        {
            List<string> listofUsers = new List<string>(); // lista uzytkownikow do dodania do grup
            List<string> listofGroups = new List<string>(); // lista grup do dodania uzytkownikow do nich
            
            listofUsers =  leftWindowCommander.ForFrom();
            listofGroups = rightWindowCommander.ForTo();
           
            var result = MessageBox.Show(UsersAndGroupsCommander.CreateQuestionAddTo(listofUsers, listofGroups), "", MessageBoxButton.YesNoCancel);
            if(result == MessageBoxResult.Yes)
            {
                UsersAndGroupsCommander.AddUsersToGroups(listofUsers, listofGroups);
            }
            
            BothRefresh();

        }
        


        private void DeleteUs_fromGs_Click(object sender, RoutedEventArgs e)
        {

            List<string> listofUsers = new List<string>(); // lista uzytkownikow do dodania do grup
            List<string> listofGroups = new List<string>(); // lista grup do dodania uzytkownikow do nich

            listofUsers = leftWindowCommander.ForFrom();
            listofGroups = rightWindowCommander.ForTo();

            //var result = MessageBox.Show(UsersAndGroupsCommander.CreateQuestionDelFrom(listofUsers, listofGroups), "", MessageBoxButton.YesNoCancel);
            //if (result == MessageBoxResult.Yes)
                UsersAndGroupsCommander.DeleteUsersFromGroups(listofUsers, listofGroups);
            
            BothRefresh();
        }


        private void MoveUs_toGs_Click(object sender, RoutedEventArgs e)
        {
            List<string> listofUsers = new List<string>(); // lista uzytkownikow do dodania do grup
            List<string> listofGroups = new List<string>(); // lista grup do dodania uzytkownikow do nich

            listofUsers = leftWindowCommander.ForFrom();
            listofGroups = rightWindowCommander.ForTo();



            var result = MessageBox.Show(UsersAndGroupsCommander.CreateQuestionMove(listofUsers, listofGroups, leftWindowCommander.currentGroup), "", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
            {
                UsersAndGroupsCommander.AddUsersToGroups(listofUsers, listofGroups);
                leftWindowCommander.EmptySelectedGroups(false);
            }
            
            BothRefresh();
        }

        /*
        private void RightWindow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;

            if (item != null)
            {
                rightWindowCommander.ItemDoubleClicked();
            }
        }
        
        private void LeftWindow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;

            if (item != null)
            {
                leftWindowCommander.ItemDoubleClicked();
            }
        }*/

        private void MouseDoubleClicked(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;

            if (item != null)
            {
                if (sender == LeftWindow)
                {
                    leftWindowCommander.ItemDoubleClicked();

                }
                else if(sender == RightWindow)
                {
                    rightWindowCommander.ItemDoubleClicked();

                }
            }
        }

        

        

        private void LeftWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            leftWindowCommander.DoUnselect(sender, e);
            leftWindowCommander.IfEnable();
        }

        private void RightWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            rightWindowCommander.DoUnselect(sender, e);
            rightWindowCommander.IfEnable();
        }


        private bool DoMeetTheConditions(WindowCommander from, WindowCommander to)
        {
            if (from.selectedItemsList.Count != 0 && (to.mode == Mode.usersofGroup || (to.mode == Mode.groups && to.selectedItemsList.Count != 0)) && to.listView.SelectedIndex != 0)
            {
                if (from.mode == Mode.groups)
                {
                    if (!AtLeastOneMember(from)) return false;
                }
                return true;
            }
            return false;

        }

        private void LeftWindow_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            leftWindowCommander.SelectedItemsList();
            leftWindowCommander.IfEnable();
            //NewUser.AddToLeft = leftWindowCommander.IfEnableAddUser();
            DoMeetTheConditionsLR();
        }

        private void RightWindow_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            rightWindowCommander.SelectedItemsList();
            rightWindowCommander.IfEnable();
            //NewUser.AddToRight = rightWindowCommander.IfEnableAddUser();
            DoMeetTheConditionsLR();

        }

        private bool DoMeetTheConditionsLR()
        {
            if (DoMeetTheConditions(leftWindowCommander, rightWindowCommander))
            {
                DirectionImg.Opacity = 1;
                DirectionImg.IsEnabled = true;
                this.AddUs_toGs.IsEnabled = true;
                this.DeleteUs_fromGs.IsEnabled = true;
                if(leftWindowCommander.mode != Mode.all)
                    this.MoveUs_toGs.IsEnabled = true;


                return true;
            }

            DirectionImg.Opacity = 0.2;
            DirectionImg.IsEnabled = false;
            this.AddUs_toGs.IsEnabled = false;
            this.DeleteUs_fromGs.IsEnabled = false;
            this.MoveUs_toGs.IsEnabled = false;

            return false;
        }
        
        private void LeftWindow_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            //leftWindowCommander.ItemSelected();
            //rightWindowCommander.ItemSelected();
        }

        private void RightWindow_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            //rightWindowCommander.ItemSelected();
            //leftWindowCommander.ItemSelected();
        }
        

        private void Expression_l_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Expression_l.Text.Length == 0) leftWindowCommander.regExp = "^$";
            else leftWindowCommander.regExp =  Expression_l.Text ;
            leftWindowCommander.Select();
        }

        private void Expression_r_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Expression_r.Text.Length == 0) rightWindowCommander.regExp = "^$";
            else rightWindowCommander.regExp = Expression_r.Text;
            rightWindowCommander.Select();
        }

        private void Magnifier_l_Click(object sender, RoutedEventArgs e)
        {
            leftWindowCommander.Select();
        }

        private void Magnifier_r_Click(object sender, RoutedEventArgs e)
        {
            rightWindowCommander.Select();
        }
        /*
        private void Magnifier_l_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
             leftWindowCommander.Magnifier_DoubleClick(Expression_l, Expression_l.Visibility);
        }

        private void Magnifier_r_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            rightWindowCommander.Magnifier_DoubleClick(Expression_r, Expression_r.Visibility);
        }*/
        
        private void BothRefresh()
        {
            leftWindowCommander.Refresh();
            rightWindowCommander.Refresh();
        }
        
        
        private void ChangeOpacity(Button Bt)
        {
            if (!Bt.IsEnabled) Bt.Opacity = 0.2;
            else Bt.Opacity = 1;

        }

        private void Button_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) //zmniana dot. przycisku ze środka
        {
            ChangeOpacity((Button) sender);
        }
              


        private void Add_Group_Click(object sender, RoutedEventArgs e)
        {
            Group ngr = new Group();
            ngr.ShowDialog();
            BothRefresh();

        }

        private void Refresh_Click(object sender, RoutedEventArgs e) 
        {
            BothRefresh();

        }

        private void EmptyGs_l_Click(object sender, RoutedEventArgs e)
        {
            leftWindowCommander.EmptySelectedGroups(true);
        }

        private void LeftWindow_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
