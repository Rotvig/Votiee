/// <reference path='../typings/tsd.d.ts' />
/// <reference path='./pages/frontPage/FrontPage.ts' />
/// <reference path="./pages/notFoundPage/NotFoundPage.ts" />
/// <reference path="./pages/surveyPage/surveyPage.ts" />
/// <reference path="./pages/VotingPage/VotingPage.ts" />
/// <reference path="./pages/surveySessionPage/SurveySessionPage.ts" />
/// <reference path="./pages/loginPage/LoginPage.ts" />
/// <reference path="./pages/surveysOverviewPage/SurveysOverviewPage.ts" />
/// <reference path="./pages/statisticsPage/StatisticsPage.ts" />
/// <reference path="./utils/Server.ts" />
/// <reference path="./utils/globals.ts" />
/// <reference path="./utils/Router.ts" />
/// <reference path="./models/Models.ts" />
var router = React.createFactory(Utils.Router);
var topBar = React.createFactory(SharedComponents.TopBar);
var classNameResolver = new Utils.ResolveClassNames();

interface AppState {
    loading?: boolean;
    callingState?: boolean;
    connectionIdLoaded?: boolean;
    accessTokenConfirmed?: boolean;
}

class App extends React.Component<{}, AppState> {
    state: AppState = {
        loading: false,
        callingState: false,
        connectionIdLoaded: false,
        accessTokenConfirmed: false
    };

    componentWillMount() {
        if (localStorage.getItem("accessToken") != null && localStorage.getItem("accessToken") !== "") {
            const server = new Utils.Server();
            server.call({
                url: "api/App/ConfirmUserToken",
                methodName: "GET",
                onSuccess: () => this.setState({ accessTokenConfirmed: true }),
                onFail: () => {
                    localStorage.removeItem("userName");
                    localStorage.removeItem("accessToken");
                    (<any>window).loggedIn = false;
                    (<any>window).hasAccessToken = false;
                    this.setState({ accessTokenConfirmed: true });
                }
            });
        } else {
            this.setState({accessTokenConfirmed: true});
        }
        if (localStorage.getItem("connectionId") == null) {
                const server = new Utils.Server();
                server.call({
                    url: "api/App/GetUniqueConnectionId",
                    methodName: "GET",
                    onSuccess: this.setConnectionId
                });
            } else {
                this.setState({ connectionIdLoaded: true });
            }
        }

    componentDidMount() {
        (<any>window).loading = false;
        (<any>window).stateCalling = false;
        (<any>window).watch("loading", this.onLoadingChange);
        (<any>window).watch("stateCalling", this.onCallingStateChange);
    }

    componentWillUnmount = () => {
        (<any>window).unwatch("loading");
        (<any>window).unwatch("stateCalling");
    };

    render() {
        return div({ className: classNameResolver.resolve(this.state.callingState ? "state-calling" : "state-ready") },
            div({ className: classNameResolver.resolve(this.state.loading ? "three-quarters-loader loading" : "") }),
            div({ className: classNameResolver.resolve(this.state.loading ? "overlay" : "") }),
            this.state.connectionIdLoaded && this.state.accessTokenConfirmed ? router({ className: "main-frame container" }) : null);
    }

    onLoadingChange = (id, oldVal, newVal) => {
        this.setState({ loading: newVal });
    };
    onCallingStateChange = (id, oldVal, newVal) => {
        //This state is used to indicate that the frontend is waiting a response from the backend
        this.setState({ callingState: newVal });
    };
    setConnectionId = (connectionId: string) => {
        //Set connectionId in localStorage based on received data
        localStorage.setItem("connectionId", connectionId);
        this.setState({ connectionIdLoaded: true });
    };
}

React.render(React.createElement(App, {}), document.getElementById("react"));