namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingUploadDateToSale : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sales", "UploadDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sales", "UploadDate");
        }
    }
}
