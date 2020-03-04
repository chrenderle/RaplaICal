using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using RaplaICal.Models;

namespace RaplaICal.Controller
{
    public class HomeController : Microsoft.AspNetCore.Mvc.Controller
    {
        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index(string key, int startYear, int startMonth, int startDay, int endYear, int endMonth,
            int endDay)
        {
            if ((key == null) || (startYear == 0) || (startMonth == 0) || (startDay == 0) || (endYear == 0) || (endMonth == 0) || (endDay == 0))
            {
                return BadRequest();
            }
            DateTime begin = new DateTime(startYear, startMonth, startDay);
            DateTime end = new DateTime(endYear, endMonth, endDay);
            return Index2(key, begin.ToString("yyyy-MM-dd"), end.ToString("yyyy-MM-dd"));
        }

        [HttpGet]
        //[ActionName("Index")]
        public IActionResult Index2(string key, string start, string end)
        {
            if ((key == null) || (start == null) || (end == null))
            {
                return BadRequest();
            }
            DateTime startDate = DateTime.Parse(start);
            DateTime endDate = DateTime.Parse(end);
            var events = RaplaParser.GetAllEvents(startDate, endDate, key);
            var calendar = CalCreator.Create(events);
            //time fix for
            calendar = calendar.Replace("BEGIN:VCALENDAR", "BEGIN:VCALENDAR\nX-WR-TIMEZONE:Europe/Berlin");
            var contentType = "text/calendar";
            var bytes = Encoding.UTF8.GetBytes(calendar);
            return new FileContentResult(bytes, contentType) {FileDownloadName = "events.ics"};
        }

        [HttpGet]
        public ViewResult Setup()
        {
            return View(new Configuration() {Start = DateTime.Today, End = DateTime.Today});
        }
    }
}