using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClient
{
    public class EventResponse
    {
        public string Url { get; set; }
        public string Response { get; set; }
        public static EventResponse Create(string url, string response)
        {
            EventResponse tuple = new EventResponse();
            tuple.Url = url;
            tuple.Response = response;
            return tuple;
        }
    }
}
