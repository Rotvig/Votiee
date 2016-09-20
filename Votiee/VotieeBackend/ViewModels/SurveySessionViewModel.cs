namespace VotieeBackend.ViewModels
{
    public class SurveySessionViewModel
    {
        public string SessionCode { get; set; }

        public int CurrentSurveyItem { get; set; }

        public bool SurveyItemActive { get; set; }

        public bool VotingOpen { get; set; }

        public bool ShowResults { get; set; }

        public bool SurveyActive { get; set; }

        public SurveyArchivedSessionViewModel Survey { get; set; }
    }
}