using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using messManagement.Models;
using messManagement.Models.messManagement.Models;

namespace messManagement.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("DefaultConnection")
        {
        }

        public DbSet<Manager> Managers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Mess> Messes { get; set; }
        public DbSet<MessMember> MessMembers { get; set; }
        public DbSet<MessManager> MessManagers { get; set; }
        public DbSet<RoomRent> RoomRents { get; set; }
        public DbSet<UtilityBill> UtilityBills { get; set; }
        public DbSet<MealInformation> MealInformations { get; set; }
        public DbSet<GroceryCost> GroceryCosts { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {


           

            base.OnModelCreating(modelBuilder);
        }
    }
}
