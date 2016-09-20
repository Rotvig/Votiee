using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using VotieeBackend.Migrations;
using VotieeBackend.Models;

namespace VotieeBackendTests.utils
{
    public abstract class TestExtentions : IDisposable
    {
        protected readonly VotieeDbContext dbContext;


        private int lastSurveyArchivedId;
        private int lastSurveyItemArchivedId;
        private int lastAnswerArchivedId;
        private int lastSurveyEditableId;
        private int lastSurveyItemEditableId;
        private int lastAnswerEditableId;
        private int lastSurveySessionId;
        private int lastVoteId;
        private int lastUserId;

        protected TestExtentions()
        {
            dbContext = new VotieeDbContext(Effort.DbConnectionFactory.CreateTransient());
            dbContext.Database.Create();
        }

        #region SurveyEditable
        protected int CreateSurveyEditable(string name = "", User user = null, string surveyCode = "code")
        {

            var survey = new SurveyEditable
            {
                Name = name,
                User = user,
                SurveyCode = surveyCode
            };

            dbContext.SurveyEditables.Add(survey);
            dbContext.SaveChanges();
            return lastSurveyEditableId = survey.SurveyId;
        }

        protected int CreateSurveyItemEditable(string questionText = "", int order = 1)
        {
            var survey = dbContext.SurveyEditables.Find(lastSurveyEditableId);

            var surveyItem = new SurveyItemEditable
            {
                Order = order,
                QuestionText = questionText,
            };

            survey.SurveyItems.Add(surveyItem);
            dbContext.SaveChanges();
            return lastSurveyItemEditableId = surveyItem.SurveyItemId;
        }

        protected int CreateAnswerEditable(string answerText = "", int order = 1)
        {
            var surveyItem = dbContext.SurveyItemEditables.Find(lastSurveyItemEditableId);

            var answer = new AnswerEditable
            {
                AnswerText = answerText,
                Order = order
            };

            surveyItem.Answers.Add(answer);
            dbContext.SaveChanges();
            return lastAnswerEditableId = answer.AnswerId;
        }

        #endregion

        #region SurveyArchived
        protected int CreateSurveyArchived(string name = "", User user = null, SurveyEditable template = null)
        {
            var survey = new SurveyArchived
            {
                Name = name,
                User = user,
                SurveyTemplate = template
            };

            dbContext.SurveyArchiveds.Add(survey);
            dbContext.SaveChanges();
            return lastSurveyArchivedId = survey.SurveyId;
        }

        protected int CreateSurveyItemArchived(string questionText = "", int order = 1)
        {
            var survey = dbContext.SurveyArchiveds.Find(lastSurveyArchivedId);
            var surveyItem = new SurveyItemArchived
            {
                Order = order,
                QuestionText = questionText,
            };

            survey.SurveyItems.Add(surveyItem);
            dbContext.SaveChanges();
            return lastSurveyItemArchivedId = surveyItem.SurveyItemId;
        }

        protected int CreateAnswerArchived(string answerText = "", int order = 1)
        {
            var surveyItem = dbContext.SurveyItemArchiveds.Find(lastSurveyItemArchivedId);
            var answer = new AnswerArchived
            {
                AnswerText = answerText,
                Order = order
            };

            surveyItem.Answers.Add(answer);
            dbContext.SaveChanges();
            return lastAnswerArchivedId = answer.AnswerId;
        }

        #endregion

        #region SurveySession

        protected int CreateSurveySession(string sessionCode = "", 
            bool surveyActive = true, bool surveyItemActive = true, 
            int currentSurveyItem = 1, bool votingOpen = true, 
            bool showResults = false)
        {
            var surveySession = new SurveySession
            {
                SessionCode = sessionCode,
                Survey = dbContext.SurveyArchiveds.Find(lastSurveyArchivedId),
                SurveyActive = surveyActive,
                CurrentSurveyItem = currentSurveyItem,
                SurveyItemActive = surveyItemActive,
                VotingOpen = votingOpen,
                ShowResults = showResults
            };

            dbContext.SurveySessions.Add(surveySession);
            dbContext.SaveChanges();
            return lastSurveySessionId = surveySession.SurveySessionId;
        }

        #endregion

        #region Vote

        protected int CreateVote(string connectionId = null)
        {
            var surveyItem = dbContext.SurveyItemArchiveds.Find(lastSurveyItemArchivedId);
            var vote = new Vote
            {
                Answer = dbContext.AnswerArchiveds.Find(lastAnswerArchivedId),
                UserConnectionId = connectionId
            };

            surveyItem.Votes.Add(vote);
            dbContext.SaveChanges();
            return lastVoteId = vote.VoteId;
        }

        #endregion

        #region User

        protected int CreateUser(string aspUserId = "", 
            List<SurveyEditable> surveyEditables = null, 
            List<SurveyArchived> surveyArchiveds = null)
        {
            var user = new User()
            {
                AspUserId = aspUserId,
                SurveyEditables = surveyEditables ?? new List<SurveyEditable>(),
                SurveyArchiveds = surveyArchiveds ?? new List<SurveyArchived>()
            };

            dbContext.Users.Add(user);
            dbContext.SaveChanges();
            return lastUserId = user.UserId;
        }

        #endregion

        public void Dispose()
        {
            dbContext.Dispose();
        }
    }
}
