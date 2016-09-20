/// <reference path="components/SurveyItemsSessionView.ts" />
var SurveyItemsSessionViewComponent = React.createFactory(SurveySessionPage.SurveyItemsSessionView);
var classNameResolver = new Utils.ResolveClassNames();

module SurveySessionPage {

    export interface SurveySessionPageProps {
        sessionCode: string;
    }

    export interface SurveySessionPageState {
        surveySession: Models.SurveySessionViewModel;
    }

    export class SurveySessionPage extends React.Component<SurveySessionPageProps, SurveySessionPageState> {
        state: SurveySessionPageState = {
            surveySession: new Models.SurveySessionViewModel()
        };

        componentWillMount = () => {
            //Call server to get initial SurveyItem data from survey data  
            this.loadSurveySession();

            //Connect to hub to receive signalR notifications
            this.connectToHub();
        };

        render() {
            //Create header to show if survey is active or not
            const activeLabel = (this.state.surveySession.SurveyActive) ?
                null :
                span({ className: "label label-danger" }, "Not active");

            //Return view
            return div({ className: "survey-session-page" },

                //Show Top bar
                topBar({ pageName: "Session" }),

                //Show survey information
                div({ className: "survey-info-box" },
                    h4({ className: "survey-name" },
                        this.state.surveySession.Survey.Name + " ",
                        activeLabel),
                    h4({ className: "connect-text" },
                        "Connect to Survey with ID: ",
                        this.props.sessionCode),
                    //Button to toggle SurveyActive
                    button({
                            className: `${classNameResolver.resolve("btn topmenu-button", this.state.surveySession.SurveyActive ? "btn-danger" : "btn-success")}`,
                            onClick: this.toggleSurveyActive
                        },
                        this.state.surveySession.SurveyActive ? "Stop Session" : "Start Session"
                    )),
                //Show list of surveyItems
                SurveyItemsSessionViewComponent({
                    surveyItems: this.state.surveySession.Survey.SurveyItems,
                    surveySessionCode: this.props.sessionCode,
                    loadSurveySession: this.loadSurveySession,
                    surveyActive: this.state.surveySession.SurveyActive
                })
            );
        }

        loadSurveySession = () => {
            const server = new Utils.Server();
            server.call({
                url: `api/SurveySession/LoadSurveySession/${this.props.sessionCode}`,
                methodName: "GET",
                onSuccess: this.surveySessionLoaded
            });
        };
        surveySessionLoaded = (data: Models.SurveySessionViewModel) => {
            //Stop loading bar. 
            (<any>window).loading = false;
            //Set state for viewModel
            this.setState({
                surveySession: data
            });
        };

        toggleSurveyActive = () => {
            (<any>window).loading = true;
            const server = new Utils.Server();
            server.call({
                url: `api/SurveySession/ToggleSurveyActive/${this.props.sessionCode}`,
                methodName: "POST",
                onSuccess: this.loadSurveySession
            });
        };

        connectToHub = () => {
            //Connect to hub with specified surveyID
            var server = new Utils.Server();
            server.setupHubProxy({
                hubName: "SurveySessionHub",
                methods: [{ name: "updateVotes", method: () => this.loadSurveySession() }],
                onSuccess: () => { this.onHubConnected(); }
            });
        }

        onHubConnected = () => {
            /*When hub is connected, join the group for the survey 
            to receive incomming notifications from new votes.*/
            const server = new Utils.Server();
            server.invokeMethodOnHub({
                methodName: "JoinHostGroup",
                data: this.props.sessionCode
            });
        };
    }
} 