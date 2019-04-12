namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingDataMigrations2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "Password", c => c.String(nullable: false, maxLength: 21));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "Password", c => c.String(nullable: false));
        }
    }
}
