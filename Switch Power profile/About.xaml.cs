using AutomaticPowerManager;
using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();

            string version = null;

            try
            {
                string registered = "";
                if (Functions.isActivated())
                {
                    registered = "App registered";
                }
                else
                {
                    registered = "App not registered";
                }

                //// get deployment version
                version = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
                versionLbl.Content = "Version " + version + " " + registered;
            }
            catch (InvalidDeploymentException)
            {
                //// you cannot read publish version when app isn't installed 
                //// (e.g. during debug)
                versionLbl.Content = "not installed";
            }
        }


       

        private void closeAbout_Click(object sender, RoutedEventArgs e)
        {
            Close();
            
            
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)  //from https://stackoverflow.com/questions/10238694/example-using-hyperlink-in-wpf
        {
            // for .NET Core you need to add UseShellExecute = true
            // see https://docs.microsoft.com/dotnet/api/system.diagnostics.processstartinfo.useshellexecute#property-value
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }


    }
}
