using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace RB_IoT_App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Settings settings = new Settings();

        public MainPage()
        {
            this.InitializeComponent();

            SetCompanyLogo(settings.CompanyLogo);
                  
            this.Label_RoomName.Text = settings.RoomName;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += RefreshClock;
            timer.Start();
        }

        private void RefreshClock(object sender, object e)
        {
            DateTime dt = DateTime.Now;

            Label_CurrentTime.Text = dt.ToString("HH:mm tt");
        }

        private void SetCompanyLogo( string logoLocation )
        {
            if( String.IsNullOrEmpty(logoLocation) )
            {
                // This should never happen
                throw new ArgumentNullException("The company logo cannot be empty");
            }

            string imgPath = "ms-appx://";

            if( logoLocation.StartsWith("/"))
            {
                imgPath += logoLocation.Substring(1);
            }
            else
            {
                imgPath += logoLocation;
            }

            try
            {
                //BitmapImage image = new BitmapImage(new Uri( imgPath, UriKind.Absolute));
                BitmapImage image = new BitmapImage(new Uri(this.BaseUri, logoLocation));
                
                this.CompanyLogo.Source = image;
            }
            catch( Exception e )
            {
                // TODO: Add generic logging
            }
        }
    }
}
