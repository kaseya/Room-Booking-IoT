using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RB_IoT_App;

namespace RB_IoT_UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Basic_EventInTimeScope()
        {
            DateTime dtNow = DateTime.Now;
            DateTime dtStart = dtNow.AddMinutes(-5);
            DateTime dtEnd = dtNow.AddMinutes(+5);

            Assert.AreEqual(true, CalendarEventsManager.EventInTimeScope(dtNow, dtStart, dtEnd));
        }

        [TestMethod]
        public void Fail_EventInTimeScope_BadEnd()
        {
            DateTime dtNow = DateTime.Now;
            DateTime dtStart = dtNow.AddMinutes(-5);
            DateTime dtEnd = dtNow.AddMinutes(-1);

            Assert.AreEqual(false, CalendarEventsManager.EventInTimeScope(dtNow, dtStart, dtEnd));
        }

        [TestMethod]
        public void Fail_EventInTimeScope_BadStart()
        {
            DateTime dtNow = DateTime.Now;
            DateTime dtStart = dtNow.AddMinutes(+1);
            DateTime dtEnd = dtNow.AddMinutes(+2);

            Assert.AreEqual(false, CalendarEventsManager.EventInTimeScope(dtNow, dtStart, dtEnd));
        }

    }
}
