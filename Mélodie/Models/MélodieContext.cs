using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Mélodie.Models
{
    public class MélodieContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public MélodieContext() : base("name=MélodieContext")
        {
        }
        public System.Data.Entity.DbSet<Mélodie.Models.Users> Users { get; set; }
        public System.Data.Entity.DbSet<Mélodie.Models.Courses> Course { get; set; }
        public System.Data.Entity.DbSet<Mélodie.Models.Lessons> Lesson { get; set; }
        public System.Data.Entity.DbSet<Mélodie.Models.Roles> Role { get; set; }
        public System.Data.Entity.DbSet<Mélodie.Models.Questions> Questions { get; set; }
    
    }
}
