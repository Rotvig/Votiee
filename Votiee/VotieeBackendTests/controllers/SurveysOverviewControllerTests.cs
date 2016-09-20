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
    public class SurveysOverviewControllerTests : TestExtentions
    {
        private readonly SurveysOverviewController controller;

        public SurveysOverviewControllerTests()
        {
            var claim = new Claim("TestUnit", "MyUserId");
            var identity = A.Fake<ClaimsIdentity>();
            A.CallTo(() => identity.FindFirst(A<string>._)).Returns(claim);
            var user = A.Fake<IPrincipal>();
            A.CallTo(() => user.Identity).Returns(identity);

            controller = new SurveysOverviewController(dbContext, user) { Request = new HttpRequestMessage() };
            controller.Request.SetConfiguration(new HttpConfiguration());
        }

        [Fact]
        public void LoadSurveyOverview()
        {
            //Arrange
            var surveyId1 = CreateSurveyEditable(name: "numberOne", surveyCode: "Survey1");
            CreateSurveyItemEditable(questionText: "What?");
            var surveyId2 = CreateSurveyEditable(name: "numberTwo", surveyCode: "Survey2");
            CreateSurveyItemEditable(questionText: "Who?");
            var userId = CreateUser("MyUserId", new List<SurveyEditable>()
            {
                dbContext.SurveyEditables.Find(surveyId1),
                dbContext.SurveyEditables.Find(surveyId2)
            });

            //Act
            var result = controller.LoadSurveysOverview();

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
            SurveysOverviewViewModel viewModel;
            result.TryGetContentValue(out viewModel).ShouldBe(true);
            viewModel.Surveys.Count.ShouldBe(2);
            viewModel.Surveys.Single(x => x.SurveyCode == "Survey1").Name.ShouldBe("numberOne");
            viewModel.Surveys.Single(x => x.SurveyCode == "Survey2").Name.ShouldBe("numberTwo");
        }

        [Fact]
        public void LoadSurveyOverviewReturnsBadRequestWhenNotLoggedIn()
        {
            //Arrange
            var identity = A.Fake<ClaimsIdentity>();
            A.CallTo(() => identity.FindFirst(A<string>._)).Returns(null);
            var user = A.Fake<IPrincipal>();
            A.CallTo(() => user.Identity).Returns(identity);

            var failController = new SurveysOverviewController(dbContext, user) { Request = new HttpRequestMessage() };
            failController.Request.SetConfiguration(new HttpConfiguration());

            //Act
            var result = failController.LoadSurveysOverview();

            //Assert
            result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public void CanDeleteSurvey()
        {
            //Arrange
            var surveyId1 = CreateSurveyEditable(name: "numberOne", surveyCode: "DasSurvey");
            CreateSurveyItemEditable(questionText: "What?");
            var surveyId2 = CreateSurveyEditable(name: "numberTwo");
            CreateSurveyItemEditable(questionText: "Who?");
            var userId = CreateUser("MyUserId", new List<SurveyEditable>()
            {
                dbContext.SurveyEditables.Find(surveyId1),
                dbContext.SurveyEditables.Find(surveyId2)
            });

            //Act
            var result = controller.DeleteSurvey("DasSurvey");
            result.IsSuccessStatusCode.ShouldBe(true);

            //Assert
            var user = dbContext.Users.Find(userId);
            user.SurveyEditables.Count(x => x.Name == "numberTwo").ShouldBe(1);
        }


        [Fact]
        public void DeleteSurveyReturnsBadRequestWhenNotLoggedIn()
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
                User = user,
                SurveyCode = "TheSurvey"
            };

            dbContext.SurveyEditables.Add(survey);
            dbContext.SaveChanges();

            //Act
            var result = controller.DeleteSurvey("TheSurvey");

            //Assert
            result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }
    }
}
