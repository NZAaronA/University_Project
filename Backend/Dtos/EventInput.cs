using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace A2.Dto
{
    public class EventInput
    {
        [Required]
        public string start { get; set; }
        [Required]
        public string end { get; set; }
        public string summary { get; set; }
        public string description { get; set; }
        [Required]
        public string location { get; set; }
    }
}