using System.Collections.Generic;

namespace VotieeBackend.ViewModels
{
    public class SurveyViewModel
    {
        public int SurveyId { get; set; }
        public string Name { get; set; }
        public List<SurveyItemViewModel> SurveyItems { get; set; }
    }
}