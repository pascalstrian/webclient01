using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClient
{
    public class TimeRange
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public TimeRange(DateTime from, DateTime to)
        {
            this.From = from;
            this.To = to;
        }

        public IEnumerable<DateTime> ToDayList()
        {
            int numDays = (To - From).Days + 1;
            return Enumerable.Range(0, numDays)
                .Select(day => From.AddDays(day));
        }

        public static IEnumerable<DateTime> RangeToDayList(DateTime from, DateTime to)
        {
            TimeRange range = new TimeRange(from, to);
            return range.ToDayList();
        }

        public static TimeRange DayListToRange(IEnumerable<DateTime> dayList)
        {
            return new TimeRange(dayList.First(), dayList.Last());
        }

        public override bool Equals(object obj)
        {
            TimeRange other = obj as TimeRange;
            if (other == null) {
                return false;
            }
            else {
                return this.From == other.From && this.To == other.To;
            }
        }

        public override string ToString()
        {
            return this.From + " - " + this.To;
        }
    }
}
