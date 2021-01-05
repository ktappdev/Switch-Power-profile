using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutomaticPowerManager
{
    class Functions
    {
        public static string path = ".\\Test1";



        public static string Writedb(string prog)
        {

            //https://docs.microsoft.com/en-us/troubleshoot/dotnet/csharp/read-write-text-file
            //int x;
            try
            {
                //Open the File
                StreamWriter sw = new StreamWriter(path, true); // creates file if it dosen't exist

                sw.WriteLine(prog);
                //close the file
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }
            return "";
        }


        public static List<string> Readdb()
        {
            var lines = new List<string>();
            string line;
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader(path);
                //Read the first line of text
                line = sr.ReadLine();
                //Continue to read until you reach end of file
                while (line != null)
                {
                    //add lines to list
                    lines.Add(line);
                    //Read the next line
                    line = sr.ReadLine();
                }
                //close the file
                sr.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("Please add programs to be monitored\nSystem will automatically switch to High Performance when program runs\n" +
                    "Please set programs that wont always be running");
            }
            finally
            {
                // Console.WriteLine("Executing finally block.");
            }



            return lines;
        }


        public static int GetCurrentPowerProfile()
        {
            ProcessStartInfo ps = new ProcessStartInfo();
            ps.CreateNoWindow = true;
            ps.UseShellExecute = false;
            ps.FileName = "cmd.exe";
            ps.Arguments = @"/c powercfg -getactivescheme";
            ps.RedirectStandardOutput = true;
            var proc = Process.Start(ps);

            string s = proc.StandardOutput.ReadToEnd();

            if (s.Contains(MainWindow.LowGUID))
            {
                return 1;
            }
            else if (s.Contains(MainWindow.BalancedGUID))
            {
                return 2;
            }
            else
            {
                return 3;
            }

        }




        public static Boolean GetChargingStatus() //Work in progress, not really implimented
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


        public static Dictionary<string, string> GetAllPowerProfiles() // working on this 
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
                    // add something here
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
    }
}