/// <reference path="./button.ts" />

var ButtonComp = React.createFactory(SharedComponents.Button);

module SharedComponents {
    export interface TopBarState {
        loggedIn?: boolean;
        userName?: string;
    }

    export interface TopbarProps {
        pageName: string;
    }

    export class TopBar extends React.Component<TopbarProps, TopBarState> {
        state: TopBarState = {
            loggedIn: false,
            userName: ""
        };

        componentWillMount = () => {
            (<any>window).loggedIn = false;
            (<any>window).watch("loggedIn", this.onLoggedInStateChanged);

            if (localStorage.getItem("accessToken") == null || localStorage.getItem("accessToken") === "")
                return;

            var userName = localStorage.getItem("userName");
            (<any>window).loggedIn = true;
            this.setState({
                userName: userName
            });
        };

        componentWillUnmount = () => {
            (<any>window).unwatch("loggedIn");
        };

        render() {
            const account = this.state.loggedIn ?
                div({
                    className: `registered-user${(window.location.hash === "#menu-page/"
                        || window.location.hash === "") ? " disabled" : null}`,
                        role: "button",
                        onClick: this.goToMainMenu
                    },
                    span({ className: "glyphicon glyphicon-align-justify" })
                ) :
                div({
                        className: "anonymous-user",
                        role: "button",
                        onClick: () => window.location.href = `#login-page/`
                    },
                    span({ className: "glyphicon glyphicon-user" }));

            return div({ className: "topbar" },
                ButtonComp({
                    className: "float-item back-button",
                    onClick: () => window.history.back(),
                    disabled: (window.location.hash === "" ||
                    (window.location.hash === "#menu-page/" && this.state.loggedIn)),
                    glyphIcon: "glyphicon glyphicon-chevron-left"
                }),
                div({ className: "float-item page-name" }, this.props.pageName),
                div({ className: "float-item account-button" }, account)
            );
        }

        onLoggedInStateChanged = (id, oldVal, newVal) => {
            this.setState({ loggedIn: newVal });
        };

        goToMainMenu = () => {
            //Go to SurveysOverviewPage
            window.location.href = `#menu-page/`;
        };
    }
} 