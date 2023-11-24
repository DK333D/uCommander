using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApplication1
{

    public static class UsersAndGroupsCommander
    {


        //public static string uName;
        //public static string password;

        public static bool CzyNieTylko(string name) //sprawdza czy napis sklada sie tylko z '.' lub ' '
        {

            Match match = Regex.Match(name, @"^[\. ]+$");
            if (match.Success) return false;
            return true;
        }
    
        public static bool IsNameCorrect(string name, bool checkDots_spaces) //sprawdza czy warunki poprawnej nazwy sa spelnione, max okresla maksymalna liczbe znakow
        {

            for (int i = 0; i < name.Length; i++)
            {
                //cout << userName[i];
                if (name[i] == 34 || name[i] == 47 || name[i] == 124 || (name[i] > 41 && name[i] < 45) || (name[i] > 57 && name[i] < 65) || (name[i] > 90 && name[i] < 94))
                    return false;
            }
            if(checkDots_spaces)
                if (!CzyNieTylko(name)) return false;

            return true;

            //Match match = Regex.Match(name, @"[\*+/:;<\=>?@[\\\]|\")"
        }
        
        //funkcja zwracajaca informacje czy uzytkownik lub grupa juz istnieje
        public static bool DoesExist(string name)
        {
            if (IsItExistingUser(name) || IsItExistingGroup(name)) return true;
            return false;
        }


        public static bool IsItExistingUser(string name)
        {
            List<Element> Users = new List<Element>();
            Users = AllUsersList();

            for (int i = 0; i < Users.Count; i++)
            {
                if (Users[i].Ename.ToLower() == name.ToLower()) return true;
            }
            return false;
        }
        public static bool IsItExistingGroup(string name)
        {
            List<Element> Groups = new List<Element>();
            Groups = UsersAndGroupsCommander.GroupsList();

            for (int i = 0; i < Groups.Count; i++)
            {
                if (Groups[i].Ename.ToLower() == name.ToLower()) return true;
            }
            return false;
        }

        public static bool DoesBelong(string userName, string groupName)
        {
            List<Element> users = UsersofGroupList(groupName);
            for(int i = 0; i < users.Count; i++)
            {
                if (users[i].Ename == userName)
                    return true;
            }
            return false;
        }

        public static bool CreateUser (string name, string password, string password_c, string fullname, string comment, bool active, bool passwordchg, bool passwordreq, bool logonpasswordchg, string expires, List<string> groups)
        {
            if (DoesExist(name))
            {
                MessageBox.Show("Blad:\nUzytkownik lub grupa o takiej nazwie juz istnieje.");
                return false;
            }
            if (!(IsNameCorrect(name, true)))
            {
                MessageBox.Show("Blad:\nNie mozna uzyc nazwy " + name + ". Nazwy nie moga byc zlozone z samych kropek lub spacji i nie moga zawierac ponizszych znakow:\n/ \\ \" [ ] : | < > + = ; , ? * @ ");
                return false;
            }
            if (password != password_c)
            {
                MessageBox.Show("Blad:\nHaslo nie zostalo poprawnie potwierdzone. Upewnij sie, ze haslo i jego potwierdzenie dokladnie do siebie pasuja.");
                return false;
            }
            if (expires != "never")
                expires = expires.Substring(2, 2) + '/' + expires.Substring(5, 2) + '/' + expires.Substring(8, 2);
            AddUserWithOptions(name, password, fullname, comment, active, passwordchg, passwordreq, logonpasswordchg, expires, groups);
            return true;
        }


        public static bool CreateUsers(string prefiks, string _od, string _do, string path, string fullname, string comment, bool active, bool passwordchg, bool passwordreq, bool logonpasswordchg, string expires, List<string> groups)
        {

            string doPliku = "";
            string name = prefiks + _od;
            string password = "";
            List<string> juzIstnieja = new List<string>();
            string fullnameWithSufks = "";

            if (!(UsersAndGroupsCommander.IsNameCorrect(prefiks, false)))
            {
                MessageBox.Show("Blad:\nNie mozna uzyc nazwy rozpoczynajacej sie od " + name + ". Nazwy nie moga byc zlozone z samych kropek lub spacji i nie moga zawierac ponizszych znakow:\n / \\ \" [ ] : | < > + = ; , ? * @ ");
                return false;
            }

            if (_od.Length < 1 || _do.Length < 1)
            {
                MessageBox.Show("Musisz podać wartości sufiksów, ponieważ tworzysz wielu użytkowników.");
                return false;
            }

            if (!IfOnlyNumbers(_od) || !IfOnlyNumbers(_do))
            {
                MessageBox.Show("Sufiks może być tylko liczbą.");
                return false;
            }

            if ((Int32.Parse(_do) - Int32.Parse(_od)) < 0)
            {
                MessageBox.Show("Wartość pierwszego sufiksu nie może być większa od wartości ostatniego!");
                return false;
            }

            if (path.Length < 1 )
            {
                MessageBox.Show("Musisz podać ścieżkę do pliku, ponieważ tworzysz wielu użytkowników z losowymi hasłami.");
                return false;
            }

            if (expires != "never")
                expires = expires.Substring(2, 2) + '/' + expires.Substring(5, 2) + '/' + expires.Substring(8, 2);


            for (int sufiks = Convert.ToInt32(_od); sufiks <= Convert.ToInt32(_do); sufiks++)
            {
                name = prefiks + sufiks;
                fullnameWithSufks = fullname + '_' + sufiks;
                if (UsersAndGroupsCommander.DoesExist(name))
                {
                    juzIstnieja.Add(name);
                    continue;
                }
                else
                {
                    password = GeneratePassword(5);
                    doPliku += "nazwa: " + name + ", haslo: " + password + " | ";
                    AddUserWithOptions(name, password, fullnameWithSufks, comment, active, passwordchg, passwordreq, logonpasswordchg, expires, groups);
                }

            }
            if(juzIstnieja.Count > 0)
            {
                doPliku += " | Użytkownicy, którzy nie zostali stoworzeni, ponieważ już istnieją: ";
                for(int i=0; i<juzIstnieja.Count; i++)
                {
                    doPliku += juzIstnieja[i] + " | ";
                }
            }
            File.WriteAllText(path, doPliku);
            return true;
        }
        public static bool CreateGroup(string name)
        {
            if (DoesExist(name))
            {
                MessageBox.Show("Blad:\nUzytkownik lub grupa o takiej nazwie juz istnieje.");
            }
            else if (!(IsNameCorrect(name, true)))
            {
                MessageBox.Show("Blad:\nNie mozna uzyc nazwy " + name + ". Nazwy nie moga byc zlozone z samych kropek lub spacji i nie moga zawierac ponizszych znakow:\n/ \\ \" [ ] : | < > + = ; , ? * @ ");
                
            }
            else return true;
            
            return false;
        }
        public static void AddUser(string name)//, string logonpasswordchg, bool passwordchg, string expiresWhen, bool active, string fullname, string uComment )
        {
            
            string command = "/C net user " + '"' + name + '"' + " /add";
            RunCommand(command);
        }

        public static void AddUserWithOptions(string name, string password, string fullname, string comment, bool active, bool passwordchg, bool passwordreq, bool logonpasswordchg, string expires, List<string> Groups)
        {
            AddUser(name);
            UserOptions(name, password, fullname, comment, active, passwordchg, passwordreq, logonpasswordchg, expires, Groups);// Groups);
        }

        public static void AddGroup(string name)
        {
            if((bool)CreateGroup(name))
            {
                string command = "/C net localgroup " + '"' + name + '"' + " /add";
                RunCommand(command);
            }
           
        }

        public static void UserPassword(string name, string password)//, string logonpasswordchg, bool passwordchg, string expiresWhen, bool active, string fullname, string uComment )
        {
            string command = "/C net user " + '"' + name + '"' + ' ' + '"' + password + '"';
            RunCommand(command);
        }

        


        public static void DelGroup(string name)
        {
                string command = "/C net localgroup " + '"' + name + '"' + " /delete";
                RunCommand(command);
        }




        public static void UserOptions(string name, string password, string fullname, string comment, bool active, bool passwordchg, bool passwordreq, bool logonpasswordchg, string expires, List<string> Gs)//, List<string> GsToAddUsTo) //expires i times
        {
            string command = "/C net user " + '"' + name + '"' + " /fullname:" + '"' + fullname + '"' + " /comment:" + '"' + comment + '"';
            //------------
            if (active == true)
                command += " /active:yes";
            else
                command += " /active:no";
            //-------------
            if (passwordchg == true)
                command += " /passwordchg:yes";
            else
                command += " /passwordchg:no";
            //------------
            if (passwordreq == true)
                command += " /passwordreq:yes";
            else
                command += " /passwordreq:no";
            //------------
            if (logonpasswordchg == true)
                command += " /logonpasswordchg:yes";
            else
                command += " /logonpasswordchg:no";
            //------------

            command += " /expires:" + expires;
            RunCommand(command);


            if(password.Length > 0)
            {
                command = "/C net user " + '"' + name + '"' + ' ' + '"' + password + '"';
                RunCommand(command);
            }

            List<string> Us = new List<string>
            {
                name
            };

            for (int i=0; i<Gs.Count; i++)
            {
                AddUsersToGroups(Us, Gs);
            }
           
        }


        public static void AddUserToGroup(string userName, string groupName)
        {
                string command = "/C net localgroup " + '"' + groupName + '"' + ' ' + '"' + userName + '"' + " /add";
                RunCommand(command);
        }

        public static void DeleteUsers(List<string> toBeDeleted)
        {
            string command;
            for (int i = 0; i < toBeDeleted.Count; i++)
            {
                if (DoesExist(toBeDeleted[i]))
                {
                    command = "/C net user " + '"' + toBeDeleted[i] + '"' + " /delete";
                    RunCommand(command);
                }
                else
                {
                    MessageBox.Show("Odśwież okna!");

                }
            }
            
        }
        /*
        public static void DeleteGroup(string GroupName)
        {
            if(DoesExist(GroupName))
            {
                string command = "/C net localgroup " + '"' + GroupName + '"' + " /delete";
                RunCommand(command);
            }
            else
            {
                MessageBox.Show("Odśwież okna!");
            }
        }*/
        
        public static void DeleteGroups(List<string> toBeDeleted)
        {
            var resultG = MessageBox.Show(UsersAndGroupsCommander.CreateQuestionDelGroups(toBeDeleted), "", MessageBoxButton.YesNoCancel);
            if (resultG == MessageBoxResult.Yes)
            {
                for (int i = 0; i < toBeDeleted.Count; i++)
                {
                    if (DoesExist(toBeDeleted[i]))
                    {
                        string command = "/C net localgroup " + '"' + toBeDeleted[i] + '"' + " /delete";
                        RunCommand(command);
                    }
                    else
                    {
                        MessageBox.Show("Odśwież okna!");
                    }
                }

            }
        }
        

        public static List<Element> GroupsList()
        {
            
            Element e;
            List<Element> groupsList = new List<Element>();
            {
                DirectoryEntry machine = new DirectoryEntry("WinNT://" + Environment.MachineName + ",Computer");
                foreach (DirectoryEntry child in machine.Children)
                {
                    if (child.SchemaClassName == "Group")
                    {
                        e = new Element("G", child.Name);
                        groupsList.Add(e);
                    }
                }
                return groupsList;
            }
        }

        
        public static List<Element> AllUsersList() //argument - nazwa grupy, ktorej uzytkownicy beda wyswietlani 
        {
            Element e;
            List<Element> usersList = new List<Element>();

            DirectoryEntry machine = new DirectoryEntry("WinNT://" + Environment.MachineName + ",Computer");
            foreach (DirectoryEntry child in machine.Children)
            {   
                if (child.SchemaClassName == "User")
                {
                    e = new Element("U", child.Name);
                    usersList.Add(e);
                }
            }
            return usersList;
        }
            
        
        public static List<Element> UsersofGroupList(string groupName) //argument - nazwa grupy, ktorej uzytkownicy beda wyswietlani 
        {
            Element e;
            List<Element> usersofGroupList = new List<Element>();
            /*List<string> uoG = new List<string>();

            for (int i = 0; i < usersofGroupList.Count; i++)
                uoG.Add(usersofGroupList[i].Ename);*/


            List<Element> allUsersList = new List<Element>();
            allUsersList = UsersAndGroupsCommander.AllUsersList();

            DirectoryEntry localMachine = new DirectoryEntry
            ("WinNT://" + Environment.MachineName + ",Computer");
            DirectoryEntry admGroup = localMachine.Children.Find
                (groupName, "group");
            object members = admGroup.Invoke("members", null);
            

            foreach (object groupMember in (IEnumerable)members)
            {
                DirectoryEntry member = new DirectoryEntry(groupMember);
                e = new Element("U", member.Name);
                if (allUsersList.Exists(x => x.Ename.Contains(e.Ename)))
                    usersofGroupList.Add(e);
            }
            return usersofGroupList;
        }

        

        public static string GeneratePassword(int length)
        {
            ArrayList c = new ArrayList( );
            Int16 i=0;
            for (i = (Int16)'0'; i <= '9'; i++)
                c.Add(Convert.ToChar(i));
            for (i = (Int16)'a'; i <= 'z'; i++)
                c.Add(Convert.ToChar(i));
            for (i = (Int16)'A'; i <= 'Z'; i++)
                c.Add(Convert.ToChar(i));

            Random r=new Random();
            string p= "";
            for(int j = 0; j < length; j++)
            {
                p = p + c[r.Next(0, c.Count - 1)];
            } 
            
            return p;
        }

        public static void DeleteSelected(List<string> toRemoveList)
        {
            
        }

        public static void AddUsersToGroups(List<string> fromList, List<string> toList)
        {
            string command = "";

            for (int i = 0; i < toList.Count; i++)
            {
                for (int j = 0; j < fromList.Count; j++)
                {
                    if (!DoesBelong(fromList[j], toList[i]))
                    {
                        command = "/C net localgroup " + '"' + toList[i] + '"' + ' ' + '"' + fromList[j] + '"' + " /add";
                        RunCommand(command);
                    }
                    
                }
            }
        }


        public static void DeleteUsersFromGroups(List<string> fromList, List<string> toList)
        {
            var result = MessageBox.Show(UsersAndGroupsCommander.CreateQuestionDelFrom(fromList, toList), "", MessageBoxButton.YesNoCancel);
            if (result != MessageBoxResult.Yes) return;
            for (int i = 0; i < toList.Count; i++)
            {
                for (int j = 0; j < fromList.Count; j++)
                {
                    if (DoesBelong(fromList[j], toList[i]))
                    {
                        DeleteUserFromGroup(fromList[j], toList[i]);
                    }

                }
            }
        }

        /*public static void DeleteUsersFromGroup(List<string> usersList, string group)
        { 
            for (int j = 0; j < usersList.Count; j++)
            {
                if (DoesBelong(usersList[j], group))
                {
                    DeleteUserFromGroup(usersList[j], group);
                }
            }
        }*/

        public static void DeleteUserFromGroup(string user, string group)
        {
            string command = "/C net localgroup " + '"' + group + '"' + ' ' + '"' + user + '"' + " /delete";
            RunCommand(command);
        }

        /*
        public static List<Str2> AddItemsToLv_Times()
        {
            List<Str2> ListofDays = new List<Str2>();
            Str2 item;
            for(int i=0; i<7; i++)
            {
                item = new Str2(weekDays[i], "");
                ListofDays.Add(item);
            }
            
            return ListofDays;
        }*/
        public static string CreateQuestionAddTo(List<string> U, List<string> G)
        {
            int preferedCountU = 100;
            int preferedCountG = 100;

            int realCountU=0;
            int realCountG=0;



            string question = "Czy na pewno chcesz dodać użytkowników:\n";
            string qusers = "";
            string qgroups = "";

            for (int i = 0; i < U.Count - 1; i++)
            {
                realCountU += U[i].Length;

                qusers += U[i] + ", ";

            }
            realCountU += U[U.Count - 1].Length;
            qusers += U[U.Count - 1] ;

            if (realCountU > preferedCountU)
            {
                question += qusers.Substring(0, realCountU) + "...";
            }
            else question += qusers;



            question += "\ndo grup:\n";
            
            for (int i = 0; i < G.Count - 1; i++)
            {
                realCountG += G[i].Length;
                qgroups += G[i] + ", ";

            }
            realCountG += G[G.Count - 1].Length;
            qgroups += G[G.Count - 1];



            if (realCountG > preferedCountG)
            {
                question += qgroups.Substring(0, realCountG) + "...?";
            }
            else question += qgroups + "?";

            return question;

        }

        public static string CreateQuestionDelFrom(List<string> U, List<string> G)
        {
            int preferedCountU = 100;
            int preferedCountG = 100;

            int realCountU = 0;
            int realCountG = 0;



            string question = "Czy na pewno chcesz usunąć użytkowników:\n";
            string qusers = "";
            string qgroups = "";

            for (int i = 0; i < U.Count - 1; i++)
            {
                realCountU += U[i].Length;
                qusers += U[i] + ", ";

            }
            realCountU += U[U.Count - 1].Length;
            qusers += U[U.Count - 1];

            if (realCountU > preferedCountU)
            {
                question += qusers.Substring(0, realCountU) + "...";
            }
            else question += qusers;





            if(G.Count == 1)
            {
                question += "\nz grupy ";
                realCountG += G[0].Length;
                qgroups += G[0];

            }
            else
            {
                question += "\nz grup:\n";
                for (int i = 0; i < G.Count - 1; i++)
                {
                    realCountG += G[i].Length;
                    qgroups += G[i] + ", ";
                }
                realCountG += G[G.Count - 1].Length;
                qgroups += G[G.Count - 1];
            


            }
                if (realCountG > preferedCountG)
                {
                    question += qgroups.Substring(0, realCountG) + "...?";
                }

                else question += qgroups + "?";

            return question;
            
        }

        public static string CreateQuestionDel(List<string> list, bool Users_notGroups)
        {
            int preferedCount = 100;
            int realCount = 0;
            string part = "";

            string question = "Czy na pewno chcesz usunąć ";

            if (Users_notGroups)
                question += "użytkowników:\n";
            else
                question += "z grupy:\n";

            
            for (int i = 0; i < list.Count - 1; i++)
            {
                realCount += list[i].Length;

                part += list[i] + ", ";

            }
            realCount += list[list.Count - 1].Length;
            part += list[list.Count - 1];
            
            if (realCount > preferedCount)
            {
                question += part.Substring(0, realCount) + "...";
            }
            else question += part;

            question += "?";
            
            return question;
        }


        public static string CreateQuestionMove(List<string> U, List<string> G, string currentGroup)
        {
            int preferedCountU = 100;
            int preferedCountG = 100;

            int realCountU = 0;
            int realCountG = 0;



            string question = "Czy na pewno chcesz przenieść użytkowników:\n";
            string qusers = "";
            string qgroups = "";

            for (int i = 0; i < U.Count - 1; i++)
            {
                realCountU += U[i].Length;
                qusers += U[i] + ", ";

            }
            realCountU += U[U.Count - 1].Length;
            qusers += U[U.Count - 1];

            if (realCountU > preferedCountU)
            {
                question += qusers.Substring(0, realCountU) + "...";
            }
            else question += qusers;

            

            if (G.Count == 1)
            {
                question += " do grupy ";
                realCountG += G[0].Length;
                qgroups += G[0];

            }
            else
            {
                question += "\ndo grup:\n";
                for (int i = 0; i < G.Count - 1; i++)
                {
                    realCountG += G[i].Length;
                    qgroups += G[i] + ", ";
                }
                realCountG += G[G.Count - 1].Length;
                qgroups += G[G.Count - 1];



            }
            if (realCountG > preferedCountG)
            {
                question += qgroups.Substring(0, realCountG) + "...?";
            }

            else question += qgroups + "?";

            return question;

        }

        public static string CreateQuestionEmpty(List<string> listG)
        {
            int preferedCount = 100;
            int realCount = 0;
            string part = "";

            string question = "Czy na pewno chcesz opróżnić te grupy:\n";
            
            for (int i = 0; i < listG.Count - 1; i++)
            {
                realCount += listG[i].Length;
                part += listG[i] + ", ";
            }

            realCount += listG[listG.Count - 1].Length;
            part += listG[listG.Count - 1];

            if (realCount > preferedCount)
            {
                question += part.Substring(0, realCount) + "...";
            }
            else question += part;
            question += "?";

            return question;
        }


        public static string CreateQuestionDelGroups(List<string> listG)
        {
            int preferedCount = 100;
            int realCount = 0;
            string part = "";

            string question = "Czy na pewno chcesz usunąć te grupy:\n";

            for (int i = 0; i < listG.Count - 1; i++)
            {
                realCount += listG[i].Length;
                part += listG[i] + ", ";
            }

            realCount += listG[listG.Count - 1].Length;
            part += listG[listG.Count - 1];

            if (realCount > preferedCount)
            {
                question += part.Substring(0, realCount) + "...";
            }
            else question += part;
            question += "?";

            return question;
        }

        public static bool IfOnlyNumbers(string liczba)
        {
            string pattern = @"^\d+$";
            Regex r = new Regex(pattern, RegexOptions.IgnoreCase);

            Match m = r.Match(liczba);
            while (m.Success)
            {
                return true;
            }
            return false;
        }


        static private void RunCommand(string command)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.StartInfo.Arguments = command;
            cmd.Start();
            cmd.WaitForExit();
        }

    }
           
    
}
