using Microsoft.AspNet.SignalR;

namespace VotieeBackend.Hubs
{
    public class SurveySessionHub : Hub
    {
        public void JoinHostGroup(string sessionCode)
        {
            //Add ConnectionID to group based on surveyId
            Groups.Add(Context.ConnectionId, "SurveySessionHostGroup_" + sessionCode.ToUpper());
        }

        public void JoinParticipantGroup(string sessionCode)
        {
            //Add ConnectionID to group based on surveyId
            Groups.Add(Context.ConnectionId, "SurveySessionParticipantGroup_" + sessionCode.ToUpper());
        }
    }
}