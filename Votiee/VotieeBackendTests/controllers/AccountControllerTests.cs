using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using FakeItEasy;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Shouldly;
using VotieeBackend;
using VotieeBackend.Controllers;
using VotieeBackend.Models;
using VotieeBackendTests.utils;
using Xunit;


namespace VotieeBackendTests.controllers
{
    public class AccountControllerTests : TestExtentions
    {

        private readonly AccountController controller;
        private readonly ApplicationUserManager userManager;

        public AccountControllerTests()
        {           
            userManager = A.Fake<ApplicationUserManager>();

            controller = new AccountController(dbContext, userManager) { Request = new HttpRequestMessage() };
            controller.Request.SetConfiguration(new HttpConfiguration());
        }

        [Fact]
        public void Register()
        {
            //Arrange
            A.CallTo(() => userManager.CreateAsync(A<ApplicationUser>._, A<string>._)).Returns(Task.FromResult(IdentityResult.Success));
            var registerBindingModel = new RegisterBindingModel()
            {
                Email = "test@test.dk",
                Password = "Testkode12_",
                ConfirmPassword = "Testkode12_"
            };

            //Act
            Task<HttpResponseMessage> result = controller.Register(registerBindingModel);

            //Assert
            result.GetAwaiter().GetResult().IsSuccessStatusCode.ShouldBe(true);
            var user = dbContext.Users.First();
            user.ShouldNotBeNull();
            user.AspUserId.ShouldNotBe("");
        }

        [Fact]
        public void RegisterReturnsBadRequestWhenModelStateIsInvalid()
        {
            //Arrange
            var registerBindingModel = new RegisterBindingModel()
            {
                Email = "testtest.dk",
                Password = "Testkode60_",
                ConfirmPassword = "Testkode12_"
            };
            controller.ModelState.AddModelError("Title", "Empty");

            //Act
            Task<HttpResponseMessage> result = controller.Register(registerBindingModel);

            //Assert
            result.GetAwaiter().GetResult().StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public void RegisterReturnsBadRequestWhenUserManagerCreateAsyncFails()
        {
            //Arrange
            A.CallTo(() => userManager.CreateAsync(A<ApplicationUser>._, A<string>._)).Returns(Task.FromResult(IdentityResult.Failed()));
            var registerBindingModel = new RegisterBindingModel()
            {
                Email = "testtest.dk",
                Password = "Testkode12_",
                ConfirmPassword = "Testkode12_"
            };

            //Act
            Task<HttpResponseMessage> result = controller.Register(registerBindingModel);

            //Assert
            result.GetAwaiter().GetResult().StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

    }
}
