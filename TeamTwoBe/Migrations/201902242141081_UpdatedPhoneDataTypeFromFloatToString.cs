namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedPhoneDataTypeFromFloatToString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "Phone", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "Phone", c => c.Single(nullable: false));
        }
    }
}
