using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Switch_Power_profile
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            
            InitializeComponent();
            
            

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

                LabelHigh.Content = "Selected";
                string s = proc.StandardOutput.ReadToEnd();
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
            Process[] processCollection = Process.GetProcesses();
            foreach (Process p in processCollection)
            {
                if(p.ProcessName.ToLower() != "svchost")
                {
                    ListBoxProcesses.Items.Add(p.ProcessName);
                }
               
            }
        }
    }
}
