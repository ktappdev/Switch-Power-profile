using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
//using System.Management;
//using System.Text;
//using System.Threading.Tasks;
using System.Windows;
using Forms = System.Windows.Forms;
using Switch_Power_profile;
using System.Text.RegularExpressions;

namespace AutomaticPowerManager
{
    class Functions
    {
        public static string usernamePath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}";
        public static string DirPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\AppData\Local\Automatic Power Manager";
        public static string watchlistPath = $@"{DirPath}\watchlist.cfg";
        public static string SettingsPath = $@"{DirPath}\settings.cfg";
        public static string errorLogPath = $@"{DirPath}\error.log";
        public static string activationPath = $@"{DirPath}\activation";


        

        public static void WriteErrorToLog(string er)
        {
            try
            {
                //Open the File
                StreamWriter sw = new StreamWriter(errorLogPath, true); // creates file if it dosen't exist

                sw.WriteLine(er);
                //close the file
                sw.Close();
            }
            catch (Exception e)
            {
                StreamWriter sw = new StreamWriter(errorLogPath, true); // creates file if it dosen't exist

                sw.WriteLine(e);
                //close the file
                sw.Close();
            }
        }


        public static Boolean isActivated()
        {
            string keyFromFile = "";
            try
            {
                keyFromFile = ReadActivationFile();
            }
            catch (Exception e)
            {

                WriteErrorToLog(e.ToString());
            }
            if ((keyFromFile != "") &
                    (CheckKey(keyFromFile) == true))
            {
                return true;

            }
            else
            {
                return false;
            }
        }

        private static Boolean CheckKey(string key)
        {
            Regex reg = new Regex(ActivationScreen.regFormat);
            bool result = reg.IsMatch(key);
            //List<string> serialInput = new List<string>();
            string serialInput = key;

            if (result == true)
            {
                if (int.Parse(serialInput[0].ToString()) +
                    int.Parse(serialInput[1].ToString()) +
                    int.Parse(serialInput[2].ToString()) +
                    int.Parse(serialInput[3].ToString()) == 10 &
                        int.Parse(serialInput[5].ToString()) +
                        int.Parse(serialInput[6].ToString()) +
                        int.Parse(serialInput[7].ToString()) +
                        int.Parse(serialInput[8].ToString()) == 10 &
                            int.Parse(serialInput[10].ToString()) +
                            int.Parse(serialInput[11].ToString()) +
                            int.Parse(serialInput[12].ToString()) +
                            int.Parse(serialInput[13].ToString()) == 10 &
                                int.Parse(serialInput[15].ToString()) +
                                int.Parse(serialInput[16].ToString()) +
                                int.Parse(serialInput[17].ToString()) +
                                int.Parse(serialInput[18].ToString()) == 10 &
                                    serialInput.Substring(20) == Functions.usernameToAscii())
                {

                    return true;
                }
                else
                {

                    return false;
                }

            }
            else
            {

                return false;
            }
        }
    

        public static void WriteActivation(string activate)
        {
            try
            {
                //Open the File
                StreamWriter sw = new StreamWriter(activationPath); // creates file if it dosen't exist

                sw.WriteLine(activate);
                sw.WriteLine("-Please don't edit - will lose activation");
                //close the file
                sw.Close();
            }
            catch (Exception e)
            {
                StreamWriter sw = new StreamWriter(errorLogPath, true); // creates file if it dosen't exist

                sw.WriteLine(e);
                //close the file
                sw.Close();
            }
        }

        public static string ReadActivationFile()
        {
            var lines = new List<string>();
            string line;
            try
            {
                StreamReader sr = new StreamReader(activationPath);
                line = sr.ReadLine();
                while (line != null)
                {
                    lines.Add(line);
                    line = sr.ReadLine();
                }
                sr.Close();
            }
            catch (Exception e)
            {
                WriteErrorToLog(e.ToString());
            }

            return lines[0];
        }



        public static void CreateAppDir()
        {
            try
            {
                //directory exists or nah.
                if (Directory.Exists(DirPath))
                {
                    
                    return;
                }

                //create the directory.
                DirectoryInfo dir = Directory.CreateDirectory(DirPath);

                // Delete the directory.
                //dir.Delete();

            }
            catch (Exception e)
            {
                Functions.WriteErrorToLog(e.ToString());
            }
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
                WriteErrorToLog(e.ToString());
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
                MessageBox.Show("Please add programs to be monitored\nSystem will automatically switch to High Performance when program runs\n");
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
                StreamReader sr = new StreamReader(SettingsPath);
                line = sr.ReadLine();
                while (line != null)
                {
                    lines.Add(line);
                    line = sr.ReadLine();
                }

                sr.Close();
            }
            catch (Exception)
            {
                
                lines.Add("True");
                lines.Add("True");
                lines.Add("10");
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
            //SelectQuery Sq = new SelectQuery("Win32_Battery");
            //ManagementObjectSearcher objOSDetails = new ManagementObjectSearcher(Sq);
            //ManagementObjectCollection osDetailsCollection = objOSDetails.Get();
            //StringBuilder sb = new StringBuilder();

            //Boolean Charging;

            ////ListBoxProcesses.Items.Add(osDetailsCollection);

            //foreach (ManagementObject mo in osDetailsCollection)
            //{
            //    if ((ushort)mo["BatteryStatus"] == 2)
            //    {
            //        return Charging = true;
            //    }
            //    else
            //    {
            //        //SetLowProfile();
            //        return Charging = false;
            //    }

            //}
            //return Charging;




            PowerLineStatus status = (PowerLineStatus)Forms.SystemInformation.PowerStatus.PowerLineStatus;
            if (status == PowerLineStatus.Offline)
            {
                //MessageBox.Show("Running on Battery");
                return false;
            }

            else
            {
                //MessageBox.Show("Running on Power");
                return true;
            }

        }

        public static string usernameToAscii()
        {
            string usernameAscii = "";
            string choppedUsername = "";
            var tempLs = usernamePath.Split(Path.DirectorySeparatorChar);
            if (tempLs[2].Length > 6)
            {
                choppedUsername = tempLs[2].Substring(0, 6);
            }
            else
            {
                choppedUsername = tempLs[2];
            }

            byte[] asciiChar = Encoding.ASCII.GetBytes(choppedUsername);
            foreach (var item in asciiChar)
            {
                usernameAscii += item;
            }
            long _toDivide = Convert.ToInt64(usernameAscii);
            long result = _toDivide / 2;
            string resultString = result.ToString();
            if (resultString.Length < 18)
            {
                int amount = 18 - resultString.Length;
                for (int i = 0; i < amount; i++)
                {
                    resultString = resultString + "0";
                }
            }
            return resultString;
        }




        public static Dictionary<string, string> GetAllPowerProfiles() // working on this - think it's all good now
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
                    // MessageBox.Show(e.ToString()); this was crashing the prhogram maybe 
                    // throw;
                    WriteErrorToLog(e.ToString());

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