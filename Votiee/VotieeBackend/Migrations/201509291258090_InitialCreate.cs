namespace VotieeBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AnswerArchiveds",
                c => new
                    {
                        AnswerId = c.Int(nullable: false, identity: true),
                        AnswerText = c.String(),
                        Order = c.Int(nullable: false),
                        SurveyItem_SurveyItemId = c.Int(),
                    })
                .PrimaryKey(t => t.AnswerId)
                .ForeignKey("dbo.SurveyItemArchiveds", t => t.SurveyItem_SurveyItemId)
                .Index(t => t.SurveyItem_SurveyItemId);
            
            CreateTable(
                "dbo.SurveyItemArchiveds",
                c => new
                    {
                        SurveyItemId = c.Int(nullable: false, identity: true),
                        QuestionText = c.String(),
                        Order = c.Int(nullable: false),
                        Survey_SurveyId = c.Int(),
                    })
                .PrimaryKey(t => t.SurveyItemId)
                .ForeignKey("dbo.SurveyArchiveds", t => t.Survey_SurveyId)
                .Index(t => t.Survey_SurveyId);
            
            CreateTable(
                "dbo.SurveyArchiveds",
                c => new
                    {
                        SurveyId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ArchiveDate = c.DateTime(),
                        SurveyTemplate_SurveyId = c.Int(),
                        User_UserId = c.Int(),
                    })
                .PrimaryKey(t => t.SurveyId)
                .ForeignKey("dbo.SurveyEditables", t => t.SurveyTemplate_SurveyId)
                .ForeignKey("dbo.Users", t => t.User_UserId)
                .Index(t => t.SurveyTemplate_SurveyId)
                .Index(t => t.User_UserId);
            
            CreateTable(
                "dbo.SurveyEditables",
                c => new
                    {
                        SurveyId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        User_UserId = c.Int(),
                    })
                .PrimaryKey(t => t.SurveyId)
                .ForeignKey("dbo.Users", t => t.User_UserId)
                .Index(t => t.User_UserId);
            
            CreateTable(
                "dbo.SurveyItemEditables",
                c => new
                    {
                        SurveyItemId = c.Int(nullable: false, identity: true),
                        QuestionText = c.String(),
                        Order = c.Int(nullable: false),
                        Survey_SurveyId = c.Int(),
                    })
                .PrimaryKey(t => t.SurveyItemId)
                .ForeignKey("dbo.SurveyEditables", t => t.Survey_SurveyId)
                .Index(t => t.Survey_SurveyId);
            
            CreateTable(
                "dbo.AnswerEditables",
                c => new
                    {
                        AnswerId = c.Int(nullable: false, identity: true),
                        AnswerText = c.String(),
                        Order = c.Int(nullable: false),
                        SurveyItem_SurveyItemId = c.Int(),
                    })
                .PrimaryKey(t => t.AnswerId)
                .ForeignKey("dbo.SurveyItemEditables", t => t.SurveyItem_SurveyItemId)
                .Index(t => t.SurveyItem_SurveyItemId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        AspUserId = c.String(),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.Votes",
                c => new
                    {
                        VoteId = c.Int(nullable: false, identity: true),
                        Answer_AnswerId = c.Int(),
                        SurveyItemArchived_SurveyItemId = c.Int(),
                    })
                .PrimaryKey(t => t.VoteId)
                .ForeignKey("dbo.AnswerArchiveds", t => t.Answer_AnswerId)
                .ForeignKey("dbo.SurveyItemArchiveds", t => t.SurveyItemArchived_SurveyItemId)
                .Index(t => t.Answer_AnswerId)
                .Index(t => t.SurveyItemArchived_SurveyItemId);
            
            CreateTable(
                "dbo.SurveySessions",
                c => new
                    {
                        SurveySessionId = c.Int(nullable: false, identity: true),
                        SessionCode = c.String(),
                        SurveyActive = c.Boolean(nullable: false),
                        SurveyItemActive = c.Boolean(nullable: false),
                        CurrentSurveyItem = c.Int(nullable: false),
                        Survey_SurveyId = c.Int(),
                    })
                .PrimaryKey(t => t.SurveySessionId)
                .ForeignKey("dbo.SurveyArchiveds", t => t.Survey_SurveyId)
                .Index(t => t.Survey_SurveyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SurveySessions", "Survey_SurveyId", "dbo.SurveyArchiveds");
            DropForeignKey("dbo.Votes", "SurveyItemArchived_SurveyItemId", "dbo.SurveyItemArchiveds");
            DropForeignKey("dbo.Votes", "Answer_AnswerId", "dbo.AnswerArchiveds");
            DropForeignKey("dbo.SurveyEditables", "User_UserId", "dbo.Users");
            DropForeignKey("dbo.SurveyArchiveds", "User_UserId", "dbo.Users");
            DropForeignKey("dbo.SurveyItemEditables", "Survey_SurveyId", "dbo.SurveyEditables");
            DropForeignKey("dbo.AnswerEditables", "SurveyItem_SurveyItemId", "dbo.SurveyItemEditables");
            DropForeignKey("dbo.SurveyArchiveds", "SurveyTemplate_SurveyId", "dbo.SurveyEditables");
            DropForeignKey("dbo.SurveyItemArchiveds", "Survey_SurveyId", "dbo.SurveyArchiveds");
            DropForeignKey("dbo.AnswerArchiveds", "SurveyItem_SurveyItemId", "dbo.SurveyItemArchiveds");
            DropIndex("dbo.SurveySessions", new[] { "Survey_SurveyId" });
            DropIndex("dbo.Votes", new[] { "SurveyItemArchived_SurveyItemId" });
            DropIndex("dbo.Votes", new[] { "Answer_AnswerId" });
            DropIndex("dbo.AnswerEditables", new[] { "SurveyItem_SurveyItemId" });
            DropIndex("dbo.SurveyItemEditables", new[] { "Survey_SurveyId" });
            DropIndex("dbo.SurveyEditables", new[] { "User_UserId" });
            DropIndex("dbo.SurveyArchiveds", new[] { "User_UserId" });
            DropIndex("dbo.SurveyArchiveds", new[] { "SurveyTemplate_SurveyId" });
            DropIndex("dbo.SurveyItemArchiveds", new[] { "Survey_SurveyId" });
            DropIndex("dbo.AnswerArchiveds", new[] { "SurveyItem_SurveyItemId" });
            DropTable("dbo.SurveySessions");
            DropTable("dbo.Votes");
            DropTable("dbo.Users");
            DropTable("dbo.AnswerEditables");
            DropTable("dbo.SurveyItemEditables");
            DropTable("dbo.SurveyEditables");
            DropTable("dbo.SurveyArchiveds");
            DropTable("dbo.SurveyItemArchiveds");
            DropTable("dbo.AnswerArchiveds");
        }
    }
}
