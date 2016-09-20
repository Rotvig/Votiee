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
    public class StatisticsOverviewControllerTests : TestExtentions
    {
        private readonly StatisticsOverviewController controller;

        public StatisticsOverviewControllerTests()
        {
            var claim = new Claim("TestUnit", "MyUserId");
            var identity = A.Fake<ClaimsIdentity>();
            A.CallTo(() => identity.FindFirst(A<string>._)).Returns(claim);
            var user = A.Fake<IPrincipal>();
            A.CallTo(() => user.Identity).Returns(identity);

            controller = new StatisticsOverviewController(dbContext, user) { Request = new HttpRequestMessage() };
            controller.Request.SetConfiguration(new HttpConfiguration());
        }

        [Fact]
        public void CanLoadStatisticsOverview()
        {
            //Arrange
            var user = new User
            {
                AspUserId = "MyUserId"
            };
            var survey1 = new SurveyEditable
            {
                Name = "SomeAweSomeName",
                User = user
            };

            var survey2 = new SurveyEditable
            {
                Name = "JOEYMOY",
                User = user
            };

            dbContext.SurveyEditables.Add(survey1);
            dbContext.SurveyEditables.Add(survey2);
            dbContext.SaveChanges();
            CreateSurveyArchived(name: "Niels", user: user, template: survey1);
            CreateSurveyArchived(name: "Kjeld",  user: user, template: survey1);

            CreateSurveyArchived(name: "Niller", user: user, template: survey2);


            dbContext.SaveChanges();
            
            //Act
            var result = controller.LoadStatisticsOverview();
            
            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
            List<StatisticOverviewSurveys> viewModel;
            result.TryGetContentValue(out viewModel).ShouldBe(true);
            viewModel.Count.ShouldBe(2);

            var surveyResult1 = viewModel.Single(x => x.Name == "SomeAweSomeName");
            surveyResult1.Surveys.Count.ShouldBe(2);
            surveyResult1.Surveys.ShouldContain(x => x.Name == "Niels");
            surveyResult1.Surveys.ShouldContain(x => x.Name == "Kjeld");

            var surveyResult2 = viewModel.Single(x => x.Name == "JOEYMOY");
            surveyResult2.Surveys.Count.ShouldBe(1);
            surveyResult2.Surveys.ShouldContain(x => x.Name == "Niller");
        }

        [Fact]
        public void ReturnsNotFoundWhenUserIsNotFound()
        {
            //Arrange

            //Act
            var result = controller.LoadStatisticsOverview();

            //Assert
            result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public void LoadStatisticsOverviewReturnsNotFoundWhenNotLoggedIn()
        {
            //Arrange
            var identity = A.Fake<ClaimsIdentity>();
            A.CallTo(() => identity.FindFirst(A<string>._)).Returns(null);
            var user = A.Fake<IPrincipal>();
            A.CallTo(() => user.Identity).Returns(identity);

            var failController = new StatisticsOverviewController(dbContext, user) { Request = new HttpRequestMessage() };
            failController.Request.SetConfiguration(new HttpConfiguration());

            //Act
            var result = failController.LoadStatisticsOverview();

            //Assert
            result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public void CanDeleteSurvey()
        {
            //Arrange
            //var editableId = CreateSurveyEditable(name: "SomeAweSomeName");
            var surveyId = CreateSurveyArchived(name: "Niels");
            var userId = CreateUser(aspUserId: "MyUserId", surveyArchiveds: new List<SurveyArchived>()
            {
                dbContext.SurveyArchiveds.Find(surveyId),
            });

            //Act
            var result = controller.DeleteSurvey(surveyId);

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);   
            dbContext.SurveyArchiveds.Where(x => x.User.AspUserId == "MyUserId").Count(x => !x.Deleted).ShouldBe(0);
        }


        [Fact]
        public void DeleteSurveyReturnsUnauthorizedWhenNotLoggedIn()
        {
            //Arrange
            var user = new User
            {
                AspUserId = "Foo"
            };

            CreateUser("MyUserId");
            var survey = new SurveyEditable
            {
                Name = "SomeAweSomeName",
                User = user
            };

            dbContext.SurveyEditables.Add(survey);
            dbContext.SaveChanges();
            var surveyId = CreateSurveyArchived(name: "Niels", user: user, template: survey);

            //Act
            var result = controller.DeleteSurvey(surveyId);

            //Assert
            result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }
    }
}
