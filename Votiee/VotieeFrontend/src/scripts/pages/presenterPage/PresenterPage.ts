/// <reference path="../../controls/Piechart.ts"/>
var pieChart = React.createFactory(SharedComponents.Piechart);
var legendComponent = React.createFactory(SharedComponents.Legend);

module PresenterPage {
    export interface PresenterPageProps {
        sessionCode: string;
    }

    export interface PresenterState {
        surveyActive?: boolean;
        surveyItemActive?: boolean;
        votingOpen?: boolean;
        showResults?: boolean;
        selectedAnswer?: number;
        surveyItem?: Models.SurveyItemViewModel;
        answerResults?: Models.AnswerResultViewModel[];
    }

    export class PresenterPage extends React.Component<PresenterPageProps, PresenterState> {

        state: PresenterState ={
            surveyActive: false,
            surveyItemActive: false,
            votingOpen: true,
            showResults: false,
            selectedAnswer: 0,
            surveyItem: new Models.SurveyItemViewModel(),
            answerResults: []
        };

        componentWillMount() {
            // This is to remove max-width from body
            $("body").addClass("presenter-mode");
            //Load SurveySession data
            this.loadSurveySession();

            //Connect to hub to receive signalR events
            this.connectToHub();
        }

        componentWillReceiveProps(nextProps: PresenterPageProps) {
            // If the SessionCode hasent changed then dont reload the data
            // This is to avoid the initial change, so it dosent go into a loop
            if (this.props.sessionCode === nextProps.sessionCode)
                return;
            //Load SurveySession data
            this.loadSurveySession(nextProps.sessionCode);

            //Connect to hub to receive signalR events
            this.connectToHub(nextProps.sessionCode);
        }

        componentWillUnmount() {
            $("body").removeClass("presenter-mode");
        }

        render() {
            let questionText = null;
            if (!this.state.surveyActive) {
                questionText = h1({}, "Survey is not active...");
            } else {
                questionText = (this.state.surveyItem != null && this.state.surveyItem.QuestionText != null)
                    ? h1({ className: "question-text" }, this.state.surveyItem.QuestionText)
                    : div({ className: "waiting" },
                        h1({}, "Waiting for something to happen..."),
                        div({ className: "ball-loader" })
                    );
            }

            const answerResults = this.state.showResults
                ? div({ className: "show-results-view" },
                    pieChart({ height: 300, width: 300, answerResults: this.state.answerResults, showLegends: true, sliceText:  (d: Models.AnswerResultViewModel) => d.VoteCount}))
                : null;

            const answerPosibilities = this.state.surveyActive && answerResults === null && this.state.surveyItem != null
                ? _.map(this.state.surveyItem.Answers, (x: Models.Answer) =>
                    div({ className: "answer-posi" }, span({className: "answer-text"}, x.AnswerText)))
                : null;

            return div({ className: "presenter-page" },
                div({ className: "header" },
                    div({ className: "page-top-box" },
                        img({ className: "img-responsive logo-image", src: "/images/votiee_logo.png" })
                    ),
                    span({}, `Go to www.votiee.com and enter ${this.props.sessionCode} to participate`)),
                div({ className: "content" },
                    questionText,
                    answerResults,
                    answerPosibilities));
        }

        loadSurveySession = (nextSessionCode: string = null) => {
            var sessionCode = nextSessionCode != null ? nextSessionCode : this.props.sessionCode;
            // Gets the surveySession from server
            var server = new Utils.Server();
            server.call({
                url: `api/Presenter/LoadSurveySession/${sessionCode}`,
                methodName: "GET",
                onSuccess: this.surveyLoaded,
                onFail: this.surveyNotLoaded
            });
        };
        surveyLoaded = (data: Models.PresenterViewModel) => {
            //Remove loading spinner
            (<any>window).loading = false;

            //Set state data based on received data
            this.setState({
                surveyActive: data.SurveyActive,
                surveyItemActive: data.SurveyItemActive,
                surveyItem: data.SurveyItem,
                selectedAnswer: data.SelectedAnswer,
                votingOpen: data.VotingOpen,
                showResults: data.ShowResults,
                answerResults: data.AnswerResults
            });
        };
        surveyNotLoaded = () => {
            //Show 'not found page' if survey is not loaded.
            window.location.href = `#not-found-page`;
        };
        connectToHub = (nextSessionCode: string = null) => {
            var sessionCode = nextSessionCode != null ? nextSessionCode : this.props.sessionCode;
            //Connect to hub with specified surveyID
            var server = new Utils.Server;

            server.setupHubProxy({
                hubName: "SurveySessionHub",
                methods: [
                    {
                        name: "update",
                        method: () => this.loadSurveySession()
                    }
                ],
                onSuccess: () => { this.onHubConnected(sessionCode); }
            });
        };
        onHubConnected = (sessionCode: string) => {
            /*When hub is connected, join the group for the survey 
            to receive incomming notifications from host*/
            const server = new Utils.Server();
            server.invokeMethodOnHub({
                methodName: "JoinParticipantGroup",
                data: sessionCode
            });
        };
    }
}