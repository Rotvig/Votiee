//Both ResultsView and AnswerResultView has to be included with createFactory in the file using ResultsView. Use the following:
//var ResultsView = React.createFactory(SharedComponents.ResultsView);
//var AnswerResultViewComponent = React.createFactory(SharedComponents.AnswerResultView);

module SharedComponents {
    //Results
    export interface ResultsViewProps {
        answerResults: Models.AnswerResultViewModel[];
    }

    export class ResultsView extends React.Component<ResultsViewProps, {}> {
        render() {
            return div({ className: "answer-results table-background" },
                //Show table with answers and their vote count
                table({ className: "table table-striped results-table" },
                    thead({},
                        th({}, ""),
                        th({}, "Votes"),
                        th({}, "Answer")
                    ),
                    tbody({},
                        _.map(this.props.answerResults,
                            (answerResult, i) => AnswerResultViewComponent({ answerResult: answerResult, pieColor: colors[i]}))
                        )),
                pieChart({
                    answerResults: this.props.answerResults,
                    height: 250,
                    width: 250,
                    sliceText: (d: Models.AnswerResultViewModel, i: number) => d.VoteCount
                }));
        }
    }

    //AnsweResultsView
    export interface AnswerResultViewProps {
        answerResult: Models.AnswerResultViewModel;
        pieColor: string;
    }

    export class AnswerResultView extends React.Component<AnswerResultViewProps, {}> {
        render() {
            //Return table row with vote count and answerText
            return tr({ className: `answer-result${(this.props.answerResult.Marked) ? " info" : null}` },
                td({className: "pie-color", style: {backgroundColor: this.props.pieColor}}),
                td({ className: "vote-count" }, this.props.answerResult.VoteCount),
                td({ className: "answer-text" }, this.props.answerResult.AnswerText));
        }
    }
}