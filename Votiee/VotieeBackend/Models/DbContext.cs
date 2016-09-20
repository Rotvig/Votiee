using System.Data.Common;
using System.Data.Entity;

namespace VotieeBackend.Models
{
    public class VotieeDbContext : DbContext
    {
        public VotieeDbContext() : base("name=DefaultConnection")
        {
            
        }
        public VotieeDbContext(string connectionString) : base("name=" + connectionString)
        {
            if (!Database.Exists())
                Database.Create();
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<VotieeDbContext>());
        }

        public VotieeDbContext(DbConnection connection) : base(connection, true)
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<SurveyEditable> SurveyEditables { get; set; }
        public DbSet<SurveyArchived> SurveyArchiveds { get; set; }
        public DbSet<SurveyItemEditable> SurveyItemEditables { get; set; }
        public DbSet<SurveyItemArchived> SurveyItemArchiveds { get; set; }
        public DbSet<AnswerEditable> AnswerEditables { get; set; }
        public DbSet<AnswerArchived> AnswerArchiveds { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<SurveySession> SurveySessions { get; set; }

    }

}