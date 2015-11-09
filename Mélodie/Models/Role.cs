using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mélodie.Models
{
    public class Roles
    {
        public int ID { get; set; }
        public string type { get; set; }
        public string description { get; set; }

        public String idToString()
        {
            return ID.ToString();
        }
    }
}