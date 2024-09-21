using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace messManagement.Models
{
    namespace messManagement.Models
    {
        public class RoomRent
        {
            public int Id { get; set; }
            public int MessId { get; set; }
            public int UserId { get; set; }
            public decimal Rent { get; set; }

            public virtual Mess Mess { get; set; }
            public virtual User User { get; set; }
        }
    }

}