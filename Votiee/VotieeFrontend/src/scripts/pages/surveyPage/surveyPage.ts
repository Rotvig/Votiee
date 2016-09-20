/// <reference path="./components/surveyItemsView.ts" />
var Button = React.createFactory(SharedComponents.Button);
var surveyItemsViewComponent = React.createFactory(SurveyPage.SurveyItemsView);
var topBar = React.createFactory(SharedComponents.TopBar);
var classNameResolver = new Utils.ResolveClassNames();

module SurveyPage {

    export enum valueIdentifier {
        QuestionText,
        AnswerText
    }

    export interface SurveyPageProps {
        surveyCode?: string;
    }

    export interface SurveyPageState {
        survey?: Models.SurveyViewModel;
        editSurveyName?: boolean;
        loggedIn?: boolean;
        surveyPageCallingState?: boolean;
        savingTimerOn?: boolean;
        savingTimerId?: any;
        errorMessage?: string;
        errorMessageTimerId?: any;
    }

    export class SurveyPage extends React.Component<SurveyPageProps, SurveyPageState> {

        state: SurveyPageState = {
            survey: new Models.SurveyViewModel(),
            editSurveyName: false,
            loggedIn: false,
            surveyPageCallingState: false,
            savingTimerOn: false,
            errorMessage: ""
        };

        componentWillMount = () => {
            (<any>window).surveyPageCallingState = false;
            (<any>window).watch("surveyPageCallingState", this.onCallingStateChange);
            //Get login status
            if (localStorage.getItem("accessToken") != null)
                this.setState({ loggedIn: true });

            this.loadSurvey();
        };

        componentWillUnmount = () => {
            (<any>window).unwatch("surveyPageCallingState");
        };

        render() {
            //Show survey name and let user edit it when clicked
            const surveyNameField = div({ className: "survey-name"},this.state.editSurveyName ?
                //Show input for Survey
                div({ className: "survey-name-form" }, input({
                    className: "form-control survey-name-input",
                    autoFocus: true,
                    onFocus: () => { $(".survey-name-input").val(this.state.survey.Name); },
                    onBlur: this.changeSurveyName,
                    value: this.state.survey.Name,
                    onChange: this.updateName,
                    onKeyUp: this.surveyNameInputOnKeyUp
                }),
                    button({
                        className: "btn btn-default survey-name survey-name-ok-button",
                        onClick: this.changeSurveyName
                    }, "Ok")
                ) :
                //Show Survey name text
                h4({
                        className: "survey-name-text-field"
                    },
                    span({
                            className: "survey-name-change-button",
                            role: "button",
                            onClick: () => { this.setState({ editSurveyName: true }); }
                        },
                        span({ className: "glyphicon glyphicon-pencil" }),
                        span({ className: "name survey-name-text" }, this.state.survey.Name))
                ));

            //Message about saving
            const saveAnimation = (this.state.surveyPageCallingState || this.state.savingTimerOn) ? div({ className: "spinner three-quarters-loader" }) : null;
            const savingText = div({ className: "spinner-text" }, (this.state.surveyPageCallingState || this.state.savingTimerOn) ? "Saving Changes..." : "All changes saved...");

            //Error-message and SurveyCode message
            const textMessage = div({ className: "text-message" }, (this.state.errorMessage !== "")
                ? p({ className: "error-message" },
                    this.state.errorMessage)
                : ((!this.state.loggedIn) ? p({ className: "code-message" },
                    `To load this survey later, enter the code '${this.props.surveyCode}' in the main menu.`)
                    : null)
                );

            //Return render
            return div({ className: "survey-page" },
                //Show Top bar
                topBar({ pageName: "Survey" }),
                div({ className: "survey-page-menu" },
                    div({ className: "start-bar" },
                        div({ className: "float-div" },
                            //Edit Survey Name
                            surveyNameField
                        ),
                        //Saving message
                        div({ className: "survey-spinner" },
                            saveAnimation,
                            savingText),
                        //Start Survey Session Button
                        button({
                            className: "start-button btn btn-default",
                            onClick: this.handleStart
                        }, "Start Survey"),
                        //Text message for error and SurveyCode
                        textMessage),
                    surveyItemsViewComponent({
                        SurveyItems: this.state.survey.SurveyItems,
                        ReloadSurvey: this.loadSurvey
                    }),
                    //Add question button
                    button({
                        className: "btn btn-default add-button",
                        onClick: this.createNewSurveyItem
                    }, "Add question"))
            );

        }

        loadSurvey = (callback?: Function) => {
            //Get survey
            const server = new Utils.Server();
            server.call({
                url: `api/Surveys/loadSurvey/${this.props.surveyCode}`,
                methodName: "GET",
                onSuccess: (data) => this.surveyLoaded(data, callback),
                onFail: this.surveyNotLoaded
            });
        };

        surveyLoaded = (data: Models.SurveyViewModel, callback?: Function) => {
            (<any>window).loading = false;

            this.setState({ survey: data });
            callback ? callback() : () => { };
        };

        surveyNotLoaded = () => {
            window.location.href = `#not-found-page`;
        };

        handleStart = () => {
            //Loading spinner
            (<any>window).loading = true;

            const server = new Utils.Server();
            server.call({
                url: `api/Service/CreateSurveySession/${this.props.surveyCode}`,
                methodName: "GET",
                onSuccess: this.startSurveySession,
                onFail: this.onStartFail
            });
        };

        onStartFail = () => {
            //StopLoading spinner
            (<any>window).loading = false;

            //Handle timed errorMessage
            if (this.state.errorMessageTimerId) {
                window.clearTimeout(this.state.errorMessageTimerId);
            }
            this.setState({
                errorMessage: "Can't start empty survey...",
                errorMessageTimerId: window.setTimeout(() => {
                    this.setState({ errorMessage: "" });
                }, 4000),
                savingTimerOn: false
            });
        };
        startSurveySession = (sessionCode: string) => {
            window.location.href = `#survey-session-page/${sessionCode}`;
        };

        createNewSurveyItem = (event) => {
            const target = event.target;
            (<any>window).surveyPageCallingState = true;
            const server = new Utils.Server();
            server.call({
                    url: `api/Surveys/CreateNewSurveyItem/${this.state.survey.SurveyId}`,
                    methodName: "POST",
                    onSuccess: () => {
                        (<any>window).surveyPageCallingState = false;
                        this.loadSurvey(() => {
                            $(target).prev().last().find(".question-input").focus()
                        });
                    }
                }
            );
        };

        changeSurveyName = () => {
            (<any>window).surveyPageCallingState = true;
            //Update server on blur with new survey name
            const server = new Utils.Server();
            server.call({
                    url: "api/Surveys/ChangeSurveyName",
                    methodName: "POST",
                    data: {
                        Id: this.state.survey.SurveyId,
                        Text: this.state.survey.Name
                    },
                    onSuccess: () => (<any>window).surveyPageCallingState = false
                }
            );

            //Set state for name to not in edit mode
            this.setState({ editSurveyName: false });

        };

        updateName = (event) => {
            //Update state with new survey name
            this.state.survey.Name = event.target.value;
            this.setState({ survey: this.state.survey });
        };

        surveyNameInputOnKeyUp = (event) => {
            //Blur onEnter keyup in survey-name input
            if (event.which === 13) {
                $(".survey-name-input").blur();
            }
        };
        onCallingStateChange = (id, oldVal, newVal) => {
            //This state is used to indicate that the frontend is waiting a response from the backend
            if (newVal) {
                // If a timeout exist then clear it
                if (this.state.savingTimerId) {
                    window.clearTimeout(this.state.savingTimerId);
                }
                this.setState({
                    savingTimerOn: true,
                    savingTimerId: window.setTimeout(this.onSavingTimerOff, 1400), // Create a new timeout
                    errorMessage: ""
                });
            }

            this.setState({
                surveyPageCallingState: newVal
            });
        };
        onSavingTimerOff = () => {
            this.setState({ savingTimerOn: false });
        };
    }
}