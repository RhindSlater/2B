namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changingCardColumnNamesForAPI : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cards", "low", c => c.Single(nullable: false));
            AddColumn("dbo.Cards", "average", c => c.Single(nullable: false));
            AddColumn("dbo.Cards", "high", c => c.Single(nullable: false));
            DropColumn("dbo.Cards", "PriceLow");
            DropColumn("dbo.Cards", "PriceAverage");
            DropColumn("dbo.Cards", "PriceHigh");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Cards", "PriceHigh", c => c.Single(nullable: false));
            AddColumn("dbo.Cards", "PriceAverage", c => c.Single(nullable: false));
            AddColumn("dbo.Cards", "PriceLow", c => c.Single(nullable: false));
            DropColumn("dbo.Cards", "high");
            DropColumn("dbo.Cards", "average");
            DropColumn("dbo.Cards", "low");
        }
    }
}
