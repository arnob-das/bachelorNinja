using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace messManagement.Models
{
    public class MemberCostViewModel
    {
        public string UserName { get; set; }
        public decimal RoomRent { get; set; }
        public decimal TotalMeals { get; set; }
        public decimal MealCost { get; set; }
        public decimal UtilityCost { get; set; }
        public decimal TotalCost { get; set; }
        public decimal GroceryCost { get; set; }
    }

}