using System;
using System.Collections;

namespace RaplaICal.Models
{
    public class Event
    {
        public string Title { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public string Lecturer { get; set; }
        public string Location { get; set; }
    }
}