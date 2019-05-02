namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class wishlistUsersFix : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Cards", "User_ID", "dbo.Users");
            DropIndex("dbo.Cards", new[] { "User_ID" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Cards", "User_ID");
            AddForeignKey("dbo.Cards", "User_ID", "dbo.Users", "ID");
        }
    }
}
