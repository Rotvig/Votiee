using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using VotieeBackend.Models;

namespace VotieeBackend.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<VotieeDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "VotieeBackend.Models.VotieeDbContext";
        }

        protected override void Seed(VotieeDbContext context)
        {
            #region Test data

            var condeemedSurvey = context.SurveyEditables.SingleOrDefault(x => x.SurveyCode == "7W2PR6D3");

            if (condeemedSurvey != null)
            {
                condeemedSurvey.SurveyCode = "7W2PR6D3Deleted";
                context.SaveChanges();
            }

            context.SurveyEditables.Add(new SurveyEditable
            {
                Name = "Survey til afvikling",
                SurveyCode = "7W2PR6D3",
                SurveyItems = new List<SurveyItemEditable>
                {
                    new SurveyItemEditable
                    {
                        Order = 1,
                        QuestionText = "Spørgsmål 1",
                        Answers = new List<AnswerEditable>
                        {
                            new AnswerEditable
                            {
                                Order = 1,
                                AnswerText = "Svar 1"
                            }
                        }
                    },
                    new SurveyItemEditable
                    {
                        Order = 2,
                        QuestionText = "Spørgsmål 2",
                        Answers = new List<AnswerEditable>
                        {
                            new AnswerEditable
                            {
                                Order = 1,
                                AnswerText = "Svar 2"
                            }
                        }
                    }
                }
            });

            var condeemedSurvey2 = context.SurveyEditables.SingleOrDefault(x => x.SurveyCode == "8W2PR6D3");

            if (condeemedSurvey2 != null)
            {
                condeemedSurvey2.SurveyCode = "8W2PR6D3Deleted";
                context.SaveChanges();
            }

            context.SurveyEditables.Add(new SurveyEditable
            {
                Name = "Survey ikke til afvikling",
                SurveyCode = "8W2PR6D3",
                SurveyItems = new List<SurveyItemEditable>
                {
                    new SurveyItemEditable
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
                    }
                }
            });

            var sessions = context.SurveySessions.Where(x => x.SessionCode == "YTGHS").ToList();

            if (sessions.Count > 0)
            {
                foreach (var surveySession in sessions)
                {
                    surveySession.SessionCode = "YTGHSDeleted";
                }
            }

            context.SurveyArchiveds.Add(new SurveyArchived
            {
                Name = "Survey",
                SurveyTemplate = context.SurveyEditables.Local.Last(),
                SurveyItems = new List<SurveyItemArchived>
                {
                    new SurveyItemArchived
                    {
                        Order = 1,
                        QuestionText = "Question 1",
                        Answers = new List<AnswerArchived>
                        {
                            new AnswerArchived
                            {
                                Order = 1,
                                AnswerText = "Answer"
                            }
                        }
                    },
                    new SurveyItemArchived
                    {
                        Order = 2,
                        QuestionText = "Question 1",
                        Answers = new List<AnswerArchived>
                        {
                            new AnswerArchived
                            {
                                Order = 1,
                                AnswerText = "Answer"
                            }
                        }
                    }
                }
            });

            context.SurveySessions.Add(new SurveySession
            {
                Survey = context.SurveyArchiveds.Local.Last(),
                CurrentSurveyItem = 1,
                SurveyActive = true,
                SessionCode = "YTGHS",
                VotingOpen = false,
                SurveyItemActive = false,
                ShowResults = false
            });

            #endregion

            #region Setting up users

            var applicationDb = new ApplicationDbContext();
            var loginUser1 = applicationDb.Users.SingleOrDefault(x => x.UserName == "ny@test.dk");
            if (loginUser1 != null)
                applicationDb.Users.Remove(loginUser1);

            var loginUser2 = applicationDb.Users.SingleOrDefault(x => x.UserName == "testikkefunk@test.dk");
            if (loginUser2 != null)
                applicationDb.Users.Remove(loginUser2);

            var loginUser3 = applicationDb.Users.SingleOrDefault(x => x.UserName == "tomtest@test.dk");
            if (loginUser3 != null)
                applicationDb.Users.Remove(loginUser3);

            applicationDb.SaveChanges();

            //Create test user
            applicationDb.Users.AddOrUpdate(new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "tomtest@test.dk",
                Email = "tomtest@test.dk",
                PasswordHash = "AFbslKfzKQA1PiS0bp552npmMrFzyXlHfqk2/qg1Jp1+rJpWZZrjtNuvW7HCsKzPRA==",
                SecurityStamp = "e2fb139d-5d60-489f-9339-537bcbc45b8c"
            });

            applicationDb.SaveChanges();

            var testUser = applicationDb.Users.SingleOrDefault(x => x.UserName == "tomtest@test.dk");
            //Add custom user
            context.Users.Add(new User
            {
                AspUserId = testUser.Id
            });

            var loginUser4 = applicationDb.Users.SingleOrDefault(x => x.UserName == "test@test.dk");
            if (loginUser4 != null)
                applicationDb.Users.Remove(loginUser4);

            applicationDb.SaveChanges();

            //Create test user
            applicationDb.Users.AddOrUpdate(new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "test@test.dk",
                Email = "test@test.dk",
                PasswordHash = "AFbslKfzKQA1PiS0bp552npmMrFzyXlHfqk2/qg1Jp1+rJpWZZrjtNuvW7HCsKzPRA==",
                SecurityStamp = "f2fb139d-5d60-489f-9339-537bcbc45b8c"
            });
            applicationDb.SaveChanges();

            var testUser2 = applicationDb.Users.SingleOrDefault(x => x.UserName == "test@test.dk");

            foreach (var survey in context.SurveyEditables.Where(x => x.SurveyCode == "922PR6D3").ToList())
            {
                survey.SurveyCode = "922PR6D3Deleted";
            }
            context.SaveChanges();

            foreach (var survey in context.SurveyEditables.Where(x => x.SurveyCode == "102PR6D3").ToList())
            {
                survey.SurveyCode = "102PR6D3Deleted";
            }
            context.SaveChanges();

            //Add custom user
            var surveyEditable = new SurveyEditable
            {
                Name = "Test Survey",
                SurveyCode = "922PR6D3",
                SurveyItems = new List<SurveyItemEditable>
                {
                    new SurveyItemEditable
                    {
                        Order = 1,
                        QuestionText = "Spørgsmål 1",
                        Answers = new List<AnswerEditable>
                        {
                            new AnswerEditable
                            {
                                Order = 1,
                                AnswerText = "Svar 1"
                            },
                            new AnswerEditable
                            {
                                Order = 2
                            }
                        }
                    }
                }
            };

            var answerArchived = new AnswerArchived
            {
                Order = 1,
                AnswerText = "Svar1"
            };

            context.Users.Add(new User
            {
                AspUserId = testUser2.Id,
                SurveyEditables = new List<SurveyEditable>
                {
                    surveyEditable,
                    new SurveyEditable
                    {
                        Name = "Votiee",
                        SurveyCode = "102PR6D3",
                        SurveyItems = new List<SurveyItemEditable>
                        {
                            new SurveyItemEditable
                            {
                                Order = 1,
                                QuestionText = "Spørgsmål",
                                Answers = new List<AnswerEditable>
                                {
                                    new AnswerEditable
                                    {
                                        AnswerText = "Svar",
                                        Order = 1
                                    },
                                    new AnswerEditable
                                    {
                                        Order = 2
                                    }
                                }
                            }
                        }
                    }
                },
                SurveyArchiveds = new List<SurveyArchived>
                {
                    new SurveyArchived
                    {
                        Name = "Statistik survey",
                        SurveyTemplate = surveyEditable,
                        SurveyItems = new List<SurveyItemArchived>
                        {
                            new SurveyItemArchived
                            {
                                Order = 1,
                                QuestionText = "Spørgsmål 1",
                                Answers = new List<AnswerArchived>
                                {
                                    answerArchived
                                },
                                Votes = new List<Vote>
                                {
                                    new Vote
                                    {
                                        Answer = answerArchived,
                                        UserConnectionId = Guid.NewGuid().ToString()
                                    },
                                    new Vote
                                    {
                                        Answer = answerArchived,
                                        UserConnectionId = Guid.NewGuid().ToString()
                                    }
                                }
                            },
                            new SurveyItemArchived
                            {
                                Order = 2,
                                QuestionText = "Spørgsmål 2",
                                Answers = new List<AnswerArchived>
                                {
                                    new AnswerArchived
                                    {
                                        Order = 1,
                                        AnswerText = "Svar 1"
                                    }
                                }
                            }
                        }
                    },
                    new SurveyArchived
                    {
                        Name = "Sletbar survey",
                        SurveyTemplate = surveyEditable,
                        SurveyItems = new List<SurveyItemArchived>
                        {
                            new SurveyItemArchived
                            {
                                Order = 1,
                                QuestionText = "Spørgsmål 1",
                                Answers = new List<AnswerArchived>
                                {
                                    new AnswerArchived
                                    {
                                        Order = 1,
                                        AnswerText = "Svar1"
                                    }
                                }
                            }
                        }
                    },
                    new SurveyArchived
                    {
                        Name = "Test statistik Survey",
                        SurveyTemplate = surveyEditable,
                        SurveyItems = new List<SurveyItemArchived>
                        {
                            new SurveyItemArchived
                            {
                                Order = 1,
                                QuestionText = "Spørgsmål 1",
                                Answers = new List<AnswerArchived>
                                {
                                    new AnswerArchived
                                    {
                                        Order = 1,
                                        AnswerText = "Svar1"
                                    }
                                }
                            }
                        }
                    }
                }
            });

            #endregion

            context.SaveChanges();
        }
    }
}

