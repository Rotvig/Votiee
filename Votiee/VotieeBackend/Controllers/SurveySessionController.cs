using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using VotieeBackend.Hubs;
using VotieeBackend.Models;
using VotieeBackend.Utils;
using VotieeBackend.ViewModels;

namespace VotieeBackend.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/SurveySession")]
    public class SurveySessionController : ApiControllerWithHub<SurveySessionHub>
    {
        private readonly VotieeDbContext db;

        public SurveySessionController(VotieeDbContext DB)
        {
            db = DB;
        }

        [HttpGet]
        [Route("LoadSurveySession/{sessionCode}")]
        public HttpResponseMessage LoadSurveySession(string sessionCode)
        {
            SurveySessionViewModel surveySessionViewModel;

            try
            {
                //Load SurveySession from SessionCode
                var surveySession = db.SurveySessions.Single(x => x.SessionCode == sessionCode);

                HandleUserAuth(surveySession);

                //Create resultsViewModel with data for the active SurveyItem from SurveySession
                surveySessionViewModel = MapSurveySessionToSurveySessionViewModel(surveySession);
                surveySessionViewModel = MapStatesToSurveyItemSessionViewModel(surveySessionViewModel);

                //Notify Participants about updated data if session shows results
                if (surveySession.ShowResults)
                    NotifyParticipants(sessionCode);
            }
            catch (Exception)
            {
                //Send error message to frontend if data wasn't found in the database
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            //Send ResultsViewModel to frontend
            return Request.CreateResponse(HttpStatusCode.OK, surveySessionViewModel);
        }

        [HttpPost]
        [Route("ToggleSurveyActive/{sessionCode}")]
        public HttpResponseMessage ToggleSurveyActive(string sessionCode)
        {
            try
            {
                //Load SurveySession from SessionCode
                var surveySession = db.SurveySessions.Single(x => x.SessionCode == sessionCode);

                HandleUserAuth(surveySession);
                //Toggle SurveyActive, set surveyItemActive to false and notify Participants
                surveySession.SurveyActive = !surveySession.SurveyActive;
                surveySession.SurveyItemActive = false;
                surveySession.ShowResults = false;
                surveySession.VotingOpen = false;
                db.SaveChanges();
                NotifyParticipants(sessionCode);
            }
            catch (Exception)
            {
                //Send error message to frontend if data wasn't found in the database
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            //Send ResultsViewModel to frontend
            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }


        [HttpPost]
        [Route("ChangeActiveSurveyItem")]
        public HttpResponseMessage ChangeActiveSurveyItem(SurveyItemData data)
        {
            try
            {
                //Load SurveySession from SessionCode
                var surveySession = db.SurveySessions.Single(x => x.SessionCode == data.SessionCode.ToUpper());

                HandleUserAuth(surveySession);
                //Change currently active Item and notify Participants
                ChangeCurrentSurveyItem(surveySession, data);
                NotifyParticipants(surveySession.SessionCode);
            }
            catch (Exception e)
            {
                //Send error message to frontend if data wasn't found in the database
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            //Send ResultsViewModel to frontend
            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }

        [HttpPost]
        [Route("ToggleVotingOpen")]
        public HttpResponseMessage ToggleVotingOpen(SurveyItemData data)
        {
            try
            {
                //Load SurveySession from SessionCode
                var surveySession = db.SurveySessions.Single(x => x.SessionCode == data.SessionCode.ToUpper());

                HandleUserAuth(surveySession);

                //Toggle voting open and notify Participants
                surveySession.VotingOpen = !surveySession.VotingOpen;
                db.SaveChanges();
                NotifyParticipants(surveySession.SessionCode);
            }
            catch (Exception)
            {
                //Send error message to frontend if data wasn't found in the database
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            //Send ResultsViewModel to frontend
            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }

        [HttpPost]
        [Route("ToggleShowResults")]
        public HttpResponseMessage ToggleShowResults(SurveyItemData data)
        {
            try
            {
                //Load SurveySession from SessionCode
                var surveySession = db.SurveySessions.Single(x => x.SessionCode == data.SessionCode.ToUpper());

                HandleUserAuth(surveySession);

                if (surveySession.CurrentSurveyItem == data.Order)
                    {
                        //Toggle show results if already active
                        surveySession.ShowResults = !surveySession.ShowResults;
                        surveySession.SurveyItemActive = true;
                    }
                    else
                    {
                        //Set state values if not yet active (makes sure it's paused)
                        surveySession.CurrentSurveyItem = data.Order;
                        surveySession.SurveyItemActive = true;
                        surveySession.VotingOpen = false;
                        surveySession.ShowResults = true;
                    }

                    db.SaveChanges();
                    NotifyParticipants(surveySession.SessionCode);
            }
            catch (Exception)
            {
                //Send error message to frontend if data wasn't found in the database
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            //Send ResultsViewModel to frontend
            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }

        public class SurveyItemData
        {
            public string SessionCode { get; set; }
            public int Order { get; set; }
        }

        #region Private Methods

        private void ChangeCurrentSurveyItem(SurveySession surveySession, SurveyItemData data)
        {
            if (surveySession.CurrentSurveyItem == data.Order)
            {
                //Toggle SurveyItemActive if already selected
                surveySession.SurveyItemActive = !surveySession.SurveyItemActive;
            }
            else
            {
                //Make SurveyItemActive true and set order if not yet selected
                surveySession.CurrentSurveyItem = data.Order;
                surveySession.SurveyItemActive = true;
            }

            //Automatically start VotingOpen if none has voted yet - else set on pause
            surveySession.VotingOpen =
                surveySession.Survey.SurveyItems.Single(x => x.Order == data.Order).Votes.Count == 0;

            //Show results should be false when changing active survey
            surveySession.ShowResults = false;

            db.SaveChanges();
        }

        private User GetCurrentUser()
        {
            string currentUserId;
            User user = null;
            try
            {
                //Find the userId of the user that is logged in
                currentUserId = User.Identity.GetUserId();

                //Find current User
                user = db.Users.Single(x => x.AspUserId == currentUserId);
            }
            catch (Exception)
            {
                // ignored
            }

            return user;
        }

        private void NotifyParticipants(string sessionCode)
        {
            //Send update message to the the resultspages attached to the SurveyItems. 
            Hub.Clients.Group("SurveySessionParticipantGroup_" + sessionCode.ToUpper()).update();
        }

        private SurveySessionViewModel MapSurveySessionToSurveySessionViewModel(SurveySession surveySession)
        {
            return new SurveySessionViewModel
            {
                SessionCode = surveySession.SessionCode,
                CurrentSurveyItem = surveySession.CurrentSurveyItem,
                ShowResults = surveySession.ShowResults,
                SurveyItemActive = surveySession.SurveyItemActive,
                VotingOpen = surveySession.VotingOpen,
                SurveyActive = surveySession.SurveyActive,
                Survey = MapSurveyToSurveyStatisticsViewModel(surveySession.Survey)
            };
        }

        private SurveyArchivedSessionViewModel MapSurveyToSurveyStatisticsViewModel(SurveyArchived survey)
        {
            return new SurveyArchivedSessionViewModel
            {
                Name = survey.Name,
                ArchiveDate = survey.ArchiveDate,
                TemplateName = survey.SurveyTemplate.Name,
                SurveyItems = MapSurveyItemsToSurveyItemSessionViewModel(survey.SurveyItems.OrderBy(x => x.Order))
            };
        }

        private List<SurveyItemSessionViewModel> MapSurveyItemsToSurveyItemSessionViewModel(
            IEnumerable<SurveyItemArchived> surveyItems)
        {
            return surveyItems.Select(surveyItem => new SurveyItemSessionViewModel
            {
                QuestionText = surveyItem.QuestionText,
                AnswerResults = MapAnswersToAnswerResultViewModel(surveyItem.Answers.OrderBy(x => x.Order)),
                Order = surveyItem.Order,
                Active = false,
                ShowResults = false,
                VoteCount = surveyItem.Votes.Count
            }).ToList();
        }

        private List<AnswerResultViewModel> MapAnswersToAnswerResultViewModel(IEnumerable<IAnswer> answers)
        {
            return answers.Select(answer => new AnswerResultViewModel
            {
                AnswerText = answer.AnswerText,
                Order = answer.Order,
                VoteCount = db.Votes.Count(x =>
                    x.Answer.AnswerId == answer.AnswerId)
            }).ToList();
        }

        private SurveySessionViewModel MapStatesToSurveyItemSessionViewModel(
            SurveySessionViewModel surveySession)
        {
            var surveyItem = surveySession.Survey.SurveyItems
                .Single(x => x.Order == surveySession.CurrentSurveyItem);

            surveyItem.Active = surveySession.SurveyItemActive;
            surveyItem.VotingOpen = surveySession.VotingOpen;
            surveyItem.ShowResults = surveySession.ShowResults;

            return surveySession;
        }

        private bool HandleUserAuth(SurveySession surveySession)
        {
            //Check that user is allowed to access the session
            if (surveySession.Survey.User == null || surveySession.Survey.User.UserId == GetCurrentUser()?.UserId)
                return true;

            throw new ArgumentException("The current User was not conncted to this SurveySession");
        }

        #endregion
    }
}

