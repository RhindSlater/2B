namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removingColumnFromCards : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Cards", "User_ID", "dbo.Users");
            DropIndex("dbo.Cards", new[] { "User_ID" });
            CreateTable(
                "dbo.Collection",
                c => new
                    {
                        User_ID = c.Int(nullable: false),
                        Card_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_ID, t.Card_ID })
                .ForeignKey("dbo.Users", t => t.User_ID, cascadeDelete: true)
                .ForeignKey("dbo.Cards", t => t.Card_ID, cascadeDelete: true)
                .Index(t => t.User_ID)
                .Index(t => t.Card_ID);
            
            DropColumn("dbo.Cards", "User_ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Cards", "User_ID", c => c.Int());
            DropForeignKey("dbo.Collection", "Card_ID", "dbo.Cards");
            DropForeignKey("dbo.Collection", "User_ID", "dbo.Users");
            DropIndex("dbo.Collection", new[] { "Card_ID" });
            DropIndex("dbo.Collection", new[] { "User_ID" });
            DropTable("dbo.Collection");
            CreateIndex("dbo.Cards", "User_ID");
            AddForeignKey("dbo.Cards", "User_ID", "dbo.Users", "ID");
        }
    }
}
