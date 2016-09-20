using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Http;
using FakeItEasy;
using Shouldly;
using VotieeBackend.Controllers;
using VotieeBackend.Models;
using VotieeBackendTests.utils;
using Xunit;

namespace VotieeBackendTests.controllers
{
    public class ServiceControllerTests : TestExtentions
    {
        private ServiceController controller;

        public ServiceControllerTests()
        {
            var claim = new Claim("TestUnit", "MyUserId");
            var identity = A.Fake<ClaimsIdentity>();
            A.CallTo(() => identity.FindFirst(A<string>._)).Returns(claim);
            var user = A.Fake<IPrincipal>();
            A.CallTo(() => user.Identity).Returns(identity);

            controller = new ServiceController(dbContext, user) { Request = new HttpRequestMessage() };
            controller.Request.SetConfiguration(new HttpConfiguration());
        }

        [Fact]
        public void CreateSurveySession()
        {
            //Arrange
            var surveyEditableId = CreateSurveyEditable(name: "My Best Survey", surveyCode: "MyCode");
            CreateSurveyItemEditable(questionText: "What?");
            CreateAnswerEditable(answerText: "Cow");

            //Act
            var result = controller.CreateSurveySession("MyCode");

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
            string sessionCode;
            result.TryGetContentValue(out sessionCode).ShouldBe(true);
            var surveySession = dbContext.SurveySessions.Single(x => x.SessionCode == sessionCode);
            surveySession.Survey.SurveyId.ShouldBe(surveyEditableId);
            surveySession.Survey.SurveyItems.First().QuestionText.ShouldBe("What?");
            surveySession.Survey.SurveyItems.First().Answers.First().AnswerText.ShouldBe("Cow");
        }

        [Fact]
        public void CreateSurveySessionWithUser()
        {
            //Arrange
            var surveyEditableId = CreateSurveyEditable(name: "My Best Survey", surveyCode: "MyCode");
            CreateSurveyItemEditable(questionText: "What?");
            CreateAnswerEditable(answerText: "Cow");
            var userId = CreateUser("MyUserId", new List<SurveyEditable>()
            {
                dbContext.SurveyEditables.Find(surveyEditableId)
            });

            //Act
            var result = controller.CreateSurveySession("MyCode");

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
            string sessionCode;
            result.TryGetContentValue(out sessionCode).ShouldBe(true);
            var surveySession = dbContext.SurveySessions.Single(x => x.SessionCode == sessionCode);
            surveySession.Survey.SurveyId.ShouldBe(surveyEditableId);
            surveySession.Survey.SurveyItems.First().QuestionText.ShouldBe("What?");
            surveySession.Survey.SurveyItems.First().Answers.First().AnswerText.ShouldBe("Cow");
        }

        [Fact]
        public void CreateSurveySessionWithUserReturnsBadRequestWhenSurveySessionDoesNotBelongToUser()
        {
            //Arrange
            var surveyEditableId = CreateSurveyEditable(name: "My Best Survey", surveyCode: "MyCode");
            CreateSurveyItemEditable(questionText: "What?");
            CreateAnswerEditable(answerText: "Cow");
            var userId = CreateUser("OtherUserId", new List<SurveyEditable>()
            {
                dbContext.SurveyEditables.Find(surveyEditableId)
            });

            //Act
            var result = controller.CreateSurveySession("MyCode");

            //Assert
            result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public void CreateSurveySessionReturnsBadRequestWhenSurveySessionDoesNotExist()
        {
            //Arrange

            //Act
            var result = controller.CreateSurveySession("NotExisting");

            //Assert
            result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public void CanCreateSurvey()
        {
            //Arrange
            CreateUser("MyUserId");

            //Act
            var result = controller.CreateNewSurvey();

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
            var survey = dbContext.SurveyEditables.Find(1);
            survey.SurveyItems.Count.ShouldBe(1);
        }

        [Fact]
        public void ReactivateSurveySessionWithExistingSession()
        {
            //Arrange
            var surveyArchivedId = CreateSurveyArchived(name: "My Best Survey");
            CreateSurveySession(sessionCode: "TestCode");

            //Act
            var result = controller.ReactivateSurveySession(surveyArchivedId);

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
            string sessionCode;
            result.TryGetContentValue(out sessionCode).ShouldBe(true);
            sessionCode.ShouldBe("TestCode");
        }

        [Fact]
        public void ReactivateSurveySessionWithNoExistingSession()
        {
            //Arrange
            var surveyArchivedId = CreateSurveyArchived(name: "My Best Survey");

            //Act
            var result = controller.ReactivateSurveySession(surveyArchivedId);

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
            string sessionCode;
            result.TryGetContentValue(out sessionCode).ShouldBe(true);
        }

        [Fact]
        public void ReactivateSurveySessionReturnsBadRequestWhenSurveyArchivedDoesntExist()
        {
            //Arrange

            //Act
            var result = controller.ReactivateSurveySession(60);

            //Assert
            result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public void CanCheckIfSurveyItemExists()
        {
            //Arrange
            CreateSurveyArchived();
            CreateSurveySession(sessionCode: "NISSE");
            CreateSurveyItemArchived();

            //Act
            var result = controller.CheckIfSessionExists("Nisse");

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
        }

        [Fact]
        public void CanNotFindSurveyItemReturnsNotFound()
        {
            //Act
            var result = controller.CheckIfSessionExists("Nisse");

            //Assert
            result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public void CanNotFindSurveyItemWhenSurveySessionIsNotActiveReturnsBadRequest()
        {
            //Arrange
            CreateSurveyArchived();
            CreateSurveySession(sessionCode: "NISSE", surveyActive: false);

            //Act
            var result = controller.CheckIfSessionExists("NISSE");

            //Assert
            result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public void CheckIfPresenterSessionExistsWithExistingSession()
        {
            //Arrange
            CreateSurveySession(sessionCode: "TestCode");

            //Act
            var result = controller.CheckIfPresenterSessionExists("TestCode");

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
        }

        [Fact]
        public void CheckIfPresenterSessionExistsWithNoExistingSession()
        {
            //Arrange

            //Act
            var result = controller.CheckIfPresenterSessionExists("TestCode");

            //Assert
            result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public void CheckIfSurveyEditableExistsWithExistingSurvey()
        {
            //Arrange
            CreateSurveyEditable(surveyCode: "ThisCode");

            //Act
            var result = controller.CheckIfSurveyEditableExists("ThisCode");

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
        }

        [Fact]
        public void CheckIfSurveyEditableExistsReturnsNotFoundWithNoExistingSurvey()
        {
            //Arrange
            var surveyEditableId = CreateSurveyEditable();

            //Act
            var result = controller.CheckIfSurveyEditableExists("Other");

            //Assert
            result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public void CheckIfSurveyEditableExistsReturnsBadRequestWhenSurveyHasOwner()
        {
            //Arrange
            var surveyEditableId = CreateSurveyEditable(surveyCode: "LeSurvey");
            CreateUser(surveyEditables: 
                new List<SurveyEditable>() {dbContext.SurveyEditables.Find(surveyEditableId)});

            //Act
            var result = controller.CheckIfSurveyEditableExists("LeSurvey");

            //Assert
            result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

    }
}
