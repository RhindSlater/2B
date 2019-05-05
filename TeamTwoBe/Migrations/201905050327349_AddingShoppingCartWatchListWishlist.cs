namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingShoppingCartWatchListWishlist : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Followers", newName: "Followers");
            RenameTable(name: "dbo.Wishlist", newName: "Wishlist");
            RenameColumn(table: "dbo.Followers", name: "User_ID", newName: "Follower");
            RenameColumn(table: "dbo.Followers", name: "User_ID1", newName: "Following");
            RenameIndex(table: "dbo.Followers", name: "IX_User_ID", newName: "IX_Follower");
            RenameIndex(table: "dbo.Followers", name: "IX_User_ID1", newName: "IX_Following");
            CreateTable(
                "dbo.ShoppingCart",
                c => new
                    {
                        User_ID = c.Int(nullable: false),
                        Sale_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_ID, t.Sale_ID })
                .ForeignKey("dbo.Users", t => t.User_ID, cascadeDelete: false)
                .ForeignKey("dbo.Sales", t => t.Sale_ID, cascadeDelete: false)
                .Index(t => t.User_ID)
                .Index(t => t.Sale_ID);
            
            CreateTable(
                "dbo.Watchlist",
                c => new
                    {
                        User_ID = c.Int(nullable: false),
                        Sale_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_ID, t.Sale_ID })
                .ForeignKey("dbo.Users", t => t.User_ID, cascadeDelete: false)
                .ForeignKey("dbo.Sales", t => t.Sale_ID, cascadeDelete: false)
                .Index(t => t.User_ID)
                .Index(t => t.Sale_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Watchlist", "Sale_ID", "dbo.Sales");
            DropForeignKey("dbo.Watchlist", "User_ID", "dbo.Users");
            DropForeignKey("dbo.ShoppingCart", "Sale_ID", "dbo.Sales");
            DropForeignKey("dbo.ShoppingCart", "User_ID", "dbo.Users");
            DropIndex("dbo.Watchlist", new[] { "Sale_ID" });
            DropIndex("dbo.Watchlist", new[] { "User_ID" });
            DropIndex("dbo.ShoppingCart", new[] { "Sale_ID" });
            DropIndex("dbo.ShoppingCart", new[] { "User_ID" });
            DropTable("dbo.Watchlist");
            DropTable("dbo.ShoppingCart");
            RenameIndex(table: "dbo.Followers", name: "IX_Following", newName: "IX_User_ID1");
            RenameIndex(table: "dbo.Followers", name: "IX_Follower", newName: "IX_User_ID");
            RenameColumn(table: "dbo.Followers", name: "Following", newName: "User_ID1");
            RenameColumn(table: "dbo.Followers", name: "Follower", newName: "User_ID");
            RenameTable(name: "dbo.Wishlist", newName: "Wishlist");
            RenameTable(name: "dbo.Followers", newName: "Followers");
        }
    }
}
