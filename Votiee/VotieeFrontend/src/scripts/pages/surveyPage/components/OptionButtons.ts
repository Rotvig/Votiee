/// <reference path="../../../controls/Button.ts" />
var Button = React.createFactory(SharedComponents.Button);

module SurveyPage {

    export interface OptionButtonsProps {
        deleteEvent: Function;
        upEvent: Function;
        downEvent: Function;
        setOptionState?: (state: boolean) => void;
        lastItem: boolean;
        firstItem: boolean;
    }

    export interface OptionButtonsState {
        optionsButtonsShow?: boolean;
    }

    export class OptionButtons extends React.Component<OptionButtonsProps, OptionButtonsState> {

        state: OptionButtonsState = {
            optionsButtonsShow: false
        };

        render() {
            const optionButtons =  this.state.optionsButtonsShow
                ? div({},
                    Button({
                        className: "delete-button delete-question-button",
                        onClick: () => {
                            this.props.deleteEvent();
                            this.setState({ optionsButtonsShow: false });
                            this.props.setOptionState ? this.props.setOptionState(false) : () => { };
                        },
                        glyphIcon: "glyphicon glyphicon-trash"
                    }),
                    div({ className: "move-buttons" },
                        //Up button
                        Button({
                            className: "menu-up",
                            onClick: (event) => {
                                this.props.upEvent($(event.target).closest(".item-view"));
                                this.setState({ optionsButtonsShow: false });
                                this.props.setOptionState ? this.props.setOptionState(false) : () => { };
                            },
                            disabled:this.props.firstItem,
                            glyphIcon: "glyphicon glyphicon-menu-up"
                        }),
                        //Down button
                        Button({
                            className: "menu-down",
                            onClick: (event) => {
                                this.props.downEvent($(event.target).closest(".item-view"));
                                this.setState({ optionsButtonsShow: false });
                                this.props.setOptionState ? this.props.setOptionState(false) : () => { };
                            },
                            disabled: this.props.lastItem,
                            glyphIcon: "glyphicon glyphicon-menu-down"
                        })),
                    Button({
                        className: "option-button",
                        onClick: () => {
                            this.setState({ optionsButtonsShow: false });
                            this.props.setOptionState ? this.props.setOptionState(false) : () => {};
                        },
                        glyphIcon: "glyphicon glyphicon-option-vertical"
                    }))
                : Button({
                    className: "option-button" + (this.state.optionsButtonsShow ? " option-buttons-shown" : " option-buttons-hidden"),
                    onClick: () => {
                        this.hideOptionButtons();
                        this.setState({ optionsButtonsShow: true });
                        this.props.setOptionState ? this.props.setOptionState(true) : () =>{};
                    },
                    glyphIcon: "glyphicon glyphicon-option-vertical"
                });

            return div({ className: "option-buttons" }, optionButtons);
        }

        hideOptionButtons = () => {
            //Hide all buttons that are not hidden
            var items = $(".survey-items-view").find(".option-button");
            _.each(items, (item) => {
                (!$(item).hasClass("option-buttons-hidden")) ? item.click() : null;
            })
        }

    }
}  