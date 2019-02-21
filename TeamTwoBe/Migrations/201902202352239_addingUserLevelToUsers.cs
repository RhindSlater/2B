namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingUserLevelToUsers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "UserLevel_ID", c => c.Int());
            CreateIndex("dbo.Users", "UserLevel_ID");
            AddForeignKey("dbo.Users", "UserLevel_ID", "dbo.AccountTypes", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "UserLevel_ID", "dbo.AccountTypes");
            DropIndex("dbo.Users", new[] { "UserLevel_ID" });
            DropColumn("dbo.Users", "UserLevel_ID");
        }
    }
}
