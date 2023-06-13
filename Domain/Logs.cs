﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Logs
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Message { get; set; }
        [Required]
        public string? Level { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }
        public string? Exception { get; set; }
    }
}
