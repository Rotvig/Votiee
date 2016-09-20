module SharedComponents {

    export interface InputButtonProps {
        handleButtonPressed?: Function;
        buttonText?: string;
        inputPlaceholder?: string;
        inputButtonText?: string;
    }

    export interface InputButtonState {
        buttonToggle?: boolean;
        input?: string;
    }

    export class InputButton extends React.Component<InputButtonProps, InputButtonState> {
        state: InputButtonState = {
            buttonToggle: false,
            input: ""
        };

        render() {
            //Show toggle button or input field based on if the toggle button has been pressed
            //If no buttonText is given always show input
            return (this.state.buttonToggle || this.props.buttonText === null) ?
                //Show input field and go button if pressed
                div({ className: "input-button" },
                    //Show input field
                    input({
                        className: "input-button-input form-control",
                        autoFocus: true,
                        onBlur: () => {
                            //Hide input field if field is left empty
                            if (this.state.input === "" && this.props.buttonText !== null) {
                                this.setState({ buttonToggle: false });
                            }
                        },
                        placeholder: this.props.inputPlaceholder,
                        onKeyUp: (event) => {
                            if (event.which === 13 && this.state.input !== "") {
                                //Call handle function if button is pressed
                                this.props.handleButtonPressed(this.state.input);
                            }
                        },
                        //Update text state when typing in input field
                        onChange: (event) => { this.setState({ input: (<any>event.target).value }); }
                    }),
                    //Show Go button
                    button({
                        className: "btn btn-default input-button-load",
                        //Go to presenter-page with typed session code if pressed
                        onClick: () => this.props.handleButtonPressed(this.state.input),
                        disabled: (this.state.input === "")
                    }, this.props.inputButtonText)
                ) :
                //Show button if not pressed
                div({ className: "input-button" },
                    button({
                        className: "btn btn-default input-button-toggle",
                        onClick: () => { this.setState({ buttonToggle: true }); }
                    }, this.props.buttonText)
                );
        }
    }
} 