module LoginPage {

    export interface LoginState {
        email?: string;
        password?: string;
        confirmPassword?: string;
        errorMessage?: string;
    }

    export class Login extends React.Component<{}, LoginState> {
        state: RegisterState = {
            email: "",
            password: "",
            errorMessage: ""
        };

        render() {

            return div({ className: "login-part" },
                //Headline for Register
                h4({}, "Login to existing user account"),
                //Input field for email
                div({ className: "form-group" },
                    label({}, "Email"),
                    input({
                            className: "input-field form-control email-input",
                            'type': "email",
                            placeholder: "Type Email",
                            tabIndex: 1,
                            onChange: (e: any) => {
                                this.setState({ email: e.target.value });
                            },
                            onKeyUp: (event) => {
                                if (event.which === 13 && (this.state.password !== "")) {
                                    //Call handle function if enter button is pressed and all inputs have data
                                    this.handleLogin();
                                }
                            }
                        }
                    ),
                    //Input for password
                    div({ className: "form-group" },
                        label({}, "Password"),
                        input({
                            className: "input-field form-control password-input",
                            'type': "password",
                            placeholder: "Type password",
                            tabIndex: 2,
                            onChange: (e: any) => {
                                this.setState({ password: e.target.value });
                            },
                            onKeyUp: (event) => {
                                if (event.which === 13 && this.state.email !== "") {
                                    //Call handle function if enter button is pressed and all inputs have data
                                    this.handleLogin();
                                }
                            }
                        })
                    ),
                    button({
                        className: "btn btn-default login-button",
                        tabIndex: 3,
                        onClick: this.handleLogin
                    }, "Login"),
                    ((this.state.errorMessage !== "") ? p({ className: "error-message" }, this.state.errorMessage) : null)
                )
            );
        }

        handleLogin = () => {
            //Get data written in the input fields
            var loginData = {
                grant_type: "password",
                username: this.state.email,
                password: this.state.password
            };

            //Start loading bar. Is disabled when surveyOverview is loaded in SurveysOverviewPage, or on login fail
            (<any>window).loading = true;

            //Call server to login
            const server = new Utils.Server();
            server.login({
                data: loginData,
                onSuccess: this.loginSucceeded,
                onFail: this.loginFailed
            });
        };

        loginSucceeded = (data: any) => {
            //Set localStorage to user data
            localStorage.setItem("accessToken", data.access_token);
            localStorage.setItem("userName", data.userName);
            (<any>window).loggedIn = true;
            //Go to surveys overview page
            window.location.href = `#menu-page/`;
        };

        loginFailed = (data: any) => {
            //Stop loading bar. 
            (<any>window).loading = false;

            //Set error message
            this.setState({ errorMessage: "The user was not found or the password was incorrect." });
        };

    }
}