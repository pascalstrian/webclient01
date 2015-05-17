using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClient
{
    public interface IWebDAO
    {
        Task<EventResponse> GetDataAsync(string url);
        Task GetDataAsync(string url, Action<string, string> processAction);
    }
}
