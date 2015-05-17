using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebClient
{
    public class WebClientClass
    {
        public IList<string> FilesWritten { get; private set; } // deprecated because stateful
        private IWebDAO WebDao { get; set; }
        private ILog Logger { get; set; }

        public WebClientClass(IWebDAO webDAO = null, ILog logger = null)
        {
            this.FilesWritten = new List<string>();
            this.WebDao = webDAO ?? new WebDAO();
            this.Logger = logger ?? LogManager.GetLogger(typeof(WebClientClass));
        }

        public void DownloadStateful(IEnumerable<string> urls)
        {
            Task webTask1 = WebDao.GetDataAsync(urls.ElementAt(0), WriteToFileStateful);
            Task webTask2 = WebDao.GetDataAsync(urls.ElementAt(1), WriteToFileStateful);
            Task.WaitAll(webTask1, webTask2);
        }

        public IEnumerable<string> DownloadUrls(IEnumerable<string> urls)
        {
            IList<Task<EventResponse>> tasks = urls
                .Select(url => WebDao.GetDataAsync(url))
                .ToList();
            IEnumerable<string> fileNames = Task
                .WhenAll<EventResponse>(tasks)
                .Result
                .Select(resTuple => WriteToFile(resTuple.Url, resTuple.Response));
            return fileNames;
        }

        public IEnumerable<string> DownloadUrlsWithPLinq(IEnumerable<string> urls, int numThreads = 1)
        {
            List<string> result = urls
                .AsParallel()
                .WithDegreeOfParallelism(numThreads)
                .Select(url => WebDao.GetDataAsync(url).Result)
                .Select(resTuple => WriteToFile(resTuple.Url, resTuple.Response))
                .ToList();
            return result;
        }

        public IEnumerable<string> DownloadRanges(IEnumerable<EventRequest> inList, int numThreads = 1) 
        {
            List<string> fileNames = inList
                .AsParallel()
                .WithDegreeOfParallelism(numThreads)
                .SelectMany(req => ToDayRequests(req))
                .Select(req => WebDao.GetDataAsync(req.ToString()).Result)
                .Select(resTuple => WriteToFile(resTuple.Url, resTuple.Response))
                .ToList();
            List<string> fileNamesOrdered = fileNames
                .OrderBy(fileName => fileName)
                .ToList();
                //.GroupBy(paramList => paramList.Item1)
                //.Select(grp => DownloadByItem1(grp.Key, grp.ToList()))
                //.ToList();
            return fileNamesOrdered;
        }

        public static IEnumerable<EventRequest> ToDayRequests(EventRequest req)
        {
            // a) expand all days from range
            // b) for each day create a day-range (spanning only one day)
            // c) for each day-range create a request
            IEnumerable<EventRequest> dayRequests = TimeRange.RangeToDayList(req.From, req.To) // a
                .Select(day => TimeRange.DayListToRange(new List<DateTime>() { day }))         // b
                .Select(range => EventRequest.Create(req.Channel, range.From, range.To));      // c
            return dayRequests;
        }

        private IEnumerable<string> DownloadByItem1(string key, List<Tuple<string, string>> list)
        {
            throw new NotImplementedException();
        }

        private string WriteToFile(string url, string pageResponse)
        {
            //TODO: write pageResponse into file
            Logger.Debug("WriteToFile: "
                + "url: " + url + ", "
                + "thread id: " + Thread.CurrentThread.ManagedThreadId);
            string fileWritten = url + ".html";
            return fileWritten;
        }

        private void WriteToFileStateful(string url, string pageResponse)
        {
            //TODO: write pageResponse into file
            string fileWritten = url + ".html";
            FilesWritten.Add(fileWritten);
        }
    }
}
