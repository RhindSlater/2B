namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedFieldsToCardsModelForApiCalls : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cards", "fname", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cards", "fname");
        }
    }
}
