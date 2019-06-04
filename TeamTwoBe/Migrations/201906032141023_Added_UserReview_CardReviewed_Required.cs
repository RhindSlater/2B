namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_UserReview_CardReviewed_Required : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserReviews", "CardReviewed_ID", "dbo.Sales");
            DropIndex("dbo.UserReviews", new[] { "CardReviewed_ID" });
            AlterColumn("dbo.UserReviews", "CardReviewed_ID", c => c.Int(nullable: false));
            CreateIndex("dbo.UserReviews", "CardReviewed_ID");
            AddForeignKey("dbo.UserReviews", "CardReviewed_ID", "dbo.Sales", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserReviews", "CardReviewed_ID", "dbo.Sales");
            DropIndex("dbo.UserReviews", new[] { "CardReviewed_ID" });
            AlterColumn("dbo.UserReviews", "CardReviewed_ID", c => c.Int());
            CreateIndex("dbo.UserReviews", "CardReviewed_ID");
            AddForeignKey("dbo.UserReviews", "CardReviewed_ID", "dbo.Sales", "ID");
        }
    }
}
