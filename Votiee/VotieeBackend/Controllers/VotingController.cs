using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VotieeBackend.Hubs;
using VotieeBackend.Models;
using VotieeBackend.Utils;
using VotieeBackend.ViewModels;

namespace VotieeBackend.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/Voting")]
    public class VotingController : ApiControllerWithHub<SurveySessionHub>
    {
        private readonly VotieeDbContext db;

        public VotingController(VotieeDbContext DB)
        {
            db = DB;
        }

        [HttpPost]
        [Route("LoadSurveySession")]
        public HttpResponseMessage LoadSurveySession([FromBody] LoadInfo data)
        {
            VotingViewModel votingViewModel;

            try
            {
                //Find session from sessionCode
                var surveySession = db.SurveySessions.Single(x => x.SessionCode == data.SessionCode.ToUpper());

                //Check that user has a connectionId
                if (data.ConnectionId == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest);

                //Get viewModel for the current active SurveyItem
                votingViewModel = GetVotingViewModel(surveySession, data.ConnectionId);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            //Return data to frontend
            return Request.CreateResponse(HttpStatusCode.OK, votingViewModel);
        }


        [HttpPost]
        [Route("SubmitVote")]
        public HttpResponseMessage SubmitVote([FromBody] ReceivedVote data)
        {
            SurveyItemArchived surveyItem;
            AnswerArchived selectedAnswer;
            SurveySession surveySession;

            try
            {
                //Check that session is active and that the current SurveyItem matches the one from the incoming vote
                surveySession = db.SurveySessions.Single(x => x.SessionCode == data.SessionCode);
                        
                if (data.SurveyItemOrder != surveySession.CurrentSurveyItem 
                    || !surveySession.SurveyItemActive || !surveySession.VotingOpen)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);                  
                }

                //Check that the user hasn't voted already and that the user has a connection string
                var currentSurveyItem =
                    surveySession.Survey.SurveyItems.Single(x => x.Order == surveySession.CurrentSurveyItem);

                if (data.ConnectionId == null || 
                    currentSurveyItem.Votes.SingleOrDefault(x => x.UserConnectionId == data.ConnectionId) != null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                //Find the SurveyItem with the selected id
                surveyItem = surveySession.Survey.SurveyItems.Single(x => x.Order == data.SurveyItemOrder);

                //Find the selected answer based on Order attribute
                selectedAnswer = surveyItem.Answers.Single(x => x.Order == data.AnswerSelected);
            }
            catch (Exception e)
            {
                //Return error if SurveyItems or answer wasn't found
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            //Create new vote with reference to
            var vote = new Vote
            {
                Answer = selectedAnswer,
                UserConnectionId = data.ConnectionId
            };

            //Return error if model state is invalid
            if (!ModelState.IsValid) return Request.CreateResponse(HttpStatusCode.BadRequest);

            //Save data to database
            surveyItem.Votes.Add(vote);
            db.SaveChanges();

            //Send update message to the the resultspages attached to the SurveyItems. 
            Hub.Clients.Group("SurveySessionHostGroup_" + surveySession.SessionCode).updateVotes();

            //Return succes message
            return Request.CreateResponse(HttpStatusCode.OK, data.AnswerSelected);
        }      

        private VotingViewModel GetVotingViewModel(SurveySession surveySession, string connectionId)
        {
            //Check if an survey is currently active. Return (almost) empty viewModel if not.
            if (!surveySession.SurveyActive)
            {
                return new VotingViewModel { SurveyActive = false };
            }

            //Check if an surveyItem is currently active. Return (almost) empty viewModel if not.
            if (!surveySession.SurveyItemActive)
            {
                return new VotingViewModel { SurveyActive = true, SurveyItemActive = false };
            }

            //Find the Surveys with the selected id
            var survey = surveySession.Survey;

            //Get currently active SurveyItem
            var surveyItem = survey.SurveyItems.Single(x => x.Order == surveySession.CurrentSurveyItem);

            //Get vote number if user has voted already
            var vote = surveyItem.Votes.SingleOrDefault(x => x.UserConnectionId == connectionId);

            //Create SurveyItems view model
            var surveyItemViewModel = new SurveyItemViewModel
            {
                SurveyItemId = surveySession.CurrentSurveyItem,
                Order = surveyItem.Order,
                QuestionText = surveyItem.QuestionText,
                Answers = new List<AnswerViewModel>()
            };

            //Create answerViewModels from answers
            foreach (var answerViewModel in surveyItem.Answers.OrderBy(x => x.Order).Select(answer => new AnswerViewModel
            {
                AnswerText = answer.AnswerText,
                Order = answer.Order
            }))
            {
                //Add answer to SurveyItemViewModel
                surveyItemViewModel.Answers.Add(answerViewModel);
            }

            //Add answerResults if show results is true
            var answerResults = new List<AnswerResultViewModel>();
            if (surveySession.ShowResults)
            {
                answerResults = surveyItem.Answers.OrderBy(x => x.Order).Select(answer => new AnswerResultViewModel
                {
                    AnswerText = answer.AnswerText,
                    Order = answer.Order,
                    VoteCount = surveyItem.Votes.Count(x =>
                        x.Answer.AnswerId == answer.AnswerId),
                    Marked = (vote != null) && (vote.Answer.Order == answer.Order)
                }).ToList();
            }

            //Create VotingViewModel
            var votingViewModel = new VotingViewModel
            {
                SurveyActive = surveySession.SurveyActive,
                SurveyItemActive = surveySession.SurveyItemActive,
                Answered = (vote != null),
                SelectedAnswer = vote?.Answer.Order ?? 0,
                VotingOpen = surveySession.VotingOpen,
                ShowResults = surveySession.ShowResults,
                SurveyItem = surveyItemViewModel,
                AnswerResults = answerResults
            };

            return votingViewModel;
        }
    }

    public class LoadInfo
    {
        public string SessionCode { get; set; }
        public string ConnectionId { get; set; }
    }

    public class ReceivedVote
    {
        public string SessionCode { get; set; }
        public string ConnectionId { get; set; }
        public int SurveyItemOrder { get; set; }
        public int AnswerSelected { get; set; }
    }

}
