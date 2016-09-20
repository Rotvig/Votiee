using System;
using System.Collections.Generic;
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
    [RoutePrefix("api/Surveys")]
    public class SurveyController : ApiController
    {
        private readonly VotieeDbContext db;

        public SurveyController(VotieeDbContext DB)
        {
            db = DB;
        }

        public SurveyController(VotieeDbContext DB, IPrincipal user)
        {
            db = DB;
            User = user;
        }

        [HttpGet]
        [Route("LoadSurvey/{surveyCode}")]
        public HttpResponseMessage LoadSurvey(string surveyCode)
        {
            SurveyViewModel surveyViewModel;

            try
            {
                //Find the Surveys with the selected surveyCode
                var survey = db.SurveyEditables.Single(x => x.SurveyCode == surveyCode);

                //Check that the user is allowed to see the Survey
                if (survey.User != null)
                {
                    if (User.Identity.GetUserId() != survey.User.AspUserId)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }

                //Create Survey view model
                surveyViewModel = new SurveyViewModel
                {
                    SurveyId = survey.SurveyId,
                    Name = survey.Name,
                    SurveyItems =
                        survey.SurveyItems.Count > 0
                            ? MapSurveyItemsToSurveyItemsViewmodel(survey.SurveyItems)
                            : new List<SurveyItemViewModel>()
                };

                var surveyItemViewModel = surveyViewModel.SurveyItems.LastOrDefault();
                if (surveyItemViewModel != null)
                    surveyItemViewModel.Last = true;

                surveyViewModel.SurveyItems.ForEach(x =>
                {
                    var answerViewModel = x.Answers.LastOrDefault();
                    if (answerViewModel != null)
                        answerViewModel.Last = true;
                });
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            

            //Return data to frontend
            return Request.CreateResponse(HttpStatusCode.OK, surveyViewModel);
        }

        [HttpPost]
        [Route("CreateNewSurveyItem/{surveyId}")]
        public HttpResponseMessage CreateNewSurveyItem(int surveyId)
        {
            //Find survey
            var survey = db.SurveyEditables.Find(surveyId);

            //Check that user is allowed to make changes to the Survey
            if (survey.User != null)
            {
                if (User.Identity.GetUserId() != survey.User.AspUserId)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }

            //Create new SurveyItem with order
            var newSurveyItem = new SurveyItemEditable
            {
                Order = survey.SurveyItems.Count + 1,
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
            };

            survey.SurveyItems.Add(newSurveyItem);

            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }

        [HttpPost]
        [Route("CreateNewAnswer/{surveyItemId}")]
        public HttpResponseMessage CreateNewAnswer(int surveyItemId)
        {
            //Find SurveyItem
            var surveyItem = db.SurveyItemEditables.Find(surveyItemId);

            //Check that user is allowed to make changes to the Survey
            if (surveyItem.Survey.User != null)
            {
                if (User.Identity.GetUserId() != surveyItem.Survey.User.AspUserId)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }

            //Add new answer and order
            surveyItem.Answers.Add(new AnswerEditable
            {
                Order = surveyItem.Answers.Count + 1
            });

            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }

        [HttpPost]
        [Route("SaveSurveyItem")]
        public HttpResponseMessage SaveSurveyItem([FromBody] updateData data)
        {
            //Find SurveyItem
            var surveyItem = db.SurveyItemEditables.Find(data.Id);

            //Check that user is allowed to make changes to the Survey
            if (surveyItem.Survey.User != null)
            {
                if (User.Identity.GetUserId() != surveyItem.Survey.User.AspUserId)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }

            //Save Changes
            surveyItem.QuestionText = data.Text;
            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }

        [HttpPost]
        [Route("SaveAnswer")]
        public HttpResponseMessage SaveAnswer([FromBody] updateData data)
        {
            //Find Answer
            var answer = db.AnswerEditables.Find(data.Id);

            //Check that user is allowed to make changes to the Survey
            if (answer.SurveyItem.Survey.User != null)
            {
                if (User.Identity.GetUserId() != answer.SurveyItem.Survey.User.AspUserId)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }

            //Save Changes
            answer.AnswerText = data.Text;
            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }

        [HttpPost]
        [Route("DeleteAnswer/{answerId}")]
        public HttpResponseMessage DeleteAnswer(int answerId)
        {
            //Get answer and surveyItem
            var answer = db.AnswerEditables.Find(answerId);
            var surveyItem = db.SurveyItemEditables.Find(answer.SurveyItem.SurveyItemId);

            //Check that user is allowed to make changes to the Survey
            if (surveyItem.Survey.User != null)
            {
                if (User.Identity.GetUserId() != surveyItem.Survey.User.AspUserId)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }

            //Remove answer
            surveyItem.Answers.Remove(answer);

            //Reorder the Answers
            var index = 1;
            foreach (var item in surveyItem.Answers)
            {
                item.Order = index;
                index++;
            }

            db.SaveChanges();

            //Return the new surveyItem
            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }

        [HttpPost]
        [Route("DeleteSurveyItem/{surveyItemId}")]
        public HttpResponseMessage DeleteSurveyItem(int surveyItemId)
        {
            //Get surveyItem and Survey
            var surveyItem = db.SurveyItemEditables.Find(surveyItemId);
            var survey = db.SurveyEditables.Find(surveyItem.Survey.SurveyId);

            //Check that user is allowed to make changes to the Survey
            if (survey.User != null)
            {
                if (User.Identity.GetUserId() != survey.User.AspUserId)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }

            //Delete SurveyItem
            survey.SurveyItems.Remove(surveyItem);

            //Reorder the SurveyItems
            var index = 1;
            foreach (var item in survey.SurveyItems)
            {
                item.Order = index;
                index++;
            }

            db.SaveChanges();

            //Return the new surveyItems
            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }

        [HttpPost]
        [Route("MoveSurveyItemUp/{surveyItemId}")]
        public HttpResponseMessage MoveSurveyItemUp(int surveyItemId)
        {
            //Find surveyItem
            var surveyItem = db.SurveyItemEditables.Find(surveyItemId);

            //Check that user is allowed to make changes to the Survey
            if (surveyItem.Survey.User != null)
            {
                if (User.Identity.GetUserId() != surveyItem.Survey.User.AspUserId)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }

            // Cannot move the first surveyItem up
            if (surveyItem.Order != 1)
            {
                // Swop the order of the affected items
                var survey = db.SurveyEditables.Find(surveyItem.Survey.SurveyId);
                var item = survey.SurveyItems.Single(x => x.Order == surveyItem.Order - 1);
                item.Order = item.Order + 1;
                surveyItem.Order = surveyItem.Order - 1;
                db.SaveChanges();
            }

            //Return the new surveyItems
            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }

        [HttpPost]
        [Route("MoveSurveyItemDown/{surveyItemId}")]
        public HttpResponseMessage MoveSurveyItemDown(int surveyItemId)
        {
            //Find surveyItem and Survey
            var surveyItem = db.SurveyItemEditables.Find(surveyItemId);
            var survey = db.SurveyEditables.Find(surveyItem.Survey.SurveyId);

            //Check that user is allowed to make changes to the Survey
            if (survey.User != null)
            {
                if (User.Identity.GetUserId() != survey.User.AspUserId)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }

            // Cannot move the last surveyItem down
            if (surveyItem.Order != survey.SurveyItems.OrderBy(x => x.Order).LastOrDefault()?.Order)
            {
                // Swop the order of the affected items
                var item = survey.SurveyItems.Single(x => x.Order == surveyItem.Order + 1);
                item.Order = item.Order - 1;
                surveyItem.Order = surveyItem.Order + 1;
                db.SaveChanges();
            }

            //Return the new surveyItems
            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }

        [HttpPost]
        [Route("MoveAnswerUp/{answerId}")]
        public HttpResponseMessage MoveAnswerUp(int answerId)
        {
            //Find answer
            var answerEditable = db.AnswerEditables.Find(answerId);

            //Check that user is allowed to make changes to the Survey
            if (answerEditable.SurveyItem.Survey.User != null)
            {
                if (User.Identity.GetUserId() != answerEditable.SurveyItem.Survey.User.AspUserId)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }

            // Cannot move the first Answer up
            if (answerEditable.Order != 1)
            {
                // Swop the order of the affected items
                var surveyItemEditable = db.SurveyItemEditables.Find(answerEditable.SurveyItem.SurveyItemId);
                var item = surveyItemEditable.Answers.Single(x => x.Order == answerEditable.Order - 1);
                item.Order = item.Order + 1;
                answerEditable.Order = answerEditable.Order - 1;
                db.SaveChanges();
            }

            //Return the new surveyItems
            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }

        [HttpPost]
        [Route("MoveAnswerDown/{answerId}")]
        public HttpResponseMessage MoveAnswerDown(int answerId)
        {
            var answerEditable = db.AnswerEditables.Find(answerId);
            var surveyItemEditable = db.SurveyItemEditables.Find(answerEditable.SurveyItem.SurveyItemId);

            //Check that user is allowed to make changes to the Survey
            if (answerEditable.SurveyItem.Survey.User != null)
            {
                if (User.Identity.GetUserId() != answerEditable.SurveyItem.Survey.User.AspUserId)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }

            // Cannot move the last Answer down
            if (answerEditable.Order != surveyItemEditable.Answers.OrderBy(x => x.Order).LastOrDefault()?.Order)
            {
                // Swop the order of the affected items
                var item = surveyItemEditable.Answers.Single(x => x.Order == answerEditable.Order + 1);
                item.Order = item.Order - 1;
                answerEditable.Order = answerEditable.Order + 1;
                db.SaveChanges();
            }

            //Return the new surveyItems
            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }

        [HttpPost]
        [Route("ChangeSurveyName")]
        public HttpResponseMessage ChangeSurveyName(updateData data)
        {
            //Find Survey
            var survey = db.SurveyEditables.Find(data.Id);

            //Check that user is allowed to make changes to the Survey
            if (survey.User != null)
            {
                if (User.Identity.GetUserId() != survey.User.AspUserId)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }

            //Replacing the old name with the new data
            survey.Name = data.Text;
            db.SaveChanges();

            //Returns status code 200 
            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }

        private List<SurveyItemViewModel> MapSurveyItemsToSurveyItemsViewmodel(
            IEnumerable<SurveyItemEditable> surveyItems)
        {
            return surveyItems.Select(surveyItem => new SurveyItemViewModel
            {
                QuestionText = surveyItem.QuestionText,
                Answers =
                    surveyItem.Answers.Count > 0
                        ? MapAnswersToAnswerViewmodels(surveyItem.Answers)
                        : new List<AnswerViewModel>(),
                SurveyItemId = surveyItem.SurveyItemId,
                Order = surveyItem.Order,
                Last = false
            }).OrderBy(x => x.Order).ToList();
        }

        private List<AnswerViewModel> MapAnswersToAnswerViewmodels(IEnumerable<IAnswer> answers)
        {
            return answers.Select(answer => new AnswerViewModel
            {
                AnswerText = answer.AnswerText,
                Order = answer.Order,
                AnswerId = answer.AnswerId,
                Last = false
            }).OrderBy(x => x.Order).ToList();
        }
    }

    public class updateData
    {
        public string Text { get; set; }
        public int Id { get; set; }
    }
}
