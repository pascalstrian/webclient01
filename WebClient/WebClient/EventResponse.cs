using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClient
{
    public class EventResponse
    {
        public EventRequest Request { get; set; }

        public string Response { get; set; }

        public static EventResponse Create(EventRequest req, string response)
        {
            EventResponse tuple = new EventResponse();
            tuple.Request = req;
            tuple.Response = response;
            return tuple;
        }
    }
}
