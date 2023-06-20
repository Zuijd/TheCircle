using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Storage
    {
        [Required]
        public DateTime Timestamp { get; set; }
        [Key]
        public string Chunk { get; set; }

        public Storage(DateTime timestamp, string chunk)
        {
            this.Timestamp = timestamp;
            this.Chunk = chunk;
        }
    }
}
