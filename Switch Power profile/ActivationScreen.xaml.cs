using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Switch_Power_profile
{
    /// <summary>
    /// Interaction logic for ActivationScreen.xaml
    /// </summary>
    public partial class ActivationScreen : Window
    {
        const string regFormat = "^[0-9]{4}-[0-9]{4}-[0-9]{4}-[0-9]{4}$";
        public ActivationScreen()
        {
            InitializeComponent();
            validLabel.Visibility = Visibility.Collapsed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
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
                                int.Parse(serialInput[18].ToString()) == 10)
                {
                    validLabel.Visibility = Visibility.Visible;
                    validLabel.Content = "Valid Serial";
                }
                else
                {
                    validLabel.Visibility = Visibility.Visible;
                    validLabel.Content = "Wrong Serial";
                }

            }
            else
            {
                validLabel.Visibility = Visibility.Visible;
                //validLabel.Visibility = Visibility.Collapsed;
                validLabel.Content = "Wrong Serial";
            }
        }
        private void TextChangedEventHandler(object sender, TextChangedEventArgs e) //this was set from the xaml file
        {
            validLabel.Visibility = Visibility.Collapsed;
        }
    }
}
