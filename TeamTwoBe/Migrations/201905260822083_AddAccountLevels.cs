namespace TeamTwoBe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAccountLevels : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO dbo.Conditions(CardCondition) VALUES('Mint')");
            Sql("INSERT INTO dbo.Conditions(CardCondition) VALUES('Near mint')");
            Sql("INSERT INTO dbo.Conditions(CardCondition) VALUES('Excellent')");
            Sql("INSERT INTO dbo.Conditions(CardCondition) VALUES('Good')");
            Sql("INSERT INTO dbo.Conditions(CardCondition) VALUES('Light played')");
            Sql("INSERT INTO dbo.Conditions(CardCondition) VALUES('Played')");
            Sql("INSERT INTO dbo.Conditions(CardCondition) VALUES('Poor')");
            Sql("INSERT INTO dbo.Grades(Grading) VALUES('Ungraded')");
            Sql("INSERT INTO dbo.Grades(Grading) VALUES('PSA1')");
            Sql("INSERT INTO dbo.Grades(Grading) VALUES('PSA2')");
            Sql("INSERT INTO dbo.Grades(Grading) VALUES('PSA3')");
            Sql("INSERT INTO dbo.Grades(Grading) VALUES('PSA4')");
            Sql("INSERT INTO dbo.Grades(Grading) VALUES('PSA5')");
            Sql("INSERT INTO dbo.Grades(Grading) VALUES('PSA6')");
            Sql("INSERT INTO dbo.Grades(Grading) VALUES('PSA7')");
            Sql("INSERT INTO dbo.Grades(Grading) VALUES('PSA8')");
            Sql("INSERT INTO dbo.Grades(Grading) VALUES('PSA9')");
            Sql("INSERT INTO dbo.Grades(Grading) VALUES('PSA10')");
            Sql("INSERT INTO dbo.AccountTypes(AccountLevel) Values('Admin')");
            Sql("INSERT INTO dbo.AccountTypes(AccountLevel) Values('Guest')");
            Sql("INSERT INTO dbo.AccountTypes(AccountLevel) Values('Premium')");
            Sql("INSERT INTO dbo.CardTypes(Name) Values('Yugioh')");
            Sql("INSERT INTO dbo.CardTypes(Name) Values('Pokemon')");
            Sql("INSERT INTO dbo.Users(Password, City, Email, Phone, UserLevel_ID, IsDeleted, FirstName, LastName, Username, IsLocked, DisplayPicture, cookie ) Values('1','','','',1,0,'','','',0,'','')");
            Sql("INSERT INTO dbo.Cards(Name, Cardtype_ID, Rarity, image_url, low, average, high, print_tag, apiID) Values('',1,'','https://www.colorhexa.com/101010.png',0,0,0,'','')");
            Sql("INSERT INTO dbo.Sales(Price, CardCondition_ID, CardGrade_ID,ForAuction, Card_ID, IsSold, Buyer_ID, Seller_ID, UploadDate, IsVerified) VALUES(0,1,1,0,1,1,NULL,1,'5/5/2019 12:00:00 AM',0)");
        }

        public override void Down()
        {
            Sql("DELETE FROM dbo.Sales WHERE ID = 1");
            Sql("DELETE FROM dbo.Cards WHERE ID = 1");
            Sql("DELETE FROM dbo.Users WHERE Password = '1' ");
            Sql("DELETE FROM dbo.CardTypes WHERE ID = 2 ");
            Sql("DELETE FROM dbo.CardTypes WHERE ID = 1 ");
            Sql("DELETE FROM dbo.AccountTypes WHERE AccountLevel = 'Admin' ");
            Sql("DELETE FROM dbo.AccountTypes WHERE AccountLevel = 'Guest' ");
            Sql("DELETE FROM dbo.AccountTypes WHERE AccountLevel = 'Premium' ");
            Sql("DELETE FROM dbo.Grades WHERE ID < 12");
            Sql("DELETE FROM dbo.Conditions WHERE ID < 8");
        }
    }
}
