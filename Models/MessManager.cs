using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace messManagement.Models
{
    public class MessManager
    {
        public int Id { get; set; }
        public int MessId { get; set; }
        public int ManagerId { get; set; }

        public virtual Mess Mess { get; set; }
        public virtual Manager Manager { get; set; }
    }
}