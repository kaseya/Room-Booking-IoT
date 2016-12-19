using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RB_IoT_App
{
    public class CalendarEvent
    {
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string OrganizerName { get; set; }
        public string OrganizerEmail { get; set; }
    }
}
