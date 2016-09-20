/// <reference path="./SurveyView.ts" />
var surveyView = React.createFactory(SurveysOverviewPage.SurveyView);

module SurveysOverviewPage {

    export interface SurveysViewProps {
        surveys: Array<Models.SurveysOverviewSurveyViewModel>;
        surveyChanged: Function;
    }

    export class SurveysView extends React.Component<SurveysViewProps, {}> {

        //Show a list of all the Surveys
        render() {
            return div({ className: "survey-items-view" },
                _.map(this.props.surveys, (survey) => surveyView({ surveyCode: survey.SurveyCode, surveyName: survey.Name, surveyChanged: this.props.surveyChanged})));
        }

    }
} 