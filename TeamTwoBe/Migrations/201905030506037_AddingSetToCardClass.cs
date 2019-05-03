namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingSetToCardClass : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cards", "print_tag", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cards", "print_tag");
        }
    }
}
