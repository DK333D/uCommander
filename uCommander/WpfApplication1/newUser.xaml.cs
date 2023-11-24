using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// Logika interakcji dla klasy newUser.xaml
    /// </summary>
    public partial class NewUser : Window
    {
        //public object UsersAndGroupsCommander { get; private set; }

        //public static string[] weekDays = { "Pn", "Wt", "Sr", "Cz", "Pt", "Sa", "Nd" };
        //public static bool AddToLeft = false;
        //public static bool AddToRight = false;

        public List<string> Gs_l = new List<string>(); // lista grup do dodania użytkownikow do nich -lewa
        //public List<string> Gs_r = new List<string>(); // lista grup do dodania użytkownikow do nich -prawa
        public bool areGroups = false;


        public NewUser()
        {
            InitializeComponent();
            //AddToG_l.IsEnabled = AddToLeft;
            //AddToG_r.IsEnabled = AddToRight;

        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            List<string> Gs = new List<string>();
            string expires = "";

            if ((bool)Rb_expires_never.IsChecked)
            {
                expires = "never";
            }
            else
            {
                expires = Rb_expires_Dp.Content.ToString();
            }
            bool bClean = (bool)Clean.IsChecked;
            
            if (areGroups)
            {
                var resultG = System.Windows.MessageBox.Show("Czy chcesz dodać stworzonych użytkowników do grupy otwartej lub grup zanaczonych w lewym oknie?", "", MessageBoxButton.YesNoCancel);
                if (resultG == MessageBoxResult.Yes)
                {
                    Gs = Gs_l;
                }
            }
            
            if ((bool)Rb_one.IsChecked)
            {
                if (UsersAndGroupsCommander.CreateUser(NameUser.Text, Password.Password, Password_confirmation.Password, Fullname.Text, Comment.Text, (bool)Rb_active_yes.IsChecked, (bool)Rb_passwordchg_yes.IsChecked, (bool)Rb_passwordreq_yes.IsChecked, (bool)Rb_logonpasswordchg_yes.IsChecked, expires, Gs) && bClean)
                {
                    this.Close();
                    NewUser nu = new NewUser();
                    nu.ShowDialog();
                }
            }
            else
            {
                string path = FilePath.Text;
                if (UsersAndGroupsCommander.CreateUsers(Prefiks.Text, Od.Text, Do.Text, path, Fullname.Text, Comment.Text, (bool)Rb_active_yes.IsChecked, (bool)Rb_passwordchg_yes.IsChecked, (bool)Rb_passwordreq_yes.IsChecked, (bool)Rb_logonpasswordchg_yes.IsChecked, expires, Gs) && bClean)
                {
                    this.Close();
                    NewUser nu = new NewUser();
                    nu.ShowDialog();
                }
            }
            

        }


        private void NameUser_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NameUser.Text.Length < 1 && UsersAndGroupsCommander.IsNameCorrect(NameUser.Text, true)) Create.IsEnabled = false;
            else Create.IsEnabled = true;
        }
        

        private void Dp_expires_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Dp_expires_CalendarClosed(object sender, RoutedEventArgs e)
        {
            Rb_expires_Dp.IsChecked = true;
        }


        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            oneUser.Visibility = Visibility.Visible;
            moreUsers.Visibility = Visibility.Collapsed;
        }

        private void RadioButton_Checked_2(object sender, RoutedEventArgs e)
        {
            oneUser.Visibility = Visibility.Collapsed;
            moreUsers.Visibility = Visibility.Visible;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        public string GetPath()
        {
            string path = "";
            SaveFileDialog sv = new SaveFileDialog
            {
                Filter = "Text Documents(*.txt)|*.txt|All Files(*.*)|*.*"
            };
            if (sv.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                path = sv.FileName;
            return path;
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            FilePath.Text = GetPath();

        }
        private void TextChanged()
        {
            if (Prefiks.Text.Length < 1) Create.IsEnabled = false;
            else Create.IsEnabled = true;
        }

        
        private void Prefiks_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged();
            Od.MaxLength = Do.MaxLength = Prefiks.MaxLength - Prefiks.Text.Length;

        }
        

        private void Rb_one_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Rb_more.Focus();
            }

        }

        private void Rb_more_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if ((bool)Rb_one.IsChecked)
                    NameUser.Focus();
                else
                    Prefiks.Focus();
            }

        }

        private void NameUser_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Password.Focus();
            }


        }

        private void Password_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Password_confirmation.Focus();
            }

        }

        private void Password_confirmation_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Fullname.Focus();
            }

        }


        private void Prefiks_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Od.Focus();
            }

        }

        private void Od_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Do.Focus();
            }

        }

        private void Do_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FilePath.Focus();
            }

        }

        private void FilePath_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Browse.Focus();
            }

        }
        private void Browse_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Cb_test_0.Focus();
            }

        }

        private void Cb_test_0_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Fullname.Focus();
        }

        private void Fullname_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Comment.Focus();
            }

        }

        private void Comment_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Rb_active_yes.Focus();
            }

        }

        private void Rb_active_yes_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Rb_active_no.Focus();
            }

        }

        private void Rb_active_no_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Rb_passwordchg_yes.Focus();
            }

        }

        private void Rb_passwordchg_yes_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Rb_passwordchg_no.Focus();
            }

        }

        private void Rb_passwordchg_no_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Rb_passwordreq_yes.Focus();
            }

        }

        private void Rb_passwordreq_yes_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Rb_passwordreq_no.Focus();
            }

        }

        private void Rb_passwordreq_no_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Rb_logonpasswordchg_yes.Focus();
            }

        }

        private void Rb_logonpasswordchg_yes_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Rb_logonpasswordchg_no.Focus();
            }

        }

        private void Rb_logonpasswordchg_no_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Rb_expires_never.Focus();
            }

        }

        private void Rb_expires_never_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Rb_expires_Dp.Focus();
            }
            
        }
        

        private void Rb_expires_Dp_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Clean.Focus();
            }

        }

        private void Clean_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Create.IsEnabled)
                    Create.Focus();
                else
                    CloseWindow.Focus();
            }

        }

        private void AddToG_l_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Create.IsEnabled)
                    Create.Focus();
                else
                    CloseWindow.Focus();
            }

        }

        private void AddToG_r_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Create.IsEnabled)
                    Create.Focus();
                else
                    CloseWindow.Focus();
            }

        }

        private void CloseWindow_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Cb_test_1.Focus();
        }

        private void Cb_test_1_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Rb_one.Focus();

        }

        private void AddToG_l_Checked(object sender, RoutedEventArgs e)
        {
            //AddToG_r.IsChecked = false;
        }

        private void AddToG_r_Checked(object sender, RoutedEventArgs e)
        {
            //AddToG_l.IsChecked = false;

        }

        private void Rb_expires_Dp_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Dp_expires.Text.ToString().Length != 10)
            {
                Rb_expires_never.IsChecked = true;
            };
        }

        /*

       

        private void Rb_logonpasswordchg_no_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Rb_expires_never.Focus();
            }

        */
    }

}