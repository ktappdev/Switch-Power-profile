using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Threading;
using System.Management;
using System.IO;

namespace AutomaticPowerManager
{
    public partial class MainWindow : Window
    {
        //static Dictionary<string, string> guids = Functions.GetAllPowerProfiles();

        //static public string LowGUID = guids["Power"];
        //static public string BalancedGUID = guids["Balanced"];
        //static public string HighGUID = guids["High"];


        public MainWindow()
        {
            

            InitializeComponent();
            Functions.GetAllPowerProfiles();
            ReadAndUpdateUi();
            
            GetProcessesList();
            UpdateMonListBox(Functions.Readdb());
            MonitorPrograms();

            //GetChargingStatus();


            //var ts = new ThreadStart(SetProfileOnConnect);
            //var backgroundThread = new Thread(ts);
            //backgroundThread.Start();

        }

        

        public void ReadAndUpdateUi()
        {
            int State = Functions.GetCurrentPowerProfile();

            switch (State)
            {
                case 1:
                    SetLowProfile();
                    break;
                case 2:
                    SetBalancedProfile();
                    break;
                case 3:
                    SetHighProfile();
                    break;
                default:
                    SetBalancedProfile();
                    break;
            }
        }

        public void SetLowProfile()
        {
            int State = Functions.GetCurrentPowerProfile();
            try
            {
                if (LabelLow.Visibility != Visibility.Visible && State != 1)
                {
                    ProcessStartInfo ps = new ProcessStartInfo();
                    ps.CreateNoWindow = true;
                    ps.UseShellExecute = false;
                    ps.FileName = "cmd.exe";
                    ps.Arguments = $@"/c powercfg -setactive {Functions.GetAllPowerProfiles()["Power"]}";
                    ps.RedirectStandardOutput = true;
                    var proc = Process.Start(ps);

                    //string s = proc.StandardOutput.ReadToEnd();
                    //MessageBox.Show(s);

                    LabelLow.Content = $"Selected";
                    LabelLow.Visibility = Visibility.Visible;
                    LabelBalanced.Visibility = Visibility.Hidden;
                    LabelHigh.Visibility = Visibility.Hidden;
                }
                else
                {
                    LabelLow.Content = "SELECTED";
                    LabelLow.Visibility = Visibility.Visible;
                    LabelBalanced.Visibility = Visibility.Hidden;
                    LabelHigh.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception e)
            {

                throw;
            }
            
        }


        public void SetBalancedProfile()
        {
            int State = Functions.GetCurrentPowerProfile();

            if (LabelBalanced.Visibility != Visibility.Visible && State != 2)
            {
                ProcessStartInfo ps = new ProcessStartInfo();
                ps.CreateNoWindow = true;
                ps.UseShellExecute = false;
                ps.FileName = "cmd.exe";
                ps.Arguments = $@"/c powercfg -setactive {Functions.GetAllPowerProfiles()["Balanced"]}";
                ps.RedirectStandardOutput = true;
                var proc = Process.Start(ps);
                
                //string s = proc.StandardOutput.ReadToEnd();
                //MessageBox.Show(s);
                LabelBalanced.Content = "Selected";
                LabelBalanced.Visibility = Visibility.Visible;
                LabelHigh.Visibility = Visibility.Hidden;
                LabelLow.Visibility = Visibility.Hidden;
            }
            else
            {
                LabelBalanced.Content = "SELECTED";
                LabelBalanced.Visibility = Visibility.Visible;
                LabelHigh.Visibility = Visibility.Hidden;
                LabelLow.Visibility = Visibility.Hidden;
            }
        }

        public void SetHighProfile()
        {
            int State = Functions.GetCurrentPowerProfile();
            if (LabelHigh.Visibility != Visibility.Visible && State != 3)
            {

                ProcessStartInfo ps = new ProcessStartInfo();
                ps.CreateNoWindow = true;
                ps.UseShellExecute = false;
                ps.FileName = "cmd.exe";
                ps.Arguments = $@"/c powercfg -setactive {Functions.GetAllPowerProfiles()["High"]}";
                ps.RedirectStandardOutput = true;
                var proc = Process.Start(ps);
                //string s = proc.StandardOutput.ReadToEnd();
                //MessageBox.Show(s);
                LabelHigh.Content = "Selected";
                LabelHigh.Visibility = Visibility.Visible;
                LabelLow.Visibility = Visibility.Hidden;
                LabelBalanced.Visibility = Visibility.Hidden;
            }
            else
            {
                LabelHigh.Content = "SELCETED";
                LabelHigh.Visibility = Visibility.Visible;
                LabelLow.Visibility = Visibility.Hidden;
                LabelBalanced.Visibility = Visibility.Hidden;
            }
        }


        



        public void SetProfileOnConnect()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                if (Functions.GetChargingStatus() == true)
                {
                    if (Functions.GetCurrentPowerProfile() != 3)
                    {
                        SetBalancedProfile();
                    }
                }
                else if (Functions.GetChargingStatus() == false)
                {
                    SetLowProfile();
                }
            }));
        }



        public void UpdateMonListBox(List<string> lines)
        {
            ListBoxMonProcesses.Items.Clear();
            foreach (string line in lines)
            {
                ListBoxMonProcesses.Items.Add(line); // populate a listbox or setup a return
            }

            //foreach (string line in lines)
            //{
                
            //    //ListBoxMonProcesses.Items.Add(line);
            //    foreach (string item in ListBoxMonProcesses.Items)
            //    {
            //        if (item != line)
            //        {

            //            ListBoxMonProcesses.Items.Add(line); // populate a listbox or setup a return
            //        }

            //    }

            //}
        }


        public void GetProcessesList()
        {
            Task task = new Task(() =>
            {

                List<string> processlist = new List<string>();

                Process[] processCollection = Process.GetProcesses();
                foreach (Process p in processCollection)
                {

                    if (p.ProcessName.ToLower() != "svchost"
                    && p.ProcessName.ToLower() != "taskmgr"
                    && p.ProcessName.ToLower() != "spoolsv"
                    && p.ProcessName.ToLower() != "lsass"
                    && p.ProcessName.ToLower() != "csrss"
                    && p.ProcessName.ToLower() != "smss"
                    && p.ProcessName.ToLower() != "winlogon"
                    && p.ProcessName.ToLower() != "services"
                    && p.ProcessName.ToLower() != "rundll32"
                    && p.ProcessName.ToLower() != "system"
                    && p.ProcessName.ToLower() != "name of this program"
                    && !processlist.Contains(p.ProcessName))
                    {
                        processlist.Add(p.ProcessName);
                    }
                }
                foreach (string item in ListBoxMonProcesses.Items)
                {
                    if (processlist.Contains(item))
                    {
                        processlist.Remove(item);
                    }
                }
                this.Dispatcher.Invoke(() =>
                {
                    ListBoxProcesses.Items.Clear();
                });
                processlist.Sort();
                foreach (var process in processlist)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        ListBoxProcesses.Items.Add(process); // populate a listbox or setup a return
                        //if (process.ToLower() == "simplenote")
                        //    {
                        //        MessageBox.Show("Simplenote running!");
                        //    }
                    });

                }
                // load up monitor list
                
                processlist.Clear();
                //Thread.Sleep(1000);

            });
            task.Start();
        }





        public void MonitorPrograms() // works
        {
            Task task = new Task(() =>
            {
                while (true)
                {
                    try
                    {
                        Process[] processCollection = Process.GetProcesses();
                        foreach (Process p in processCollection)
                        {
                            foreach (string item in ListBoxMonProcesses.Items)
                            {
                                if(p.ProcessName == item)
                                {
                                    MessageBox.Show($"{p.ProcessName} running");

                                    
                                    this.Dispatcher.Invoke(() =>
                                    {
                                        SetHighProfile();
                                    });
                                    //Thread.Sleep(10000);
                                    break;

                                }
                                
                               
                            }

                        }
                    }
                    catch(Exception e)
                    {

                    }

                    
                    Thread.Sleep(10000);

                    //    this.Dispatcher.Invoke(() =>
                    //{
                    //    SetHighProfile();
                    //    });
                }
                
                
               
            });
            task.Start();
        }




        private void Add_Monitor(object sender, RoutedEventArgs e)
        {
            if (ListBoxProcesses.SelectedItem != null)
            {
                ListBoxMonProcesses.Items.Add(ListBoxProcesses.SelectedItem);
                Functions.Writedb(ListBoxProcesses.SelectedItem.ToString()); // write to db

                ListBoxProcesses.Items.Remove(ListBoxProcesses.SelectedItem);

            }
            else
            {
                MessageBox.Show("Please select a process to monitor");
            }
        }


        private void SetLowButton(object sender, RoutedEventArgs e)
        {
            SetLowProfile();
        }

        private void SetBalancedButton(object sender, RoutedEventArgs e)
        {
            SetBalancedProfile();
        }

        private void SetHighButton(object sender, RoutedEventArgs e)
        {
            SetHighProfile();
        }

        

        private void GetProcessButton(object sender, RoutedEventArgs e)
        {
            GetProcessesList();

        }



        private void RemoveMonitorButton(object sender, RoutedEventArgs e)
        {
            List<string> NewMonItems = new List<string>();

            if (ListBoxMonProcesses.SelectedItem != null)
            {
                //ListBoxProcesses.Items.Add(ListBoxMonProcesses.SelectedItem); //re-add the process to the running processes list
                ListBoxMonProcesses.Items.Remove(ListBoxMonProcesses.SelectedItem);
                //File.WriteAllText(Functions.path, ListBoxMonProcesses.Items.ToString());
                
                foreach (string item in ListBoxMonProcesses.Items) // create a temp list of current items in listbox
                {                    
                    NewMonItems.Add(item);
                }

                File.Delete(Functions.path);
                File.AppendAllLines(Functions.path, NewMonItems.ToArray()); //make a new file with the items from the listbox
                //UpdateMonListBox(Functions.Readdb());
            }
            else
            {
                MessageBox.Show("No item selected");
            }
        }



        private void MonitorMode_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //present a warning messagebox yes/no
            if (MessageBox.Show("Clear Monitor List?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                ListBoxMonProcesses.Items.Clear();
                File.Delete(Functions.path);
            }
            
            

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
