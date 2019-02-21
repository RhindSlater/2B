namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class linkingUsersAndSalesTables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sales", "Seller_ID", c => c.Int());
            CreateIndex("dbo.Sales", "Seller_ID");
            AddForeignKey("dbo.Sales", "Seller_ID", "dbo.Users", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sales", "Seller_ID", "dbo.Users");
            DropIndex("dbo.Sales", new[] { "Seller_ID" });
            DropColumn("dbo.Sales", "Seller_ID");
        }
    }
}
