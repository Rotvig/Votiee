using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using VotieeBackend.Models;
using VotieeBackend.ViewModels;


namespace VotieeBackend.Controllers
{
    [RoutePrefix("api/Statistics")]
    public class StatisticsController : ApiController
    {
        private readonly VotieeDbContext db;

        public StatisticsController(VotieeDbContext DB)
        {
            db = DB;
        }

        public StatisticsController(VotieeDbContext DB, IPrincipal user)
        {
            db = DB;
            User = user;
        }

        [HttpGet]
        [Authorize]
        [Route("LoadSurveyStatistics/{id}")]
        public HttpResponseMessage LoadSurveyStatistics(int id)
        {
            SurveyStatisticsViewModel surveyStatisticsViewModel;

            try
            {
                //Find the userId of the user that is logged in
                var currentUserId = User.Identity.GetUserId();

                //Find current User
                var user = db.Users.Single(x => x.AspUserId == currentUserId);

                //Find surveyArchived. Throws error if it doesn't exist or doesn't belong to the user.
                var surveyArchived = user.SurveyArchiveds.Single(x => x.SurveyId == id);

                //Map to SurveyStatisticsViewModel
                surveyStatisticsViewModel = MapSurveyStatisticsViewModel(surveyArchived);

            }
            catch (Exception e)
            {
                //Send error message to frontend if data wasn't found in the database
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            //Return data to frontend
            return Request.CreateResponse(HttpStatusCode.OK, surveyStatisticsViewModel);
        }
 
        #region Private Methods

        private SurveyStatisticsViewModel MapSurveyStatisticsViewModel(SurveyArchived surveyArchived)
        {
            //Create SurveyStatisticsViewModel based on SurveyArchived
            var surveyStatisticsViewModel = new SurveyStatisticsViewModel()
            {
                Name = surveyArchived.Name,
                TemplateName = surveyArchived.SurveyTemplate.Name,
                ArchiveDate = surveyArchived.ArchiveDate
            };

            //Map all surveyItems as SurveyItemStatisticsViewModels
            foreach (var surveyItem in surveyArchived.SurveyItems.OrderBy(x => x.Order))
            {
                var surveyItemViewModel = new SurveyItemStatisticsViewModel()
                {
                    QuestionText = surveyItem.QuestionText,
                    Order = surveyItem.Order,
                };

                //Add all ansers as AnswerResultViewModels
                foreach (var answer in surveyItem.Answers.OrderBy(x => x.Order))
                {
                    surveyItemViewModel.AnswerResults.Add(new AnswerResultViewModel()
                    {
                        AnswerText = answer.AnswerText,
                        Order = answer.Order,
                        VoteCount = surveyItem.Votes.Count(x =>
                            x.Answer.AnswerId == answer.AnswerId)
                    });
                }

                surveyStatisticsViewModel.SurveyItems.Add(surveyItemViewModel);
            }

            return surveyStatisticsViewModel;
        }

        #endregion
    }

}
