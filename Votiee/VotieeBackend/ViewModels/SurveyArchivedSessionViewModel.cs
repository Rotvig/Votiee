using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VotieeBackend.ViewModels
{
    public class SurveyArchivedSessionViewModel
    {
        public SurveyArchivedSessionViewModel()
        {
            SurveyItems = new List<SurveyItemSessionViewModel>();
        }

        public string Name { get; set; }
        public string TemplateName { get; set; }
        public DateTime? ArchiveDate { get; set; }

        public virtual List<SurveyItemSessionViewModel> SurveyItems { get; set; }
    }
}