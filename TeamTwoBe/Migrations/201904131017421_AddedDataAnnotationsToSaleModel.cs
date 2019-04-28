namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDataAnnotationsToSaleModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Sales", "Seller_ID", "dbo.Users");
            DropForeignKey("dbo.Sales", "Card_ID", "dbo.Cards");
            DropForeignKey("dbo.Sales", "CardCondition_ID", "dbo.Conditions");
            DropForeignKey("dbo.Sales", "CardGrade_ID", "dbo.Grades");
            DropIndex("dbo.Sales", new[] { "Card_ID" });
            DropIndex("dbo.Sales", new[] { "CardCondition_ID" });
            DropIndex("dbo.Sales", new[] { "CardGrade_ID" });
            DropIndex("dbo.Sales", new[] { "Seller_ID" });
            AlterColumn("dbo.Sales", "Card_ID", c => c.Int(nullable: false));
            AlterColumn("dbo.Sales", "CardCondition_ID", c => c.Int(nullable: false));
            AlterColumn("dbo.Sales", "CardGrade_ID", c => c.Int(nullable: false));
            AlterColumn("dbo.Sales", "Seller_ID", c => c.Int(nullable: false));
            CreateIndex("dbo.Sales", "Card_ID");
            CreateIndex("dbo.Sales", "CardCondition_ID");
            CreateIndex("dbo.Sales", "CardGrade_ID");
            CreateIndex("dbo.Sales", "Seller_ID");
            AddForeignKey("dbo.Sales", "Seller_ID", "dbo.Users", "ID", cascadeDelete: true);
            AddForeignKey("dbo.Sales", "Card_ID", "dbo.Cards", "ID", cascadeDelete: true);
            AddForeignKey("dbo.Sales", "CardCondition_ID", "dbo.Conditions", "ID", cascadeDelete: true);
            AddForeignKey("dbo.Sales", "CardGrade_ID", "dbo.Grades", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sales", "CardGrade_ID", "dbo.Grades");
            DropForeignKey("dbo.Sales", "CardCondition_ID", "dbo.Conditions");
            DropForeignKey("dbo.Sales", "Card_ID", "dbo.Cards");
            DropForeignKey("dbo.Sales", "Seller_ID", "dbo.Users");
            DropIndex("dbo.Sales", new[] { "Seller_ID" });
            DropIndex("dbo.Sales", new[] { "CardGrade_ID" });
            DropIndex("dbo.Sales", new[] { "CardCondition_ID" });
            DropIndex("dbo.Sales", new[] { "Card_ID" });
            AlterColumn("dbo.Sales", "Seller_ID", c => c.Int());
            AlterColumn("dbo.Sales", "CardGrade_ID", c => c.Int());
            AlterColumn("dbo.Sales", "CardCondition_ID", c => c.Int());
            AlterColumn("dbo.Sales", "Card_ID", c => c.Int());
            CreateIndex("dbo.Sales", "Seller_ID");
            CreateIndex("dbo.Sales", "CardGrade_ID");
            CreateIndex("dbo.Sales", "CardCondition_ID");
            CreateIndex("dbo.Sales", "Card_ID");
            AddForeignKey("dbo.Sales", "CardGrade_ID", "dbo.Grades", "ID");
            AddForeignKey("dbo.Sales", "CardCondition_ID", "dbo.Conditions", "ID");
            AddForeignKey("dbo.Sales", "Card_ID", "dbo.Cards", "ID");
            AddForeignKey("dbo.Sales", "Seller_ID", "dbo.Users", "ID");
        }
    }
}
