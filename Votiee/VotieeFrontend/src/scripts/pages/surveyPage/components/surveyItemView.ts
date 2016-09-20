/// <reference path="./AnswerInputView.ts" />
/// <reference path="OptionButtons.ts"/>
var OptionsButtons = React.createFactory(SurveyPage.OptionButtons);
var AnswerViewComponent = React.createFactory(SurveyPage.AnswerInputView);
var Button = React.createFactory(SharedComponents.Button);

module SurveyPage {

    export interface SurveyItemViewProps {
        SurveyItem: Models.SurveyItemViewModel;
        ReloadSurvey: (callback?: Function) => void;
        surveyItemNumber: number;
    }

    export interface SurveyItemState {
        SurveyItem?: Models.SurveyItemViewModel;
        toggleCollapse?: boolean;
        questionText?: string;

    }

    export class SurveyItemView extends React.Component<SurveyItemViewProps, SurveyItemState> {
        state: SurveyItemState = {
            SurveyItem: this.props.SurveyItem,
            toggleCollapse: false,
            questionText: this.props.SurveyItem.QuestionText
        };

        componentWillReceiveProps(nextProps: SurveyItemViewProps) {
            if (!_.isEqual(this.props.SurveyItem, nextProps.SurveyItem)) {
                this.setState({
                    SurveyItem: nextProps.SurveyItem,
                    questionText: nextProps.SurveyItem.QuestionText
                });
            }
        }

        render() {
            const collapse = this.state.toggleCollapse ? "collapse" : "collapse in";
            const glyphIcon = this.state.toggleCollapse ? "glyphicon-menu-up" : "glyphicon-menu-down";
            return div({ className: "survey-item-view item-view" },
                div({ className: "survey-item" },
                    div({ className: "expand-toolbar" },
                        label({ className: "question-label" },
                            this.state.questionText === "" || this.state.questionText == null ? `Question ${this.props.surveyItemNumber}` :
                            `Question ${this.props.surveyItemNumber}: ${this.state.questionText}`),    
                            OptionsButtons({
                                deleteEvent: this.deleteSurveyItem,
                                downEvent: this.moveSurveyItemDown,
                                upEvent: this.moveSurveyItemUp,
                                firstItem: this.state.SurveyItem.Order === 1,
                                lastItem: this.state.SurveyItem.Last
                            })),
                    Button({
                        className: "toggle-button",
                        glyphIcon: `glyphicon ${glyphIcon}`,
                        onClick: () => this.setState({ toggleCollapse: !this.state.toggleCollapse })
                    }),
                    div({ className: `survey-item-background ${collapse}` },
                        div({ className: "survey-item-input" },
                            label({ className: "question-input-label" }, "Question:"),
                            input({
                                className: "form-control question-input",
                                value: this.state.questionText,
                                placeholder: "Type here...",
                                onBlur: this.saveChanges,
                                onChange: this.onChange,
                                onKeyUp: (event) => {
                                    if (event.which === 13) {
                                        //Save changes if Enter button is pressed
                                        this.saveChanges(event);
                                        $("input").blur();
                                    }
                                }
                            })),
                        _.map(this.state.SurveyItem.Answers, (answer: Models.Answer) => AnswerViewComponent({ Answer: answer, ReloadSurvey: this.props.ReloadSurvey })),
                        button({
                            className: "btn btn-default add-button",
                            onClick: this.createNewAnswer
                        }, "Add answer")))
            );
        }

        saveChanges = (event) => {
            if (!(this.state.questionText === this.state.SurveyItem.QuestionText)) {
                (<any>window).surveyPageCallingState = true;
                const server = new Utils.Server();
                server.call({
                    url: "api/Surveys/SaveSurveyItem",
                    methodName: "POST",
                    data: {
                        Id: this.props.SurveyItem.SurveyItemId,
                        Text: this.state.questionText
                    },
                    onSuccess: () => (<any>window).surveyPageCallingState = false
                });
            }
        };

        onChange = (event) => {
            this.setState({ questionText: event.target.value });
        };

        deleteSurveyItem = () => {
            (<any>window).surveyPageCallingState = true;
            const server = new Utils.Server();
            server.call({
                url: `api/Surveys/DeleteSurveyItem/${this.props.SurveyItem.SurveyItemId}`,
                methodName: "POST",
                onSuccess: () => {
                    (<any>window).surveyPageCallingState = false;
                    this.props.ReloadSurvey();
                }
            });
        };

        createNewAnswer = (event) => {
            const target = event.target;
            (<any>window).surveyPageCallingState = true;
            const server = new Utils.Server();
            server.call({
                url: `api/Surveys/CreateNewAnswer/${this.props.SurveyItem.SurveyItemId}`,
                methodName: "POST",
                onSuccess: () => {
                    (<any>window).surveyPageCallingState = false;
                    this.props.ReloadSurvey(() => {
                        $(target).prev().find(".answer-input").focus()
                    });
                }
            });
        };

        moveSurveyItemUp = (target) => {
            (<any>window).surveyPageCallingState = true;
            const server = new Utils.Server();
            server.call({
                url: `api/Surveys/MoveSurveyItemUp/${this.props.SurveyItem.SurveyItemId}`,
                methodName: "POST",
                onSuccess: () => {
                    (<any>window).surveyPageCallingState = false;
                    this.props.ReloadSurvey(() => {
                        $(target).prev().find(".option-button").first().click();
                    });
                }
            }); 
        }

        moveSurveyItemDown = (target) => {
            (<any>window).surveyPageCallingState = true;
            const server = new Utils.Server();
            server.call({
                url: `api/Surveys/MoveSurveyItemDown/${this.props.SurveyItem.SurveyItemId}`,
                methodName: "POST",
                onSuccess: () => {
                    (<any>window).surveyPageCallingState = false;
                    this.props.ReloadSurvey(() => {
                        $(target).next().find(".option-button").first().click();
                    });
                }
            });
        }
    }
} 