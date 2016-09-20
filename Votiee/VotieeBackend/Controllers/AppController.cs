using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using VotieeBackend.Models;

namespace VotieeBackend.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/App")]
    public class AppController : ApiController
    {
        private readonly VotieeDbContext db;

        public AppController(VotieeDbContext DB)
        {
            db = DB;
        }

        [HttpGet]
        [Route("GetUniqueConnectionId")]
        public HttpResponseMessage GetUniqueConnectionId()
        {
            //Gererate unique guid and return to browser (where it is saved in localStorage)
            var connectionId = Guid.NewGuid();

            return Request.CreateResponse(HttpStatusCode.OK, connectionId);
        }

        [HttpGet]
        [Route("ConfirmUserToken")]
        public HttpResponseMessage ConfirmUserToken()
        {
            try
            {
                //Find the userId of the user that is logged in
                var currentUserId = User.Identity.GetUserId();

                //Find current User
                db.Users.Single(x => x.AspUserId == currentUserId);
            }
            catch (Exception)
            {

                return Request.CreateResponse(HttpStatusCode.Conflict);
            }

            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }
    }
}