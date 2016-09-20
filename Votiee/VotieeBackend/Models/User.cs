using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VotieeBackend.Models
{
    public class User
    {
        public User()
        {
            SurveyEditables = new List<SurveyEditable>();
            SurveyArchiveds = new List<SurveyArchived>();
        }

        [Key]
        public int UserId { get; set; }

        public string AspUserId { get; set; }

        public virtual ICollection<SurveyEditable> SurveyEditables { get; set; } 

        public virtual ICollection<SurveyArchived> SurveyArchiveds { get; set; }

    }
}