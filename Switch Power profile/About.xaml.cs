using System.Deployment.Application;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace Switch_Power_profile
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About
    {
        public About()
        {
            InitializeComponent();

            try
            {
                string registered;
                if (Functions.IsActivated())
                {
                    registered = "App registered";
                }
                else
                {
                    registered = "App not registered";
                }

                //// get deployment version
                var version = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
                VersionLbl.Content = "Version " + version + " " + registered;
            }
            catch (InvalidDeploymentException)
            {
                //// you cannot read publish version when app isn't installed 
                //// (e.g. during debug)
                VersionLbl.Content = "not installed or debug mode";
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
