namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dataAnnotationChanges2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cards", "image_url", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cards", "image_url");
        }
    }
}
