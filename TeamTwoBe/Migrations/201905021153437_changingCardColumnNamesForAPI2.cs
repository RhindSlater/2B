namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changingCardColumnNamesForAPI2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Cards", "low", c => c.Double(nullable: false));
            AlterColumn("dbo.Cards", "average", c => c.Double(nullable: false));
            AlterColumn("dbo.Cards", "high", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Cards", "high", c => c.Single(nullable: false));
            AlterColumn("dbo.Cards", "average", c => c.Single(nullable: false));
            AlterColumn("dbo.Cards", "low", c => c.Single(nullable: false));
        }
    }
}
