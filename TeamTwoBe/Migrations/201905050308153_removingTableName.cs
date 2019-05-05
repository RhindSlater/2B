namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removingTableName : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Collection", newName: "Collection");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Collection", newName: "Collection");
        }
    }
}
