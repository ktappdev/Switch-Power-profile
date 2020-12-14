using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Management;

namespace Switch_Power_profile
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static public string LowGUID = "b75672a1-4a7f-4485-9a64-0445bab63964";
        static public string BalancedGUID = "381b4222-f694-41f0-9685-ff5bb260df2e";
        static public string HighGUID = "ca85b8be-8830-40df-94f7-86fe770005f1";

        public MainWindow()
        {
            
            InitializeComponent();
            ReadAndUpdateUi();



            var ts = new ThreadStart(GetChargingStatus);
            var backgroundThread = new Thread(ts);
            backgroundThread.Start();

            //MessageBox.Show($"{GetChargingStatus()}\nCurrent Profile = {GetCurrentPowerProfile()}");


            // scan to see what its on when the prog starts

        }

        public void ReadAndUpdateUi()
        {
            int State = GetCurrentPowerProfile();

            switch (State)
            {
                case 1:
                    LabelLow.Content = "Selected";
                    LabelLow.Visibility = Visibility.Visible;
                    LabelBalanced.Visibility = Visibility.Hidden;
                    LabelHigh.Visibility = Visibility.Hidden;
                    break;
                case 2:
                    LabelBalanced.Content = "Selected";
                    LabelBalanced.Visibility = Visibility.Visible;
                    LabelHigh.Visibility = Visibility.Hidden;
                    LabelLow.Visibility = Visibility.Hidden;
                    break;
                case 3:
                    LabelHigh.Content = "Selected";
                    LabelHigh.Visibility = Visibility.Visible;
                    LabelLow.Visibility = Visibility.Hidden;
                    LabelBalanced.Visibility = Visibility.Hidden;
                    break;
                default:
                    LabelBalanced.Content = "Selected";
                    LabelBalanced.Visibility = Visibility.Visible;
                    LabelHigh.Visibility = Visibility.Hidden;
                    LabelLow.Visibility = Visibility.Hidden;
                    break;
            }
        }

        static public void GetChargingStatus()
        {
            SelectQuery Sq = new SelectQuery("Win32_Battery");
            ManagementObjectSearcher objOSDetails = new ManagementObjectSearcher(Sq);
            ManagementObjectCollection osDetailsCollection = objOSDetails.Get();
            StringBuilder sb = new StringBuilder();

            Boolean Charging = false;

            foreach (ManagementObject mo in osDetailsCollection)
            {
                if ((ushort)mo["BatteryStatus"] == 2)
                {
                    Charging = true;
                    
                }
                else
                {
                    Charging = false;
                    
                }
            }
            //MessageBox.Show(Charging.ToString());
           
        }


        static public int GetCurrentPowerProfile()
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




        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (LabelLow.Visibility != Visibility.Visible)
            {
                ProcessStartInfo ps = new ProcessStartInfo();
                ps.CreateNoWindow = true;
                ps.UseShellExecute = false;
                ps.FileName = "cmd.exe";
                ps.Arguments = @"/c powercfg -setactive b75672a1-4a7f-4485-9a64-0445bab63964";
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
                LabelLow.Content = "Already selected";
            }
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (LabelBalanced.Visibility != Visibility.Visible)
            {
                ProcessStartInfo ps = new ProcessStartInfo();
                ps.CreateNoWindow = true;
                ps.UseShellExecute = false;
                ps.FileName = "cmd.exe";
                ps.Arguments = @"/c powercfg -setactive 381b4222-f694-41f0-9685-ff5bb260df2e";
                ps.RedirectStandardOutput = true;
                var proc = Process.Start(ps);

                string s = proc.StandardOutput.ReadToEnd();

                LabelBalanced.Content = "Selected";
                LabelBalanced.Visibility = Visibility.Visible;
                LabelHigh.Visibility = Visibility.Hidden;
                LabelLow.Visibility = Visibility.Hidden;
            }
            else
            {
                LabelBalanced.Content = "Already selected";
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (LabelHigh.Visibility != Visibility.Visible)
            {

                ProcessStartInfo ps = new ProcessStartInfo();
                ps.CreateNoWindow = true;
                ps.UseShellExecute = false;
                ps.FileName = "cmd.exe";
                ps.Arguments = @"/c powercfg -setactive ca85b8be-8830-40df-94f7-86fe770005f1";
                ps.RedirectStandardOutput = true;
                var proc = Process.Start(ps);
                string s = proc.StandardOutput.ReadToEnd();

                LabelHigh.Content = "Selected";
                LabelHigh.Visibility = Visibility.Visible;
                LabelLow.Visibility = Visibility.Hidden;
                LabelBalanced.Visibility = Visibility.Hidden;
            }
            else
            {
                LabelHigh.Content = "Already selected";
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

            Task task = new Task(() =>
            {
                
                List<string> processlist = new List<string>();

                Process[] processCollection = Process.GetProcesses();
                foreach (Process p in processCollection)
                {
                    if (p.ProcessName.ToLower() != "svchost")
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
                        ListBoxProcesses.Items.Add(process);
                    });
                        
                }
                processlist.Clear();
                Thread.Sleep(1000);
                
            });
            task.Start();
            
        }


    }
}
