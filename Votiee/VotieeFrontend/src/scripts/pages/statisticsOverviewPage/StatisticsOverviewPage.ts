///<reference path="components/StatisticOverviewItemView.ts"/>
var topBar = React.createFactory(SharedComponents.TopBar);
var Button = React.createFactory(SharedComponents.Button);
var statisticOverviewItemView = React.createFactory(StatisticsOverviewPage.StatisticOverviewItemView);

module StatisticsOverviewPage {

    export interface StatisticsOverviewPageState {
        surveys?: Array<Models.StatisticOverviewSurveysViewModel>;
        userName?: string;
    }

    export class SurveysOverviewPage extends React.Component<{}, StatisticsOverviewPageState> {
        state: StatisticsOverviewPageState = {
            surveys: Array<Models.StatisticOverviewSurveysViewModel>(),
            userName: ""
        };

        componentDidMount() {
            this.loadStatisticOverview();
        }

        render() {
            //Create No Statistics text when no statistics are loaded
            const noStatistics = (this.state.surveys.length === 0) ?
                div({ className: "no-statistics" },
                    p({}, "You have no survey statistics yet..."))
                : null;

            return div({ className: "statistics-overview-page" },
                //Show Top bar
                topBar({ pageName: "Statistics" }),

                //Show user information
                div({ className: "statistics-overview-page-menu" },
                    div({ className: "info-box" },
                        div({ className: "header-information" },
                            img({ className: "img-responsive logo-image", src: "/images/votiee_logo.png" }),
                            span({ className: "glyphicon glyphicon-user user-icon" }),
                            span({}, `${this.state.userName}`)
                        ))
                ),
                //Title
                div({ className: "page-title" }, "Statistics"),
                //Show no statistics if no statistics has been created
                noStatistics,
                //Show statisticsOverview
                div({ className: "statistics-overview-view" },
                    _.map(this.state.surveys, x => statisticOverviewItemView({ survey: x, surveyStatisticChanged: this.loadStatisticOverview }))
                )
            );
        }

        loadStatisticOverview = () => {
            //LoadStatisticsOverview
            const server = new Utils.Server();
            server.call({
                url: `api/StatisticsOverview/LoadStatisticsOverview`,
                methodName: "GET",
                onSuccess: this.statisticsOverviewLoaded
            });
        }

        statisticsOverviewLoaded = (data: Array<Models.StatisticOverviewSurveysViewModel>) => {
            //Stop loading bar. 
            (<any>window).loading = false;
            //Set state for viewModel
            this.setState({
                surveys: data,
                userName: localStorage.getItem("userName")
            });
        }
    }
} 