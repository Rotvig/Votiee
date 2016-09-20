using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Http;
using FakeItEasy;
using Microsoft.AspNet.Identity;
using Shouldly;
using VotieeBackend.Controllers;
using VotieeBackend.Models;
using VotieeBackend.ViewModels;
using VotieeBackendTests.utils;
using Xunit;

namespace VotieeBackendTests.controllers
{
    public class StatisticsControllerTests : TestExtentions
    {
        private readonly StatisticsController controller;

        public StatisticsControllerTests()
        {
            var claim = new Claim("TestUnit", "MyUserId");
            var identity = A.Fake<ClaimsIdentity>();
            A.CallTo(() => identity.FindFirst(A<string>._)).Returns(claim);
            var user = A.Fake<IPrincipal>();
            A.CallTo(() => user.Identity).Returns(identity);

            controller = new StatisticsController(dbContext, user) { Request = new HttpRequestMessage() };
            controller.Request.SetConfiguration(new HttpConfiguration());
        }

        [Fact]
        public void LoadSurveyStatistics()
        {
            //Arrange
            var surveyTemplateId = CreateSurveyEditable(name: "MySurveyTemplate");
            var surveyId = CreateSurveyArchived(name: "MySurvey", template: dbContext.SurveyEditables.Find(surveyTemplateId));
            CreateSurveyItemArchived("MyQuestion");
            CreateAnswerArchived("MyAnswer");
            CreateUser(aspUserId: "MyUserId", surveyArchiveds: new List<SurveyArchived>()
            {
                dbContext.SurveyArchiveds.Find(surveyId)
            });

            //Act
            var result = controller.LoadSurveyStatistics(surveyId);

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
            SurveyStatisticsViewModel viewModel;
            result.TryGetContentValue(out viewModel).ShouldBe(true);
            viewModel.Name.ShouldBe("MySurvey");
            viewModel.TemplateName.ShouldBe("MySurveyTemplate");
            viewModel.SurveyItems.First().QuestionText.ShouldBe("MyQuestion");
            viewModel.SurveyItems.First().AnswerResults.First().AnswerText.ShouldBe("MyAnswer");
        }

        [Fact]
        public void LoadSurveyStatisticsReturnsBadRequestWhenTryingToLoadAnotherUsersSurvey()
        {
            //Arrange
            var surveyTemplateId = CreateSurveyEditable(name: "MySurveyTemplate");
            var surveyId = CreateSurveyArchived(name: "MySurvey", template: dbContext.SurveyEditables.Find(surveyTemplateId));
            CreateSurveyItemArchived("MyQuestion");
            CreateAnswerArchived("MyAnswer");
            CreateUser(aspUserId: "OtherUserId", surveyArchiveds: new List<SurveyArchived>()
            {
                dbContext.SurveyArchiveds.Find(surveyId)
            });
            CreateUser(aspUserId: "MyUserId", surveyArchiveds: new List<SurveyArchived>());

            //Act
            var result = controller.LoadSurveyStatistics(surveyId);

            //Assert
            result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public void LoadSurveyStatisticsReturnsBadRequestWhenTryingToLoadNonExistingSurvey()
        {
            //Arrange
            var surveyTemplateId = CreateSurveyEditable(name: "MySurveyTemplate");
            var surveyId = CreateSurveyArchived(name: "MySurvey", template: dbContext.SurveyEditables.Find(surveyTemplateId));
            CreateSurveyItemArchived("MyQuestion");
            CreateAnswerArchived("MyAnswer");
            CreateUser(aspUserId: "MyUserId", surveyArchiveds: new List<SurveyArchived>());

            //Act
            var result = controller.LoadSurveyStatistics(surveyId+60);

            //Assert
            result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public void LoadSurveyStatisticsReturnsBadRequestWhenNotLoggedIn()
        {
            //Arrange
            var identity = A.Fake<ClaimsIdentity>();
            A.CallTo(() => identity.FindFirst(A<string>._)).Returns(null);
            var user = A.Fake<IPrincipal>();
            A.CallTo(() => user.Identity).Returns(identity);

            var failController = new StatisticsController(dbContext, user) { Request = new HttpRequestMessage() };
            failController.Request.SetConfiguration(new HttpConfiguration());

            //Act
            var result = failController.LoadSurveyStatistics(60);

            //Assert
            result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }
    }
}
