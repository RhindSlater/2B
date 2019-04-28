namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingrarity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cards", "Rarity", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cards", "Rarity");
        }
    }
}
