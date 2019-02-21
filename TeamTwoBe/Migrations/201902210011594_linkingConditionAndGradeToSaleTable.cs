namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class linkingConditionAndGradeToSaleTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sales", "CardCondition_ID", c => c.Int());
            AddColumn("dbo.Sales", "CardGrade_ID", c => c.Int());
            CreateIndex("dbo.Sales", "CardCondition_ID");
            CreateIndex("dbo.Sales", "CardGrade_ID");
            AddForeignKey("dbo.Sales", "CardCondition_ID", "dbo.Conditions", "ID");
            AddForeignKey("dbo.Sales", "CardGrade_ID", "dbo.Grades", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sales", "CardGrade_ID", "dbo.Grades");
            DropForeignKey("dbo.Sales", "CardCondition_ID", "dbo.Conditions");
            DropIndex("dbo.Sales", new[] { "CardGrade_ID" });
            DropIndex("dbo.Sales", new[] { "CardCondition_ID" });
            DropColumn("dbo.Sales", "CardGrade_ID");
            DropColumn("dbo.Sales", "CardCondition_ID");
        }
    }
}
