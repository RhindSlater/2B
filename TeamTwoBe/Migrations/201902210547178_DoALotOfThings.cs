namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DoALotOfThings : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Cards", "User_ID", "dbo.Users");
            DropIndex("dbo.Cards", new[] { "User_ID" });
            CreateTable(
                "dbo.WishList",
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
            
            DropColumn("dbo.Cards", "User_ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Cards", "User_ID", c => c.Int());
            DropForeignKey("dbo.CardUsers", "User_ID", "dbo.Users");
            DropForeignKey("dbo.CardUsers", "Card_ID", "dbo.Cards");
            DropIndex("dbo.WishList", new[] { "User_ID" });
            DropIndex("dbo.WishList", new[] { "Card_ID" });
            DropTable("dbo.WishList");
            CreateIndex("dbo.Cards", "User_ID");
            AddForeignKey("dbo.Cards", "User_ID", "dbo.Users", "ID");
        }
    }
}
