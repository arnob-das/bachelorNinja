using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace messManagement.Models
{
    public class UserDashboardViewModel
    {
        public decimal RoomRent { get; set; }
        public decimal UtilityCost { get; set; }
        public decimal TotalCost { get; set; }
        public int TotalMeals { get; set; }
        public decimal MealRate {  get; set; }
        public decimal TotalUserGroceryCost { get; set; }
        public decimal UserMealCost { get; set; }


    }

}