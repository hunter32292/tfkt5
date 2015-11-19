using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mélodie.Models
{
    public class Users
    {
        public int ID { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string password_salt { get; set; }
        public string password_hash { get; set; }
        public string role_id { get; set; }
    }
}