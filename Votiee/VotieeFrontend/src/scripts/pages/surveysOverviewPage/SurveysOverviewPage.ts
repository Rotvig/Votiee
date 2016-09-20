/// <reference path="./components/SurveysView.ts" />
var topBar = React.createFactory(SharedComponents.TopBar);
var surveysView = React.createFactory(SurveysOverviewPage.SurveysView);
var Button = React.createFactory(SharedComponents.Button);

module SurveysOverviewPage {

    export interface SurveysOverviewPageState {
        surveysOverview?: Models.SurveysOverviewViewModel;
        userName?: string;
    }

    export class SurveysOverviewPage extends React.Component<{}, SurveysOverviewPageState> {
        state: SurveysOverviewPageState = {
            surveysOverview: new Models.SurveysOverviewViewModel(),
            userName: ""
        };

        //Get all the user's surveys
        componentWillMount = () => {
            this.loadSurveysOverview();
        };

        render() {

            //Create No Surveys text when no surveys are loaded
            const noSurveys = (this.state.surveysOverview.Surveys.length === 0) ?
                div({ className: "no-surveys" },
                    p({}, "You haven't created any surveys yet..."))
                : null;

            return div({ className: "surveys-overview-page" },
                //Show Top bar
                topBar({ pageName: "Surveys" }),

                //Show user information
                div({ className: "surveys-overview-page-menu" },
                    div({ className: "info-box" },
                        div({ className: "header-information" },
                            img({ className: "img-responsive logo-image", src: "/images/votiee_logo.png" }),
                            span({ className: "glyphicon glyphicon-user user-icon" }),
                            span({}, `${this.state.userName}`)
                        ))
                ),
                //Title
                div({ className: "page-title" }, "Surveys"),
                //Show no surveys if no surveys has been created
                noSurveys,
                //Show SurveysOverview
                surveysView({
                    surveys: this.state.surveysOverview.Surveys,
                    surveyChanged: this.loadSurveysOverview
                }),
                //Create new Survey button
                button({
                    className: "menu-button create-button btn btn-default",
                    onClick: this.handleCreateSurvey
                },
                    "Create New Survey")
            );
        }

        loadSurveysOverview = () => {
            const server = new Utils.Server();
            server.call({
                url: `api/SurveysOverview/LoadSurveysOverview`,
                methodName: "GET",
                onSuccess: this.surveysOverviewLoaded
            }); 
        }

        surveysOverviewLoaded = (data: Models.SurveysOverviewViewModel) => {
            //Stop loading bar. 
            (<any>window).loading = false;
            //Set state for viewModel and userName
            this.setState({
                surveysOverview: data,
                userName: localStorage.getItem("userName")
            });
        }

        handleCreateSurvey = () => {
            //Is disabled when survey is loaded in SurveyPage
            (<any>window).loading = true;

            const server = new Utils.Server();
            server.call({
                url: `api/Service/CreateNewSurvey`,
                methodName: "POST",
                onSuccess: (data: string) => window.location.href = `#survey-page/${data}`
            });
        };
    }
} 