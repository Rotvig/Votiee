/// <reference path='../../../controls/ResultsView.ts' />
var ResultsView = React.createFactory(SharedComponents.ResultsView);
var AnswerResultViewComponent = React.createFactory(SharedComponents.AnswerResultView);
var classNameResolver = new Utils.ResolveClassNames();

module SurveySessionPage {

    export interface SurveyItemSessionViewProps {
        surveyItem: Models.SurveyItemSessionViewModel;
        surveySessionCode: string;
        loadSurveySession: Function;
        surveyActive: boolean;
    }

    export interface SurveyItemSessionViewState {
        toggleCollapse?: boolean;
    }

    export class SurveyItemSessionView extends React.Component<SurveyItemSessionViewProps, SurveyItemSessionViewState> {
        state: SurveyItemSessionViewState ={
            toggleCollapse: this.props.surveyItem.Active
        };

        componentWillReceiveProps(nextProps: SurveyItemSessionViewProps) {
            this.setState({ toggleCollapse: nextProps.surveyItem.Active });
        }

        render() {

            //Show question headline with state label
            const questionHeadline =
                h4({ className: "item-numb-label" }, `Question ${this.props.surveyItem.Order} `,
                    (this.props.surveyItem.Active ?
                    ((this.props.surveyItem.VotingOpen) ?
                        span({ className: "label label-success" }, "Active") :
                        span({ className: "label label-warning" }, "Paused")) :
                    ((this.props.surveyItem.VoteCount > 0) ?
                        span({ className: "label label-info" }, "Voting Done") :
                        span({ className: "label label-default" }, "Not Started"))),
                    (this.props.surveyItem.ShowResults) ?
                    span({ className: "label label-primary label-shows-results" }, "Shows Results") : null
                );

            //Active button (start/stop)
            const activeButton =
                button({
                        className: "active-button control-button btn btn-default btn-sm",
                        onClick: this.toggleActiveSurveyItem,
                        disabled: !this.props.surveyActive
                    }, span({}, (this.props.surveyItem.Active ? "Stop" : "Start ") + " "),
                    span({ className: `glyphicon ${this.props.surveyItem.Active ? "glyphicon-stop" : "glyphicon-play"}` }));

            //Pause button
            const pauseButton =
                (this.props.surveyItem.Active) ? button({
                            className: "pause-button control-button btn btn-default btn-sm",
                            onClick: this.toggleVotingOpen
                        }, span({}, (this.props.surveyItem.VotingOpen ? "Pause" : "Resume ") + " "),
                        span({ className: `glyphicon ${this.props.surveyItem.VotingOpen ? "glyphicon-pause" : "glyphicon-play"}` }))
                    : null;

            //Show results button
            const resultsButton =
                button({
                        className: "show-results-button control-button btn btn-default btn-sm",
                        onClick: this.toggleShowResults,
                        disabled: !this.props.surveyActive
                    }, span({}, (this.props.surveyItem.ShowResults ? "Hide Results " : "Show Results ") + " "),
                    span({ className: "glyphicon glyphicon-stats" }));

            const collapse = this.state.toggleCollapse ? "collapse in" : "collapse";
            const glyphIcon = this.state.toggleCollapse ? "glyphicon-menu-up" : "glyphicon-menu-down";
            return div({ className: "survey-item-view" },
                div({},
                    //Show Question
                    questionHeadline,
                    p({ className: "question-text" }, this.props.surveyItem.QuestionText),
                    //Control Buttons
                    activeButton,
                    pauseButton,
                    resultsButton,
                    //Toggle button
                    Button({
                        className: "toggle-button",
                        glyphIcon: `glyphicon ${glyphIcon}`,
                        onClick: () => this.setState({ toggleCollapse: !this.state.toggleCollapse })
                    })),
                div({ className: collapse },
                    //Show table with answers and their vote count
                    ResultsView({ answerResults: this.props.surveyItem.AnswerResults })
                ));
        }

        //Start or stop the current SurveyItem
        toggleActiveSurveyItem = () => {
            //Call Server to change status for ActiveSurveyItem and reload viewmodel data on success.
            (<any>window).loading = true;
            const server = new Utils.Server();
            server.call({
                url: `api/SurveySession/ChangeActiveSurveyItem/`,
                methodName: "POST",
                data: {
                    SessionCode: this.props.surveySessionCode,
                    Order: this.props.surveyItem.Order
                },
                onSuccess: () => { this.props.loadSurveySession(); }
            });
        };

        //Pause or resume the current SurveyItem
        toggleVotingOpen = () => {
            //Call Server to change status for ActiveSurveyItem and reload viewmodel data on success.
            (<any>window).loading = true;
            const server = new Utils.Server();
            server.call({
                url: `api/SurveySession/ToggleVotingOpen/`,
                methodName: "POST",
                data: {
                    SessionCode: this.props.surveySessionCode,
                    Order: this.props.surveyItem.Order
                },
                onSuccess: () => { this.props.loadSurveySession(); }
            });
        };

        //Pause or resume the current SurveyItem
        toggleShowResults = () => {
            //Call Server to change status for ActiveSurveyItem and reload viewmodel data on success.
            (<any>window).loading = true;
            const server = new Utils.Server();
            server.call({
                url: `api/SurveySession/ToggleShowResults/`,
                methodName: "POST",
                data: {
                    SessionCode: this.props.surveySessionCode,
                    Order: this.props.surveyItem.Order
                },
                onSuccess: () => { this.props.loadSurveySession(); }
            });
        };
    }
}