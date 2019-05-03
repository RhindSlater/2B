namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class DoStuffPlease1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Followers",
                c => new
                {
                    User_ID = c.Int(nullable: false),
                    User_ID1 = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.User_ID, t.User_ID1 })
                .ForeignKey("dbo.Users", t => t.User_ID)
                .ForeignKey("dbo.Users", t => t.User_ID1)
                .Index(t => t.User_ID)
                .Index(t => t.User_ID1);

            CreateTable(
                "dbo.Wishlist",
                c => new
                {
                    Card_ID = c.Int(nullable: false),
                    User_ID = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.Card_ID, t.User_ID })
                .ForeignKey("dbo.Cards", t => t.Card_ID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.User_ID, cascadeDelete: true)
                .Index(t => t.Card_ID)
                .Index(t => t.User_ID);

        }

        public override void Down()
        {
            DropForeignKey("dbo.Wishlist", "User_ID", "dbo.Users");
            DropForeignKey("dbo.Wishlist", "Card_ID", "dbo.Cards");
            DropForeignKey("dbo.Followers", "User_ID1", "dbo.Users");
            DropForeignKey("dbo.Followers", "User_ID", "dbo.Users");
            DropIndex("dbo.Wishlist", new[] { "User_ID" });
            DropIndex("dbo.Wishlist", new[] { "Card_ID" });
            DropIndex("dbo.Followers", new[] { "User_ID1" });
            DropIndex("dbo.Followers", new[] { "User_ID" });
            DropTable("dbo.Wishlist");
            DropTable("dbo.Followers");
        }
    }
}
