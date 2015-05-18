using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using log4net;

namespace WebClient.Test
{
    [TestFixture]
    class WebClientClassTest
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WebClientClassTest));

        [Test]
        [Ignore("deprecated")]
        public void DownloadStateful_in_out()
        {
            //Arrange
            IWebDAO webDAO = new WebDAOFake(Logger);
            WebClientClass webClient = new WebClientClass(webDAO);
            IEnumerable<string> urls = new List<string>() { "url1", "url2" };

            //Act
            webClient.DownloadStateful(urls);
            IList<string> filesWritten = webClient.FilesWritten;

            //Assert
            Assert.IsNotEmpty(filesWritten);
            Assert.AreEqual("url1.html", filesWritten[0]);
            Assert.AreEqual("url2.html", filesWritten[1]);
        }

        [Test]
        public void DownloadUrls_SplitByDay_Returns3x5Files()
        {
            //Arrange
            ILog logMock = Mock.Of<ILog>();
            IWebDAO webDAO = new WebDAOFake(Logger);
            WebClientClass webClient = new WebClientClass(webDAO, logMock);
            IEnumerable<string> channels = new List<string>() { "chan0", "chan1", "chan2", "chan3", "chan4", "chan5", "chan6", "chan7", "chan8", "chan9" };
            //IEnumerable<string> channels = new List<string>() { "chan0", "chan1", "chan2" };
            IEnumerable<EventRequest> requests = channels
                .Select(url => EventRequest.Create(url, new DateTime(2015, 01, 01), new DateTime(2015, 01, 05)));

            //Act
            IEnumerable<string> filesWritten = webClient.DownloadUrls(requests, numThreads: 8, splitByDay: true);

            //Assert
            Logger.Info("\r\nfilesWritten:\r\n" + string.Join("\r\n", filesWritten.Select(file => file.ToString())));
            Assert.IsNotEmpty(filesWritten);
            Assert.AreEqual(50, filesWritten.Count());
            Assert.AreEqual("chan0_2015-01-01_2015-01-01.html", filesWritten.ElementAt(0));
            Assert.AreEqual("chan0_2015-01-02_2015-01-02.html", filesWritten.ElementAt(1));
            Assert.AreEqual("chan1_2015-01-01_2015-01-01.html", filesWritten.ElementAt(5));
        }

        [Test]
        public void DownloadUrls_SplitByDayAndZipToChannel_Returns3Zipfiles()
        {
            //Arrange
            ILog logMock = Mock.Of<ILog>();
            IWebDAO webDAO = new WebDAOFake(Logger);
            WebClientClass webClient = new WebClientClass(webDAO, logMock);
            IEnumerable<string> channels = new List<string>() { "chan0", "chan1", "chan2" };
            IEnumerable<EventRequest> requests = channels
                .Select(url => EventRequest.Create(url, new DateTime(2015, 01, 01), new DateTime(2015, 01, 05)));

            //Act
            IEnumerable<string> filesWritten = webClient.DownloadUrls(requests, numThreads: 8, splitByDay: true, doZip: true);

            //Assert
            Logger.Info("\r\nfilesWritten:\r\n" + string.Join("\r\n", filesWritten.Select(file => file.ToString())));
            Assert.IsNotEmpty(filesWritten);
            Assert.AreEqual(3, filesWritten.Count());
            Assert.AreEqual("chan0.zip", filesWritten.ElementAt(0));
            Assert.AreEqual("chan1.zip", filesWritten.ElementAt(1));
            Assert.AreEqual("chan2.zip", filesWritten.ElementAt(2));
        }

        [Test]
        public void DownloadUrls_ByRange_Returns3Files()
        {
            //Arrange
            ILog logMock = Mock.Of<ILog>();
            IWebDAO webDAO = new WebDAOFake(logMock);
            WebClientClass webClient = new WebClientClass(webDAO, logMock);
            IEnumerable<string> channels = new List<string>() { "chan0", "chan1", "chan2" };
            IEnumerable<EventRequest> requests = channels
                .Select(url => EventRequest.Create(url, new DateTime(2015, 01, 01), new DateTime(2015, 01, 05)));

            //Act
            IEnumerable<string> filesWritten = webClient.DownloadUrls(requests, numThreads: 8, splitByDay: false);

            //Assert
            Logger.Info("\r\nfilesWritten:\r\n" + string.Join("\r\n", filesWritten.Select(file => file.ToString())));
            Assert.IsNotEmpty(filesWritten);
            Assert.AreEqual(3, filesWritten.Count());
            Assert.AreEqual("chan0_2015-01-01_2015-01-05.html", filesWritten.ElementAt(0));
            Assert.AreEqual("chan1_2015-01-01_2015-01-05.html", filesWritten.ElementAt(1));
            Assert.AreEqual("chan2_2015-01-01_2015-01-05.html", filesWritten.ElementAt(2));
        }

        [Test]
        public void ToDayRequests_in_out()
        {
            //Arrange
            EventRequest req = EventRequest.Create("chan01", new DateTime(2015, 01, 01), new DateTime(2015, 01, 05));
            EventRequest reqExp1 = EventRequest.Create("chan01", new DateTime(2015, 01, 01), new DateTime(2015, 01, 01));
            EventRequest reqExp2 = EventRequest.Create("chan01", new DateTime(2015, 01, 02), new DateTime(2015, 01, 02));

            //Act
            IEnumerable<EventRequest> reqList = WebClientClass.ToDayRequests(req);

            //Assert
            Assert.IsNotEmpty(reqList);
            Assert.AreEqual(reqExp1, reqList.ElementAt(0));
            Assert.AreEqual(reqExp2, reqList.ElementAt(1));
        }

    }
}
;