using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VotieeBackend.ViewModels
{
    public class SurveyItemViewModel
    {
        public int SurveyItemId { get; set; }
        public int Order { get; set; }
        public string QuestionText { get; set; }
        public List<AnswerViewModel> Answers { get; set; }
        public bool Last { get; set; }
    }
}