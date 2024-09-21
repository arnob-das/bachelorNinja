using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace messManagement.Models
{
    public class MessMember
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MessId { get; set; }

        public virtual User User { get; set; }
        public virtual Mess Mess { get; set; }
    }

}