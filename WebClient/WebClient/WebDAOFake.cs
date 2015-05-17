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

        public Task<EventResponse> GetDataAsync(string url)
        {
            //Tuple<string, string> urlTupel = Tuple.Create<string, string>(url, "fake response");
            //return Task.Run<Tuple<string, string>>(() => urlTupel);
            Logger.Debug("\r\nGetDataAsync:  "
                + "url: " + url + ", "
                + "thread id: " + Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(4000);
            EventResponse tupel = EventResponse.Create(url, "fake content");
            return Task.Run<EventResponse>(() => tupel);
        }

        public Task GetDataAsync(string url, Action<string, string> processAction)
        {
            return Task.Run(() => { processAction.Invoke(url, "fake response"); });
        }
    }
}
