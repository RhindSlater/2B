namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Target201904131017421 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contacts", "Subject", c => c.String());
            AddColumn("dbo.Contacts", "Message", c => c.String());
            DropColumn("dbo.Contacts", "FirstName");
            DropColumn("dbo.Contacts", "LastName");
            DropColumn("dbo.Contacts", "Comment");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Contacts", "Comment", c => c.String());
            AddColumn("dbo.Contacts", "LastName", c => c.String());
            AddColumn("dbo.Contacts", "FirstName", c => c.String());
            DropColumn("dbo.Contacts", "Message");
            DropColumn("dbo.Contacts", "Subject");
        }
    }
}
