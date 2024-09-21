namespace messManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createMess : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Managers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false, maxLength: 100),
                        PhoneNo = c.String(nullable: false, maxLength: 15),
                        NidNo = c.String(nullable: false, maxLength: 20),
                        Email = c.String(nullable: false),
                        Password = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Messes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MessName = c.String(),
                        MessLocation = c.String(),
                        MessOwnerName = c.String(),
                        MessOwnerPhoneNo = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MessManagers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MessId = c.Int(nullable: false),
                        ManagerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Managers", t => t.ManagerId, cascadeDelete: true)
                .ForeignKey("dbo.Messes", t => t.MessId, cascadeDelete: true)
                .Index(t => t.MessId)
                .Index(t => t.ManagerId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false, maxLength: 100),
                        PhoneNo = c.String(nullable: false, maxLength: 15),
                        NidNo = c.String(nullable: false, maxLength: 20),
                        Email = c.String(nullable: false),
                        Password = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MessManagers", "MessId", "dbo.Messes");
            DropForeignKey("dbo.MessManagers", "ManagerId", "dbo.Managers");
            DropIndex("dbo.MessManagers", new[] { "ManagerId" });
            DropIndex("dbo.MessManagers", new[] { "MessId" });
            DropTable("dbo.Users");
            DropTable("dbo.MessManagers");
            DropTable("dbo.Messes");
            DropTable("dbo.Managers");
        }
    }
}
