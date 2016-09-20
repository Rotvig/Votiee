using System.Collections.Generic;

namespace VotieeBackend.ViewModels
{
    public class PresenterViewModel
    {
        public PresenterViewModel()
        {
            AnswerResults = new List<AnswerResultViewModel>();
        }

        public bool SurveyActive { get; set; }
        public bool SurveyItemActive { get; set; }
        public int SelectedAnswer { get; set; }
        public bool VotingOpen { get; set; }
        public bool ShowResults { get; set; }
        public SurveyItemViewModel SurveyItem { get; set; }
        public ICollection<AnswerResultViewModel> AnswerResults { get; set; }
    }
}