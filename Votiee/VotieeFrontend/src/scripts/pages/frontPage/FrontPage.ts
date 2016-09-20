/// <reference path='../../controls/Topbar.ts' />
/// <reference path='../../controls/InputButton.ts' />
var topBar = React.createFactory(SharedComponents.TopBar);
var inputButton = React.createFactory(SharedComponents.InputButton);
var classNameResolver = new Utils.ResolveClassNames();

module FrontPage {

    //State interface
    export interface FrontPageState {
        errorMessage?: string;
    }

    export class FrontPage extends React.Component<{}, FrontPageState> {

        state: FrontPageState = {
            errorMessage: ""
        };

        render() {
            //Return render
            return div({ className: "front-page" },

                //Show Top bar
                topBar({ pageName: "" }),

                //Show logo
                div({ className: "page-top-box" },
                    img({ className: "img-responsive logo-image", src: "/images/votiee_logo.png" })
                ),

                //Show Content
                div({ className: "front-page-content" },

                    //Let the user access a survey for voting by typing it's id
                    p({ className: "participate-text" }, "Participate in survey"),
                    div({ className: "menu-button participate-button" },
                        inputButton({
                            handleButtonPressed: this.handleParticipate,
                            buttonText: null,
                            inputPlaceholder: "Enter Survey ID...",
                            inputButtonText: "Join"
                        })
                    ),
                    (this.state.errorMessage != "") ? p({ className: "session-not-found-text" }, this.state.errorMessage) : null,

                    //Button for Survey Creator menu
                    div({className: "editor-button-part"},
                        span({}, "Want to create your own surveys?"),
                    br(),
                    button({
                        className: "editor-button btn btn-default",
                        onClick: () => window.location.href = "#menu-page/"
                    }, "Go to Menu"))

                ));
        }

        handleParticipate = (input: string) => {
            //Start loading bar. Is disabled when voting is loaded
            (<any>window).loading = true;

            //Check on server that the SurveySession exists
            var server = new Utils.Server();
            server.call({
                url: `api/Service/CheckIfSessionExists/${input}`,
                onSuccess: () => window.location.href = window.location.href = `#voting-page/${(input).toUpperCase()}`,
                onFail: (data: any) => this.setErrorMessage(data.responseText.substring(1, data.responseText.length - 1))
            });
        };

        setErrorMessage = (errorMessage: any) => {
            //Stop loading bar
            (<any>window).loading = false;

            //Set "not found" text on page
            this.setState({ errorMessage: errorMessage });
        };

    }
}