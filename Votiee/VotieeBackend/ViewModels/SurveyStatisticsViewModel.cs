using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VotieeBackend.ViewModels
{
    public class SurveyStatisticsViewModel
    {

        public SurveyStatisticsViewModel()
        {
            SurveyItems = new List<SurveyItemStatisticsViewModel>();
        }

        public string Name { get; set; }
        public string TemplateName { get; set; }
        public DateTime? ArchiveDate { get; set; }

        public virtual List<SurveyItemStatisticsViewModel> SurveyItems { get; set; }
    }
}