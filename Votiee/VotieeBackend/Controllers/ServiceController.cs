using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using VotieeBackend.Models;
using System.Security.Principal;

namespace VotieeBackend.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/Service")]
    public class ServiceController : ApiController
    {
        private readonly VotieeDbContext db;

        private const string NewSurveyName = "My Survey";
        private const int SessionCodeLength = 5;
        private const int SurveyCodeLength = 8;

        public ServiceController(VotieeDbContext db)
        {
            this.db = db;
        }

        public ServiceController(VotieeDbContext DB, IPrincipal user)
        {
            db = DB;
            User = user;
        }

        [HttpGet]
        [Route("CreateSurveySession/{surveyCode}")]
        public HttpResponseMessage CreateSurveySession(string surveyCode)
        {
            SurveySession surveySession;

            try
            {
                //Load survey from id
                var surveyEditable = db.SurveyEditables.Single(x => x.SurveyCode == surveyCode);

                //Send error back if survey is empty
                if (surveyEditable.IsEmpty())
                    return Request.CreateResponse(HttpStatusCode.BadRequest);

                //Check that the user is allowed to start the Survey Sesssion
                if (surveyEditable.User != null)
                {
                    if (User.Identity.GetUserId() != surveyEditable.User.AspUserId)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }

                //Convert SurveyEditable to new SurverArchived
                var surveyArchived = surveyEditable.Archive();

                //Create SurveySession for Survey. Set CurrentSurveyItem to 1 to start from the first SurveyItem.
                surveySession = new SurveySession
                {
                    Survey = surveyArchived,
                    SurveyActive = true,
                    CurrentSurveyItem = surveyArchived.SurveyItems.First().Order,
                    SurveyItemActive = false,
                    VotingOpen = false,
                    ShowResults = false,
                    SessionCode = GenerateSessionCode()
                };

                //Save SurveySession to database
                db.SurveySessions.Add(surveySession);
                db.SaveChanges();

            }
            catch (Exception)
            {
                //Send error message to frontend if data wasn't found in the database
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            //Send SessionCode to frontend. Used to open the correct resultspage.
            return Request.CreateResponse(HttpStatusCode.OK, surveySession.SessionCode);
        }

        [HttpPost]
        [Route("CreateNewSurvey")]
        public HttpResponseMessage CreateNewSurvey()
        {
            if (!Singleton.IsTesting)
            {
                //Performance fix, it takes too long to add a survey
                db.Configuration.AutoDetectChangesEnabled = false;
            }

            //Create new survey
            var survey = new SurveyEditable
            {
                Name = NewSurveyName,
                SurveyCode = GenerateSurveyCode(),
                Deleted = false
            };

            //Link survey to user if logged in
            var currentUserId = User.Identity.GetUserId();

            if (currentUserId != null)
            {
                //Find current User and add to survey
                survey.User = db.Users.Single(x => x.AspUserId == currentUserId);
            }

            //Add default surveyitem and answer
            survey.SurveyItems.Add(new SurveyItemEditable
            {
                Order = 1,
                Answers = new List<AnswerEditable>
                {
                    new AnswerEditable
                    {
                        Order = 1
                    },
                    new AnswerEditable
                    {
                        Order = 2
                    }
                }
            });

            db.SurveyEditables.Add(survey);
            db.ChangeTracker.DetectChanges();
            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, survey.SurveyCode);
        }

        [HttpGet]
        [Route("CheckIfSessionExists/{sessionCode}")]
        public HttpResponseMessage CheckIfSessionExists(string sessionCode)
        {
            try
            {
                var test = Request.CreateResponse(HttpStatusCode.BadRequest, "Wrong ID... Use the Survey ID and not the Survey Editing ID.");

                //Check if the code is mistakenly a SurveyCode
                if (sessionCode.Length == SurveyCodeLength)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Wrong ID... Use the Survey ID and not the Survey Editing ID.");
                }

                //Check if the code is in a wrong format
                if (sessionCode.Length != SessionCodeLength)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Wrong ID... The Survey ID should be " + SessionCodeLength + " characters long.");
                }

                //Find Survey Session
                var surveySession = db.SurveySessions.Single(x => x.SessionCode == sessionCode.ToUpper());

                //Check if it's active
                if (!surveySession.SurveyActive)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "The survey is not active.");
                }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "No survey was found...");
            }

            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }

        [HttpGet]
        [Route("CheckIfPresenterSessionExists/{sessionCode}")]
        public HttpResponseMessage CheckIfPresenterSessionExists(string sessionCode)
        {

            try
            {
                //Load session from id
                var surveySession = db.SurveySessions.Single(x => x.SessionCode == sessionCode);

            }
            catch (Exception e)
            {
                //Send error message to frontend if data wasn't found in the database
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            //Send SessionCode to frontend. Used to open the correct resultspage.
            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }

        [HttpGet]
        [Route("CheckIfSurveyEditableExists/{surveyCode}")]
        public HttpResponseMessage CheckIfSurveyEditableExists(string surveyCode)
        {
            try
            {
                //Load survey from id
                var surveyEditable = db.SurveyEditables.Single(x => x.SurveyCode == surveyCode);

                //Send error if the survey is owned by a registered user
                if (surveyEditable.User != null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                //Send error message to frontend if data wasn't found in the database
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            //Send SessionCode to frontend. Used to open the correct resultspage.
            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }

        [HttpGet]
        [Route("ReactivateSurveySession/{id}")]
        public HttpResponseMessage ReactivateSurveySession(int id)
        {
            SurveySession surveySession;

            try
            {
                //Load survey from id
                var surveyArchived = db.SurveyArchiveds.Find(id);

                //Check that the user is allowed to start the Survey Sesssion
                if (surveyArchived.User != null)
                {
                    if (User.Identity.GetUserId() != surveyArchived.User.AspUserId)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }

                //Check if a survey session object already exists for the surveyArchived
                surveySession = db.SurveySessions.SingleOrDefault(x => 
                    x.Survey.SurveyId == surveyArchived.SurveyId);

                //Create surveySession if it doesn't exists already
                if (surveySession == null)
                {
                    surveySession = new SurveySession()
                    {
                        Survey = surveyArchived,
                        SessionCode = GenerateSessionCode(),
                        SurveyActive = false,
                        CurrentSurveyItem = 1,
                        SurveyItemActive = false,
                        VotingOpen = false,
                        ShowResults = false
                    };

                    db.SurveySessions.Add(surveySession);      
                }
                else
                {
                    //Make sure that it starts not as active (should be activated by the user)
                    surveySession.SurveyActive = false;
                    surveySession.SurveyItemActive = false;
                    surveySession.VotingOpen = false;
                    surveySession.ShowResults = false;
                }
            
                db.SaveChanges();
            }
            catch (Exception)
            {
                //Send error message to frontend if data wasn't found in the database
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            //Send SessionCode to frontend. Used to open the correct resultspage.
            return Request.CreateResponse(HttpStatusCode.OK, surveySession.SessionCode);
        }

        private string GenerateSessionCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var random = new Random();
            var randomString = "";

            while (randomString == "")
            {
                //Generate random string
                randomString = new string(
                 Enumerable.Repeat(chars, SessionCodeLength)
                           .Select(s => s[random.Next(s.Length)])
                           .ToArray());

                //Clear string if generated sessionCode is already in use
                if (db.SurveySessions.SingleOrDefault(x => x.SessionCode == randomString) != null)
                    randomString = "";
            }

            return randomString;
        }

        private string GenerateSurveyCode()
        {
            const string chars = "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789";
            var random = new Random();
            var randomString = "";

            while (randomString == "")
            {
                //Generate random string
                randomString = new string(
                 Enumerable.Repeat(chars, SurveyCodeLength)
                           .Select(s => s[random.Next(s.Length)])
                           .ToArray());

                //Clear string if generated sessionCode is already in use
                if (db.SurveyEditables.SingleOrDefault(x => x.SurveyCode == randomString) != null)
                    randomString = "";
            }

            return randomString;
        }

    }
}