using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VotieeBackend.ViewModels
{
    public class VotingViewModel
    {
        public VotingViewModel()
        {
            AnswerResults = new List<AnswerResultViewModel>();
        }

        public bool SurveyActive { get; set; }
        public bool SurveyItemActive { get; set; }
        public bool Answered { get; set; }
        public int SelectedAnswer { get; set; }
        public bool VotingOpen { get; set; }
        public bool ShowResults { get; set; }
        public SurveyItemViewModel SurveyItem { get; set; }
        public ICollection<AnswerResultViewModel> AnswerResults { get; set; }
    }
}