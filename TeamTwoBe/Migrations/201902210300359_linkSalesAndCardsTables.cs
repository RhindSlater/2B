namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class linkSalesAndCardsTables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sales", "Card_ID", c => c.Int());
            CreateIndex("dbo.Sales", "Card_ID");
            AddForeignKey("dbo.Sales", "Card_ID", "dbo.Cards", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sales", "Card_ID", "dbo.Cards");
            DropIndex("dbo.Sales", new[] { "Card_ID" });
            DropColumn("dbo.Sales", "Card_ID");
        }
    }
}
