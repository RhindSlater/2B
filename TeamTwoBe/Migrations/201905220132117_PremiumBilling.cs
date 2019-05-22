namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PremiumBilling : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PremiumBillings",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        NextBillingDate = c.DateTime(nullable: false),
                        Amount = c.Double(nullable: false),
                        Member_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.Member_ID)
                .Index(t => t.Member_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PremiumBillings", "Member_ID", "dbo.Users");
            DropIndex("dbo.PremiumBillings", new[] { "Member_ID" });
            DropTable("dbo.PremiumBillings");
        }
    }
}
