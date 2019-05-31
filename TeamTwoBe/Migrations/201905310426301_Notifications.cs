namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Notifications : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Message = c.String(),
                        Seen = c.Boolean(nullable: false),
                        NotifyUser_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.NotifyUser_ID)
                .Index(t => t.NotifyUser_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notifications", "NotifyUser_ID", "dbo.Users");
            DropIndex("dbo.Notifications", new[] { "NotifyUser_ID" });
            DropTable("dbo.Notifications");
        }
    }
}
