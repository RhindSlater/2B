namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class linkingBiddingAndUserTables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bids", "Bidder_ID", c => c.Int());
            CreateIndex("dbo.Bids", "Bidder_ID");
            AddForeignKey("dbo.Bids", "Bidder_ID", "dbo.Users", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bids", "Bidder_ID", "dbo.Users");
            DropIndex("dbo.Bids", new[] { "Bidder_ID" });
            DropColumn("dbo.Bids", "Bidder_ID");
        }
    }
}
