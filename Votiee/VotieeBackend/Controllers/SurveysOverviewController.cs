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
    [RoutePrefix("api/SurveysOverview")]
    public class SurveysOverviewController : ApiController
    {
        private readonly VotieeDbContext db;

        public SurveysOverviewController(VotieeDbContext DB)
        {
            db = DB;
        }

        public SurveysOverviewController(VotieeDbContext DB, IPrincipal user)
        {
            db = DB;
            User = user;
        }

        [HttpGet]
        [Authorize]
        [Route("LoadSurveysOverview")]
        public HttpResponseMessage LoadSurveysOverview()
        {
            SurveysOverviewViewModel surveysOverviewViewModel;

            try
            {
                //Find the userId of the user that is logged in
                var currentUserId = User.Identity.GetUserId();

                //Find current User
                var user = db.Users.Single(x => x.AspUserId == currentUserId);

                //Create SurveyOverviewViewModel
                surveysOverviewViewModel = new SurveysOverviewViewModel();

                //Find all the user's surveys and add to view model
                foreach (var surveysOverviewSurveyViewModel in user.SurveyEditables
                    .Where(x => !x.Deleted && !x.IsEmpty()).OrderByDescending(x => x.SurveyId)
                    .Select(survey => new SurveysOverviewSurveyViewModel
                {
                    SurveyCode = survey.SurveyCode,
                    Name = survey.Name
                }))
                {
                    surveysOverviewViewModel.Surveys.Add(surveysOverviewSurveyViewModel);
                }
            }
            catch (Exception)
            {
                //Send error message to frontend if data wasn't found in the database
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            //Return data to frontend
            return Request.CreateResponse(HttpStatusCode.OK, surveysOverviewViewModel);
        }

        [HttpPost]
        [Authorize]
        [Route("DeleteSurveyEditable/{surveyCode}")]
        public HttpResponseMessage DeleteSurvey(string surveyCode)
        {
            try
            {
                //Find Survey from user
                var currentUserId = User.Identity.GetUserId();
                var user = db.Users.Single(x => x.AspUserId == currentUserId);
                var survey = user.SurveyEditables.Single(x => x.SurveyCode == surveyCode);

                //Delete Survey
                survey.Deleted = true;
                db.SaveChanges();
            }
            catch (Exception)
            {
                //Send error message to frontend if data wasn't found in the database
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }
    }

    public class SurveysOverviewViewModel
    {
        public SurveysOverviewViewModel()
        {
            Surveys = new List<SurveysOverviewSurveyViewModel>();
        }

        public List<SurveysOverviewSurveyViewModel> Surveys { get; set; }
    }

    public class SurveysOverviewSurveyViewModel
    {
        public string SurveyCode { get; set; }
        public string Name { get; set; }
    }
}