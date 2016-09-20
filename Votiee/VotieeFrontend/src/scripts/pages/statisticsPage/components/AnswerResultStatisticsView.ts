
module StatisticsPage {

    export interface AnswerResultStatisticsViewProps {
        answerResult: Models.AnswerResultViewModel;
    }

    export class AnswerResultStatisticsView extends React.Component<AnswerResultStatisticsViewProps, {}> {
        render() {
            //Return table row with vote count and answerText
            return tr({ className: "answer-result " },
                td({ className: "vote-count" }, this.props.answerResult.VoteCount),
                td({ className: "answer-text" }, this.props.answerResult.AnswerText));
        }
    }
}