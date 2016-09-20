using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VotieeBackend.ViewModels
{
    public class ResultsViewModel
    {
        public ResultsViewModel()
        {
            AnswerResults = new List<AnswerResultViewModel>();
        }

        public int SurveyItemId { get; set; }

        public string QuestionText { get; set; }

        public bool LastSurveyItem { get; set; }

        public ICollection<AnswerResultViewModel> AnswerResults { get; set; }
    }
}