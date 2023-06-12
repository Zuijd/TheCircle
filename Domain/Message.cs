using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Message
    {
        [Required]
        public int Id { get; set; } 
        [Required]
        public User User { get; set; } = null!;
        [Required]
        public string MessageBody { get; set; } = null!;
        [Required]
        public DateTime TimeStamp { get; set; } = DateTime.Now;
    }
}
