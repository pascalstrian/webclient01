using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace WebClient.Test
{
    [TestFixture]
    class TimeRangeTest
    {
        [Test]
        public void ToDayList_in_out()
        {
            //ARRANGE
            TimeRange range = new TimeRange(new DateTime(2015, 01, 01), new DateTime(2015, 01, 05));

            //ACT
            IEnumerable<DateTime> dayList = range.ToDayList();

            //ASSERT
            Assert.IsNotEmpty(dayList);
            Assert.AreEqual(new DateTime(2015, 01, 02), dayList.ElementAt(1));
        }

        [Test]
        public void DayListToRange_in_out()
        {
            //ARRANGE
            IEnumerable<DateTime> dayList = new List<DateTime>() { new DateTime(2015, 01, 01) };
            TimeRange rangeExp = new TimeRange(new DateTime(2015, 01, 01), new DateTime(2015, 01, 01));

            //ACT
            TimeRange result = TimeRange.DayListToRange(dayList);

            //ASSERT
            Assert.AreEqual(rangeExp, result);
        }
    }
}
