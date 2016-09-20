using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VotieeBackend.ViewModels
{
    public class SurveyItemSessionViewModel : SurveyItemStatisticsViewModel
    {
        public bool Active { get; set; }
        public bool ShowResults { get; set; }
        public bool VotingOpen { get; set; }
        public int VoteCount { get; set; }
    }
}