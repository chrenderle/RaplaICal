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
            var events = RaplaParser.GetAllEvents(begin, end);
            var calendar = CalCreator.Create(events);
            calendar = calendar.Replace("BEGIN:VCALENDAR", "BEGIN:VCALENDAR\nX-WR-TIMEZONE:Europe/Berlin");
            var contentType = "text/calendar";
            var bytes = Encoding.UTF8.GetBytes(calendar);
            var result = new FileContentResult(bytes, contentType);
            result.FileDownloadName = "events.ics";
            return result;
        }
    }
}