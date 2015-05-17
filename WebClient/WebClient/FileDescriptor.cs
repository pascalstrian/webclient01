using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClient
{
    public class FileDescriptor
    {
        public string Directory { get; private set; }
        public string Filename { get; private set; }
        public string Channel { get; private set; }
        public DateTime? From { get; private set; }
        public DateTime? To { get; private set; }

        public FileDescriptor(string dir, string name, string channel, DateTime? from, DateTime? to)
        {
            this.Directory = dir;
            this.Filename = name;
            this.Channel = channel;
            this.From = from;
            this.To = to;
        }

    }
}
