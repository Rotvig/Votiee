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
    public class PresenterControllerTests : TestExtentions
    {
        private readonly PresenterController controller;

        public PresenterControllerTests()
        {
            controller = new PresenterController(dbContext) { Request = new HttpRequestMessage() };
            controller.Request.SetConfiguration(new HttpConfiguration());
        }

        [Fact]
        public void CanLoadSurveySession()
        {
            //Arrange
            CreateSurveyArchived();
            CreateSurveySession(sessionCode: "FISK", surveyActive: true,
                surveyItemActive: true, votingOpen: true);
            CreateSurveyItemArchived(questionText: "Jørgen Leth");
            CreateAnswerArchived(order: 1);

            //Act
            var result = controller.LoadSurveySession("FISK");

            //Asssert
            result.IsSuccessStatusCode.ShouldBe(true);
            PresenterViewModel votingViewModel;
            result.TryGetContentValue(out votingViewModel).ShouldBe(true);
            votingViewModel.SurveyItem.QuestionText.ShouldBe("Jørgen Leth");
            votingViewModel.SurveyItem.Answers.Count.ShouldBe(1);
            votingViewModel.AnswerResults.Count.ShouldBe(0);
        }

        [Fact]
        public void CanLoadSurveySessionWithShowResults()
        {
            //Arrange
            CreateSurveyArchived();
            CreateSurveySession(sessionCode: "FISK", surveyActive: true,
                surveyItemActive: true, votingOpen: false, showResults: true);
            CreateSurveyItemArchived(questionText: "Jørgen Leth");
            CreateAnswerArchived(order: 1);
            CreateVote("myConnection");
            CreateAnswerArchived(order: 2);

            //Act
            var result = controller.LoadSurveySession("FISK");

            //Asssert
            result.IsSuccessStatusCode.ShouldBe(true);
            PresenterViewModel votingViewModel;
            result.TryGetContentValue(out votingViewModel).ShouldBe(true);
            votingViewModel.SurveyItem.QuestionText.ShouldBe("Jørgen Leth");
            votingViewModel.SurveyItem.Answers.Count.ShouldBe(2);
            votingViewModel.AnswerResults.Count.ShouldBe(2);
            votingViewModel.AnswerResults.Single(x => x.Order == 1).VoteCount.ShouldBe(1);
        }

        [Fact]
        public void CanLoadSurveySessionWhenSurveyItemIsNotActive()
        {
            //Arrange
            CreateSurveySession(sessionCode: "FISK", surveyActive: true, surveyItemActive: false);

            //ACT
            var result = controller.LoadSurveySession("FISK");

            //Asssert
            result.IsSuccessStatusCode.ShouldBe(true);
            PresenterViewModel votingViewModel;
            result.TryGetContentValue(out votingViewModel).ShouldBe(true);
            votingViewModel.SurveyItem.ShouldBe(null);
            votingViewModel.SurveyItemActive.ShouldBe(false);
        }

        [Fact]
        public void CanLoadSurveySessionWhenSurveyIsNotActive()
        {
            //Arrange
            CreateSurveySession(sessionCode: "FISK", surveyActive: false);

            //ACT
            var result = controller.LoadSurveySession("FISK");

            //Asssert
            result.IsSuccessStatusCode.ShouldBe(true);
            PresenterViewModel votingViewModel;
            result.TryGetContentValue(out votingViewModel).ShouldBe(true);
            votingViewModel.SurveyItem.ShouldBe(null);
            votingViewModel.SurveyActive.ShouldBe(false);
        }
    }
}
