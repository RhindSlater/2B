namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingCollectionToUsers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cards", "User_ID", c => c.Int());
            CreateIndex("dbo.Cards", "User_ID");
            AddForeignKey("dbo.Cards", "User_ID", "dbo.Users", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Cards", "User_ID", "dbo.Users");
            DropIndex("dbo.Cards", new[] { "User_ID" });
            DropColumn("dbo.Cards", "User_ID");
        }
    }
}
