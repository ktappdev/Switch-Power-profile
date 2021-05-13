
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using AutomaticPowerManager;

namespace Switch_Power_profile
{
    /// <summary>
    /// Interaction logic for ActivationScreen.xaml
    /// </summary>
    public partial class ActivationScreen : Window
    {
        public const string regFormat = "^[0-9]{4}-[0-9]{4}-[0-9]{4}-[0-9]{4}-[0-9]{18}$";
        public ActivationScreen()
        {
            InitializeComponent();
            validLabel.Visibility = Visibility.Collapsed;
            challangeCodeBox.Text = Functions.usernameToAscii();
        }

        private void demoBtn_Click(object sender, RoutedEventArgs e)
        {
            Functions.WriteActivation("5041-3025-1414-7201-" + Functions.usernameToAscii());
            //MainWindow mw = new MainWindow();
            //mw.InitializeComponent();
            //mw.GetSettingsAndUpdate(Functions.ReadSettings());
            //Functions.GetAllPowerProfiles();
            //mw.ReadAndUpdateUi();
            //mw.GetProcessesList();
            //mw.UpdateMonListBox(Functions.ReadWatchlist());
            //Functions.AddApplicationToStartup(Convert.ToBoolean(Functions.ReadSettings()[0]));
            //mw.MonitorPrograms();

            //this.Close();
            Restart();

        }


        private void Restart()
        {
            System.Windows.Forms.Application.Restart();
            Application.Current.Shutdown();
        }


        private void validateBtn_Click_1(object sender, RoutedEventArgs e)
        {

            Regex reg = new Regex(regFormat);
            bool result = reg.IsMatch(serialInputBox.Text);
            //List<string> serialInput = new List<string>();
            string serialInput = serialInputBox.Text;

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
                    validLabel.Visibility = Visibility.Visible;
                    validLabel.Content = "Valid Serial";
                    

                    Functions.WriteActivation(serialInput);
                    this.Close();

                }
                else
                {
                    validLabel.Visibility = Visibility.Visible;
                    validLabel.Content = "Wrong Serial";
                    Functions.WriteErrorToLog("Wrong serial");
                }

            }
            else
            {
                validLabel.Visibility = Visibility.Visible;
                //validLabel.Visibility = Visibility.Collapsed;
                validLabel.Content = "Wrong Serial";
                Functions.WriteErrorToLog("Wrong serial");
            }
        }
        private void TextChangedEventHandler(object sender, TextChangedEventArgs e) //this was set from the xaml file
        {
            validLabel.Visibility = Visibility.Collapsed;
        }

        private void copyBtn_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(challangeCodeBox.Text);
        }
    }
}
