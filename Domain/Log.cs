using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Log
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }

        public Log(string username, string message)
        {
            Username = username;
            Message = message;
            Timestamp = DateTime.Now;
        }

        public override string ToString()
        {
            return $"[{Timestamp}] {Username}: {Message}";
        }
    }
}
