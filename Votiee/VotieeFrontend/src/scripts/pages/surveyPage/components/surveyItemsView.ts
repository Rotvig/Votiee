/// <reference path="./surveyItemView.ts" />
var SurveyItemViewComponent = React.createFactory(SurveyPage.SurveyItemView);

module SurveyPage {

    export interface SurveyItemsViewProps {
        SurveyItems: Array<Models.SurveyItemViewModel>;
        ReloadSurvey: Function;
    }

    export interface SurveyItemsState {
        SurveyItems: Array<Models.SurveyItemViewModel>;
    }

    export class SurveyItemsView extends React.Component<SurveyItemsViewProps, SurveyItemsState> {
        render() {
            return div({ className: "survey-items-view" },
                _.map(this.props.SurveyItems, (surveyItem, index) => SurveyItemViewComponent({SurveyItem: surveyItem, ReloadSurvey: this.reloadSurvey, surveyItemNumber: index+1 })));
        }

        reloadSurvey = (callback?: Function) => {
            this.props.ReloadSurvey(callback);
        };
    }
}