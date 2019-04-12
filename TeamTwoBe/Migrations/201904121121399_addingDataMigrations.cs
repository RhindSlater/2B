namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingDataMigrations : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "FirstName", c => c.String(nullable: false));
            AlterColumn("dbo.Users", "LastName", c => c.String(nullable: false));
            AlterColumn("dbo.Users", "Username", c => c.String(nullable: false));
            AlterColumn("dbo.Users", "Password", c => c.String(nullable: false));
            AlterColumn("dbo.Users", "Email", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "Email", c => c.String());
            AlterColumn("dbo.Users", "Password", c => c.String());
            AlterColumn("dbo.Users", "Username", c => c.String());
            AlterColumn("dbo.Users", "LastName", c => c.String());
            AlterColumn("dbo.Users", "FirstName", c => c.String());
        }
    }
}
