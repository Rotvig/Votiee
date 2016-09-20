using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Shouldly;
using VotieeBackend.Controllers;
using VotieeBackend.ViewModels;
using VotieeBackendTests.utils;
using Xunit;

namespace VotieeBackendTests.controllers
{
    public class SurveySessionControllerTests : TestExtentions
    {
        private SurveySessionController controller;

        public SurveySessionControllerTests()
        {
            controller = new SurveySessionController(dbContext) { Request = new HttpRequestMessage() };
            controller.Request.SetConfiguration(new HttpConfiguration());
        }

        [Fact]
        public void LoadSurveySession()
        {
            //Arrange
            var editableId = CreateSurveyEditable();
            CreateSurveyArchived(template: dbContext.SurveyEditables.Find(editableId));
            CreateSurveyItemArchived(questionText: "What?");
            CreateAnswerArchived(answerText: "Cow");
            CreateSurveySession(sessionCode: "John12");

            //Act
            var result = controller.LoadSurveySession("John12");

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
            SurveySessionViewModel resultsViewModel;
            result.TryGetContentValue(out resultsViewModel).ShouldBe(true);
            resultsViewModel.Survey.SurveyItems.Single().QuestionText.ShouldBe("What?");
            resultsViewModel.Survey.SurveyItems.Single().AnswerResults.Single().AnswerText.ShouldBe("Cow");
        }

        [Fact]
        public void LoadSurveySessionReturnsNotFoundWhenSurveyDoesNotExist()
        {
            //Arrange

            //Act
            var result = controller.LoadSurveySession("John12");

            //Assert
            result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public void LoadSurveySessionReturnsNotFoundWhenDifferentUserThanHostIsRequestingSurveySession()
        {
            //Arrange
            var editableId = CreateSurveyEditable();
            CreateUser();
            var userId = CreateUser();
            CreateSurveyArchived(user: dbContext.Users.Find(userId), template: dbContext.SurveyEditables.Find(editableId));
            CreateSurveySession(sessionCode: "John12");

            //Act
            var result = controller.LoadSurveySession("John12");

            //Assert
            result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public void CanToggleSurveyActive()
        {
            //Arrange
            var editableId = CreateSurveyEditable();
            CreateSurveyArchived(template: dbContext.SurveyEditables.Find(editableId));
            var surveySessionId  = CreateSurveySession(sessionCode: "John12", surveyActive: false);

            //Act
            var result = controller.ToggleSurveyActive("John12");

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
            dbContext.SurveySessions.Find(surveySessionId).SurveyActive.ShouldBe(true);
        }

        [Fact]
        public void CanChangeActiveSurveyItem()
        {
            //Arrange
            var editableId = CreateSurveyEditable();
            CreateSurveyArchived(template: dbContext.SurveyEditables.Find(editableId));
            CreateSurveyItemArchived(order: 1);
            CreateSurveyItemArchived(order: 2);
            var surveySessionId = CreateSurveySession(sessionCode: "JOHN12", currentSurveyItem: 1, surveyItemActive: false);

            //Act
            var result = controller.ChangeActiveSurveyItem(new SurveySessionController.SurveyItemData()
            {
                Order = 2,
                SessionCode = "John12"
            });

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
            var surveySession = dbContext.SurveySessions.Find(surveySessionId);
            surveySession.CurrentSurveyItem.ShouldBe(2);
            surveySession.SurveyItemActive.ShouldBe(true);
        }

        [Fact]
        public void CanToggleActiveSurveyItemChange()
        {
            //Arrange
            var editableId = CreateSurveyEditable();
            CreateSurveyArchived(template: dbContext.SurveyEditables.Find(editableId));
            CreateSurveyItemArchived(order: 1);
            var surveySessionId = CreateSurveySession(sessionCode: "JOHN12", currentSurveyItem: 1, surveyItemActive: true, showResults: true);

            //Act
            var result = controller.ChangeActiveSurveyItem(new SurveySessionController.SurveyItemData()
            {
                Order = 1,
                SessionCode = "John12"
            });

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
            var surveySession = dbContext.SurveySessions.Find(surveySessionId);
            surveySession.CurrentSurveyItem.ShouldBe(1);
            surveySession.SurveyItemActive.ShouldBe(false);
            surveySession.ShowResults.ShouldBe(false);
        }

        [Fact]
        public void CanToggleVotingOpen()
        {
            //Arrange
            var editableId = CreateSurveyEditable();
            CreateSurveyArchived(template: dbContext.SurveyEditables.Find(editableId));
            CreateSurveyItemArchived(order: 1);
            var surveySessionId = CreateSurveySession(sessionCode: "JOHN12", votingOpen: true);

            //Act
            var result = controller.ToggleVotingOpen(new SurveySessionController.SurveyItemData
            {
                Order = 1,
                SessionCode = "John12"
            });

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
            dbContext.SurveySessions.Find(surveySessionId).VotingOpen.ShouldBe(false);
        }

        [Fact]
        public void CanToggleShowResults()
        {
            //Arrange
            var editableId = CreateSurveyEditable();
            CreateSurveyArchived(template: dbContext.SurveyEditables.Find(editableId));
            CreateSurveyItemArchived(order: 1);
            var surveySessionId = CreateSurveySession(sessionCode: "JOHN12", showResults: true);

            //Act
            var result = controller.ToggleShowResults(new SurveySessionController.SurveyItemData
            {
                Order = 1,
                SessionCode = "John12"
            });

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
            dbContext.SurveySessions.Find(surveySessionId).ShowResults.ShouldBe(false);
        }

    }
}
