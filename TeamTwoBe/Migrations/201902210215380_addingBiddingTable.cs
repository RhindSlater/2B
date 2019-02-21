namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingBiddingTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bids",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TimeStamps = c.DateTime(nullable: false),
                        BidAmount = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Bids");
        }
    }
}
