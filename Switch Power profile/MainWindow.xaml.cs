using System;
using System.Collections.Generic;
//using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Threading;
//using System.Management;
using System.IO;
using Forms = System.Windows.Forms;
using System.Reflection;
using Switch_Power_profile;
//using System.Drawing;

namespace AutomaticPowerManager
{
    public partial class MainWindow : Window
    {
        //static Dictionary<string, string> guids = Functions.GetAllPowerProfiles();

        public static Forms.NotifyIcon notifyIcon = new Forms.NotifyIcon();
        public static List<string> CurrentlyRunningList = new List<string>();
        public MainWindow()
        {
            notifyIcon.Icon = new System.Drawing.Icon(@"C:\Users\KenDaBeatMaker\source\repos\Switch Power profile\Switch Power profile\images\500White.ico");
            notifyIcon.Visible = true;
            notifyIcon.Text = "Automatic Power Manager - KTAD";
            notifyIcon.DoubleClick += Notify_DoubleClick; //Notify_Click;
            notifyIcon.BalloonTipClicked += Baloon_Clicked;
            notifyIcon.ContextMenuStrip = new Forms.ContextMenuStrip();

            //dont need these for first version of the app

            notifyIcon.ContextMenuStrip.Items.Add(new Forms.ToolStripMenuItem("Show App", null, TrayIconShowApp));
            notifyIcon.ContextMenuStrip.Items.Add(new Forms.ToolStripMenuItem("Exit", null, TrayIconExit));


            InitializeComponent();
            RunningInstance(); //works, needs further testing

            Functions.CreateAppDir();
            GetSettingsAndUpdate(Functions.ReadSettings());
            Functions.GetAllPowerProfiles();
            ReadAndUpdateUi();
            GetProcessesList();
            UpdateMonListBox(Functions.ReadWatchlist());
            Functions.AddApplicationToStartup(Convert.ToBoolean(Functions.ReadSettings()[0]));
            MonitorPrograms();
        }



        private void Notify_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            notifyIcon.Visible = false;
            WindowState = WindowState.Normal;
        }



        public static void RunningInstance()
        {
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);

            foreach (Process process in processes)
            {
                
                if (process.Id != current.Id) // find the other instance by name    && process.ProcessName == "Automatic Power manager"
                {
                    
                    if (Assembly.GetExecutingAssembly().Location.
                         Replace("/", "\\") == current.MainModule.FileName)

                    {

                        //MessageBox.Show("Already running");
                        //notifyIcon.Dispose();
                        //process.CloseMainWindow();
                        process.Kill();
                        //Environment.Exit(0);
                        

                    }
                }
            }
              
        }




    private void Baloon_Clicked(object sender, EventArgs e)
        {
            this.Show();
            notifyIcon.Visible = false;
            WindowState = WindowState.Normal;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            //notifyIcon.BalloonTipTitle = "APM Running Minimized";
            //notifyIcon.BalloonTipText = "Click icon to show app";

            this.Hide();

            notifyIcon.Visible = true;
            notifyIcon.ShowBalloonTip(500, "APM Running Minimized", "Click icon to show app", Forms.ToolTipIcon.Info);

        }

        //private void Notify_Click(object sender, Forms.MouseEventArgs e)
        //{
        //    this.Show();
        //    notifyIcon.Visible = false;
        //    WindowState = WindowState.Normal;

        //}




        private void TrayIconShowApp(object sender, EventArgs e)
        {
            this.Show();
            notifyIcon.Visible = false;
            WindowState = WindowState.Normal;
        }

        private void TrayIconExit(object sender, EventArgs e)
        {
            //MonitorMode.IsChecked = !MonitorMode.IsChecked.Value; 
            notifyIcon.Dispose();
            System.Environment.Exit(0);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            notifyIcon.Dispose();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Do you want to exit? Click \"No\" to minimize to system tray", "Exit?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                e.Cancel = true;
                this.Hide();
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(500, "APM Running Minimized", "Click icon to show app", Forms.ToolTipIcon.Info);
            }
            
        }



        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                this.Hide();
                notifyIcon.Visible = true;
                
                notifyIcon.ShowBalloonTip(500, "APM Running Minimized", "Click icon to show app", Forms.ToolTipIcon.Info);
                
            }
            else if (this.WindowState == WindowState.Normal)
            {
                notifyIcon.Visible = false;
            }
        }

        public void GetSettingsAndUpdate(List<string> settingsData)
        {
            try
            {
                if (settingsData[1] == "True")
                {
                    MonitorMode.IsChecked = true;
                }
                else if (settingsData[1] == "False")
                {
                    MonitorMode.IsChecked = false;
                }

                if (settingsData[0] == "True")
                {
                    Startup.IsChecked = true;
                }
                else if (settingsData[0] == "False")
                {
                    Startup.IsChecked = false;
                }
                RateSlider.Value = Convert.ToDouble(settingsData[2]);
                RateSlider.Minimum = 5.0;
                RateSlider.Maximum = 20.0;

            }
            catch (Exception)
            {
            }

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
                    notifyIcon.ShowBalloonTip(1000, "System switched to Low Powered", "No monitored program is running and on battery", Forms.ToolTipIcon.Info);
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
                notifyIcon.ShowBalloonTip(1000, "System switched to Balanced", "No monitored program is running and Power plugged in", Forms.ToolTipIcon.Info);

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
                notifyIcon.ShowBalloonTip(1000, "System switched to High Performane", $"{CurrentlyRunningList[0]} is running", Forms.ToolTipIcon.Info);
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
                    && p.ProcessName.ToLower() != "automatic power manager"
                    && p.ProcessName.ToLower() != "idle"
                    && p.ProcessName.ToLower() != "systemsettings"
                    && p.ProcessName.ToLower() != "wininit"
                    && p.ProcessName.ToLower() != "armsvc"
                    && p.ProcessName.ToLower() != "winlogon"
                    && p.ProcessName.ToLower() != "wdfmgr"
                    && p.ProcessName.ToLower() != "spoolsv"
                    && p.ProcessName.ToLower() != "explorer"
                    && p.ProcessName.ToLower() != "fontdrvhost"
                    && p.ProcessName.ToLower() != "wmiprvse"
                    && p.ProcessName.ToLower() != "PresentationFontCache".ToLower()
                    && p.ProcessName.ToLower() != "esif_uf".ToLower()
                    && p.ProcessName.ToLower() != "IntelCpHDCPSvc".ToLower()
                    && p.ProcessName.ToLower() != "AsLdrSrv64".ToLower()
                    && p.ProcessName.ToLower() != "nvcontainer".ToLower()
                    && p.ProcessName.ToLower() != "ibtsiva".ToLower()
                    && p.ProcessName.ToLower() != "OfficeClickToRun".ToLower()
                    && p.ProcessName.ToLower() != "PnkBstrA".ToLower()
                    && p.ProcessName.ToLower() != "LDSvc".ToLower()
                    && p.ProcessName.ToLower() != "dasHost".ToLower()
                    && p.ProcessName.ToLower() != "USBChargerService".ToLower()
                    && p.ProcessName.ToLower() != "dllhost".ToLower()
                    && p.ProcessName.ToLower() != "ROGGamingCenterService".ToLower()
                    && p.ProcessName.ToLower() != "SgrmBroker".ToLower()
                    && p.ProcessName.ToLower() != "dwm".ToLower()
                    && p.ProcessName.ToLower() != "taskhostw".ToLower()
                    && p.ProcessName.ToLower() != "SearchApp".ToLower()
                    && p.ProcessName.ToLower() != "nvsphelper64".ToLower()
                    && p.ProcessName.ToLower() != "NVIDIA Share".ToLower()
                    && p.ProcessName.ToLower() != "conhost".ToLower()
                    && p.ProcessName.ToLower() != "WUDFHost".ToLower()
                    && p.ProcessName.ToLower() != "esrv_svc".ToLower()
                    && p.ProcessName.ToLower() != "applicationframehost".ToLower()
                    && p.ProcessName.ToLower() != "apsdaemon".ToLower()
                    && p.ProcessName.ToLower() != "ashidsrv64".ToLower()
                    && p.ProcessName.ToLower() != "asmonstartuptask64".ToLower()
                    && p.ProcessName.ToLower() != "atkexcomsvc".ToLower()
                    && p.ProcessName.ToLower() != "comppkgsrv".ToLower()
                    && p.ProcessName.ToLower() != "esrv".ToLower()
                    && p.ProcessName.ToLower() != "iclouddrive".ToLower()
                    && p.ProcessName.ToLower() != "icloudservices".ToLower()
                    && p.ProcessName.ToLower() != "intel_pie_service".ToLower()
                    && p.ProcessName.ToLower() != "memory compression".ToLower()
                    && p.ProcessName.ToLower() != "nvidia web helper".ToLower()
                    && p.ProcessName.ToLower() != "nvdisplay.container".ToLower()
                    && p.ProcessName.ToLower() != "microsoft.servicehub.controller".ToLower()
                    && p.ProcessName.ToLower() != "msbuild".ToLower()
                    && p.ProcessName.ToLower() != "perfwatson2".ToLower()
                    && p.ProcessName.ToLower() != "ravbg64".ToLower()
                    && p.ProcessName.ToLower() != "registry".ToLower()
                    && p.ProcessName.ToLower() != "wlanext".ToLower()
                    && p.ProcessName.ToLower() != "sursvc".ToLower()
                    && p.ProcessName.ToLower() != "startmenuexperiencehost".ToLower()
                    && p.ProcessName.ToLower() != "standardcollector.service".ToLower()
                    && p.ProcessName.ToLower() != "shellexperiencehost".ToLower()
                    && p.ProcessName.ToLower() != "securesystem".ToLower()
                    && p.ProcessName.ToLower() != "runtimebroker".ToLower()
                    && p.ProcessName.ToLower() != "applemobiledeviceprocess".ToLower()
                    && p.ProcessName.ToLower() != "atkosd2".ToLower()
                    && p.ProcessName.ToLower() != "googlecrashhandler".ToLower()
                    && p.ProcessName.ToLower() != "googlecrashhandler64".ToLower()
                    && p.ProcessName.ToLower() != "hxtsr".ToLower()
                    && p.ProcessName.ToLower() != "intelcphecisvc".ToLower()
                    && p.ProcessName.ToLower() != "jhi_service".ToLower()
                    && p.ProcessName.ToLower() != "cmd".ToLower()
                    
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
            //if (MonitorMode.IsChecked == false)
            //{
            //    return;
            //}
            //List<string> CurrentlyRunningList = new List<string>();
            Boolean ListEmpty = false;
            Boolean MonitorModeOnOrOff = true;
            //Boolean NoRunningProgram = false;
            int t = 10000;
            Task task = new Task(() => //run these commands on another thread
            {
                while (true)
                {
                    

                    this.Dispatcher.Invoke(() =>
                    {
                        
                        MonitorModeOnOrOff = (bool)MonitorMode.IsChecked;
                        

                    });

                    
                    if(MonitorModeOnOrOff == true)
                    {
                        try
                        {

                            if (ListBoxMonProcesses.Items.Count == 0)
                            {
                                ListEmpty = true; //check to see if the list is empty and set this to true if it is
                            }
                            if (ListBoxMonProcesses.Items.Count > 0)
                            {
                                CurrentlyRunningList.Clear(); 
                                ListEmpty = false;
                                Process[] processCollection = Process.GetProcesses();
                                foreach (Process p in processCollection)
                                {
                                    foreach (string item in ListBoxMonProcesses.Items)
                                    {
                                        if (p.ProcessName == item)
                                        {
                                            CurrentlyRunningList.Add(item);

                                            break;

                                        }
                                        


                                    }

                                }

                            }
                            

                            if (ListEmpty == true || CurrentlyRunningList.Count == 0) // Boss logic :) so simple don't need a list tho
                            {
                                if (Functions.GetChargingStatus() == false)
                                {
                                    this.Dispatcher.Invoke(() =>
                                    {

                                        SetLowProfile();
                                        //ListEmpty = true;
                                    });
                                }
                                else
                                {
                                    this.Dispatcher.Invoke(() =>
                                    {

                                        SetBalancedProfile();
                                        //ListEmpty = true;
                                    });
                                }
                                
                            }
                            else
                            {
                                this.Dispatcher.Invoke(() =>
                                {
                                    SetHighProfile();
                                   

                                });
                            }

                        }

                        catch (Exception e)
                        {
                            //currently not doing anything with the error, there really isnt any error i can think of o_o
                        }
                    }


                    this.Dispatcher.Invoke(() =>
                    {
                        t = (int)RateSlider.Value * 1000;

                    });

                    
                    Thread.Sleep(t); //sleep for 10 seconds

                }
                
                
               
            });
            task.Start(); //starts the threaded task
        }




        private void Add_Monitor(object sender, RoutedEventArgs e)
        {
            if (ListBoxProcesses.SelectedItem != null)
            {
                ListBoxMonProcesses.Items.Add(ListBoxProcesses.SelectedItem);
                Functions.WriteWatchlist(ListBoxProcesses.SelectedItem.ToString()); // write to db

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

                File.Delete(Functions.watchlistPath);
                File.AppendAllLines(Functions.watchlistPath, NewMonItems.ToArray()); //make a new file with the items from the listbox
                //UpdateMonListBox(Functions.Readdb());
            }
            else
            {
                MessageBox.Show("No item selected");
            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //present a warning messagebox yes/no
            if (MessageBox.Show("Clear Monitor List?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                ListBoxMonProcesses.Items.Clear();
                File.Delete(Functions.watchlistPath);
            }

        }


        private void MonitorMode_Checked(object sender, RoutedEventArgs e)
        {

            //ManualSet = false;


            //monitormodestatus = (bool)MonitorMode.IsChecked;
            //startupstatus = (bool)Startup.IsChecked;
            try
            {
                File.Delete(Functions.SettingsPath);
            }
            catch (Exception)
            {

                Functions.CreateAppDir();
            }

            Functions.WriteSettings(Startup.IsChecked.Value.ToString());
            Functions.WriteSettings(MonitorMode.IsChecked.Value.ToString());
            Functions.WriteSettings(RateSlider.Value.ToString());





        }



        private void MonitorMode_Unchecked(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Disable Automatic Power Management?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {

                try
                {
                    File.Delete(Functions.SettingsPath);
                }
                catch (Exception)
                {

                    Functions.CreateAppDir();
                }

                Functions.WriteSettings(Startup.IsChecked.Value.ToString());
                Functions.WriteSettings(MonitorMode.IsChecked.Value.ToString());
                Functions.WriteSettings(RateSlider.Value.ToString());

            }
            
        }


        private void startup_Unchecked(object sender, RoutedEventArgs e)
        {
            Functions.AddApplicationToStartup(false); // writes to the registry
            try
            {
                File.Delete(Functions.SettingsPath);
            }
            catch (Exception)
            {

                Functions.CreateAppDir();
            }

            Functions.WriteSettings(Startup.IsChecked.Value.ToString());
            Functions.WriteSettings(MonitorMode.IsChecked.Value.ToString());
            Functions.WriteSettings(RateSlider.Value.ToString());

        }

        private void startup_Checked(object sender, RoutedEventArgs e)
        {
            Functions.AddApplicationToStartup(true);
            try
            {
                File.Delete(Functions.SettingsPath);
            }
            catch (Exception)
            {

                Functions.CreateAppDir();
            }

            Functions.WriteSettings(Startup.IsChecked.Value.ToString());
            Functions.WriteSettings(MonitorMode.IsChecked.Value.ToString());
            Functions.WriteSettings(RateSlider.Value.ToString());
        }

        private void RateSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            try
            {
                File.Delete(Functions.SettingsPath);
            }
            catch (Exception)
            {

                Functions.CreateAppDir();
            }

            Functions.WriteSettings(Startup.IsChecked.Value.ToString());
            Functions.WriteSettings(MonitorMode.IsChecked.Value.ToString());
            Functions.WriteSettings(RateSlider.Value.ToString());

        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            notifyIcon.Dispose();
        }

        private void about_button_Click(object sender, RoutedEventArgs e)
        {
            
           
            About about = new About();
            about.ShowDialog();

        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
