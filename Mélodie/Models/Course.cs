using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mélodie.Models
{
    public class Course
    {
        public int ID { get; set; }
        public string title { get; set; }
        public int user_id { get; set; }
        public string description { get; set; }
    }
}