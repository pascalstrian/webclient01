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
        private IWebDAO WebDao { get; set; }
        private ILog Logger { get; set; }

        public WebClientClass(IWebDAO webDAO = null, ILog logger = null)
        {
            this.FilesWritten = new List<string>();
            this.WebDao = webDAO ?? new WebDAO();
            this.Logger = logger ?? LogManager.GetLogger(typeof(WebClientClass));
        }

        public IEnumerable<string> DoWork(IEnumerable<EventRequest> reqList, int numThreads = 1, bool splitByDay = false, bool doZip = false)
        {
            IEnumerable<EventRequest> requests = PartitionRequests(reqList, splitByDay);
            IEnumerable<FileDescriptor> fileDescList = DownloadData(numThreads, requests);
            IEnumerable<string> outFiles = fileDescList.Select(desc => desc.Filename);
            if (doZip) {
                outFiles = PackFiles(numThreads, fileDescList);
            }
            //d) move files in toSend:
            IEnumerable<string> movedFiles = MoveFiles(outFiles);
            return outFiles;
        }

        private static IEnumerable<EventRequest> PartitionRequests(IEnumerable<EventRequest> reqList, bool splitByDay)
        {
            IEnumerable<EventRequest> requests = reqList;
            if (splitByDay) {
                requests = reqList.SelectMany(req => ToDayRequests(req));
            }
            return requests;
        }

        private IEnumerable<FileDescriptor> DownloadData(int numThreads, IEnumerable<EventRequest> requests)
        {
            IEnumerable<EventResponse> responseList = requests
                .AsParallel()
                .WithDegreeOfParallelism(numThreads)
                .Select(req => WebDao.GetDataAsync(req).Result);
            IEnumerable<FileDescriptor> fileDescList = responseList
                .Select(res => WriteToFile(GetFileName(res.Request), res))
                .OrderBy(desc => desc.Filename)
                .ToList();
            return fileDescList;
        }

        private IEnumerable<string> PackFiles(int numThreads, IEnumerable<FileDescriptor> fileDescList)
        {
            IEnumerable<string> outFiles = fileDescList
                .AsParallel()
                .WithDegreeOfParallelism(numThreads)
                .GroupBy(fileDesc => GetZipFileName(fileDesc))
                .Select(grp => WriteToZip(grp.Key, grp.ToList()))
                .OrderBy(zipFile => zipFile);
            return outFiles;
        }

        private IEnumerable<string> MoveFiles(IEnumerable<string> outFiles)
        {
            //TODO: more files from sourceDir to targetDir!
            string sourceDir = "Work";
            string targetDir = "ToSend";
            IEnumerable<string> movedFiles = outFiles.Select(file => targetDir + "\\" + file);
            return movedFiles;
        }

        private string WriteToZip(string key, List<FileDescriptor> files)
        {
            //TODO: store files into zip archive!
            string dir = ".";        //todo
            string zipWritten = key; //todo
            IEnumerable<string> filesToZip = files.Select(desc => desc.Filename);
            Logger.Debug("\r\nzipWritten: " + zipWritten
                + "\r\nthread: " + Thread.CurrentThread.ManagedThreadId);
            return zipWritten;
        }

        private string GetZipFileName(FileDescriptor fileDesc)
        {
            string zipName = fileDesc.Channel + ".zip";
            return zipName;
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

        private FileDescriptor WriteToFile(string filename, EventResponse response)
        {
            EventRequest req = response.Request;
            string dir = "."; //todo
            string fileWritten = WriteToFile(filename, response.Content);
            FileDescriptor fileDesc = new FileDescriptor(dir, fileWritten, req.Channel, req.From, req.To);
            return fileDesc;
        }

        private string WriteToFile(string filename, string pageResponse)
        {
            //TODO: write pageResponse into file
            Logger.Debug("WriteToFile: "
                + "file: " + filename + ", "
                + "thread id: " + Thread.CurrentThread.ManagedThreadId);
            return filename;
        }

        private string GetFileName(EventRequest req)
        {
            return GetFileName(req.Channel, req.From, req.To);
        }

        private string GetFileName(string channel, DateTime from, DateTime? to = null)
        {
            return channel + "_" + from.ToString("yyyy-MM-dd") + "_" + to.Value.ToString("yyyy-MM-dd") + ".html";
        }

        #region deprecated methods

        public IList<string> FilesWritten { get; private set; } // deprecated because stateful

        public void DownloadStateful(IEnumerable<string> urls)
        {
            Task webTask1 = WebDao.GetDataAsync(urls.ElementAt(0), WriteToFileStateful);
            Task webTask2 = WebDao.GetDataAsync(urls.ElementAt(1), WriteToFileStateful);
            Task.WaitAll(webTask1, webTask2);
        }

        //public IEnumerable<string> DownloadUrlsWithTasks(IEnumerable<string> urls)
        //{
        //    IList<Task<EventResponse>> tasks = urls
        //        .Select(url => WebDao.GetDataAsync(url))
        //        .ToList();
        //    IEnumerable<string> fileNames = Task
        //        .WhenAll<EventResponse>(tasks)
        //        .Result
        //        .Select(resTuple => WriteToFile(resTuple.Request, resTuple.Response));
        //    return fileNames;
        //}

        private void WriteToFileStateful(string url, string pageResponse)
        {
            //TODO: write pageResponse into file
            string fileWritten = url + ".html";
            FilesWritten.Add(fileWritten);
        }

        public IEnumerable<string> DownloadUrlsSequential(IEnumerable<EventRequest> reqList, bool splitByDay = false)
        {
            if (splitByDay)
            {
                reqList = reqList.SelectMany(req => ToDayRequests(req));
            }
            IEnumerable<string> fileNames = reqList
                .Select(req => WebDao.GetDataAsync(req).Result)
                .Select(resTuple => WriteToFile(GetFileName(resTuple.Request), resTuple.Content))
                .ToList();
            List<string> fileNamesOrdered = fileNames
                .OrderBy(fileName => fileName)
                .ToList();
            return fileNamesOrdered;
        }

        #endregion
    }
}
