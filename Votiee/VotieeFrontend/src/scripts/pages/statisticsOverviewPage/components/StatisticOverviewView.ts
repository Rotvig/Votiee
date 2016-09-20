var Button = React.createFactory(SharedComponents.Button);

module StatisticsOverviewPage {

    export interface StatisticOverviewViewProps {
        survey: Models.StatisticsArchivedSurveysViewModel;
        surveyStatisticChanged: Function;
    }

    export interface StatisticOverviewViewState {
        toggleDeleteConfirmation?: boolean;
    }

    export class StatisticOverviewView extends React.Component<StatisticOverviewViewProps, StatisticOverviewViewState> {

        state: StatisticOverviewViewState ={
            toggleDeleteConfirmation: false
        }; 
        
        //Update the state when recieving new props
        componentWillReceiveProps(nextProps: StatisticOverviewViewProps) {
            this.setState({ toggleDeleteConfirmation: false });
        }

        render() {
            const deleteButton = this.state.toggleDeleteConfirmation
                ? div({ className: "delete-confirmation-buttons" },
                    button({
                        className: "btn btn-default btn-sm cancel",
                        onClick: () => this.setState({ toggleDeleteConfirmation: !this.state.toggleDeleteConfirmation })
                    }, "Cancel"),
                    button({
                        className: "btn btn-danger btn-sm delete",
                        onClick: this.deleteSurvey
                    }, "Delete"))
                : Button({
                    onClick: () => this.setState({ toggleDeleteConfirmation: !this.state.toggleDeleteConfirmation }),
                    glyphIcon: "glyphicon glyphicon-trash stats-button"
                });

            const date = new Date(this.props.survey.ArchiveDate);
            return div({ className: "statistic-overview-view" },
                div({ className: "statistic-info" },
                    div({ className: "archived-date" }, date.toLocaleDateString() + " " + date.getHours() + "." + date.getMinutes()),
                    div({ className: "achived-survey-name" }, this.props.survey.Name)),
                this.state.toggleDeleteConfirmation
                ? null
                : Button({
                    className: "stats-button show-stats-button",
                    glyphIcon: "glyphicon glyphicon-stats",
                    onClick: () => { window.location.href = `#statistics-page/${this.props.survey.SurveyId}` }
                }),
                deleteButton
            );
        }

        deleteSurvey = () => {
            const server = new Utils.Server();
            server.call({
                url: `api/StatisticsOverview/DeleteSurveyArchived/${this.props.survey.SurveyId}`,
                methodName: "POST",
                onSuccess: () => this.props.surveyStatisticChanged()
            });
        };

        //Button:Reactivate the survey session from the selected survey
        handleReactivateSession = () => {
            const server = new Utils.Server();
            server.call({
                url: `api/Service/ReactivateSurveySession/${this.props.survey.SurveyId}`,
                methodName: "GET",
                onSuccess: this.startSurveySession
            });
        }; //Go to results page
        private startSurveySession = (sessionCode: string) => {
            window.location.href = `#survey-session-page/${sessionCode}`;
        };
    }
}