/// <reference path="components/UserMenuView.ts" />
/// <reference path="components/AnonymousMenuView.ts" />
var topBar = React.createFactory(SharedComponents.TopBar);
var userMenuView = React.createFactory(MenuPage.UserMenuView);
var anonymousMenuView = React.createFactory(MenuPage.AnonymousMenuView);

module MenuPage {

    export interface MenuPageState {
        loggedIn?: boolean;
    }

    export class MenuPage extends React.Component<{}, MenuPageState> {
        state: MenuPageState = {
            loggedIn: false
        };

        componentWillMount = () => {
            //Stop loading spinner
            (<any>window).loading = false;

            //Set state to indicate if user is logged in or is anonnymous
            if (localStorage.getItem("accessToken") != null)
                this.setState({loggedIn: true});
        };

        render() {
            return div({ className: "menu-page" },

                //Show Top bar
                topBar({ pageName: "Menu" }),
                this.state.loggedIn ? 
                    userMenuView() :
                    anonymousMenuView()               
            );
        }
    }
} 