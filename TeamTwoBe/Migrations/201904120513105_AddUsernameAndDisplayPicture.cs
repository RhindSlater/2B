namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUsernameAndDisplayPicture : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "FirstName", c => c.String());
            AddColumn("dbo.Users", "LastName", c => c.String());
            AddColumn("dbo.Users", "Username", c => c.String());
            AddColumn("dbo.Users", "IsLocked", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "DisplayPicture", c => c.String());
            DropColumn("dbo.Users", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "Name", c => c.String());
            DropColumn("dbo.Users", "DisplayPicture");
            DropColumn("dbo.Users", "IsLocked");
            DropColumn("dbo.Users", "Username");
            DropColumn("dbo.Users", "LastName");
            DropColumn("dbo.Users", "FirstName");
        }
    }
}
