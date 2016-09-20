using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;

namespace VotieeBackend.Models
{

    public interface ISurvey
    {

        int SurveyId { get; set; }

        string Name { get; set; }

        //public virtual ICollection<ISurveyItem> SurveyItems { get; set; }

        User User { get; set; } //public virtual

    }

    public class SurveyEditable : ISurvey
    {
        public SurveyEditable()
        {
            SurveyItems = new List<SurveyItemEditable>();
        }

        [Key]
        public int SurveyId { get; set; }

        public string SurveyCode { get; set; }

        public string Name { get; set; }

        public bool Deleted { get; set; }

        public virtual ICollection<SurveyItemEditable> SurveyItems { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<SurveyArchived> ArchivedCopies { get; set; } 

        #region Methods

        //Create a copy of the this editable object as an archived object
        public SurveyArchived Archive()
        {
            var surveyArchived = new SurveyArchived()
            {
                Name = Name,
                User = User,
                SurveyTemplate = this,
                ArchiveDate = DateTime.Now
            };

            //Archive all non empty surveyItems
            foreach (var surveyItem in 
                SurveyItems.Where(surveyItem => !surveyItem.IsEmpty()))
            {
                surveyArchived.SurveyItems.Add(surveyItem.Archive());
            }

            return surveyArchived;
        }

        public bool IsEmpty()
        {
            //Return true if QuestionText and all AnswerTexts are empty
            return (SurveyItems.Count(surveyItem => !surveyItem.IsEmpty()) == 0);
        }

        #endregion
    }

    public class SurveyArchived : ISurvey
    {
        public SurveyArchived()
        {
            SurveyItems = new List<SurveyItemArchived>();
        }

        [Key]
        public int SurveyId { get; set; }

        public string Name { get; set; }

        public DateTime? ArchiveDate { get; set; }

        public bool Deleted { get; set; }

        public virtual ICollection<SurveyItemArchived> SurveyItems { get; set; }

        public virtual User User { get; set; }

        public virtual SurveyEditable SurveyTemplate { get; set; }
    }
}