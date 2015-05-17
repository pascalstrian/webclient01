using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebClient
{
    public class WebDAO : IWebDAO
    {
        public async Task<EventResponse> GetDataAsync(EventRequest req)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:9000/");
                string response = await client.GetStringAsync(req.ToString());
                return EventResponse.Create(req, response);
            }
        }

        public async Task GetDataAsync(string url, Action<string, string> processFunc)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:9000/");
                string response = await client.GetStringAsync(url);
                processFunc.Invoke(url, response);
            }
        }
    }
}
