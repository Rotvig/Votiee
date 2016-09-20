/// <reference path="./SurveyItemSessionView.ts" />
var SurveyItemSessionViewComponent = React.createFactory(SurveySessionPage.SurveyItemSessionView);

module SurveySessionPage {

    export interface SurveyItemsSessionViewProps {
        surveyItems: Array<Models.SurveyItemSessionViewModel>;
        surveySessionCode: string;
        loadSurveySession: Function;
        surveyActive: boolean;
    }

    export class SurveyItemsSessionView extends React.Component<SurveyItemsSessionViewProps,{}> {

        //Render SurveyItems
        render() {
            return div({ className: "survey-items-view" },
                _.map(this.props.surveyItems,
                    (surveyItem) => SurveyItemSessionViewComponent({
                        surveyItem: surveyItem,
                        surveySessionCode: this.props.surveySessionCode,
                        loadSurveySession: this.props.loadSurveySession,
                        surveyActive: this.props.surveyActive
                    })));
        }
    }
}