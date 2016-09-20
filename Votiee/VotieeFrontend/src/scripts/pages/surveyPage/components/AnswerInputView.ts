/// <reference path="../../../controls/Button.ts" />
/// <reference path="OptionButtons.ts"/>
var OptionsButtons = React.createFactory(SurveyPage.OptionButtons);
var Button = React.createFactory(SharedComponents.Button);
var classNameResolver = new Utils.ResolveClassNames();

module SurveyPage {

    export interface AnswerItemViewProps {
        Answer: Models.Answer;
        ReloadSurvey: Function;
    }

    export interface AnswerItemViewState {
        AnswerText?: string;
        order?: number;
        optionsButtonsShow?: boolean;
        last?: boolean;
    }

    export class AnswerInputView extends React.Component<AnswerItemViewProps, AnswerItemViewState> {
        state: AnswerItemViewState = {
            AnswerText: this.props.Answer.AnswerText,
            order: this.props.Answer.Order,
            optionsButtonsShow: false,
            last: this.props.Answer.Last
        };

        componentWillReceiveProps(nextProps: AnswerItemViewProps) {
            if (!_.isEqual(this.props.Answer, nextProps.Answer)) {
                this.setState({
                    AnswerText: nextProps.Answer.AnswerText,
                    order: nextProps.Answer.Order,
                    last: nextProps.Answer.Last
                });
            }
        }

        render() {
            return div({ className: "answer-input-view item-view form-group" },
                label({ className: "answer-label" }, `Answer ${this.props.Answer.Order}:`),
                div({ className: "answer-field" },
                    input({
                        className: classNameResolver.resolve("answer-input form-control", this.state.optionsButtonsShow ? "option-buttons-shown": ""),
                        value: this.state.AnswerText,
                        placeholder: "Write answer",
                        onBlur: this.saveChanges,
                        onChange: this.onChange,
                        onKeyUp: (event) => {
                            if (event.which === 13) {
                                //Save changes if Enter button is pressed
                                this.saveChanges(event);
                                $('input').blur();
                            }
                        }
                    }),
                    OptionsButtons({
                        deleteEvent: this.deleteAnswer,
                        downEvent: this.moveAnswerDown,
                        upEvent: this.moveAnswerUp,
                        setOptionState: (state: boolean) => this.setState({ optionsButtonsShow: state }),
                        firstItem: this.state.order === 1,
                        lastItem: this.state.last
                    })));
        }

        saveChanges = (event) => {
            if (!(this.props.Answer.AnswerText === this.state.AnswerText)) {
                (<any>window).surveyPageCallingState = true;
                const server = new Utils.Server();
                server.call({
                    url: "api/Surveys/SaveAnswer",
                    methodName: "POST",
                    data: {
                        Id: this.props.Answer.AnswerId,
                        Text: this.state.AnswerText
                    },
                    onSuccess: () => (<any>window).surveyPageCallingState = false
                });
            }
        };

        onChange = (event) => {
            this.setState({ AnswerText: event.target.value });
        };

        deleteAnswer = () => {
            (<any>window).surveyPageCallingState = true;
            const server = new Utils.Server();
            server.call({
                url: `api/Surveys/DeleteAnswer/${this.props.Answer.AnswerId}`,
                methodName: "POST",
                onSuccess: () => {
                    (<any>window).surveyPageCallingState = false;
                    this.props.ReloadSurvey();
                }
            });
        };

        moveAnswerUp = (target) => {
            (<any>window).surveyPageCallingState = true;
            const server = new Utils.Server();
            server.call({
                url: `api/Surveys/MoveAnswerUp/${this.props.Answer.AnswerId}`,
                methodName: "POST",
                onSuccess: () => {
                    (<any>window).surveyPageCallingState = false;
                    this.props.ReloadSurvey(() => {             
                        $(target).prev().find(".option-button").first().click();
                    });
                }
            });
        }

        moveAnswerDown = (target) => {
            (<any>window).surveyPageCallingState = true;
            const server = new Utils.Server();
            server.call({
                url: `api/Surveys/MoveAnswerDown/${this.props.Answer.AnswerId}`,
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