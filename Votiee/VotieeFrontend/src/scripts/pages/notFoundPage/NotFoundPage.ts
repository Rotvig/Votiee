/// <reference path='../../controls/Topbar.ts' />
var topBar = React.createFactory(SharedComponents.TopBar);

module NotFoundPage {

    export class NotFoundPage extends React.Component<{}, {}> {
        componentWillMount = () => {
            (<any>window).loading = false;
        };

        render() {

            return div({},
                //Show Top bar
                topBar({ pageName: "" }),
                h4({}, "Oooops! Something went wrong..."),
                a({ className: "btn btn-info", href: "", role: 'button' }, "Go to front page"));
        }
    }
}