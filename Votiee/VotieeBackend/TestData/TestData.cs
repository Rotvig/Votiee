using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using VotieeBackend.Models;

namespace VotieeBackend.TestData
{
    public static class TestData
    {
        public static void SetupTestData(VotieeDbContext context)
        {
            #region Survey TestData

            context.SurveyEditables.Add(new SurveyEditable
            {
                Name = "The Test One",
                User = null,
                SurveyCode = "TESTSURV"
            });

            #endregion

            #region VotingPage and PresenterPage TestData

            var survey = context.SurveyArchiveds.Add(new SurveyArchived
            {
                Name = "foo",
                SurveyItems = new List<SurveyItemArchived>
                {
                    new SurveyItemArchived
                    {
                        QuestionText = "test",
                        Order = 1,
                        Answers = new List<AnswerArchived>
                        {
                            new AnswerArchived
                            {
                                Order = 1,
                                AnswerText = "test"
                            }
                        }
                    },
                    new SurveyItemArchived
                    {
                        QuestionText = "Test2",
                        Order = 2,
                        Answers = new List<AnswerArchived>
                        {
                            new AnswerArchived
                            {
                                Order = 1,
                                AnswerText = "test2"
                            }
                        }
                    }
                }
            });

            // Adding Vote to surveyItem
            survey.SurveyItems.First().Votes.Add(new Vote
            {
                Answer = survey.SurveyItems.First().Answers.First()
            });

            context.SurveySessions.Add(new SurveySession
            {
                SessionCode = "FOOOO",
                Survey = survey,
                SurveyActive = true,
                SurveyItemActive = true,
                VotingOpen = true,
                CurrentSurveyItem = 1
            });

            context.SurveySessions.Add(new SurveySession
            {
                SessionCode = "CLOSE",
                Survey = survey,
                SurveyActive = true,
                SurveyItemActive = true,
                VotingOpen = false,
                CurrentSurveyItem = 1,
                ShowResults = false
            });

            context.SurveySessions.Add(new SurveySession
            {
                SessionCode = "INACT",
                Survey = survey,
                SurveyActive = true,
                SurveyItemActive = false,
                VotingOpen = false,
                CurrentSurveyItem = 1,
                ShowResults = false
            });

            context.SurveySessions.Add(new SurveySession
            {
                SessionCode = "RESUL",
                Survey = survey,
                SurveyActive = true,
                SurveyItemActive = true,
                VotingOpen = false,
                CurrentSurveyItem = 1,
                ShowResults = true
            });

            #endregion

            #region User Login

            var applicationDb = new ApplicationDbContext();

            //Make sure that test user for test on LoginPage is deleted
            var loginUser = applicationDb.Users.SingleOrDefault(x => x.UserName == "login@testmail.com");
            if (loginUser != null)
                applicationDb.Users.Remove(loginUser);

            //Create data for new testUser
            var userSurveyEditable2 = context.SurveyEditables.Add(new SurveyEditable
            {
                Name = "Another Test Survey",
                SurveyCode = "TEST2",
                SurveyItems = new List<SurveyItemEditable>
                {
                    new SurveyItemEditable
                    {
                        Order = 1,
                        QuestionText = "Who am I?",
                        Answers = new List<AnswerEditable>
                        {
                            new AnswerEditable
                            {
                                Order = 1,
                                AnswerText = "Me"
                            }
                        }
                    },
                    new SurveyItemEditable
                    {
                        Order = 2,
                        QuestionText = "Who are you?",
                        Answers = new List<AnswerEditable>
                        {
                            new AnswerEditable
                            {
                                Order = 1,
                                AnswerText = "Someone"
                            }
                        }
                    }
                }
            });

            //Create data for new testUser
            var userSurveyEditable = context.SurveyEditables.Add(new SurveyEditable
            {
                Name = "MyTestSurvey",
                SurveyCode = "TEST1",
                SurveyItems = new List<SurveyItemEditable>
                {
                    new SurveyItemEditable
                    {
                        Order = 1,
                        QuestionText = "First Question",
                        Answers = new List<AnswerEditable>
                        {
                            new AnswerEditable
                            {
                                Order = 1,
                                AnswerText = "Foo1"
                            }
                        }
                    },
                    new SurveyItemEditable
                    {
                        Order = 2,
                        QuestionText = "Second Question",
                        Answers = new List<AnswerEditable>
                        {
                            new AnswerEditable
                            {
                                Order = 1,
                                AnswerText = "Foo2"
                            }
                        }
                    }
                }
            });

            //Create data for new testUser
            var userSurveyArchived = context.SurveyArchiveds.Add(new SurveyArchived
            {
                Name = "MyFirstTestSurvey",
                SurveyTemplate = userSurveyEditable,
                SurveyItems = new List<SurveyItemArchived>
                {
                    new SurveyItemArchived
                    {
                        Order = 1,
                        QuestionText = "What is first?",
                        Answers = new List<AnswerArchived>
                        {
                            new AnswerArchived
                            {
                                Order = 1,
                                AnswerText = "This question is!"
                            }
                        }
                    },
                    new SurveyItemArchived
                    {
                        Order = 2,
                        QuestionText = "What is next?",
                        Answers = new List<AnswerArchived>
                        {
                            new AnswerArchived
                            {
                                Order = 1,
                                AnswerText = "foo2"
                            }
                        }
                    }
                }
            });

            //Create data for new testUser
            var userSurveyToBeDeleted = context.SurveyEditables.Add(new SurveyEditable
            {
                Name = "Test Survey To be Deleted",
                SurveyCode = "DELTE",
                SurveyItems = new List<SurveyItemEditable>
                {
                    new SurveyItemEditable
                    {
                        Order = 1,
                        QuestionText = "Who am I?",
                    }
                }
            });


            //Create test user
            applicationDb.Users.AddOrUpdate(new ApplicationUser
            {
                Id = "ad9cdaa8-e8d6-49bd-b65c-96a47c85d662",
                UserName = "test@testmail.com",
                Email = "test@testmail.com",
                PasswordHash = "AOCh79VlUf/IoTUK91DVqrN+VDjLuCODgVBs/n5NcrjlZMKQvv55ZnayIEL9bG8+bQ==",
                SecurityStamp = "8484fbbb-3885-4984-b62a-27571b979a68"
            });
            applicationDb.SaveChanges();
            var testUser = applicationDb.Users.Single(x => x.UserName == "test@testmail.com");

            //Add custom user
            var user = context.Users.Add(new User
            {
                AspUserId = testUser.Id,
                SurveyEditables = new List<SurveyEditable>
                {
                    userSurveyEditable,
                    userSurveyEditable2,
                    userSurveyToBeDeleted
                },
                SurveyArchiveds = new List<SurveyArchived>
                {
                    userSurveyArchived
                }
            });

            #endregion

            #region SurveySessionPage TestData

            context.SurveyEditables.Add(new SurveyEditable
            {
                Name = "foo",
                User = null
            });

            context.SurveyArchiveds.Add(new SurveyArchived
            {
                Name = "le foo",
                SurveyTemplate = context.SurveyEditables.Local.Last(),
                SurveyItems = new List<SurveyItemArchived>
                {
                    new SurveyItemArchived
                    {
                        Order = 1,
                        QuestionText = "NIKO LI FRÆS",
                        Answers = new List<AnswerArchived>
                        {
                            new AnswerArchived
                            {
                                Order = 1,
                                AnswerText = "foo"
                            }
                        }
                    },
                    new SurveyItemArchived
                    {
                        Order = 2,
                        QuestionText = "NIKO LI KÅS",
                        Answers = new List<AnswerArchived>
                        {
                            new AnswerArchived
                            {
                                Order = 1,
                                AnswerText = "foo2"
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
                SessionCode = "JOHNS",
                VotingOpen = false,
                SurveyItemActive = false,
                ShowResults = false
            });

            #endregion

            context.SaveChanges();
        }
    }
}