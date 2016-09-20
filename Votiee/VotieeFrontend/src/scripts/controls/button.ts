/// <reference path="../Utils/ResolveClassNames.ts" />
var ClassNameResolver = new Utils.ResolveClassNames();

module SharedComponents {
    export interface ButtonProps {
        onClick: (event: any) => void;
        glyphIcon?: string;
        className?: string;
        text?: string;
        disabled?: boolean;
    }

    export class Button extends React.Component<ButtonProps, {}> {
        render() {
            return button({
                    className: ClassNameResolver.resolve("button-default", this.props.className),
                    onClick: this.props.onClick,
                    disabled: this.props.disabled,
                    tabIndex: -1
                },
                span({
                    className: this.props.glyphIcon
                }),
                p({}, this.props.text));
        }
    }
}