namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingSaleGradingAndConditionTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Conditions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CardCondition = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Grades",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Grading = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Sales",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Price = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Sales");
            DropTable("dbo.Grades");
            DropTable("dbo.Conditions");
        }
    }
}
