using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VotieeBackend.ViewModels
{
    public class SurveyItemStatisticsViewModel
    {
        public SurveyItemStatisticsViewModel()
        {
            AnswerResults = new List<AnswerResultViewModel>();
        }

        public string QuestionText { get; set; }
        public int Order { get; set; }

        public ICollection<AnswerResultViewModel> AnswerResults { get; set; }
    }
}