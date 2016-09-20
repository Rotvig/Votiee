using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Ajax.Utilities;

namespace VotieeBackend.Models
{
    public interface ISurveyItem
    {
        int SurveyItemId { get; set; }

        string QuestionText { get; set; }

        int Order { get; set; }

        //public virtual ICollection<IAnswer> Answers { get; set; }

        //public virtual ISurvey Survey { get; set; }
    }

    public class SurveyItemEditable : ISurveyItem
    {

        public SurveyItemEditable()
        {
            Answers = new List<AnswerEditable>();
        }

        [Key]
        public int SurveyItemId { get; set; }

        public string QuestionText { get; set; }

        public int Order { get; set; }

        public virtual ICollection<AnswerEditable> Answers { get; set; }

        public virtual SurveyEditable Survey { get; set; }

        #region Methods

        //Create a copy of the this editable object as an archived object
        public SurveyItemArchived Archive()
        {
            var surveyItemArchived = new SurveyItemArchived()
            {
                QuestionText = QuestionText,
                Order = Order
            };

            foreach (var answer in 
                Answers.Where(answer => !answer.IsEmpty()))
            {
                //Add all non empty answers
                surveyItemArchived.Answers.Add(answer.Archive());
            }

            return surveyItemArchived;
        }

        public bool IsEmpty()
        {
            //Return true if QuestionText and all AnswerTexts are empty
            return QuestionText.IsNullOrWhiteSpace() 
                && Answers.All(answer => answer.IsEmpty());
        }

        #endregion
    }

    public class SurveyItemArchived : ISurveyItem
    {

        public SurveyItemArchived()
        {
            Answers = new List<AnswerArchived>();
            Votes = new List<Vote>();
        }

        [Key]
        public int SurveyItemId { get; set; }

        public string QuestionText { get; set; }

        public int Order { get; set; }

        public virtual ICollection<Vote> Votes { get; set; }
        
        public virtual ICollection<AnswerArchived> Answers { get; set; }

        public virtual SurveyArchived Survey { get; set; }
    }
}