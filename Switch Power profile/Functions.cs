using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32; //using System.Management;
//using System.Text;
//using System.Threading.Tasks;
using Forms = System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using PowerLineStatus = System.Windows.PowerLineStatus;

namespace Switch_Power_profile
{
    class Functions
    {
        public static string UsernamePath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}";
        public static string DirPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\AppData\Local\Automatic Power Manager";
        public static string WatchlistPath = $@"{DirPath}\watchlist.cfg";
        public static string SettingsPath = $@"{DirPath}\settings.cfg";
        public static string ErrorLogPath = $@"{DirPath}\error.log";
        public static string ActivationPath = $@"{DirPath}\activation";


        

        public static void WriteErrorToLog(string er)
        {
            try
            {
                //Open the File
                var sw = new StreamWriter(ErrorLogPath, true); // creates file if it doesn't exist

                sw.WriteLine(er);
                //close the file
                sw.Close();
            }
            catch (Exception e)
            {
                var sw = new StreamWriter(ErrorLogPath, true); // creates file if it doesn't exist

                sw.WriteLine(e);
                //close the file
                sw.Close();
            }
        }


        public static Boolean IsActivated()
        {
            var keyFromFile = "";
            try
            {
                keyFromFile = ReadActivationFile();
            }
            catch (Exception e)
            {

                WriteErrorToLog(e.ToString());
            }
            if ((keyFromFile != "") &
                    CheckKey(keyFromFile))
            {
                return true;

            }
            else
            {
                return false;
            }
        }

        public static Boolean CheckKey(string key)
        {
            if (key.Length == 10)
            {
                DateTime CurrentDate = DateTime.Now;
                DateTime installDate = Convert.ToDateTime(key);
                DateTime installDatePlusOne = installDate.AddDays(1);
                return true;

                // Need to do a return here // compare dates return true or false
            }
            else if (key.Length == 38)
            {
                var reg = new Regex(ActivationScreen.RegFormat);
                var result = reg.IsMatch(key);
                //List<string> serialInput = new List<string>();
                var serialInput = key;

                if (result)
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
                        serialInput.Substring(20) == UsernameToAscii())
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
                var sw = new StreamWriter(ActivationPath); // creates file if it dosen't exist

                sw.WriteLine(activate);
                sw.WriteLine("-Please don't edit - will lose activation");
                //close the file
                sw.Close();
            }
            catch (Exception e)
            {
                var sw = new StreamWriter(ErrorLogPath, true); // creates file if it dosen't exist

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
                var sr = new StreamReader(ActivationPath);
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
                Directory.CreateDirectory(DirPath);

                // Delete the directory.
                //dir.Delete();
            }
            catch (Exception e)
            {
                WriteErrorToLog(e.ToString());
            }
        }

        public static void AddApplicationToStartup(bool startup)
            {
                var appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

                using (var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    switch (startup)
                    {
                        case true:
                            if (key != null)
                            {
                                key.SetValue("KTAD - APM", appPath);
                                key.Close();
                            }

                            break;
                        case false:
                            if (key != null) key.DeleteValue("KTAD - APM", false);
                            break;
                    }
                }

            }


        public static void CreateMissingSchemes(string schemeName)
        {
            switch (schemeName)
            {
                case "MakePower":
                {
                    var ps = new ProcessStartInfo();
                    ps.CreateNoWindow = true;
                    ps.UseShellExecute = false;
                    ps.FileName = "cmd.exe";
                    ps.Arguments = @"/c powercfg -duplicatescheme a1841308-3541-4fab-bc81-f71556f20b4a";
                    ps.RedirectStandardOutput = true;
                    Process.Start(ps);
                    break;
                }
                case "MakeBalanced":
                {
                    var ps = new ProcessStartInfo();
                    ps.CreateNoWindow = true;
                    ps.UseShellExecute = false;
                    ps.FileName = "cmd.exe";
                    ps.Arguments = @"/c powercfg /duplicatescheme 381b4222-f694-41f0-9685-ff5bb260df2e";
                    ps.RedirectStandardOutput = true;
                    Process.Start(ps);
                    break;
                }
                case "MakeHigh":
                {
                    var ps = new ProcessStartInfo();
                    ps.CreateNoWindow = true;
                    ps.UseShellExecute = false;
                    ps.FileName = "cmd.exe";
                    ps.Arguments = @"/c powercfg -duplicatescheme 8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c";
                    ps.RedirectStandardOutput = true;
                    Process.Start(ps);
                    break;
                }
            }
        }


        public static void WriteWatchlist(string prog)
        {

            //https://docs.microsoft.com/en-us/troubleshoot/dotnet/csharp/read-write-text-file
            //int x;
            try
            {
                //Open the File
                var sw = new StreamWriter(WatchlistPath, true); // creates file if it dosen't exist

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
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                var sr = new StreamReader(WatchlistPath);
                //Read the first line of text
                var line = sr.ReadLine();
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
            catch (Exception)
            {
                MessageBox.Show("Please add programs to be monitored\nSystem will automatically switch to High Performance when program runs\n");
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
                var sw = new StreamWriter(SettingsPath, true); // creates file if it dosen't exist

                sw.WriteLine(setting);
                //close the file
                sw.Close();
            }
            catch (Exception)
            {
                //Console.WriteLine("Exception: " + e.Message);
            }
            

        }


        public static List<string> ReadSettings()
        {
            var lines = new List<string>();
            string line;
            try
            {
                var sr = new StreamReader(SettingsPath);
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
            var ps = new ProcessStartInfo();
            ps.CreateNoWindow = true;
            ps.UseShellExecute = false;
            ps.FileName = "cmd.exe";
            ps.Arguments = @"/c powercfg -getactivescheme";
            ps.RedirectStandardOutput = true;
            var proc = Process.Start(ps);

            if (proc != null)
            {
                var s = proc.StandardOutput.ReadToEnd();

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




            var status = (PowerLineStatus)Forms.SystemInformation.PowerStatus.PowerLineStatus;
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

        public static string UsernameToAscii()
        {
            var usernameAscii = "";
            string choppedUsername;
            var tempLs = UsernamePath.Split(Path.DirectorySeparatorChar);
            if (tempLs[2].Length > 6)
            {
                choppedUsername = tempLs[2].Substring(0, 6);
            }
            else
            {
                choppedUsername = tempLs[2];
            }

            var asciiChar = Encoding.ASCII.GetBytes(choppedUsername);
            foreach (var item in asciiChar)
            {
                usernameAscii += item;
            }
            var toDivide = Convert.ToInt64(usernameAscii);
            var result = toDivide / 2;
            var resultString = result.ToString();
            if (resultString.Length < 18)
            {
                var amount = 18 - resultString.Length;
                for (var i = 0; i < amount; i++)
                {
                    resultString = resultString + "0";
                }
            }
            return resultString;
        }




        public static Dictionary<string, string> GetAllPowerProfiles() // working on this - think it's all good now
        {
            var allProfiles = new Dictionary<string, string>();

            var ps = new ProcessStartInfo();
            ps.CreateNoWindow = true;
            ps.UseShellExecute = false;
            ps.FileName = "cmd.exe";
            ps.Arguments = $@"/c powercfg -list";
            ps.RedirectStandardOutput = true;
            var proc = Process.Start(ps);

            if (proc != null)
            {
                var s = proc.StandardOutput.ReadToEnd();
                var result = s.Split('\n');
           
                for (var i = 0; i < result.Length; i++)
                {
                    try
                    {
                        if (result[i].ToLower().Contains("power saver")
                            || result[i].ToLower().Replace(")", "").Contains("balanced")
                            || result[i].ToLower().Contains("high performance"))
                        {
                        
                        

                            var parts = result[i].Split();

                            if (!allProfiles.Keys.Contains(parts[5].Remove(0, 1).Replace(")", ""))) // if the current one to add does not exist then
                            {
                                allProfiles.Add($"{parts[5].Remove(0, 1).Replace(")", "")}", parts[3]); //did some clean upwith replcae and remove
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
            }

            if (!allProfiles.Keys.Contains("Power"))
            {
                CreateMissingSchemes("MakePower");

            }
            if (!allProfiles.Keys.Contains("Balanced"))
            {
                CreateMissingSchemes("MakeBalanced");

            }
            if (!allProfiles.Keys.Contains("High"))
            {
                CreateMissingSchemes("MakeHigh");

            }

            return allProfiles;
        }

        public static string MakeDemoKey()
        {
            string demoKey = "";
            var installDate = DateTime.Now;
            //Console.WriteLine(newDate2.ToString());

            var firstPartOfDate = installDate.ToString(CultureInfo.InvariantCulture).Split()[0];

            //Console.WriteLine($"{firstPartOfDate[0]} This is the proper now {firstPartOfDate[0].Length}");

            //var newDate22 = newDate2.AddDays(22);
            //var t = newDate22.ToString().Split();

            //Console.WriteLine($"{t[0]} This is plus 1 day");

            //var newDate3 = new DateTime(2021, 05, 30);

            ////Thread.Sleep(2000);

            //var newDate = DateTime.Now;

            //var timePassed = newDate3.Subtract(newDate);

            //Console.WriteLine(timePassed);


            //byte[] dateAsAscii = Encoding.ASCII.GetBytes(firstPartOfDate);

            //foreach (var item in dateAsAscii)
            //{
            //    demoKey += item;
            //}
            
            return firstPartOfDate;
        }
    }
}