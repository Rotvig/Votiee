using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VotieeBackend.ViewModels
{
    public class AnswerResultViewModel
    {
        public string AnswerText { get; set; }
        public int Order { get; set; }
        public int VoteCount { get; set; }
        public bool Marked { get; set; }
    }
}