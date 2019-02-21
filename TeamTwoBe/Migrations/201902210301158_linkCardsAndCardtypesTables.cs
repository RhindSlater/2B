namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class linkCardsAndCardtypesTables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cards", "Cardtype_ID", c => c.Int());
            CreateIndex("dbo.Cards", "Cardtype_ID");
            AddForeignKey("dbo.Cards", "Cardtype_ID", "dbo.CardTypes", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Cards", "Cardtype_ID", "dbo.CardTypes");
            DropIndex("dbo.Cards", new[] { "Cardtype_ID" });
            DropColumn("dbo.Cards", "Cardtype_ID");
        }
    }
}
