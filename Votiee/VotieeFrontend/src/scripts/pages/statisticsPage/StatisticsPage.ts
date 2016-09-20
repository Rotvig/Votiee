/// <reference path="./components/surveyItemsStatisticsView.ts" />
var SurveyItemsStatisticsViewComponent = React.createFactory(StatisticsPage.SurveyItemsStatisticsView);

module StatisticsPage {

    export interface StatisticsPageProps {
        surveyArchivedId: number;
    }

    export interface StatisticsPageState {
        surveyStatistics?: Models.SurveyStatisticsViewModel;
    }

    export class StatisticsPage extends React.Component<StatisticsPageProps, StatisticsPageState> {
        state: StatisticsPageState = {
            surveyStatistics: new Models.SurveyStatisticsViewModel()
        };

        //Get the statistics data from the chosen SurveyArchived
        componentWillMount = () => {
            const server = new Utils.Server();
            server.call({
                url: `api/Statistics/LoadSurveyStatistics/${this.props.surveyArchivedId}`,
                methodName: "GET",
                onSuccess: this.surveyStatisticsLoaded
            });
        };

        render() {
            //Create date
            const date = new Date(this.state.surveyStatistics.ArchiveDate);

            //Return view
            return div({ className: "statistics-page" },

                //Show Top bar
                topBar({ pageName: "Statistics" }),

                //Show survey information
                div({ className: "survey-info-box" },
                    h4({ className: 'survey-name' }, this.state.surveyStatistics.Name),
                    p({ className: 'survey-template-name' }, "(" + this.state.surveyStatistics.TemplateName + ")"),
                    p({ className: 'survey-date' }, date.toLocaleDateString() + " " + date.toLocaleTimeString()),
                    button({
                        className: "btn btn-default reactivate-button",
                        onClick: this.handleReactivateSession
                    }, "Reactivate")
                    ),
                //Show list of surveyItems
                SurveyItemsStatisticsViewComponent({ surveyItems: this.state.surveyStatistics.SurveyItems })
            );
        }

        //Set state to viewmodel when loaded
        surveyStatisticsLoaded = (data: Models.SurveyStatisticsViewModel) => {
            //Stop loading bar. 
            (<any>window).loading = false;
            //Set state for viewModel and userName
            this.setState({
                surveyStatistics: data
            });
        }

        //Button: Reactivate the survey session from the selected survey
        handleReactivateSession = () => {
            const server = new Utils.Server();
            server.call({
                url: `api/Service/ReactivateSurveySession/${this.props.surveyArchivedId}`,
                methodName: "GET",
                onSuccess: this.startSurveySession
            });
        }

        //Go to suvey session page
        private startSurveySession = (sessionCode: string) => {
            window.location.href = `#survey-session-page/${sessionCode}`;
        }

    }
} 