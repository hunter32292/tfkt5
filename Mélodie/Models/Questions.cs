using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mélodie.Models
{
    public class Questions
    {
        public int ID { get; set; }
        public int lesson_id { get; set; }
        public string text { get; set; }
        public int points { get; set; }
        public string audioclip_path { get; set; }
    }
}