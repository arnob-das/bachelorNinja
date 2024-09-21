using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace messManagement.Models
{
    public class Mess
    {
        public int Id { get; set; }
        public string MessName { get; set; }
        public string MessLocation { get; set; }
        public string MessOwnerName { get; set; }
        public string MessOwnerPhoneNo { get; set; }

    }
}