using Microsoft.Win32;
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
        public static string DirPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\AppData\Local\Automatic Power Manager";
        public static string watchlistPath = $@"{DirPath}\watchlist.cfg";
        public static string SettingsPath = $@"{DirPath}\settings.cfg";

        public static void CreateAppDir()
        {
            try
            {
                //directory exists or nah.
                if (Directory.Exists(DirPath))
                {
                    Console.WriteLine("That path exists already.");
                    return;
                }

                //create the directory.
                DirectoryInfo dir = Directory.CreateDirectory(DirPath);
                Console.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(DirPath));

                // Delete the directory.
                //dir.Delete();
                //Console.WriteLine("The directory was deleted successfully.");
            }
            catch (Exception e)
            {
            }
            finally { }
        }

        public static void AddApplicationToStartup(Boolean startup)
            {
                string AppPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                    {
                        if(startup == true)
                        {
                            key.SetValue("KTAD - APM", AppPath);
                            key.Close();
                        }
                        if(startup == false)
                        {
                    key.DeleteValue("KTAD - APM", false);
                        }
                        

                    }

            }


        public static void CreateMissingSchemes(string schemeName)
        {
            if(schemeName == "MakePower")
            {
                ProcessStartInfo ps = new ProcessStartInfo();
                ps.CreateNoWindow = true;
                ps.UseShellExecute = false;
                ps.FileName = "cmd.exe";
                ps.Arguments = @"/c powercfg -duplicatescheme a1841308-3541-4fab-bc81-f71556f20b4a";
                ps.RedirectStandardOutput = true;
                var proc = Process.Start(ps);   
            }
            if(schemeName == "MakeBalanced")
            {
                ProcessStartInfo ps = new ProcessStartInfo();
                ps.CreateNoWindow = true;
                ps.UseShellExecute = false;
                ps.FileName = "cmd.exe";
                ps.Arguments = @"/c powercfg /duplicatescheme 381b4222-f694-41f0-9685-ff5bb260df2e";
                ps.RedirectStandardOutput = true;
                var proc = Process.Start(ps);
            }
            if(schemeName == "MakeHigh")
            {
                ProcessStartInfo ps = new ProcessStartInfo();
                ps.CreateNoWindow = true;
                ps.UseShellExecute = false;
                ps.FileName = "cmd.exe";
                ps.Arguments = @"/c powercfg -duplicatescheme 8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c";
                ps.RedirectStandardOutput = true;
                var proc = Process.Start(ps);
            }
        }


        public static void WriteWatchlist(string prog)
        {

            //https://docs.microsoft.com/en-us/troubleshoot/dotnet/csharp/read-write-text-file
            //int x;
            try
            {
                //Open the File
                StreamWriter sw = new StreamWriter(watchlistPath, true); // creates file if it dosen't exist

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
                
            }
            
        }


        public static List<string> ReadWatchlist()
        {
            var lines = new List<string>();
            string line;
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader(watchlistPath);
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
              
                //lines.Add("Please add a program");
            }



            return lines;
        }





        public static void WriteSettings(string setting)
        {

            //https://docs.microsoft.com/en-us/troubleshoot/dotnet/csharp/read-write-text-file
            //int x;
            try
            {
                //Open the File
                StreamWriter sw = new StreamWriter(SettingsPath, true); // creates file if it dosen't exist

                sw.WriteLine(setting);
                //close the file
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {

            }

        }


        public static List<string> ReadSettings()
        {
            var lines = new List<string>();
            string line;
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader(SettingsPath);
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
                //if (line == null)
                //{
                //    lines.Add("True");
                //    lines.Add("True");
                //    lines.Add("10");
                //}
                
                //close the file
                sr.Close();
            }
            catch (Exception e)
            {
                lines.Add("True");
                lines.Add("True");
                lines.Add("10");
            }
            finally
            {
                
                
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

            if (s.Contains(GetAllPowerProfiles()["Power"]))
            {
                return 1;
            }
            else if (s.Contains(GetAllPowerProfiles()["Balanced"]))
            {
                return 2;
            }
            else if (s.Contains(GetAllPowerProfiles()["High"]))
            {
                return 3;
            }
            return 0;

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
                try
                {
                    if (result[i].ToLower().Contains("power saver")
                    || result[i].ToLower().Replace(")", "").Contains("balanced")
                    || result[i].ToLower().Contains("high performance"))
                    {
                        
                        

                        string[] parts = result[i].Split();

                        if (!AllProfiles.Keys.Contains(parts[5].Remove(0, 1).Replace(")", ""))) // if the current one to add does not exist then
                        {
                            AllProfiles.Add($"{parts[5].Remove(0, 1).Replace(")", "")}", parts[3]); //did some clean upwith replcae and remove
                        }
                    }
                }
                catch (Exception e)
                {
                    throw;
                    
                }


            }

            if (!AllProfiles.Keys.Contains("Power"))
            {
                CreateMissingSchemes("MakePower");

            }
            if (!AllProfiles.Keys.Contains("Balanced"))
            {
                CreateMissingSchemes("MakeBalanced");

            }
            if (!AllProfiles.Keys.Contains("High"))
            {
                CreateMissingSchemes("MakeHigh");

            }

            return AllProfiles;
        }
    }
}