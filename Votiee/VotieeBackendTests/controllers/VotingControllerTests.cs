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
    public class VotingControllerTests : TestExtentions
    {
        private readonly VotingController controller;

        public VotingControllerTests()
        {
            controller = new VotingController(dbContext) { Request = new HttpRequestMessage() };
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
            var result = controller.LoadSurveySession(new LoadInfo()
            {
                SessionCode = "FISK",
                ConnectionId = "Test-Id"
            });

            //Asssert
            result.IsSuccessStatusCode.ShouldBe(true);
            VotingViewModel votingViewModel;
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
            var result = controller.LoadSurveySession(new LoadInfo()
            {
                SessionCode = "FISK",
                ConnectionId = "Test-Id"
            });

            //Asssert
            result.IsSuccessStatusCode.ShouldBe(true);
            VotingViewModel votingViewModel;
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
            var result = controller.LoadSurveySession(new LoadInfo()
            {
                SessionCode = "FISK",
                ConnectionId = "Test-Id"
            });

            //Asssert
            result.IsSuccessStatusCode.ShouldBe(true);
            VotingViewModel votingViewModel;
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
            var result = controller.LoadSurveySession(new LoadInfo()
            {
                SessionCode = "FISK",
                ConnectionId = "Test-Id"
            });

            //Asssert
            result.IsSuccessStatusCode.ShouldBe(true);
            VotingViewModel votingViewModel;
            result.TryGetContentValue(out votingViewModel).ShouldBe(true);
            votingViewModel.SurveyItem.ShouldBe(null);
            votingViewModel.SurveyActive.ShouldBe(false);
        }

        [Fact]
        public void CanSubmitVote()
        {
            //Arrange
            CreateSurveyArchived();
            CreateSurveySession(sessionCode: "NISSE");
            CreateSurveyItemArchived(order: 1);
            CreateAnswerArchived(order: 1);

            //Act
            var result = controller.SubmitVote(new ReceivedVote
            {
                AnswerSelected = 1,
                SessionCode = "NISSE",
                ConnectionId = "Test-Id",
                SurveyItemOrder = 1
            });

            //Assert
            result.IsSuccessStatusCode.ShouldBe(true);
        }

        [Fact]
        public void CanNotSubmitVoteWhenSurveyItemIsNotActive()
        {
            //Arrange
            CreateSurveyArchived();
            CreateSurveySession(sessionCode: "NISSE", surveyItemActive: false);
            CreateSurveyItemArchived(order: 1);
            CreateAnswerArchived(order: 1);

            //Act
            var result = controller.SubmitVote(new ReceivedVote
            {
                AnswerSelected = 1,
                ConnectionId = "Test-Id",
                SessionCode = "NISSE",
                SurveyItemOrder = 1
            });

            //Assert
            result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public void CanNotSubmitVoteWhenCurrentSurveyItemIsNotEqualToSurveyItemOrder()
        {
            //Arrange
            CreateSurveyArchived();
            CreateSurveySession(sessionCode: "NISSE", currentSurveyItem: 22);
            CreateSurveyItemArchived(order: 1);
            CreateAnswerArchived(order: 1);

            //Act
            var result = controller.SubmitVote(new ReceivedVote
            {
                AnswerSelected = 1,
                SessionCode = "NISSE",
                ConnectionId = "Test-Id",
                SurveyItemOrder = 1
            });

            //Assert
            result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public void CanNotSubmitVoteWhenUserHasVotedBefore()
        {
            //Arrange
            CreateSurveyArchived();
            CreateSurveySession(sessionCode: "NISSE", currentSurveyItem: 1, votingOpen: true);
            CreateSurveyItemArchived(order: 1);
            CreateAnswerArchived(order: 1);
            CreateVote(connectionId: "Test-Id");

            //Act
            var result = controller.SubmitVote(new ReceivedVote
            {
                AnswerSelected = 1,
                SessionCode = "NISSE",
                ConnectionId = "Test-Id",
                SurveyItemOrder = 1
            });

            //Assert
            result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public void CanNotSubmitVoteWhenVotingIsNotOpen()
        {
            //Arrange
            CreateSurveyArchived();
            CreateSurveySession(sessionCode: "NISSE", currentSurveyItem: 1, votingOpen: false);
            CreateSurveyItemArchived(order: 1);
            CreateAnswerArchived(order: 1);
            CreateVote(connectionId: "Test-Id");

            //Act
            var result = controller.SubmitVote(new ReceivedVote
            {
                AnswerSelected = 1,
                SessionCode = "NISSE",
                ConnectionId = "Test-Id",
                SurveyItemOrder = 1
            });

            //Assert
            result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }
    }
}
