namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingApiIDToCard : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cards", "apiID", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cards", "apiID");
        }
    }
}
