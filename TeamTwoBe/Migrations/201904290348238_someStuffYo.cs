namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class someStuffYo : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Cards", "Rarity");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Cards", "Rarity", c => c.String());
        }
    }
}
