using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VotieeBackend.Models;
using VotieeBackend.ViewModels;

namespace VotieeBackend.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/Presenter")]
    public class PresenterController : ApiController
    {
        private readonly VotieeDbContext db;

        public PresenterController(VotieeDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        [Route("LoadSurveySession/{sessionCode}")]
        public HttpResponseMessage LoadSurveySession(string sessionCode)
        {
            PresenterViewModel presenterViewModel;

            try
            {
                //Find session from sessionCode
                var surveySession = db.SurveySessions.Single(x => x.SessionCode == sessionCode.ToUpper());

                //Get viewModel for the current active SurveyItem
                presenterViewModel = GetPresenterViewModel(surveySession);
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            //Return data to frontend
            return Request.CreateResponse(HttpStatusCode.OK, presenterViewModel);
        }

        private PresenterViewModel GetPresenterViewModel(SurveySession surveySession)
        {
            //Check if an survey is currently active. Return (almost) empty viewModel if not.
            if (!surveySession.SurveyActive)
            {
                return new PresenterViewModel { SurveyActive = false };
            }

            //Check if an surveyItem is currently active. Return (almost) empty viewModel if not.
            if (!surveySession.SurveyItemActive)
            {
                return new PresenterViewModel { SurveyActive = true, SurveyItemActive = false };
            }

            //Find the Surveys with the selected id
            var survey = surveySession.Survey;

            //Get currently active SurveyItem
            var surveyItem = survey.SurveyItems.Single(x => x.Order == surveySession.CurrentSurveyItem);

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
                answerResults = surveyItem.Answers.Select(answer => new AnswerResultViewModel
                {
                    AnswerText = answer.AnswerText,
                    Order = answer.Order,
                    VoteCount = surveyItem.Votes.Count(x =>
                        x.Answer.AnswerId == answer.AnswerId)
                }).ToList();
            }

            //Create VotingViewModel
            var presenterViewModel = new PresenterViewModel
            {
                SurveyActive = surveySession.SurveyActive,
                SurveyItemActive = surveySession.SurveyItemActive,
                VotingOpen = surveySession.VotingOpen,
                ShowResults = surveySession.ShowResults,
                SurveyItem = surveyItemViewModel,
                AnswerResults = answerResults
            };

            return presenterViewModel;
        }
    }
}