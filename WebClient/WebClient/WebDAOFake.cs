using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebClient
{
    public class WebDAOFake : IWebDAO
    {
        private ILog Logger { get; set; }

        public WebDAOFake(ILog logger)
        {
            this.Logger = logger;
        }

        public Task<EventResponse> GetDataAsync(EventRequest req)
        {
            Logger.Debug("\r\nGetDataAsync:  "
                + "url: " + req.ToString() + ", "
                + "thread id: " + Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(4000);
            EventResponse tupel = EventResponse.Create(req, "fake content");
            return Task.Run<EventResponse>(() => tupel);
        }

        public Task GetDataAsync(string url, Action<string, string> processAction)
        {
            return Task.Run(() => { processAction.Invoke(url, "fake response"); });
        }
    }
}
