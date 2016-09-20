using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using VotieeBackend.Controllers;
using VotieeBackend.Models;
using VotieeBackendTests.utils;
using Xunit;

namespace VotieeBackendTests.controllers
{
    public class SurveyEditableModelTests : TestExtentions
    {

        [Fact]
        public void ArchiveSurvey()
        {
            //Arrange
            var user = new User()
            {
                AspUserId = "testId"
            };

            var surveyEditable = new SurveyEditable()
            {
                Name = "Sup Sup Survey",
                User = user,
                SurveyItems = new List<SurveyItemEditable>()
                {
                    new SurveyItemEditable()
                    {
                        QuestionText = "Question 1",
                        Order = 1
                    },
                    new SurveyItemEditable()
                    {
                        QuestionText = "Question 2",
                        Order = 2
                    }
                }
            };

            //Act
            var result = surveyEditable.Archive();

            //Assert
            result.Name.ShouldBe("Sup Sup Survey");
            result.User.AspUserId.ShouldBe("testId");
            result.SurveyTemplate.ShouldBe(surveyEditable);
            result.SurveyItems.Count.ShouldBe(2);
        }

        [Fact]
        public void ArchiveSurveyWithAnEmptySurveyItem()
        {
            //Arrange
            var user = new User()
            {
                AspUserId = "testId"
            };

            var surveyEditable = new SurveyEditable()
            {
                Name = "Sup Sup Survey",
                User = user,
                SurveyItems = new List<SurveyItemEditable>()
                {
                    new SurveyItemEditable()
                    {
                        QuestionText = "Question 1",
                        Order = 1
                    },
                    new SurveyItemEditable()
                    {
                        QuestionText = "",
                        Order = 2
                    }
                }
            };

            //Act
            var result = surveyEditable.Archive();

            //Assert
            result.Name.ShouldBe("Sup Sup Survey");
            result.User.AspUserId.ShouldBe("testId");
            result.SurveyTemplate.ShouldBe(surveyEditable);
            result.SurveyItems.Count.ShouldBe(1);
        }

        [Fact]
        public void SurveyItemIsEmptyReturnsFalseWhenNotEmpty()
        {

            var surveyItemEditable = new SurveyItemEditable()
            {
                QuestionText = "Question 1",
                Order = 1
            };

            //Act
            var result = surveyItemEditable.IsEmpty();

            //Assert
            result.ShouldBe(false);
        }

        [Fact]
        public void SurveyItemIsEmptyReturnsTrueWhenEmpty()
        {

            var surveyItemEditable = new SurveyItemEditable()
            {
                QuestionText = "",
                Order = 1
            };

            //Act
            var result = surveyItemEditable.IsEmpty();

            //Assert
            result.ShouldBe(true);
        }

        [Fact]
        public void ArchiveSurveyItem()
        {
            //Arrange
            var surveyItemEditable = new SurveyItemEditable()
            {
                QuestionText = "Question 1",
                Order = 1,
                Answers = new List<AnswerEditable>()
                {
                    new AnswerEditable()
                    {
                        AnswerText = "Answer 1",
                        Order = 1
                    },
                    new AnswerEditable()
                    {
                        AnswerText = "Answer 2",
                        Order = 2
                    }
                }
            };

            //Act
            var result = surveyItemEditable.Archive();

            //Assert
            result.QuestionText.ShouldBe("Question 1");
            result.Order.ShouldBe(1);
            result.Answers.Count.ShouldBe(2);
        }

        [Fact]
        public void ArchiveSurveyItemWithAnEmptyAnswer()
        {
            //Arrange
            var surveyItemEditable = new SurveyItemEditable()
            {
                QuestionText = "Question 1",
                Order = 1,
                Answers = new List<AnswerEditable>()
                {
                    new AnswerEditable()
                    {
                        AnswerText = "",
                        Order = 1
                    },
                    new AnswerEditable()
                    {
                        AnswerText = "Answer 2",
                        Order = 2
                    }
                }
            };

            //Act
            var result = surveyItemEditable.Archive();

            //Assert
            result.QuestionText.ShouldBe("Question 1");
            result.Order.ShouldBe(1);
            result.Answers.Count.ShouldBe(1);
        }

        [Fact]
        public void AnswerIsEmptyReturnsFalseWhenNotEmpty()
        {
            //Arrange
            var answerEditable = new AnswerEditable()
            {
                AnswerText = "Answer 1",
                Order = 1
            };

            //Act
            var result = answerEditable.IsEmpty();

            //Assert
            result.ShouldBe(false);
        }

        [Fact]
        public void AnswerIsEmptyReturnsTrueWhenEmpty()
        {
            //Arrange
            var answerEditable = new AnswerEditable()
            {
                AnswerText = null,
                Order = 1
            };

            //Act
            var result = answerEditable.IsEmpty();

            //Assert
            result.ShouldBe(true);
        }

        [Fact]
        public void ArchiveAnswer()
        {
            //Arrange
            var answerEditable = new AnswerEditable()
            {
                AnswerText = "Answer 1",
                Order = 1
            };

            //Act
            var result = answerEditable.Archive();

            //Assert
            result.AnswerText.ShouldBe("Answer 1");
            result.Order.ShouldBe(1);
        }

    }
}
