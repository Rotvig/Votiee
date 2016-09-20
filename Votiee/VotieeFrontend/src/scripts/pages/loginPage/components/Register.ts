module LoginPage {

    export interface RegisterState {
        email?: string;
        password?: string;
        confirmPassword?: string;
        errorMessage?: string;
    }

    export class Register extends React.Component<{}, RegisterState> {
        state: RegisterState = {
            email: "",
            password: "",
            confirmPassword: "",
            errorMessage: ""
        };

        render() {

            return div({ className: "register-part" }, 
                //Headline for Register
                h4({}, "Register new user account"),
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
                                if (event.which === 13 && (this.state.password !== "" && this.state.confirmPassword !== "")) {
                                    //Call handle function if enter button is pressed and all inputs have data
                                    this.handleRegister();
                                }
                            }
                        })
                        ),
                    //Input for password
                    div({ className: "form-group" },
                        label({}, "Password"),
                        input({
                            className: "input-field form-control password-input write-password",
                            'type': "password",
                            placeholder: "Type password",
                            tabIndex: 2,
                            onChange: (e: any) => {
                                this.setState({ password: e.target.value });
                            },
                            onKeyUp: (event) => {
                                if (event.which === 13 && (this.state.email !== "" && this.state.confirmPassword !== "")) {
                                    //Call handle function if enter button is pressed and all inputs have data
                                    this.handleRegister();
                                }
                            }
                        })
                        ),
                    //Input for Confirm Password
                    div({ className: "form-group" },
                        label({}, "Confirm Password"),
                        input({
                            className: "input-field form-control password-input repeat-password",
                            'type': "password",
                            placeholder: "Type password again",
                            tabIndex: 3,
                            onChange: (e: any) => {
                                this.setState({ confirmPassword: e.target.value });
                            },
                            onKeyUp: (event) => {
                                if (event.which === 13 && (this.state.email !== "" && this.state.password !== "")) {
                                    //Call handle function if enter button is pressed and all inputs have data
                                    this.handleRegister();
                                }
                            }                         
                        }),
                        p({className: "help-block"}, "The password has to be at least 6 characters long and use both letters and digits.")
                        ),
                    button({
                        className: "btn btn-default register-button",
                        tabIndex: 4,
                        onClick: this.handleRegister
                    }, "Create User"),
                    ((this.state.errorMessage !== "") ? p({ className: "error-message" }, this.state.errorMessage) : null)
            );

        }

        handleRegister = () => {
            //Get data written in the input fields
            var data = {
                Email: this.state.email,
                Password: this.state.password,
                ConfirmPassword: this.state.confirmPassword
            };

            //Start loading bar. Is disabled when surveyOverview is loaded in SurveysOverviewPage, or on register fail
            (<any>window).loading = true;

            //Call server to register
            const server = new Utils.Server();
            server.call({
                url: "api/Account/Register",
                methodName: "POST",
                data: data,
                onSuccess: this.registerSucceeded,
                onFail: this.registerFailed
            });
        };

        registerSucceeded = () => {
            //Log in automaticly after registering
            this.handleLogin();
        };

        registerFailed = () => {
            //Stop loading bar. 
            (<any>window).loading = false;

            //Set error message
            this.setState({ errorMessage: "Something went wrong. Check that you use a valid email, that your password follows the rules and that the confirm of the password matches the password."});
        };


        handleLogin = () => {
            var loginData = {
                grant_type: "password",
                username: this.state.email,
                password: this.state.password
            };

            const server = new Utils.Server();
            server.login({
                data: loginData,
                onSuccess: this.loginSucceeded
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
        
    }
}