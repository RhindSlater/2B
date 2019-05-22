namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_IsVerified_ToSaleController : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sales", "IsVerified", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sales", "IsVerified");
        }
    }
}
