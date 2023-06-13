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
        public float Satoshi { get; set; }
        [Required]
        public bool? Live { get; set; }
    }

    public class Live {
        [Required]
        public int Id { get; set; }
        public DateTime? DateTimeStart { get; set; }
        public DateTime? DateTimeEnd { get; set; }
        public TimeSpan? Duration { get; set; }
    }

    public class Break
    {
        [Required]
        public int Id { get; set; }
        public DateTime? DateTimeStart { get; set; }
        public DateTime? DateTimeEnd { get; set;}
        public TimeSpan? Duration { get; set; }
    }
}
