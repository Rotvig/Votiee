/// <reference path="../../../controls/InputButton.ts" />
var inputButton = React.createFactory(SharedComponents.InputButton);

module MenuPage {

    export interface UserMenuPageState {
        userName?: string;
        errorMessage?: string;
    }

    export class UserMenuView extends React.Component<{}, UserMenuPageState> {
        state: UserMenuPageState = {
            userName: "",
            errorMessage: ""
        };

        
        componentWillMount = () => {
            //Get user data
            this.setState({ userName: localStorage.getItem("userName")});
        };

        render() {
            return div({ className: "user-menu-view" },               
                //Show logo and user information
                div({ className: "info-box" },
                    div({ className: "header-information" },
                        img({ className: "img-responsive logo-image", src: "/images/votiee_logo.png" }),
                        span({ className: "glyphicon glyphicon-user user-icon" }),
                        span({}, `${this.state.userName}`)
                )),
                div({ className: "menu-buttons" },
                //Create new Survey button
                button({
                    className: "menu-button create-button btn btn-default",
                    onClick: this.handleCreateSurvey
                },
                    "Create New Survey"),
                //Load or start Survey button
                button({
                    className: "menu-button load-button btn btn-default",
                    onClick: () => {
                        (<any>window).loading = true;
                        window.location.href = "#surveys-overview-page/";
                    }
                },
                    "Edit or Start Survey"),
                //View Statistics button
                button({
                    className: "menu-button statistics-button btn btn-default",
                    onClick: () => {
                        (<any>window).loading = true;
                        window.location.href = "#statistics-overview-page/";
                    }
                },
                    "View Survey Statistics"),
                //Use as Presenter button
                div({ className: "menu-button presenter-button" },
                    inputButton({
                        handleButtonPressed: this.handleGoToPresenter,
                        buttonText: "Open Survey Presenter",
                        inputPlaceholder: "Enter Survey ID...",
                        inputButtonText: "Open"
                    })
                ),
                //Participate button
                div({ className: "menu-button participate-button" },
                    inputButton({
                        handleButtonPressed: this.handleParticipate,
                        buttonText: "Participate in Survey",
                        inputPlaceholder: "Enter Survey ID...",
                        inputButtonText: "Join"
                    })
                ),
                //Log out button
                button({
                    className: "menu-button logout-button btn btn-default",
                    onClick: this.handleLogOut
                },
                    "Log Out")),
                (this.state.errorMessage !== "") ? p({className: "error-message"}, this.state.errorMessage) : null
            );
        }

        handleCreateSurvey = () => {
            //Is disabled when survey is loaded in SurveyPage
            (<any>window).loading = true;

            const server = new Utils.Server();
            server.call({
                url: `api/Service/CreateNewSurvey`,
                methodName: "POST",
                onSuccess: (data: string) => window.location.href = `#survey-page/${data}`
            });
        };

        handleGoToPresenter = (input: string) => {
            //Start loading bar. Is disabled when Presenter is loaded
            (<any>window).loading = true;

            //Check on server that the SurveySession exists
            var server = new Utils.Server();
            server.call({
                url: `api/Service/CheckIfPresenterSessionExists/${input}`,
                onSuccess: () => window.location.href = `#presenter-page/${(input).toUpperCase() }`,
                onFail: () => this.setErrorMessage("Survey was not found...")
            });
        };

        handleParticipate = (input: string) => {
            //Start loading bar. Is disabled when voting is loaded
            (<any>window).loading = true;

            //Check on server that the SurveySession exists
            var server = new Utils.Server();
            server.call({
                url: `api/Service/CheckIfSessionExists/${input}`,
                onSuccess: () => window.location.href = window.location.href = `#voting-page/${(input).toUpperCase() }`,
                onFail: (data: any) => this.setErrorMessage(data.responseText.substring(1, data.responseText.length - 1))
            });
        };

        handleLogOut = () => {
            //Remove all login related data from localStorage
            localStorage.removeItem("accessToken");
            localStorage.removeItem("userName");
            (<any>window).loggedIn = false;
            //Go to front page
            window.location.href = `#`;
        };

        setErrorMessage = (errorMessage: string) => {
            //Make sure no spinner is on
            (<any>window).loading = false;

            //Set error message
            this.setState({errorMessage: errorMessage});
        }

    }
} 