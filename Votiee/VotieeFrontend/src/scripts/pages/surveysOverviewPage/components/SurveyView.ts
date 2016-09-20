var Button = React.createFactory(SharedComponents.Button);

module SurveysOverviewPage {

    export interface SurveyViewProps {
        surveyCode: string;
        surveyName: string;
        surveyChanged: Function;
    }

    export interface SurveyViewState {
        toggleDeleteConfirmation?: boolean;
    }

    export class SurveyView extends React.Component<SurveyViewProps, SurveyViewState> {

        state: SurveyViewState = {
            toggleDeleteConfirmation: false
        };

        //Update the state when recieving new props
        componentWillReceiveProps(nextProps: SurveyViewProps) {
            this.setState({ toggleDeleteConfirmation: false });
        }

        render() {

            const deleteButton = this.state.toggleDeleteConfirmation
                ? div({ className: "delete-confirmation-buttons" },
                    button({
                        className: "btn btn-default btn-sm",
                        onClick: () => this.setState({ toggleDeleteConfirmation: !this.state.toggleDeleteConfirmation })
                    }, "Cancel"),
                    button({
                        className: "btn btn-danger btn-sm",
                        onClick: this.deleteSurvey
                    }, "Delete"))
                : Button({
                    onClick: () => this.setState({ toggleDeleteConfirmation: !this.state.toggleDeleteConfirmation }),
                    glyphIcon: "glyphicon glyphicon-trash"
                });
            return div({ className: "survey-view" },
                //Show title of the survey
                div({ className: "survey-label" }, this.props.surveyName),

                //Show menu buttons
                div({ className: "button-group" },
                    //Let the user start a Survey session from the selected survey
                    this.state.toggleDeleteConfirmation
                    ? null
                    : Button({
                        className: "button-default start-button",
                        onClick: this.handleStartSession,
                        glyphIcon: "glyphicon glyphicon-play"
                    }),

                    //Let the user edit an existing survey
                    this.state.toggleDeleteConfirmation
                    ? null
                    : Button({
                        className: "button-default edit-button",
                        onClick: () => window.location.href = `#survey-page/${this.props.surveyCode}`,
                        glyphIcon: "glyphicon glyphicon-pencil"
                    }),
                    deleteButton
                ));
        }

        //Button: Start a new survey session from the selected survey
        handleStartSession = () => {
            const server = new Utils.Server();
            server.call({
                url: `api/Service/CreateSurveySession/${this.props.surveyCode}`,
                methodName: "GET",
                onSuccess: this.startSurveySession
            });
        };
        //Go to survey session page
        private startSurveySession = (sessionCode: string) => {
            window.location.href = `#survey-session-page/${sessionCode}`;
        };

        deleteSurvey = () => {
            const server = new Utils.Server();
            server.call({
                url: `api/SurveysOverview/DeleteSurveyEditable/${this.props.surveyCode}`,
                methodName: "POST",
                onSuccess: () => this.props.surveyChanged()
            });
        };
    }
}  
