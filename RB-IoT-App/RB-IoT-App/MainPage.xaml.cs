using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
        private readonly SynchronizationContext synchronizationContext;
        private bool showColon = true;

        Settings settings = new Settings();
        CalendarEventsManager meetingsManager = new CalendarEventsManager();
        
        public MainPage()
        {
            this.InitializeComponent();

            synchronizationContext = SynchronizationContext.Current;

            meetingsManager.GetMeetings("meetings.json");

            SetBackgroundImage(settings.BackgroundImage);
            SetCompanyLogo(settings.CompanyLogo);
                  
            this.Label_RoomName.Text = settings.RoomName;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += RefreshClock;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();

            DispatcherTimer meetingRefresher = new DispatcherTimer();
            meetingRefresher.Tick += RefreshMeetingInfo;
            meetingRefresher.Interval = new TimeSpan(0, 1, 0);
            meetingRefresher.Start();

            LoadCurrentMeetingInfo();

        }

        private void RefreshClock(object sender, object e)
        {
            animateTick(settings.TimeFormat, showColon);
        }

        private void RefreshMeetingInfo(object sender, object e)
        {
            LoadCurrentMeetingInfo();
        }

        private void LoadCurrentMeetingInfo()
        {

            CalendarEvent meetingInfo = meetingsManager.GetMeetingInfo();

            synchronizationContext.Post(new SendOrPostCallback(o =>
            {
                Label_RoomUsage.Text = GetMeetingDescription((CalendarEvent)o);
                Label_RoomHost.Text = GetMeetingHost((CalendarEvent)o);
                ShowRoomAvailability((CalendarEvent)o);

            }), meetingInfo);
 
        }

        private void ShowRoomAvailability(CalendarEvent meetingInfo)
        {
            if( meetingInfo != null )
            {
                Label_RoomAvailability.Text = "Busy";
                Label_RoomAvailabilityTimeBlock.Text = "Until " + meetingInfo.End.ToLocalTime().ToString(settings.TimeFormat);
                BackgroundCanvas.Background = new SolidColorBrush(
                                                Windows.UI.Color.FromArgb(255, 154, 5, 24));

                BuildButtonPanel(false);
            }
            else
            {
                Label_RoomAvailability.Text = "Available";
                Label_RoomAvailabilityTimeBlock.Text = "";
                BackgroundCanvas.Background = new SolidColorBrush(
                                                Windows.UI.Color.FromArgb(255, 17, 137, 46));

                BuildButtonPanel(true);
            }
        }

        // TODO: For some reason this isn't animating to blink the colon. It may be the redraw doesn't refresh fast enough
        private void animateTick(string timeFormat, bool showColon )
        {
            if( !showColon )
            {
                // TODO: This hack didn't work. Commenting out until we have a better idea of blinking the ':'
                //timeFormat = timeFormat.Replace(':', ' ');    
            }

            DateTime dt = DateTime.Now;

            Label_CurrentTime.Text = dt.ToString(timeFormat);
            
            this.showColon = !showColon;
        }

        private void SetCompanyLogo( string logoLocation )
        {
            if( String.IsNullOrEmpty(logoLocation) )
            {
                // This should never happen
                throw new ArgumentNullException("The company logo cannot be empty");
            }

            try
            {
                BitmapImage image = new BitmapImage(new Uri(this.BaseUri, logoLocation));
                
                this.CompanyLogo.Source = image;
            }
            catch( Exception e )
            {
                // TODO: Add generic logging
            }
        }

        private void SetBackgroundImage(string imgLocation)
        {
            if (String.IsNullOrEmpty(imgLocation))
            {
                // This should never happen
                throw new ArgumentNullException("The background image cannot be empty");
            }

            try
            {
                BitmapImage image = new BitmapImage(new Uri(this.BaseUri, imgLocation));

                this.ScreenBackground.Source = image;
            }
            catch (Exception e)
            {
                // TODO: Add generic logging
            }
        }

        private async void Button_StartMeeting_Click(object sender, RoutedEventArgs e)
        {
            string[] timeBlocks = GetTimeBlocks(DateTime.Now);
            
            var messageDialog = new MessageDialog("How long would you like to book this room?");
            
            messageDialog.Commands.Add(new UICommand(timeBlocks[0],new UICommandInvokedHandler(this.CommandBookOneBlock)));
            messageDialog.Commands.Add(new UICommand(timeBlocks[1],new UICommandInvokedHandler(this.CommandBookTwoBlocks)));
            messageDialog.Commands.Add(new UICommand(timeBlocks[2], new UICommandInvokedHandler(this.CommandBookThreeBlocks)));

            // Show the message dialog
            await messageDialog.ShowAsync();
        }

        private void CommandBookOneBlock(IUICommand command)
        {
            BookMeeting(1);
        }

        private void CommandBookTwoBlocks(IUICommand command)
        {
            BookMeeting(2);
        }

        private void CommandBookThreeBlocks(IUICommand command)
        {
            BookMeeting(3);
        }

        private void CommandExtendOneBlock(IUICommand command)
        {
            ExtendMeeting(1);
        }

        private void CommandExtendTwoBlocks(IUICommand command)
        {
            ExtendMeeting(2);
        }

        private void CommandExtendThreeBlocks(IUICommand command)
        {
            ExtendMeeting(3);
        }

        private string [] GetTimeBlocks(DateTime dt)
        {
            string[] timeBlocks = { "", "", "" };

            DateTime nextBlock = dt;

            if( dt.Minute < 30 )
            {
                nextBlock = nextBlock.AddMinutes(30-dt.Minute);
            }
            else
            {
                nextBlock = nextBlock.AddMinutes(60-dt.Minute);
            }

            timeBlocks[0] = "Until " + nextBlock.ToString(settings.TimeFormat);

            nextBlock = nextBlock.AddMinutes(30);
            timeBlocks[1] = "Until " + nextBlock.ToString(settings.TimeFormat);

            nextBlock = nextBlock.AddMinutes(30);
            timeBlocks[2] = "Until " + nextBlock.ToString(settings.TimeFormat);


            return timeBlocks;
        }

        private string GetMeetingDescription(CalendarEvent evt)
        {
            string desc = String.Empty;

            if( evt != null )
            {
                if( !String.IsNullOrEmpty(evt.Description) )
                {
                    desc = evt.Description;
                }
                else
                {
                    desc = "Private meeting";
                }
            }
            else
            {
                desc = "No meeting booked";
            }

            return desc;
        }

        private string GetMeetingHost(CalendarEvent evt)
        {
            string desc = String.Empty;
            var data = String.Empty;

            if (evt != null)
            {
                if (!String.IsNullOrEmpty(evt.OrganizerName))
                {
                    data = "Hosted by " + evt.OrganizerName;
                }
                else
                {
                    data = "";
                }

            }
            else
            {
                data = "";
            }

            return data;
        }

        private void BuildButtonPanel(bool roomAvailable)
        {
            if( roomAvailable )
            {
                Button_StartMeeting.Visibility = Visibility.Visible;
                Button_EndMeeting.Visibility = Visibility.Collapsed;
                Button_ExtendMeeting.Visibility = Visibility.Collapsed;
            }
            else
            {
                Button_StartMeeting.Visibility = Visibility.Collapsed;
                Button_EndMeeting.Visibility = Visibility.Visible;
                Button_ExtendMeeting.Visibility = Visibility.Visible;
            }
        }

        private void Button_EndMeeting_Click(object sender, RoutedEventArgs e)
        {
            meetingsManager.EndMeeting();
            LoadCurrentMeetingInfo();
        }

        private async void Button_ExtendMeeting_Click(object sender, RoutedEventArgs e)
        {
            CalendarEvent meetingInfo = meetingsManager.GetMeetingInfo();

            string[] timeBlocks = GetTimeBlocks(meetingInfo.End.ToLocalTime());

            var messageDialog = new MessageDialog("How long would you like to extend this room booking?");

            messageDialog.Commands.Add(new UICommand(timeBlocks[0], new UICommandInvokedHandler(this.CommandExtendOneBlock)));
            messageDialog.Commands.Add(new UICommand(timeBlocks[1], new UICommandInvokedHandler(this.CommandExtendTwoBlocks)));
            messageDialog.Commands.Add(new UICommand(timeBlocks[2], new UICommandInvokedHandler(this.CommandExtendThreeBlocks)));

            // Show the message dialog
            await messageDialog.ShowAsync();
        }

        private void BookMeeting(int blocks)
        {
            meetingsManager.BookMeeting(blocks);
            LoadCurrentMeetingInfo();
        }

        private void ExtendMeeting(int blocks)
        {
            meetingsManager.ExtendMeeting(blocks);
            LoadCurrentMeetingInfo();
        }
    }
}
