using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Streams
    {
            

        [Required]
        public int Id {  get; set; }
        public List<Live>? LiveList { get; set; }
        public List<Break>? BreakList { get; set; }
        public Decimal Satoshi { get; set; }
        public string? UserName { get; set; }
        public bool? Live { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Duration { get; set; }

        public Streams() { }

        public Streams(string username, dynamic live, dynamic startStream)
        {
            this.UserName = username;
            this.Live = live;
            this.Start = startStream;
        }
    }

    public class Live {
        [Required]
        public int Id { get; set; }
        public DateTime? DateTimeStart { get; set; }
        public DateTime? DateTimeEnd { get; set; }
        public TimeSpan? Duration { get; set; }

        // Foreign key property
        public int StreamId { get; set; }

        // Navigation property
        public Streams Stream { get; set; }
        public Live() { }
        public Live(TimeSpan live, DateTime start, DateTime end, int streamid)
        {
            this.Duration = live;
            this.DateTimeStart = start;
            this.DateTimeEnd = end;
            this.StreamId = streamid;
        }
    }

    public class Break
    {
      
        [Required]
        public int Id { get; set; }
        public DateTime? DateTimeStart { get; set; }
        public DateTime? DateTimeEnd { get; set;}
        public TimeSpan? Duration { get; set; }

        // Foreign key property
        public int StreamId { get; set; }

        // Navigation property
        public Streams Stream { get; set; }
        public Break() { }
        public Break(TimeSpan live, DateTime start, DateTime end, int streamid)
        {
            this.Duration = live;
            this.DateTimeStart = start;
            this.DateTimeEnd = end;
            this.StreamId = streamid;
        }
    }
}
