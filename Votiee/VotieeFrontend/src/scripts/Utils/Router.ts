/// <reference path="../pages/presenterPage/PresenterPage.ts" />
/// <reference path="../pages/menuPage/MenuPage.ts" />
/// <reference path="../pages/statisticsOverviewPage/StatisticsOverviewPage.ts" />
var notFoundPage = React.createFactory(NotFoundPage.NotFoundPage);
var frontPage = React.createFactory(FrontPage.FrontPage);
var menuPage = React.createFactory(MenuPage.MenuPage);
var surveyPage = React.createFactory(SurveyPage.SurveyPage);
var surveyItemPage = React.createFactory(VotingPage.SurveyItemPage);
var loginPage = React.createFactory(LoginPage.LoginPage);
var surveysOverviewPage = React.createFactory(SurveysOverviewPage.SurveysOverviewPage);
var statisticsOverviewPage = React.createFactory(StatisticsOverviewPage.SurveysOverviewPage);
var statisticsPage = React.createFactory(StatisticsPage.StatisticsPage);
var surveySessionPage = React.createFactory(SurveySessionPage.SurveySessionPage);
var presenterPage = React.createFactory(PresenterPage.PresenterPage);

module Utils {

    export interface RouterState {
        route?: string;
        hasAccessToken?: boolean;
    }

    export class Router extends React.Component<{}, RouterState> {

        state: RouterState =
        {
            route: window.location.hash.substr(1),
            hasAccessToken: localStorage.getItem("accessToken") != null && localStorage.getItem("accessToken") !== ""
        };

        componentDidMount() {
            (<any>window).hasAccessToken = localStorage.getItem("accessToken") != null && localStorage.getItem("accessToken") !== "" ;
            (<any>window).watch("hasAccessToken", this.onhasAccessTokenChange);
            
            window.addEventListener("hashchange", () => {
                this.setState({
                    route: window.location.hash.substr(1)
                });
            });
        }

        render() {
            let child = null;
            switch (this.state.route) {
            case "":
                    if (localStorage.getItem("accessToken") == null || localStorage.getItem("accessToken") === "")
                //Go to front page if not logged in
                    child = frontPage({});
                else {
                    //Go to menu page if logged in
                    child = menuPage({});
                }
                break;
            case "menu-page/":
                child = menuPage({});
                break;
            case `survey-page/${window.location.hash.substr(1).split("/")[1]}`:
                    child = surveyPage({ surveyCode: (window.location.hash.substr(1).split("/")[1]) });
                break;
            case `voting-page/${window.location.hash.substr(1).split("/")[1]}`:
                child = surveyItemPage({ sessionCode: (window.location.hash.substr(1).split("/")[1]) });
                break;
            case `presenter-page/${window.location.hash.substr(1).split("/")[1]}`:
                child = presenterPage({ sessionCode: (window.location.hash.substr(1).split("/")[1]) });
                break;
            case `survey-session-page/${window.location.hash.substr(1).split("/")[1]}`:
                child = surveySessionPage({ sessionCode: (window.location.hash.substr(1).split("/")[1]) });
                break;
            case "login-page/":
                    if (localStorage.getItem("accessToken") == null || localStorage.getItem("accessToken") === "")
                //Go to login page if not logged in
                    child = loginPage({});
                else {
                    // if you are logged in then go back
                    window.history.back();
                }
                break;
            case "surveys-overview-page/":
                child = surveysOverviewPage({});
                break;
            case "statistics-overview-page/":
                child = statisticsOverviewPage({});
                break;
            case `statistics-page/${window.location.hash.substr(1).split("/")[1]}`:
                child = statisticsPage({ surveyArchivedId: +(window.location.hash.substr(1).split("/")[1]) });
                break;
            case "not-found-page/":
                child = notFoundPage({});
                break;
            default:
                child = notFoundPage({});
            }

            return child;
        }

        onhasAccessTokenChange = (id, oldVal, newVal) => {
            this.setState({hasAccessToken: newVal});
        }
    }
}