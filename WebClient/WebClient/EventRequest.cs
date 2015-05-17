using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClient
{
    public class EventRequest
    {
        public string Channel { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public static EventRequest Create(string channel, DateTime from, DateTime to)
        {
            EventRequest tuple = new EventRequest();
            tuple.Channel = channel;
            tuple.From = from;
            tuple.To = to;
            return tuple;
        }
        public override string ToString()
        {
            return "channel=" + Channel + "&"
                + "from=" + From.ToString("yyyy-MM-dd") + "&"
                + "to=" + To.ToString("yyyy-MM-dd");
        }

        public override bool Equals(object obj)
        {
            EventRequest other = obj as EventRequest;
            if (other == null)
            {
                return false;
            }
            else
            {
                return this.Channel == other.Channel && this.From == other.From && this.To == other.To;
            }
        }
    }
}
