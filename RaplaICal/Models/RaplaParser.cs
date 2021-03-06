using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Net;
using HtmlAgilityPack;

namespace RaplaICal.Models
{
    public static class RaplaParser
    {
        public static List<Event> GetAllEvents(DateTime begin, DateTime end, string key)
        {
            var events = new List<Event>();
            while (DateTime.Compare(begin, end) <= 0)
            {
                events.AddRange(GetWeekEvents(begin, key));
                begin = begin.AddDays(7);
            }
            return events;
        }
        public static List<Event> GetWeekEvents(DateTime requestDay, string key)
        {
            Console.WriteLine($"requested day: {requestDay.ToString()}");
            var monday = requestDay.AddDays((int) requestDay.DayOfWeek * -1 + 1);
            Console.WriteLine($"monday: {monday.ToString()}");
            List<Event> events = new List<Event>();
            /*load the html document*/
            string url = $"https://rapla.dhbw-stuttgart.de/rapla?key={key}&day={monday.Day}&month={monday.Month}&year={monday.Year}";
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader responseStreamReader = new StreamReader(responseStream);
            string responseString = responseStreamReader.ReadToEnd();
            var html = new HtmlDocument();
            html.LoadHtml(responseString);
            var trs = html.DocumentNode.SelectNodes(
                "/html/body/div[@id=\"calendar\"]/table/tbody/tr");
            /*//get the day separator cells
            var days = trs[0];
            int[] dayLength = new int[5];
            var childs = days.ChildNodes;
            int dayLengthCounter = 0;
            foreach (var child in childs)
            {
                if (child.HasClass("week_header"))
                {
                    dayLength[dayLengthCounter] = (Convert.ToInt16(child.Attributes["colspan"].Value) - 1) / 2;
                    dayLengthCounter++;
                }
            }*/

            foreach (var tr in trs)
            {
                var td = tr.FirstChild;
                if (td != null)
                {
                    /*loop through the tds*/
                    var day = 0;
                    //var dayCounter = dayLength;
                    while (td.NextSibling != null)
                    {
                        td = td.NextSibling;
                        if (td.HasClass("week_separatorcell") || td.HasClass("week_separatorcell_black"))
                        {
                            day++;
                        }
                        else if (td.HasClass("week_block"))
                        {
                            string person = "";
                            if (td.SelectSingleNode("span[@class=\"person\"]") != null)
                            {
                                person = td.SelectSingleNode("span[@class=\"person\"]").InnerText;
                            }

                            string location = "";
                            string title = "not found";
                            string times = td.SelectSingleNode("a").GetDirectInnerText();
                            times = times.Substring(0, 17);
                            DateTime beginTime = DateTime.Parse(times.Substring(0, 5));
                            DateTime endTime = DateTime.Parse(times.Substring(12));
                            DateTime begin = new DateTime(monday.AddDays(day).Year, monday.AddDays(day).Month, monday.AddDays(day).Day, beginTime.Hour, beginTime.Minute, 0);
                            DateTime end = new DateTime(monday.AddDays(day).Year, monday.AddDays(day).Month, monday.AddDays(day).Day,
                                endTime.Hour, endTime.Minute, 0);
                            var table = td.SelectSingleNode("a/span[@class=\"tooltip\"]/table[@class=\"infotable\"]");
                            var trs2 = table.SelectNodes("tr");
                            foreach (var tr2 in trs2)
                            {
                                var label = tr2.SelectSingleNode("td[@class=\"label\"]");
                                var value = tr2.SelectSingleNode("td[@class=\"value\"]");
                                switch (label.InnerText)
                                {
                                    case "Veranstaltungsname:":
                                        title = value.InnerText;
                                        break;
                                    
                                    case "Ressourcen:":
                                        location = value.InnerText;
                                        break;
                                }
                            }

                            events.Add(new Event
                            {
                                Title = title,
                                Begin = begin,
                                End = end,
                                Lecturer = person,
                                Location = location
                            });
                        }
                    }
                }
            }
            return events;
        }

        public static List<Event> GetDayEvents(DateTime requestDate, string key)
        {
            return GetWeekEvents(requestDate, key).Where(x => x.Begin.Date == requestDate.Date).ToList();
        }
    }
}