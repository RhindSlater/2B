namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cards2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Cards", "fname");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Cards", "fname", c => c.String());
        }
    }
}
