///<reference path="StatisticOverviewView.ts"/>
var statisticOverviewView = React.createFactory(StatisticsOverviewPage.StatisticOverviewView);

module StatisticsOverviewPage {

    export interface StatisticOverviewItemViewProps {
        survey: Models.StatisticOverviewSurveysViewModel;
        surveyStatisticChanged: Function;
    }

    export interface StatisticOverviewItemViewState {
        toggleCollapse?: boolean;
    }

    export class StatisticOverviewItemView extends React.Component<StatisticOverviewItemViewProps, StatisticOverviewItemViewState> {
        state: StatisticOverviewItemViewState ={
            toggleCollapse: true
        }

        render() {
            const collapse = this.state.toggleCollapse ? "collapse" : "collapse in";
            const glyphIcon = this.state.toggleCollapse? "glyphicon-menu-down" : "glyphicon-menu-up";
            return div({ className: "statistic-overview-item-view" },
                div({className:  "statistic-header"},
                h4  ({ className: "survey-name" }, this.props.survey.Name),
                Button({
                    className: "toggle-button expand-button",
                    glyphIcon: "glyphicon " + glyphIcon,
                    onClick: () => this.setState({ toggleCollapse: !this.state.toggleCollapse })
                })),
                div({ className: "surveys " + collapse },
                    _.map(this.props.survey.Surveys, x => statisticOverviewView({ survey: x, surveyStatisticChanged: this.props.surveyStatisticChanged }))));
        }
    }
}