using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace RB_IoT_App
{
    public class CalendarEventsManager
    {
        private List<CalendarEvent> _meetings = new List<CalendarEvent>();
        private StorageFolder _localFolder = ApplicationData.Current.LocalFolder;

        private string defaultMeetingsFile =
            "[" +
                "{\"Description\": \"Sample Meeting\",\"Start\": \"2016-12-18T17:00:00\",\"End\": \"2016-12-18T17:30:00\"," +
                    "\"OrganizerName\": \"Alice Smith\",\"OrganizerEmail\": \"alice.smith@domain.com\"}, " +
                "{\"Description\": \"Sample Meeting 2\",\"Start\": \"2016-12-19T17:00:00\",\"End\": \"2016-12-19T18:30:00\"," +
                    "\"OrganizerName\": \"Bob Jones\",\"OrganizerEmail\": \"bob.jones@domain.com\"} " +
            "]";

        public CalendarEvent GetMeetingInfo()
        {
            CalendarEvent meetingInfo = null;

            return meetingInfo;
        }

        public CalendarEvent GetNextMeeting()
        {
            CalendarEvent nextMeeting = null;

            return nextMeeting;
        }

        public async void GetMeetings(string meetingCatalog )
        {
            List<CalendarEvent> _events = new List<CalendarEvent>();

            try
            {
                StorageFile meetingsFile = await _localFolder.GetFileAsync(meetingCatalog);
                String jsonData = await FileIO.ReadTextAsync(meetingsFile);

                _events = JsonConvert.DeserializeObject<List<CalendarEvent>>(jsonData).ToList();
            }
            catch (Exception e)
            {
                // TODO: Write to generic log
                StorageFile meetingsFile = await _localFolder.CreateFileAsync(meetingCatalog);
                await FileIO.WriteTextAsync(meetingsFile, defaultMeetingsFile);

                _events = JsonConvert.DeserializeObject<IEnumerable<CalendarEvent>>(defaultMeetingsFile).ToList();
            }

            if( _events != null && _events.Count > 0 )
            {
                DateTime dt = DateTime.Now.ToUniversalTime();

                // Discard old meetings
                _meetings = _events.Where(x => x.Start >= dt).ToList();

                // Sort the meetings to get the right order
                _meetings.OrderBy(x => x.Start).ToList();
                
            }
        }
    }
}
