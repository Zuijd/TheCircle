using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Storage
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }
        [Required]
        public byte[] Chunk { get; set; }

        public Storage(DateTime timestamp, byte[] chunk)
        {
            this.Timestamp = timestamp;
            this.Chunk = chunk;
        }
    }
}
