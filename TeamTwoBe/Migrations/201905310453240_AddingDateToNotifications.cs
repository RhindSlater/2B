namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingDateToNotifications : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notifications", "Date", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Notifications", "Date");
        }
    }
}
