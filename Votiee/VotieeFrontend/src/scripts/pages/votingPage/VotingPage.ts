/// <reference path='./components/AnswerSelector.ts' />
var answerSelector = React.createFactory(VotingPage.AnswerSelector);

module VotingPage {

    //Props and state interfaces
    export interface VotingPageProps {
        sessionCode: string;
    }

    export interface VotingPageState {
        surveyActive?: boolean;
        surveyItemActive?: boolean;
        answered?: boolean;     
        votingOpen?: boolean;
        showResults?: boolean;
        selectedAnswer?: number;
        surveyItem?: Models.SurveyItemViewModel;
        answerResults?: Models.AnswerResultViewModel[];
    }

    export class SurveyItemPage extends React.Component<VotingPageProps, VotingPageState>
    {
        state: VotingPageState = {
            surveyActive: false,
            surveyItemActive: false,
            answered: false,
            votingOpen: true,
            showResults: false,
            selectedAnswer: 0,
            surveyItem: new Models.SurveyItemViewModel(),
            answerResults: []
        }

        componentDidMount = () => {        
            (<any>window).loading = false;  

            //Call server to get SurveySession data
            this.loadSurveySession();
            
            //Connect to hub to receive notifications from SignalR
            this.connectToHub();
        }; 
        
        render() {
            //Show Question Text if a question is available
            const questionText = (this.state.surveyItem != null && this.state.surveyItem.QuestionText != null) ?
                h3({ className: "question-text" }, this.state.surveyItem.QuestionText) : null;

            //Show answers if SurveyItem is currently active - else show waiting message.
            const contextView = (this.state.surveyActive && this.state.surveyItemActive) ?
                //Show list of answers when SurveyItem is available
                div({ className: "content-box" },
                    answerSelector({
                        answered: this.state.answered,
                        votingOpen: this.state.votingOpen,
                        showResults: this.state.showResults,
                        answers: this.state.surveyItem.Answers,
                        answerResults: this.state.answerResults,
                        selectedAnswer: this.state.selectedAnswer,
                        selectAnswer: this.selectAnswer
                    })) :
                //If no SurveyItem is active
                ((this.state.surveyActive) ?
                    //Survey is active but no SurveyItem is active
                div({ className: "content-box" },
                    div({ className: "item-not-active" },
                    p({ className: "message" }, "Waiting for next question..."),
                    span({ className: "glyphicon glyphicon-tasks icon-waiting" }))
                    ) :
                    //Survey is not active
                    div({ className: "content-box" },
                        div({ className: "item-not-active" },
                            p({ className: "message" }, "The session has ended."),
                            span({ className: "glyphicon glyphicon-off icon-off" }),
                            p({ className: "message" }, "Thank you for participating!")
                            )
                    ));


            return div({ className: "voting-page" },
                //Show Top bar
                topBar({ pageName: "" }),

                //Show question
                div({ className: "question-box" }, questionText),
                    contextView            
            );
        }

        loadSurveySession = () => {
            //Call server to get SurveySession data
            var server = new Utils.Server();
            server.call({
                url: "api/Voting/LoadSurveySession",
                methodName: "POST",
                data: {
                    SessionCode: this.props.sessionCode,
                    ConnectionId: localStorage.getItem("connectionId")
                },
                onSuccess: this.surveyLoaded,
                onFail: this.surveyNotLoaded
            });
        } 

        surveyLoaded = (data: Models.VotingViewModel) => {
            //Set state data based on received data
            this.setState({
                surveyActive: data.SurveyActive,
                surveyItemActive: data.SurveyItemActive,
                surveyItem: data.SurveyItem,      
                answered: data.Answered,
                selectedAnswer: data.SelectedAnswer,
                votingOpen: data.VotingOpen,
                showResults: data.ShowResults,
                answerResults: data.AnswerResults
            });
        }

        surveyNotLoaded = () => {
            //Show 'not found page' if survey is not loaded.
            window.location.href = `#not-found-page`;
        }

        connectToHub = () => {
            //Connect to hub with specified surveyID
            var server = new Utils.Server();

            server.setupHubProxy({
                hubName: "SurveySessionHub",
                methods: [{
                    name: "update",
                    method: () => this.loadSurveySession()
                }],
                onSuccess: () => { this.onHubConnected(); }
            });
        }

        onHubConnected = () => {
            /*When hub is connected, join the group for the survey 
            to receive incomming notifications from host*/
            const server = new Utils.Server();
            server.invokeMethodOnHub({
                methodName: "JoinParticipantGroup",
                data: this.props.sessionCode
            });
        }

        selectAnswer = (answerChoice: number) => {
            //Tell the server which answer that was chosen.
            var server = new Utils.Server();
            server.call({
                url: "api/Voting/SubmitVote",
                methodName: "POST",
                onSuccess: this.loadSurveySession,
                data: {
                    SessionCode: this.props.sessionCode,
                    ConnectionId: localStorage.getItem("connectionId"),
                    SurveyItemOrder: this.state.surveyItem.Order,
                    AnswerSelected: answerChoice
                }}            
            );
        }
    }
} 