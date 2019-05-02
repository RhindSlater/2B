namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dataAnnotationChanges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Sales", "User_ID", "dbo.Users");
            DropIndex("dbo.Sales", new[] { "Seller_ID" });
            DropIndex("dbo.Sales", new[] { "User_ID" });
            DropColumn("dbo.Sales", "Seller_ID");
            RenameColumn(table: "dbo.Sales", name: "User_ID", newName: "Seller_ID");
            AddColumn("dbo.Cards", "Rarity", c => c.String());
            AlterColumn("dbo.Sales", "Seller_ID", c => c.Int(nullable: false));
            CreateIndex("dbo.Sales", "Seller_ID");
            AddForeignKey("dbo.Sales", "Seller_ID", "dbo.Users", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sales", "Seller_ID", "dbo.Users");
            DropIndex("dbo.Sales", new[] { "Seller_ID" });
            AlterColumn("dbo.Sales", "Seller_ID", c => c.Int());
            DropColumn("dbo.Cards", "Rarity");
            RenameColumn(table: "dbo.Sales", name: "Seller_ID", newName: "User_ID");
            AddColumn("dbo.Sales", "Seller_ID", c => c.Int(nullable: false));
            CreateIndex("dbo.Sales", "User_ID");
            CreateIndex("dbo.Sales", "Seller_ID");
            AddForeignKey("dbo.Sales", "User_ID", "dbo.Users", "ID");
        }
    }
}
