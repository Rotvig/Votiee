/// <reference path='./components/Register.ts' />
/// <reference path='./components/Login.ts' />
/// <reference path='../../controls/Topbar.ts' />
var topBar = React.createFactory(SharedComponents.TopBar);
var register = React.createFactory(LoginPage.Register);
var login = React.createFactory(LoginPage.Login);

module LoginPage {

    export interface LoginPageState {
        toggleRegister?: boolean;
    }

    export class LoginPage extends React.Component<{}, LoginPageState> {
        state: LoginPageState = {
            toggleRegister: false
        };

        componentWillMount = () => {
            //Go to surveys-overview-page if already logged in
            if (localStorage.getItem("accessToken") != null  && localStorage.getItem("accessToken") !== "")
                window.location.href = `#surveys-overview-page/`;
        };

        render() {

            //Show login or register view based on current toggle state
            const loginRegister = this.state.toggleRegister ? register() : login(); 
            
            //Create buttons to toggle between register or login
            const toggleButtons = div({ className: "button-group toggle-buttons" },
                    button({
                        className: "btn btn-default toggle-button login" + (this.state.toggleRegister ? "" : " active"),
                        onClick: () => { this.setState({ toggleRegister: false }); }
                    }, "Login"),
                    button({
                        className: "btn btn-default toggle-button register" + (this.state.toggleRegister ? " active" : ""),
                        onClick: () => { this.setState({ toggleRegister: true }); }
                    }, "Register"));

            //Return login-page
            return div({ className: "login-page" },
                //Show Top bar
                topBar({ pageName: "Login" }),

                //Show logo
                div({ className: "page-top-box" },
                    img({ className: "img-responsive logo-image", src: "/images/votiee_logo.png" })
                ),

                //Show buttons to toggle between login menu and register menu
                toggleButtons,
                //Show login menu or register menu
                div({className: "login-register"},
                loginRegister)
            );

        }
    }
}