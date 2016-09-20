using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Ajax.Utilities;

namespace VotieeBackend.Models
{
    public interface IAnswer
    {
        int AnswerId { get; set; }

        string AnswerText { get; set; }

        int Order { get; set; }

        //public virtual ISurveyItem SurveyItem { get; set; }
    }


    public class AnswerEditable : IAnswer
    {
        [Key]
        public int AnswerId { get; set; }

        public string AnswerText { get; set; }

        public int Order { get; set; }

        public virtual SurveyItemEditable SurveyItem { get; set; }

        #region Methods

        //Create a copy of the this editable object as an archived object
        public AnswerArchived Archive()
        {
            return new AnswerArchived
            {
                AnswerText = AnswerText,
                Order = Order
            };
        }

        public bool IsEmpty()
        {
            //Check if Answer is empty
            return AnswerText.IsNullOrWhiteSpace();
        }

        #endregion
    }

    public class AnswerArchived : IAnswer
    {
        [Key]
        public int AnswerId { get; set; }

        public string AnswerText { get; set; }

        public int Order { get; set; }

        public virtual SurveyItemArchived SurveyItem { get; set; }
    }
}