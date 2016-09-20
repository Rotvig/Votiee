/// <reference path="./surveyItemStatisticsView.ts" />
var SurveyItemStatisticsViewComponent = React.createFactory(StatisticsPage.SurveyItemStatisticsView);

module StatisticsPage {

    export interface SurveyItemsStatisticsViewProps {
        surveyItems: Array<Models.SurveyItemStatisticsViewModel>;
    }

    export class SurveyItemsStatisticsView extends React.Component<SurveyItemsStatisticsViewProps,{}> {

        //Render SurveyItems
        render() {
            return div({ className: "survey-items-view" },
                _.map(this.props.surveyItems, (surveyItem) => SurveyItemStatisticsViewComponent({surveyItem: surveyItem })));
        }
    }
}