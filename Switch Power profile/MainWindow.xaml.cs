using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Threading;
using System.Management;

namespace Switch_Power_profile
{
    public partial class MainWindow : Window
    {
        static public string LowGUID = "64a64f24-65b9-4b56-befd-5ec1eaced9b3";
        static public string BalancedGUID = "381b4222-f694-41f0-9685-ff5bb260df2e";
        static public string HighGUID = "6fecc5ae-f350-48a5-b669-b472cb895ccf";


        public MainWindow()
        {
            
            InitializeComponent();
            ReadAndUpdateUi();
            GetAllPowerProfiles();
            GetProcessesList();
            //GetChargingStatus();

            //var ts = new ThreadStart(SetProfileOnConnect);
            //var backgroundThread = new Thread(ts);
            //backgroundThread.Start();

        }

        public Dictionary<string, string> GetAllPowerProfiles() // working on this 
        {
            Dictionary<string, string> AllProfiles = new Dictionary<string, string>();

            ProcessStartInfo ps = new ProcessStartInfo();
            ps.CreateNoWindow = true;
            ps.UseShellExecute = false;
            ps.FileName = "cmd.exe";
            ps.Arguments = $@"/c powercfg -list";
            ps.RedirectStandardOutput = true;
            var proc = Process.Start(ps);

            string s = proc.StandardOutput.ReadToEnd();
            string[] result = s.Split('\n');

            for (var i = 0; i < result.Length; i++)
            {

                if (!result[i].ToLower().Contains("power saver")
                    || result[i].ToLower().Contains("balanced")
                    || result[i].ToLower().Contains("high performance")
                    || result[i].ToLower().Contains("ultimate performance"))
                {
                    
                }

                if (result[i].ToLower().Contains("power saver") 
                    || result[i].ToLower().Contains("balanced") 
                    || result[i].ToLower().Contains("high performance") 
                    || result[i].ToLower().Contains("ultimate performance"))
                {
                    string[] parts = result[i].Split();

                    //MessageBox.Show($"{parts[5]} {parts[6]} {parts[3]}");
                    AllProfiles.Add($"{parts[5]} {parts[6]}", parts[3]);
                }
                    

                //AllProfiles.Add($"{i}", result[i]);

            }

            

            return AllProfiles;
        }

        public void ReadAndUpdateUi()
        {
            int State = GetCurrentPowerProfile();

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
            if (LabelLow.Visibility != Visibility.Visible)
            {
                ProcessStartInfo ps = new ProcessStartInfo();
                ps.CreateNoWindow = true;
                ps.UseShellExecute = false;
                ps.FileName = "cmd.exe";
                ps.Arguments = $@"/c powercfg -setactive {LowGUID}";
                ps.RedirectStandardOutput = true;
                var proc = Process.Start(ps);

                string s = proc.StandardOutput.ReadToEnd();
                

                LabelLow.Content = $"Selected";
                LabelLow.Visibility = Visibility.Visible;
                LabelBalanced.Visibility = Visibility.Hidden;
                LabelHigh.Visibility = Visibility.Hidden;
            }
            else
            {
                LabelLow.Content = "SELECTED";
            }
        }


        public void SetBalancedProfile()
        {
            if (LabelBalanced.Visibility != Visibility.Visible)
            {
                ProcessStartInfo ps = new ProcessStartInfo();
                ps.CreateNoWindow = true;
                ps.UseShellExecute = false;
                ps.FileName = "cmd.exe";
                ps.Arguments = $@"/c powercfg -setactive {BalancedGUID}";
                ps.RedirectStandardOutput = true;
                var proc = Process.Start(ps);

                //string s = proc.StandardOutput.ReadToEnd();

                LabelBalanced.Content = "Selected";
                LabelBalanced.Visibility = Visibility.Visible;
                LabelHigh.Visibility = Visibility.Hidden;
                LabelLow.Visibility = Visibility.Hidden;
            }
            else
            {
                LabelBalanced.Content = "SELECTED";
            }
        }

        public void SetHighProfile()
        {
            if (LabelHigh.Visibility != Visibility.Visible)
            {

                ProcessStartInfo ps = new ProcessStartInfo();
                ps.CreateNoWindow = true;
                ps.UseShellExecute = false;
                ps.FileName = "cmd.exe";
                ps.Arguments = $@"/c powercfg -setactive {HighGUID}";
                ps.RedirectStandardOutput = true;
                var proc = Process.Start(ps);
                //string s = proc.StandardOutput.ReadToEnd();

                LabelHigh.Content = "Selected";
                LabelHigh.Visibility = Visibility.Visible;
                LabelLow.Visibility = Visibility.Hidden;
                LabelBalanced.Visibility = Visibility.Hidden;
            }
            else
            {
                LabelHigh.Content = "SELCETED";
            }
        }


        public Boolean GetChargingStatus() //Work in progress, not really implimented
        {
            SelectQuery Sq = new SelectQuery("Win32_Battery");
            ManagementObjectSearcher objOSDetails = new ManagementObjectSearcher(Sq);
            ManagementObjectCollection osDetailsCollection = objOSDetails.Get();
            StringBuilder sb = new StringBuilder();

            Boolean Charging = false;

            //ListBoxProcesses.Items.Add(osDetailsCollection);

            foreach (ManagementObject mo in osDetailsCollection)
            {
                if ((ushort)mo["BatteryStatus"] == 2)
                {
                    return Charging = true;
                }
                else
                {
                    //SetLowProfile();
                    return Charging = false;
                }

            }
            return Charging;

        }



        public void SetProfileOnConnect()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                if (GetChargingStatus() == true)
                {
                    if (GetCurrentPowerProfile() != 3)
                    {
                        SetBalancedProfile();
                    }
                }
                else if (GetChargingStatus() == false)
                {
                    SetLowProfile();
                }
            }));
        }

        public int GetCurrentPowerProfile()
        {
            ProcessStartInfo ps = new ProcessStartInfo();
            ps.CreateNoWindow = true;
            ps.UseShellExecute = false;
            ps.FileName = "cmd.exe";
            ps.Arguments = @"/c powercfg -getactivescheme";
            ps.RedirectStandardOutput = true;
            var proc = Process.Start(ps);

            string s = proc.StandardOutput.ReadToEnd();

            if (s.Contains(LowGUID))
            {
                return 1;
            }
            else if (s.Contains(BalancedGUID))
            {
                return 2;
            }
            else
            {
                return 3;
            }

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
                    && !processlist.Contains(p.ProcessName))
                    {
                        processlist.Add(p.ProcessName);
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
                processlist.Clear();
                //Thread.Sleep(1000);

            });
            task.Start();
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



        private void Add_Monitor(object sender, RoutedEventArgs e)
        {
            if (ListBoxProcesses.SelectedItem != null)
            {
                ListBoxMonProcesses.Items.Add(ListBoxProcesses.SelectedItem);
                ListBoxProcesses.Items.Remove(ListBoxProcesses.SelectedItem);
            }
            else
            {
                MessageBox.Show("Please select a process to monitor");
            }
        }

        private void GetProcessButton(object sender, RoutedEventArgs e)
        {
            GetProcessesList();
        }

        private void RemoveMonitorButton(object sender, RoutedEventArgs e)
        {
            if (ListBoxMonProcesses.SelectedItem != null)
            {
                ListBoxMonProcesses.Items.Remove(ListBoxMonProcesses.SelectedItem);
                //ListBoxProcesses.Items.Remove(ListBoxProcesses.SelectedItem);
            }
            else
            {
                MessageBox.Show("No item selected");
            }
        }

        private void MonitorMode_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
