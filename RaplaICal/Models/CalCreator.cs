using System;
using System.Collections.Generic;
using Ical.Net;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Ical.Net.Serialization.iCalendar.Serializers;

namespace RaplaICal.Models
{
    public class CalCreator
    {
        public static string Create(List<Event> events)
        {
            var calendar = new Ical.Net.Calendar();
            foreach (var event1 in events)
            {
                calendar.Events.Add(new Ical.Net.Event
                {
                    Class = "Public",
                    Summary = event1.Title,
                    Start = new CalDateTime(event1.Begin, "Europe/Berlin"),
                    End = new CalDateTime(event1.End, "Europe/Berlin"),
                    Location = event1.Location,
                    Description = event1.Lecturer
                });
            }
            var serializer = new CalendarSerializer(new SerializationContext());
            var serializedCalendar = serializer.SerializeToString(calendar);
            return serializedCalendar;
        }
    }
}