var ResultsView = React.createFactory(SharedComponents.ResultsView);
var AnswerResultViewComponent = React.createFactory(SharedComponents.AnswerResultView);

module StatisticsPage {

    export interface SurveyItemStatisticsViewProps {
        surveyItem: Models.SurveyItemStatisticsViewModel;
    }

    export class SurveyItemStatisticsView extends React.Component<SurveyItemStatisticsViewProps, {}> {
        render() {
            return div({ className: "survey-item-view" },
                //Show Question
                h4({className: "item-numb-label"}, "Question " + this.props.surveyItem.Order + ":"),
                p({className: "question-text"}, this.props.surveyItem.QuestionText),
                //Show table with answers and their vote count
                ResultsView({answerResults: this.props.surveyItem.AnswerResults})
            );
        }
    }
}