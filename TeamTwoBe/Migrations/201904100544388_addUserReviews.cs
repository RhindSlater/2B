namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUserReviews : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserReviews",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ReviewGiven = c.String(),
                        StarRating = c.Int(nullable: false),
                        CardReviewed_ID = c.Int(),
                        Reviewee_ID = c.Int(),
                        Reviewer_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Sales", t => t.CardReviewed_ID)
                .ForeignKey("dbo.Users", t => t.Reviewee_ID)
                .ForeignKey("dbo.Users", t => t.Reviewer_ID)
                .Index(t => t.CardReviewed_ID)
                .Index(t => t.Reviewee_ID)
                .Index(t => t.Reviewer_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserReviews", "Reviewer_ID", "dbo.Users");
            DropForeignKey("dbo.UserReviews", "Reviewee_ID", "dbo.Users");
            DropForeignKey("dbo.UserReviews", "CardReviewed_ID", "dbo.Sales");
            DropIndex("dbo.UserReviews", new[] { "Reviewer_ID" });
            DropIndex("dbo.UserReviews", new[] { "Reviewee_ID" });
            DropIndex("dbo.UserReviews", new[] { "CardReviewed_ID" });
            DropTable("dbo.UserReviews");
        }
    }
}
