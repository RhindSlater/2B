namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class linkingBiddingAndSalesTables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bids", "Item_ID", c => c.Int());
            CreateIndex("dbo.Bids", "Item_ID");
            AddForeignKey("dbo.Bids", "Item_ID", "dbo.Sales", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bids", "Item_ID", "dbo.Sales");
            DropIndex("dbo.Bids", new[] { "Item_ID" });
            DropColumn("dbo.Bids", "Item_ID");
        }
    }
}
