namespace VotieeBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteOnSurvey : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SurveyArchiveds", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.SurveyEditables", "Deleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SurveyEditables", "Deleted");
            DropColumn("dbo.SurveyArchiveds", "Deleted");
        }
    }
}
