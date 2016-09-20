namespace VotieeBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedSurveySession : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SurveySessions", "VotingOpen", c => c.Boolean(nullable: false));
            AddColumn("dbo.SurveySessions", "ShowResults", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SurveySessions", "ShowResults");
            DropColumn("dbo.SurveySessions", "VotingOpen");
        }
    }
}
