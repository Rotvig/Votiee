namespace VotieeBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class surveyCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SurveyEditables", "SurveyCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SurveyEditables", "SurveyCode");
        }
    }
}
