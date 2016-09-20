using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using VotieeBackend.Models;

namespace VotieeBackend.Controllers
{
    [RoutePrefix("api/StatisticsOverview")]
    public class StatisticsOverviewController : ApiController
    {
        private readonly VotieeDbContext db;

        public StatisticsOverviewController(VotieeDbContext DB)
        {
            db = DB;
        }

        public StatisticsOverviewController(VotieeDbContext DB, IPrincipal user)
        {
            db = DB;
            User = user;
        }


        [HttpGet]
        [Authorize]
        [Route("LoadStatisticsOverview")]
        public HttpResponseMessage LoadStatisticsOverview()
        {
            var statisticOverviewSurveys = new List<StatisticOverviewSurveys>();
            try
            {
                //Find the userId of the user that is logged in
                var currentUserId = User.Identity.GetUserId();

                //Find current User
                var user = db.Users.Single(x => x.AspUserId == currentUserId);

                //Find all the user's surveys and group them
                statisticOverviewSurveys.AddRange(
                    user.SurveyArchiveds.Where(x => !x.Deleted).OrderByDescending(x => x.ArchiveDate)
                    .GroupBy(x => x.SurveyTemplate).Select(survey => new StatisticOverviewSurveys
                    {
                        Name = survey.Key.Name,
                        Surveys = survey
                        .Select(surveyArchived => new StatisticsArchivedSurveys
                        {
                            Name = surveyArchived.Name,
                            ArchiveDate = surveyArchived.ArchiveDate,
                            SurveyId = surveyArchived.SurveyId
                        }).ToList()
                    }));
            }
            catch (Exception)
            {
                //Send error message to frontend if data wasn't found in the database
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            //Return data to frontend
            return Request.CreateResponse(HttpStatusCode.OK, statisticOverviewSurveys);
        }

        [HttpPost]
        [Authorize]
        [Route("DeleteSurveyArchived/{surveyId}")]
        public HttpResponseMessage DeleteSurvey(int surveyId)
        {
            try
            {
                var currentUserId = User.Identity.GetUserId();
                var user = db.Users.Single(x => x.AspUserId == currentUserId);
                var survey = db.SurveyArchiveds.Find(surveyId);
                if (survey.User.UserId == user.UserId)
                {
                    survey.Deleted = true;
                    db.SaveChanges();
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
            }
            catch (Exception)
            {
                //Send error message to frontend if data wasn't found in the database
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }
    }
}

    public class StatisticOverviewSurveys
    {
        public string Name { get; set; }
        public List<StatisticsArchivedSurveys> Surveys { get; set; }
    }

    public class StatisticsArchivedSurveys
    {
        public int SurveyId { get; set; }
        public DateTime? ArchiveDate { get; set; }
        public string Name { get; set; }
    }
