namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingBuyerAndIsSoldToSales : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Sales", "Seller_ID", "dbo.Users");
            AddColumn("dbo.Sales", "IsSold", c => c.Boolean(nullable: false));
            AddColumn("dbo.Sales", "Buyer_ID", c => c.Int());
            AddColumn("dbo.Sales", "User_ID", c => c.Int());
            CreateIndex("dbo.Sales", "Buyer_ID");
            CreateIndex("dbo.Sales", "User_ID");
            AddForeignKey("dbo.Sales", "Buyer_ID", "dbo.Users", "ID");
            AddForeignKey("dbo.Sales", "User_ID", "dbo.Users", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sales", "User_ID", "dbo.Users");
            DropForeignKey("dbo.Sales", "Buyer_ID", "dbo.Users");
            DropIndex("dbo.Sales", new[] { "User_ID" });
            DropIndex("dbo.Sales", new[] { "Buyer_ID" });
            DropColumn("dbo.Sales", "User_ID");
            DropColumn("dbo.Sales", "Buyer_ID");
            DropColumn("dbo.Sales", "IsSold");
            AddForeignKey("dbo.Sales", "Seller_ID", "dbo.Users", "ID", cascadeDelete: true);
        }
    }
}
