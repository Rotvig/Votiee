using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Http;
using FakeItEasy;
using Shouldly;
using VotieeBackend.Controllers;
using VotieeBackend.ViewModels;
using VotieeBackendTests.utils;
using Xunit;

namespace VotieeBackendTests.controllers
{
    public class SurveyControllerTests : TestExtentions
    {
        private readonly SurveyController controller;

        public SurveyControllerTests()
        {
            var claim = new Claim("TestUnit", "MyUserId");
            var identity = A.Fake<ClaimsIdentity>();
            A.CallTo(() => identity.FindFirst(A<string>._)).Returns(claim);
            var user = A.Fake<IPrincipal>();
            A.CallTo(() => user.Identity).Returns(identity);

            controller = new SurveyController(dbContext, user) {Request = new HttpRequestMessage()};
            controller.Request.SetConfiguration(new HttpConfiguration());
        }

        [Fact]
        public void CanLoadSurvey()
        {
            //Arrange      
            var surveyId = CreateSurveyEditable(surveyCode: "thiscode");
            var surveyItemId = CreateSurveyItemEditable("SomeText");

            //Act
            var result = controller.LoadSurvey("thiscode");

            //Assert
            SurveyViewModel surveyViewModel;
            result.IsSuccessStatusCode.ShouldBe(true);
            result.TryGetContentValue(out surveyViewModel).ShouldBe(true);
            surveyViewModel.SurveyId.ShouldBe(surveyId);
            surveyViewModel.SurveyItems.Count.ShouldBe(1);
            surveyViewModel.SurveyItems.First().QuestionText.ShouldBe("SomeText");
            surveyViewModel.SurveyItems.First().SurveyItemId.ShouldBe(surveyItemId);
        }

        [Fact]
        public void CanLoadSurveyReturnsNotFoundWhenSurveyDoesNotExist()
        {
            //Arrange

            //Act
            var result = controller.LoadSurvey("UNEXISITNG");

            //Assert
            result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public void CanCreateSurveyItem()
        {         
            //Arrange         
            var surveyId = CreateSurveyEditable();

            //Act
            var result = controller.CreateNewSurveyItem(surveyId);

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
            dbContext.SurveyEditables.Find(surveyId).SurveyItems.Count.ShouldBe(1);
        }

        [Fact]
        public void CanCreateNewAnswer()
        {
            //Arrange
            var surveyId = CreateSurveyEditable(); 
            var surveyItemId = CreateSurveyItemEditable();                   

            //Act
            var result = controller.CreateNewAnswer(surveyItemId);

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
            dbContext.SurveyEditables.Find(surveyId).SurveyItems.Count.ShouldBe(1);
        }

        [Fact]
        public void CanSaveSurveyItem()
        {          
            //Arrange
            CreateSurveyEditable();
            var surveyItemId = CreateSurveyItemEditable(questionText: "Admin Jensen");
            var saveViewModel = new updateData
            {
                Id = surveyItemId,
                Text = "Bob Reggae"
            };

            //Act
            var result = controller.SaveSurveyItem(saveViewModel);

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
            var surveyItem = dbContext.SurveyItemEditables.Find(1);
            surveyItem.QuestionText.ShouldBe("Bob Reggae");
        }

        [Fact]
        public void CanSaveAnswer()
        {                 
            //Arrange       
            CreateSurveyEditable();
            CreateSurveyItemEditable();
            var answerId = CreateAnswerEditable("Bobby Olsen");
            var saveViewModel = new updateData
            {
                Id = answerId,
                Text = "Brain Nielsen"
            };

            //Act
            var result = controller.SaveAnswer(saveViewModel);

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
            var answer = dbContext.AnswerEditables.Find(1);
            answer.AnswerText.ShouldBe("Brain Nielsen");
        }

        [Fact]
        public void CanDeleteAnswer()
        {        
            //Arrange    
            CreateSurveyEditable();
            var surveyItemId = CreateSurveyItemEditable();
            var answerId = CreateAnswerEditable();
            CreateAnswerEditable();

            //Act
            var result = controller.DeleteAnswer(answerId);

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
            dbContext.SurveyItemEditables.Find(surveyItemId).Answers.Count.ShouldBe(1);
        }

        [Fact]
        public void CanDeleteSurveyItem()
        {        
            //Arrange          
            var editAbleSurveyId = CreateSurveyEditable();
            var surveyItemId = CreateSurveyItemEditable();
            CreateSurveyItemEditable();

            //Act
            var result = controller.DeleteSurveyItem(surveyItemId);

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
            dbContext.SurveyEditables.Find(editAbleSurveyId).SurveyItems.Count.ShouldBe(1);
        }

        [Fact]
        public void CanChangeSurveyName()
        {
            //Arrange     
            var surveyId = CreateSurveyEditable(name: "Johnson baby !..");
            var data = new updateData
            {
                Id = surveyId,
                Text = "Niarn MAynnn..."
            };

            //Act
            var result = controller.ChangeSurveyName(data);

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
            dbContext.SurveyEditables.Find(surveyId).Name.ShouldBe("Niarn MAynnn...");
        }

        [Fact]
        public void CanMoveSurveyItemUp()
        {
            //Arrange     
            CreateSurveyEditable();
            CreateSurveyItemEditable(order: 1);
            var itemId = CreateSurveyItemEditable( order: 2);

            //Act
            var result = controller.MoveSurveyItemUp(itemId);

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
            dbContext.SurveyItemEditables.Find(itemId).Order.ShouldBe(1);
        }

        [Fact]
        public void CanNotMoveSurveyItemUpWhenOrderIsOne()
        {
            //Arrange     
            CreateSurveyEditable();
            var itemId = CreateSurveyItemEditable(order: 1);
            CreateSurveyItemEditable(order: 2);

            //Act
            var result = controller.MoveSurveyItemUp(itemId);

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
            dbContext.SurveyItemEditables.Find(itemId).Order.ShouldBe(1);
        }

        [Fact]
        public void CanMoveSurveyItemDown()
        {
            //Arrange     
            CreateSurveyEditable();
            var itemId = CreateSurveyItemEditable(order: 1);
            CreateSurveyItemEditable(order: 2);

            //Act
            var result = controller.MoveSurveyItemDown(itemId);

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
            dbContext.SurveyItemEditables.Find(itemId).Order.ShouldBe(2);
        }

        [Fact]
        public void CanNotMoveSurveyItemDownWhenItIsTheLastItem()
        {
            //Arrange     
            CreateSurveyEditable();
            CreateSurveyItemEditable(order: 1);
            var itemId = CreateSurveyItemEditable(order: 2);

            //Act
            var result = controller.MoveSurveyItemDown(itemId);

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
            dbContext.SurveyItemEditables.Find(itemId).Order.ShouldBe(2);
        }

        [Fact]
        public void CanMoveAnswerUp()
        {
            //Arrange     
            CreateSurveyEditable();
            CreateSurveyItemEditable();
            CreateAnswerEditable(order: 1);
            var answerId = CreateAnswerEditable(order: 2);

            //Act
            var result = controller.MoveAnswerUp(answerId);

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
            dbContext.AnswerEditables.Find(answerId).Order.ShouldBe(1);
        }

        [Fact]
        public void CanNotMoveAnswerUpWhenOrderIsOne()
        {
            //Arrange     
            CreateSurveyEditable();
            CreateSurveyItemEditable();
            var answerId = CreateAnswerEditable(order: 1);
            CreateAnswerEditable(order: 2);

            //Act
            var result = controller.MoveAnswerUp(answerId);

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
            dbContext.AnswerEditables.Find(answerId).Order.ShouldBe(1);
        }

        [Fact]
        public void CanMoveAnswerDown()
        {
            //Arrange     
            CreateSurveyEditable();
            CreateSurveyItemEditable();
            var answerId = CreateAnswerEditable(order: 1);
            CreateAnswerEditable(order: 2);

            //Act
            var result = controller.MoveAnswerDown(answerId);

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
            dbContext.AnswerEditables.Find(answerId).Order.ShouldBe(2);
        }

        [Fact]
        public void CanNotMoveAnswerDownWhenItIsTheLastItem()
        {
            //Arrange     
            CreateSurveyEditable();
            CreateSurveyItemEditable();
            CreateAnswerEditable(order: 1);
            var answerId = CreateAnswerEditable(order: 2);

            //Act
            var result = controller.MoveAnswerDown(answerId);

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
            dbContext.AnswerEditables.Find(answerId).Order.ShouldBe(2);
        }
    }
}
