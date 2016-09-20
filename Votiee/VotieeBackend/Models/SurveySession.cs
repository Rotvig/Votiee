using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VotieeBackend.Models
{
    public class SurveySession
    {
        [Key]
        public int SurveySessionId { get; set; }
              
        public string SessionCode { get; set; }

        public bool SurveyActive { get; set; }

        public int CurrentSurveyItem { get; set; }

        public bool SurveyItemActive { get; set; }

        public bool VotingOpen { get; set; }

        public bool ShowResults { get; set; }


        public virtual SurveyArchived Survey { get; set; }

    }
}