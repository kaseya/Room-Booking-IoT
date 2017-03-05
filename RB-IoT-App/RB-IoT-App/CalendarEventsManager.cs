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

            DateTime dt = DateTime.Now.ToUniversalTime();

            foreach ( CalendarEvent evt in _meetings )
            {
                if( EventInTimeScope( dt, evt.Start, evt.End ) )
                {
                    meetingInfo = evt;
                    break;
                }
            }

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
                _meetings = _events.Where(x => x.End >= dt).ToList();

                // Sort the meetings to get the right order
                _meetings.OrderBy(x => x.Start).ToList();
                
            }
        }

        public static bool EventInTimeScope(DateTime dtNow, DateTime startDateTime, DateTime endDateTime)
        {
            return startDateTime <= dtNow && dtNow < endDateTime;
        }

        public void EndMeeting()
        {

            DateTime dt = DateTime.Now.ToUniversalTime();

            for(int x=0; x<_meetings.Count; x++)
            {

                if (EventInTimeScope(dt, _meetings[x].Start, _meetings[x].End))
                {
                    _meetings[x].End = dt.AddMinutes(-1);
                    break;
                }
            }

            // TODO: Need to push this change back to meeting.json
        }

        public void BookMeeting(int blocks)
        {
            if( blocks < 1 || blocks > 3)
            {
                throw new ArgumentOutOfRangeException("You can only book between 1 and 3 blocks of 30 minute increments.");
            }

            DateTime startTime = DateTime.Now;
            DateTime endTime = startTime;

            if (startTime.Minute < 30)
            {
                endTime = endTime.AddMinutes(30 - startTime.Minute);
            }
            else
            {
                endTime = endTime.AddMinutes(60 - startTime.Minute);
            }

            endTime = endTime.AddMinutes(30 * (blocks-1));

            CalendarEvent evt = new CalendarEvent();
            evt.Description = "Impromptu meeting";
            evt.OrganizerName = "";
            evt.OrganizerEmail = "";
            evt.Start = startTime.ToUniversalTime();
            evt.End = endTime.ToUniversalTime();

            // TODO: We really need to check that we don't have a conflict when we add a new meeting
            _meetings.Add(evt);

        }

        public void ExtendMeeting(int blocks)
        {
            if (blocks < 1 || blocks > 3)
            {
                throw new ArgumentOutOfRangeException("You can only book between 1 and 3 blocks of 30 minute increments.");
            }

            CalendarEvent currEvent = GetMeetingInfo();

            DateTime startTime = DateTime.Now;
            DateTime endTime = currEvent.End;

            endTime = endTime.AddMinutes(30 * blocks);

            DateTime dt = DateTime.Now.ToUniversalTime();

            for (int x = 0; x < _meetings.Count; x++)
            {

                if (EventInTimeScope(dt, _meetings[x].Start, _meetings[x].End))
                {
                    _meetings[x].End = endTime.ToUniversalTime();
                    break;
                }
            }
        }
    }
}
