using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Data;
//using System.Windows.Forms;
using System.DirectoryServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace WpfApplication1
{
    public enum Mode { groups, usersofGroup, all };
    //public enum Mode_users { all, group };
    public enum State { active, nonActive };
    public enum Direction { from, to };

    
    public class WindowCommander
    {

        
        public Mode mode;// = Mode.groups;
        public State state;// = State.nonActive;
        //public Mode_users mode_users = Mode_users.all;
        public Direction direction;
        

        public ListView listView;
        public TextBlock Tb;
        public TextBlock TbInButton;
        public Button DelBtU;
        public Button DelBtG;
        public Button DelFromGroup;
        public Button Groups;
        public Image ImgGroups;
        public bool AddTo;
        public List<Element> list;
        public List<string> selectedItemsList = new List<string>();
        //public bool ifLeft = false;
        public string element0name = "wszyscy";
        public string currentGroup = "";
        //public List<string> currentSelectedItems = new List<string>();
        public string regExp = "^$";
        public bool areGroups = false;
        



        public WindowCommander(ref ListView listView, ref TextBlock TextBlock, ref Button DelBtU, ref Button DelBtG, ref Button Groups, ref Image ImgGroups)
        {

            this.listView = listView;
            this.Groups = Groups;
            this.ImgGroups = ImgGroups;
            Tb = TextBlock;
            this.DelBtU = DelBtU;
            this.DelBtG = DelBtG;
            //this.DelFromGroup = DelFromGroup;
            //TbInButton = TbInBt;
            //listView.ItemsSource = null;
            this.list = new List<Element>();
        }
        

        public void Refresh()
        {
            Element selectedElement = (Element)this.listView.SelectedItem;
            listView.UnselectAll();
            
            DelBtU.IsEnabled = false;
            DelBtG.IsEnabled = false;
            switch (mode)
            {
                case Mode.groups:
                    list = UsersAndGroupsCommander.GroupsList();
                    Tb.Text = "Wszystkie grupy";                    
                    Element element0 = new Element("", element0name);
                    list.Insert(0, element0);
                    break;

                case Mode.all:
                    list = UsersAndGroupsCommander.AllUsersList();
                    Tb.Text = "Wszyscy użytkownicy";
                    break;

                case Mode.usersofGroup:
                    areGroups = true;
                    list = UsersAndGroupsCommander.UsersofGroupList(currentGroup);
                    Tb.Text = "Użytkownicy grupy " + currentGroup;
                    break;
            }
            
            listView.ItemsSource = null;
            listView.ItemsSource = this.list;
            Select();
            IfEnable();
            
        }

        public void IfEnable()
        {
            DelBtU.IsEnabled = false;
            DelBtG.IsEnabled = false;
            if (selectedItemsList.Count != 0)
            {
                DelBtU.IsEnabled = true;
                areGroups = false;
                switch (mode)
                {
                    case Mode.all:
                        DelBtU.ToolTip = "Usuń użytkowników";
                        break;
                    case Mode.usersofGroup:
                        //DelFromGroup.IsEnabled = true;
                        areGroups = true;
                        DelBtU.ToolTip = "Usuń użytkowników z grupy";
                        break;
                    case Mode.groups:
                        if(listView.SelectedIndex != 0)
                        {
                            DelBtG.IsEnabled = true;
                            //DelFromGroup.IsEnabled = false;
                            areGroups = true;
                        }
                        DelBtU.ToolTip = "Opróżnij grupy";
                        break;

                }
            }
        }
       

        public void IfEnableInNewUser()
        {
            
        }

        public void DisplayGroups()
        {
            mode = Mode.groups;
            Refresh();
            
        }
        public void DisplayUsers()
        {
            mode = Mode.all;
            Refresh();
        }
        
        public void DeleteSelected()
        {
            Element toDeleteElement;
            List<string> toDeleteList = new List<string>();

            //int i = 0;
            //if (listView.SelectedIndex == 0 && mode == Mode.groups) i = 1;
            for (int i=0;  i < listView.SelectedItems.Count; i++)
            {
                toDeleteElement = (Element)this.listView.SelectedItems[i];
                toDeleteList.Add(toDeleteElement.Ename);
            }
            
            /*switch(mode)
            {
                //case Mode.all:
                case Mode.usersofGroup:*/
                    var resultG = MessageBox.Show(UsersAndGroupsCommander.CreateQuestionDel(toDeleteList, true), "", MessageBoxButton.YesNoCancel);
                    if (resultG == MessageBoxResult.Yes)
                        UsersAndGroupsCommander.DeleteUsers(toDeleteList);
                    /*break;
                case Mode.groups:
                    var resultU = MessageBox.Show(UsersAndGroupsCommander.CreateQuestionDel(toDeleteList, false), "", MessageBoxButton.YesNoCancel);
                    if (resultU == MessageBoxResult.Yes)
                        UsersAndGroupsCommander.DeleteGroups(toDeleteList);
                    break;*/
            //}
            Refresh();
            
        }

        public void SelectedItemsList()
        {
            List<string> selected = new List<string>();
            List<Element> tmp = UsersAndGroupsCommander.AllUsersList();
            Element element;

            int i = 0;
            if (mode == Mode.groups && listView.SelectedIndex == 0) i = 1;
            while (i < this.listView.SelectedItems.Count)
            {
                element = (Element)this.listView.SelectedItems[i];
                selected.Add(element.Ename);
                i++;
            }
            selectedItemsList = selected;
            
        }

        public void Select()//string regExp)
        {
            listView.UnselectAll();
            string s = "";
            int i = 0;
            Regex r;
            try
            {
                r = new Regex(regExp, RegexOptions.IgnoreCase);
                if (mode == Mode.groups) i = 1;
                while (i < listView.Items.Count)
                {
                    Match m = r.Match(list[i].Ename);
                    if (m.Success)
                    {
                        s += i + " ";
                        this.listView.SelectedItems.Add(listView.Items[i]);
                    }
                    i++;
                }
            }
            catch (ArgumentException) { };
           

        }


        public void ItemDoubleClicked()
        {
            Element element = new Element("","");
            element = (Element)this.listView.SelectedItem;

            //TbInButton.IsEnabled = false;
            if (mode == Mode.groups)
            {
                if(listView.SelectedIndex != 0)
                {
                    if(UsersAndGroupsCommander.IsItExistingGroup(element.Ename))
                    {
                        mode = Mode.usersofGroup;
                        currentGroup = element.Ename;
                    }
                    Refresh();

                }
                else
                {
                    mode = Mode.all;
                }

            }
            
            Refresh();
        }
        

        public void DoUnselect(object sender, MouseButtonEventArgs e)
        {
            HitTestResult r = VisualTreeHelper.HitTest(listView, e.GetPosition(listView));
            if (r.VisualHit.GetType() != typeof(ListViewItem))
                listView.UnselectAll();
        }
        
        public List<string> ForFrom()
        {
            List<string> listofUsers_ = new List<string>();
            Element element;
            switch (mode)
            {
                case Mode.groups:
                    List<Element> usersofGroup_ = new List<Element>(); // lista grup do dodania uzytkownikow do nich
                    for (int i = 0; i < listView.SelectedItems.Count; i++)
                    {
                        element = (Element)listView.SelectedItems[i];
                        if (element.Etype != "U" && element.Etype != "G") continue; 
                        usersofGroup_ = UsersAndGroupsCommander.UsersofGroupList(element.Ename);
                        for (int j = 0; j < usersofGroup_.Count; j++)
                        {
                            if (!listofUsers_.Contains(usersofGroup_[j].Ename))
                            {
                                listofUsers_.Add(usersofGroup_[j].Ename);
                            }
                        }
                    }
                    break;
                case Mode.all:
                case Mode.usersofGroup:
                    for (int i = 0; i < listView.SelectedItems.Count; i++)
                    {
                        element = (Element)listView.SelectedItems[i];
                        if (!listofUsers_.Contains(element.Ename))
                        {
                            listofUsers_.Add(element.Ename);
                        }
                    }
                    break;
            }
            return listofUsers_;
        }

        public List<string> ForTo()
        {

            List<string> listofGroups_ = new List<string>();

            Element element;
            switch (mode)
            {
                case Mode.groups:
                    for(int i=0; i<listView.SelectedItems.Count; i++)
                    {
                        element = (Element)listView.SelectedItems[i];
                        if (element.Etype != "G") continue;
                        if(!listofGroups_.Contains(element.Ename))
                        {
                            listofGroups_.Add(element.Ename);
                        }
                    }
                    break;
                case Mode.usersofGroup:
                    listofGroups_.Add(currentGroup);
                    break;

            }
            return listofGroups_;
        }


        public void EmptySelectedGroups(bool ask)
        {
            if(ask)
            {
                var resultG = MessageBox.Show(UsersAndGroupsCommander.CreateQuestionEmpty(selectedItemsList), "", MessageBoxButton.YesNoCancel);
                if (resultG != MessageBoxResult.Yes)
                {
                    Refresh();
                    return;
                }

            }

            List<string> listofUsers_ = new List<string>();
            string userName;
            Element elementG;
            List<Element> usersofGroup_ = new List<Element>(); // lista grup do dodania uzytkownikow do nich


            if(mode == Mode.groups)
            {
                for (int i = 0; i < listView.SelectedItems.Count; i++)
                {
                    elementG = (Element)listView.SelectedItems[i];
                    if (elementG.Etype != "G") continue;
                    usersofGroup_ = UsersAndGroupsCommander.UsersofGroupList(elementG.Ename);
                    for (int j = 0; j < usersofGroup_.Count; j++)
                    {
                        userName = usersofGroup_[j].Ename;
                        if (UsersAndGroupsCommander.DoesBelong(userName, elementG.Ename))
                        {
                            UsersAndGroupsCommander.DeleteUserFromGroup(userName, elementG.Ename);
                        }
                    }
                }

            }
            else if (mode == Mode.usersofGroup)
            {
                for (int i = 0; i < selectedItemsList.Count; i++)
                {
                    usersofGroup_ = UsersAndGroupsCommander.UsersofGroupList(currentGroup);
                    for (int j = 0; j < selectedItemsList.Count; j++)
                    {
                        userName = selectedItemsList[j];
                        if (UsersAndGroupsCommander.DoesBelong(userName, currentGroup))
                        {
                            UsersAndGroupsCommander.DeleteUserFromGroup(userName, currentGroup);
                        }
                    }
                }
            }
            Refresh();
        }

        public void Magnifier_DoubleClick(TextBox tb, Visibility visibility)
        {
            if (visibility == Visibility.Visible) tb.Visibility = Visibility.Hidden;
            else tb.Visibility = Visibility.Visible;
        }
        public void Delete_Click()
        {
            List<string> Gs = new List<string>()
            {
                currentGroup
            };
            switch(mode)
            {
                case Mode.groups:
                    EmptySelectedGroups(true);
                    break;
                case Mode.all:
                    DeleteSelected();
                    break;
                case Mode.usersofGroup:
                    UsersAndGroupsCommander.DeleteUsersFromGroups(selectedItemsList, Gs);
                    break;
            }
        }



    }
}
